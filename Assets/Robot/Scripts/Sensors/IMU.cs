using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

[RequireComponent(typeof(ArticulationBody))]
public class IMU : MonoBehaviour
{
    /// <summary>
    /// As detailled in the <see href="http://docs.ros.org/en/melodic/api/sensor_msgs/html/msg/ImuMsg.html"> documentation </see>
    /// quantities that are not measured have their covariance matrix first element set to -1
    /// </summary>
    private static Matrix3x3 _NotMeasuredQuantityCovariance = new Matrix3x3(-1);

    [Header("Orientation")]
    public bool measureOrientation = true;
    public Quaternion Orientation { get; private set; }
    [SerializeField]
    [Tooltip("Covariance matrix of the orientation measurement, set to all 0s if unknown.")]
    private Matrix3x3 _orientationCovariance = new Matrix3x3(0);
    
    [Header("Angular velocity")]
    public bool measureAngularVelocity = true;
    public Vector3 AngularVelocity { get; private set; }
    [SerializeField]
    [Tooltip("Covariance matrix of the angular velocity measurement, set to all 0s if unknown.")]
    private Matrix3x3 _angularVelocityCovariance = new Matrix3x3(0);

    [Header("Linear acceleration")]
    public bool measureLinearAcceleration = true;
    public Vector3 LinearAcceleration { get; private set; }
    [SerializeField]
    [Tooltip("Covariance matrix of the angular velocity measurement, set to all 0s if unknown.")]
    private Matrix3x3 _linearAccelerationCovariance = new Matrix3x3(0);

    [Header("ROS settings")]
    private ROSConnection _ros;
    [SerializeField]
    private string topicName = "imu";

    [SerializeField]
    [Tooltip("Frame in which the ImuMsg operates.")]
    private string frame = "ImuMsg";

    private ArticulationBody _articulationBody;
    private Vector3 _oldVelocity;
    private uint seq;

    void Start()
    {
        // start the ROS connection
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.RegisterPublisher<RosMessageTypes.Sensor.ImuMsg>(topicName);

        _articulationBody = GetComponent<ArticulationBody>();
        _oldVelocity = _articulationBody.velocity;
    }

    void FixedUpdate()
    {
        if (measureOrientation)
        {
            Orientation = transform.localRotation;
        }

        if (measureAngularVelocity)
        {
            AngularVelocity = _articulationBody.angularVelocity;
        }

        if (measureLinearAcceleration)
        {
            LinearAcceleration = (_articulationBody.velocity - _oldVelocity) / Time.fixedDeltaTime;
        }

        _ros.Publish(topicName, CreateImuMsgMessage());
    }

    private RosMessageTypes.Sensor.ImuMsg CreateImuMsgMessage()
    {
        RosMessageTypes.Sensor.ImuMsg message = new RosMessageTypes.Sensor.ImuMsg();

        // Header
        message.header = new RosMessageTypes.Std.HeaderMsg(ROSUtils.ROSTimeNow(), frame);

        // Data
        if (measureOrientation)
        {
            message.orientation = Orientation.To<FLU>();
            message.orientation_covariance = _orientationCovariance.FromUnityToROS().toArray();
        }
        else
        {
            message.orientation = Quaternion.identity.To<FLU>();
            message.orientation_covariance = _NotMeasuredQuantityCovariance.toArray();
        }
        if (measureAngularVelocity)
        {
            message.angular_velocity = AngularVelocity.To<FLU>();
            message.angular_velocity_covariance = _angularVelocityCovariance.FromUnityToROS().toArray();
        }
        else
        {
            message.angular_velocity = Vector3.zero.To<FLU>();
            message.angular_velocity_covariance = _NotMeasuredQuantityCovariance.toArray();
        }
        if (measureLinearAcceleration)
        {
            message.linear_acceleration = LinearAcceleration.To<FLU>();
            message.linear_acceleration_covariance = _linearAccelerationCovariance.FromUnityToROS().toArray();
        }
        else
        {
            message.linear_acceleration = Vector3.zero.To<FLU>();
            message.linear_acceleration_covariance = _NotMeasuredQuantityCovariance.toArray();
        }
        
        return message;
    }
    
}
