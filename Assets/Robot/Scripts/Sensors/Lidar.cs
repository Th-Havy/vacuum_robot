using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component simulating a 2D lidar scanner
/// Specs similar to Slamtec RPLidar A3: https://www.slamtec.com/en/Lidar/A3Spec
/// </summary>
[RequireComponent(typeof(ArticulationBody))]
public class Lidar : MonoBehaviour
{
    public enum ScanMode
    {
        SingleScan,
        ContinuousScanning
    }

    /// <summary>
    /// Base of the robot on which the lidar is attached, used as a reference for the scan angles.
    /// </summary>
    [field:SerializeField]
    public Transform RobotBase { get; private set; }

    [field:SerializeField]
    public LowLevelController RobotController { get; private set; }

    [Header("LIDAR Settings")]
    [SerializeField]
    [Tooltip("Minimum threshold below which objects are not detected by the LIDAR.")]
    private float minDistance = 0.15f; // m

    [SerializeField]
    [Tooltip("Maximum threshold above which objects are not detected by the LIDAR.")]
    private float maxDistance = 25f; // m

    [SerializeField]
    [Tooltip("Number of scan per second, one scan corresponds to a complete rotation of the LIDAR scanner.")]
    private float scanRate = 10f; // Hz

    [SerializeField]
    [Tooltip("Number of samples per full scan")]
    private uint samplesPerScan = 1024;
    public uint SamplesPerScan { get => samplesPerScan; }

    [Header("Simulation Settings")]
    [Tooltip("Select which layers can be detected by the LIDAR.")]
    public LayerMask detectedLayers = ~0; // All layers detectable by default
    [Tooltip("If the lidar ray hits a surface whose normal angle if above this, it will not be detected.")]
    [Range(0f, 90f)]
    public float maxDetectedSurfaceAngle = 90f;

    [Tooltip("Noise applied on each range measurement.")]
    public GaussianNoise measurementNoise = new GaussianNoise(0f, 0.01f);

    [Header("ROS settings")]
    public ROSPublisher<RosMessageTypes.Sensor.LaserScanMsg> publisher = new ROSPublisher<RosMessageTypes.Sensor.LaserScanMsg>("lidar", true);

    [SerializeField]
    [Tooltip("Frame in which the scan are measured.")]
    private string frame = "lidar";

    // LIDAR will rotate about the axis of the articulation body
    private ArticulationBody motor;
    private float _motorAngle = 0f; // rad

    /// <summary> 
    /// Use this property to set the motor angle (in rad), and automatically update the ArticulationBody target. 
    /// </summary>
    private float MotorAngle
    {
        get => _motorAngle;
        set
        {
            _motorAngle = value;
            ArticulationDrive drive = motor.xDrive;
            drive.target = _motorAngle * Mathf.Rad2Deg;
            motor.xDrive = drive;
        }
    }

    private bool isScanning = false; 
    private ScanMode scanMode = ScanMode.SingleScan;
    private float _timeFirstMeasurement = 0f;
    private uint currentSample = 0;
    private float[] scanRanges;
    public float[] LastMeasuredRanges { get; private set; }

    public void StartScanning(ScanMode chosenScanMode)
    {
        scanRanges = new float[samplesPerScan];
        for (int i = 0; i < samplesPerScan; i++)
        {
            scanRanges[i] = Mathf.Infinity;
        }
        currentSample = 0;
        scanMode = chosenScanMode;

        // Hard-reset the position of the motor
        motor.jointPosition = new ArticulationReducedSpace(0f);
        MotorAngle = 0f;

        isScanning = true;
    }

    void Start()
    {
        RobotController.OpenLidar(true);

        motor = GetComponent<ArticulationBody>();

        if (motor.jointType != ArticulationJointType.RevoluteJoint)
        {
            Debug.LogWarning("The ArticulationBody component on this GameObject must be a revolute joint.");
        }

        StartScanning(ScanMode.ContinuousScanning);
    }

    void FixedUpdate()
    {
        if (!isScanning)
        {
            return;
        }
        
        // Rotate LIDAR scanner (only for visual visualization + inertial forces acting on base)
        MotorAngle += 2.0f * Mathf.PI * scanRate * Time.fixedDeltaTime;

        // Perform scans that occured during physics simulation step
        float currentScanAngle = ComputeScanAngle(currentSample);
        while (currentScanAngle < MotorAngle && currentSample < samplesPerScan)
        {
            if (currentSample == 0)
            {
                _timeFirstMeasurement = Time.time;
            }

            currentScanAngle = ComputeScanAngle(currentSample);
            Ray lidarRay = new Ray(transform.position, Quaternion.AngleAxis(-currentScanAngle * Mathf.Rad2Deg, RobotBase.up) * RobotBase.forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(lidarRay, out hitInfo, maxDistance, detectedLayers))
            {
                if (hitInfo.distance > minDistance &&
                    Vector3.Angle(lidarRay.direction, hitInfo.normal) % 90f > 90f - maxDetectedSurfaceAngle)
                {
                    scanRanges[currentSample] = hitInfo.distance;
                    scanRanges[currentSample] = measurementNoise.ApplyNoise(scanRanges[currentSample]);
                    scanRanges[currentSample] = Mathf.Clamp(scanRanges[currentSample], minDistance, maxDistance);
                }
            }

            currentSample++;
        }

        // Check if a full scan has been completed
        if (currentSample == samplesPerScan)
        {
            LastMeasuredRanges = new float[samplesPerScan];
            for (int i = 0; i < samplesPerScan; i++)
            {
                LastMeasuredRanges[i] = scanRanges[i];
            }

            RosMessageTypes.Sensor.LaserScanMsg message = CreateLaserScanMessage();
            publisher.Publish(message);
            
            if (scanMode == ScanMode.SingleScan)
            {
                isScanning = false;
            }
            else
            {
                // Reset measurements
                StartScanning(ScanMode.ContinuousScanning);
            }
        }
    }

    /// <summary>
    /// Compute the scan angle (in rad) corresponding to the provided sample.
    /// </summary>
    public float ComputeScanAngle(uint scanSample)
    {
        return 2f * Mathf.PI * (float)scanSample / (float)samplesPerScan;
    }

    private RosMessageTypes.Sensor.LaserScanMsg CreateLaserScanMessage()
    {
        RosMessageTypes.Sensor.LaserScanMsg scanMessage = new RosMessageTypes.Sensor.LaserScanMsg();

        // Header
        scanMessage.header = new RosMessageTypes.Std.HeaderMsg(ROSUtils.UnityTimeToROSTime(_timeFirstMeasurement), frame);

        // Settings
        scanMessage.angle_increment = 360f / (float)samplesPerScan * Mathf.Deg2Rad;
        scanMessage.angle_min = 0f;
        scanMessage.angle_max = 360f * Mathf.Deg2Rad - scanMessage.angle_increment;
        scanMessage.time_increment = 1f / (float) samplesPerScan;
        scanMessage.scan_time = 1f / scanRate;
        scanMessage.range_min = minDistance;
        scanMessage.range_max = maxDistance;
        
        // Data
        scanMessage.ranges = LastMeasuredRanges;

        // TODO: decide what to put in the intensities field
        // scanMessage.intensities = ...

        return scanMessage;
    }
}
