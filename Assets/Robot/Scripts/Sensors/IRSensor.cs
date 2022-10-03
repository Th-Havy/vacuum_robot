using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRSensor : MonoBehaviour
{
    [Header("IR Sensor settings")]
    [SerializeField]
    [Tooltip("Direction of measurement, in local orientation.")]
    private Vector3 _sensingDirection = Vector3.forward;
    public Vector3 SensingDirection
    { 
        get { return _sensingDirection; }

        set
        {
            _sensingDirection = value;
            RequestConeColliderRebuild();
        }
    }

    [SerializeField]
    [Tooltip("Minimum threshold below which objects are not detected by the sensor.")]
    private float _minRange = 0.01f; // m
    public float MinRange
    { 
        get { return _minRange; }

        set
        {
            _minRange = value;
            RequestConeColliderRebuild();
        }
    }

    [SerializeField]
    [Tooltip("Maximum threshold above which objects are not detected by the sensor.")]
    private float _maxRange = 0.8f; // m
    public float MaxRange
    { 
        get { return _maxRange; }

        set
        {
            _maxRange = value;
            RequestConeColliderRebuild();
        }
    }

    [SerializeField]
    [Range(0f, 90f)]
    [Tooltip("The object causing the range reading may have been anywhere within -field_of_view/2 and field_of_view/2 at the measured range.")]
    private float _fov = 0f;    
    public float Fov
    { 
        get { return _fov; }

        set
        {
            _fov = value;
            RequestConeColliderRebuild();
        }
    }

    [Header("Simulation")]
    [Tooltip("Select which layers can be detected by the sensor.")]
    public LayerMask detectedLayers = ~0; // All layers detectable by default

    [Tooltip("If the sensor hits a surface whose normal angle if above this, it will not be detected.")]
    [Range(0f, 90f)]
    public float maxDetectedSurfaceAngle = 90f;
    
    [Header("ROS settings")]
    public ROSPublisher<RosMessageTypes.Sensor.RangeMsg> publisher = new ROSPublisher<RosMessageTypes.Sensor.RangeMsg>("ir_sensor", true);

    [SerializeField]
    [Tooltip("Frame in which the distance is measured.")]
    private string frame = "ir_sensor";

    public float LastMeasuredRange { get; private set; }

    private GameObject _coneChild;
    private MeshCollider _coneMeshCollider;
    private int _numVerticesCircle = 16;
    private float _closestDistanceIntersectingObject = Mathf.Infinity;

    void Start()
    {
        _coneChild = new GameObject("Cone");
        _coneChild.tag = "robot";
        _coneChild.transform.parent = transform;
        _coneChild.transform.localPosition = Vector3.zero;
        RequestConeRealignment();
        _coneChild.hideFlags = HideFlags.NotEditable;
        _coneMeshCollider = _coneChild.AddComponent<MeshCollider>();
        _coneMeshCollider.convex = true;
        _coneMeshCollider.isTrigger = true;
        RequestConeColliderRebuild();
    }

    void FixedUpdate()
    {
        float distance = Mathf.Infinity;
        
        if (Fov == 0f)
        {
            // If FOV is 0, the distance detection is done with a raycast instead of a cone collider
            distance = RaycastDetection();
        }
        else
        {
            distance = _closestDistanceIntersectingObject;
        }

        LastMeasuredRange = distance;
        RosMessageTypes.Sensor.RangeMsg message = CreateRangeMessage();

        publisher.Publish(message);

        // Reset cone intersection distance for this fixed update step
        _closestDistanceIntersectingObject = Mathf.Infinity;
    }

    void OnTriggerEnter(Collider other)
    {
        FindClosestDistance(other);
    }

    void OnTriggerStay(Collider other)
    {
        FindClosestDistance(other);
    }

    private void FindClosestDistance(Collider other)
    {
        float distance = ConeDetection(other);
        _closestDistanceIntersectingObject = Mathf.Min(distance, _closestDistanceIntersectingObject);
    }

    private float ConeDetection(Collider other)
    {
        if ((LayerMask.GetMask(LayerMask.LayerToName(other.gameObject.layer)) & detectedLayers.value) != 0)
        {
            float distance = (transform.position - other.ClosestPoint(transform.position)).magnitude;
            // Debug.Log(distance.ToString() + "\t " + Mathf.Max(distance, MinRange).ToString());
            distance = Mathf.Max(distance, MinRange);
            return distance;
        }

        return Mathf.Infinity;
    }

    private float RaycastDetection()
    {
        Ray lidarRay = new Ray(transform.position, transform.TransformDirection(SensingDirection));
        RaycastHit hitInfo;
        if (Physics.Raycast(lidarRay, out hitInfo, MaxRange, detectedLayers))
        {
            if (hitInfo.distance > MinRange &&
                Vector3.Angle(lidarRay.direction, hitInfo.normal) % 90f > 90f - maxDetectedSurfaceAngle)
            {
                return hitInfo.distance;
            }
        }

        return Mathf.Infinity;
    }

    private RosMessageTypes.Sensor.RangeMsg CreateRangeMessage()
    {
        RosMessageTypes.Sensor.RangeMsg message = new RosMessageTypes.Sensor.RangeMsg();

        // Header
        message.header = new RosMessageTypes.Std.HeaderMsg(ROSUtils.ROSTimeNow(), frame);

        // Settings
        message.radiation_type = RosMessageTypes.Sensor.RangeMsg.INFRARED;
        message.field_of_view = Mathf.Deg2Rad * Fov;
        message.min_range = MinRange;
        message.max_range = MaxRange;
        
        // Data
        message.range = LastMeasuredRange;

        return message;        
    }

    public void RequestConeRealignment()
    {
        if (!Application.isPlaying)
            return;

        _coneChild.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, SensingDirection);
    }

    public void RequestConeColliderRebuild()
    {
        if (!Application.isPlaying)
            return;

        BuildConeCollider();
    }

    private void BuildConeCollider()
    {
        // If FOV is 0, no cone is needed, a raycast will be used instead
        if (Fov == 0f)
        {
            _coneMeshCollider.sharedMesh = new Mesh();
            return;
        }

        // Cone (revolution of a triangle) or section of cone (revolution of trapeze)
        bool isFullCone = (MinRange == 0f);

        float r1 = MinRange * Mathf.Tan(Mathf.Deg2Rad * Fov / 2f);
        float r2 = MaxRange * Mathf.Tan(Mathf.Deg2Rad * Fov / 2f);

        if (r1 <= 0.0025f)
        {
            // Merge small base vertices if small base very small
            isFullCone = true;
        }

        int numVertices;
        int numTriangles;

        if (isFullCone)
        {
            numVertices = _numVerticesCircle + 2;
            numTriangles = 6 * _numVerticesCircle;
        }
        else
        {
            numVertices = 2 * _numVerticesCircle + 2;
            numTriangles = 12 * _numVerticesCircle;
        }

        List<Vector3> vertices = new List<Vector3>(numVertices);
        List<int> triangles = new List<int>(numTriangles);

        if (isFullCone)
        {
            // Circular base of cone 
            for (int i = 0; i < _numVerticesCircle; i++)
            {
                Vector3 vertex = Vector3.forward * MaxRange;
                vertex += Quaternion.Euler(0f, 0f, i * 360f / _numVerticesCircle) * Vector3.right * r2;
                vertices.Add(vertex);

                // triangle base to tip
                triangles.Add(i);
                triangles.Add((i + 1) % _numVerticesCircle);
                triangles.Add(_numVerticesCircle);

                // triangle of the base
                triangles.Add(i);
                triangles.Add((i + 1) % _numVerticesCircle);
                triangles.Add(_numVerticesCircle + 1);
            }

            // Tip of the cone
            vertices.Add(Vector3.forward * MinRange);

            // Center of the base
            vertices.Add(Vector3.forward * MaxRange);
        }
        else
        {
            // Small circle of cone
            for (int i = 0; i < _numVerticesCircle; i++)
            {
                Vector3 vertex = Vector3.forward * MinRange;
                vertex += Quaternion.Euler(0f, 0f, i * 360f / _numVerticesCircle) * Vector3.right * r1;
                vertices.Add(vertex);
            }

            // Big circle of cone
            for (int i = 0; i < _numVerticesCircle; i++)
            {
                Vector3 vertex = Vector3.forward * MaxRange;
                vertex += Quaternion.Euler(0f, 0f, i * 360f / _numVerticesCircle) * Vector3.right * r2;
                vertices.Add(vertex);

                // triangles side of cone
                triangles.Add(i);
                triangles.Add((i + 1) % _numVerticesCircle);
                triangles.Add(_numVerticesCircle + i);
                triangles.Add(_numVerticesCircle + i);
                triangles.Add(_numVerticesCircle + (i + 1) % _numVerticesCircle);
                triangles.Add((i + 1) % _numVerticesCircle);

                // triangle of the bases
                triangles.Add(i);
                triangles.Add((i + 1) % _numVerticesCircle);
                triangles.Add(2 * _numVerticesCircle);
                triangles.Add(_numVerticesCircle + i);
                triangles.Add(_numVerticesCircle + (i + 1) % _numVerticesCircle);
                triangles.Add(2 * _numVerticesCircle + 1);
            }

            // Center of the bases
            vertices.Add(Vector3.forward * MinRange);
            vertices.Add(Vector3.forward * MaxRange);
        }

        Mesh coneMesh = new Mesh();
        coneMesh.SetVertices(vertices);
        coneMesh.SetTriangles(triangles, 0);
        _coneMeshCollider.sharedMesh = coneMesh;
    }
}
