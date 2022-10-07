using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

[RequireComponent(typeof(ArticulationBody))]
public class MotorEncoder : MonoBehaviour
{
    [Header("ROS settings")]
    private ROSConnection _ros;
    [SerializeField]
    private string topicName = "encoder";

    public float MotorAngle { get; private set; }

    private ArticulationBody _articulationBody;

    void Start()
    {
        // start the ROS connection
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.RegisterPublisher<RosMessageTypes.Sensor.ImuMsg>(topicName);

        _articulationBody = GetComponent<ArticulationBody>();
    }

    void FixedUpdate()
    {
        MotorAngle = _articulationBody.jointPosition[0];
        _ros.Publish(topicName, CreateMessage());
    }

    private RosMessageTypes.Std.Float32Msg CreateMessage()
    {
        RosMessageTypes.Std.Float32Msg message = new RosMessageTypes.Std.Float32Msg(MotorAngle);
        return message;
    }
}
