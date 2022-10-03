using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class PositionSensor : MonoBehaviour
{
    [Tooltip("If true, the global position is sent, otherwise the local position.")]
    public bool globalPosition = true;

    [Header("ROS settings")]
    public ROSPublisher<RosMessageTypes.Geometry.TransformStampedMsg> publisher = new ROSPublisher<RosMessageTypes.Geometry.TransformStampedMsg>("unity_tf", true);

    [SerializeField]
    [Tooltip("Frame in which the position is measured.")]
    private string _referenceFrame = "odom";

    [SerializeField]
    [Tooltip("Frame of the link whose position is measured")]
    private string _frame = "link";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        publisher.Publish(CreateMessage());
    }

    private RosMessageTypes.Geometry.TransformStampedMsg CreateMessage()
    {
        RosMessageTypes.Geometry.TransformStampedMsg message = new RosMessageTypes.Geometry.TransformStampedMsg();

        // Header
        message.header = new RosMessageTypes.Std.HeaderMsg(ROSUtils.ROSTimeNow(), _referenceFrame);

        // Settings
        message.child_frame_id = _frame;
        
        // Data
        if (globalPosition)
        {
            message.transform = new RosMessageTypes.Geometry.TransformMsg(transform.position.To<FLU>(), transform.rotation.To<FLU>());
        }
        else
        {
            message.transform = new RosMessageTypes.Geometry.TransformMsg(transform.localPosition.To<FLU>(), transform.localRotation.To<FLU>());
        }

        return message;        
    }
}
