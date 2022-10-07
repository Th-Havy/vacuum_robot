using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowLevelController : MonoBehaviour
{
    [Header("Links")]
    public ArticulationBody baseBody;
    public ArticulationBody leftWheel;
    public ArticulationBody rightWheel;
    public ArticulationBody lidarCasing;
    public ArticulationBody lidar;

    [Header("Wheel settings")]
    [SerializeField]
    private float _wheelRadius = 0.06f;

    [SerializeField]
    [Tooltip("Distance between the wheel and center of rotation of the robot.")]
    private float _wheelDistance = 0.12f;

    public float LinearVelocity { get; set; } = 0f;
    public float AngularVelocity { get; set; } = 0f;

    private float _rightWheelAngle = 0f;
    private float _leftWheelAngle = 0f;

    void FixedUpdate()
    {
        if (_wheelRadius == 0f)
        {
            Debug.LogError("wheelRadius should be set to a non-zero value");
            return;
        }

        // Wheel velocities are a sum of desired linear and angular robot velocities 
        float leftAngularVelocity = (LinearVelocity - AngularVelocity * _wheelDistance) / _wheelRadius;
        float rightAngularVelocity = (LinearVelocity + AngularVelocity * _wheelDistance) / _wheelRadius;
        
        _leftWheelAngle += leftAngularVelocity * Time.fixedDeltaTime;
        _rightWheelAngle += rightAngularVelocity * Time.fixedDeltaTime;

        SetDriveTarget(leftWheel, _leftWheelAngle * Mathf.Rad2Deg);
        SetDriveTarget(rightWheel, _rightWheelAngle * Mathf.Rad2Deg);
    }

    public void OpenLidar(bool open = true)
    {
        if (open)
            SetDriveTarget(lidarCasing, lidarCasing.xDrive.upperLimit);
        else
            SetDriveTarget(lidarCasing, lidarCasing.xDrive.lowerLimit);
    }

    void SetDriveTarget(ArticulationBody body, float target)
    {
        ArticulationDrive drive = body.xDrive;
        drive.target = target;
        body.xDrive = drive;
    }
}
