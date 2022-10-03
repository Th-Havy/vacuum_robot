using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArticulationBody))]
public class MotorEncoder : MonoBehaviour
{
    [Header("ROS settings")]
    public ROSPublisher<RosMessageTypes.Std.Float32Msg> publisher = new ROSPublisher<RosMessageTypes.Std.Float32Msg>("encoder", true);

    public float MotorAngle { get; private set; }

    private ArticulationBody _articulationBody;

    void Start()
    {
        _articulationBody = GetComponent<ArticulationBody>();
    }

    void FixedUpdate()
    {
        MotorAngle = _articulationBody.jointPosition[0];
        publisher.Publish(CreateMessage());
    }

    private RosMessageTypes.Std.Float32Msg CreateMessage()
    {
        RosMessageTypes.Std.Float32Msg message = new RosMessageTypes.Std.Float32Msg(MotorAngle);
        return message;
    }
}
