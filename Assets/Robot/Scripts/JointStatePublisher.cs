using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

/// <summary>
/// This component will publish all of the robots non-fixed joint states.
/// </summary>
public class JointStatePublisher : MonoBehaviour
{
    private class RetrievedJoint
    {
        public Unity.Robotics.UrdfImporter.UrdfJoint joint;
        public ArticulationBody articulationBody;
    }

    [Header("ROS settings")]
    private ROSConnection _ros;
    [SerializeField]
    private string topicName = "joint_states";
    public bool includePosition = true;
    public bool includeVelocity = true;
    public bool includeEffort = true;

    private List<RetrievedJoint> _retrivedJoints;

    void Start()
    {
        RetrieveJoints();

        // start the ROS connection
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.RegisterPublisher<RosMessageTypes.Sensor.JointStateMsg>(topicName);
    }

    void FixedUpdate()
    {
        _ros.Publish(topicName, CreateMessage());
    }

    private void RetrieveJoints()
    {
        var joints = GetComponentsInChildren<Unity.Robotics.UrdfImporter.UrdfJoint>(true);

        _retrivedJoints = new List<RetrievedJoint>(joints.Length);

        foreach (var joint in joints)
        {
            if (joint.JointType != Unity.Robotics.UrdfImporter.UrdfJoint.JointTypes.Fixed)
            {
                _retrivedJoints.Add(new RetrievedJoint() {
                    joint = joint,
                    articulationBody = joint.gameObject.GetComponent<ArticulationBody>()
                });
            }
        }
    }

    private RosMessageTypes.Sensor.JointStateMsg CreateMessage()
    {
        RosMessageTypes.Sensor.JointStateMsg message = new RosMessageTypes.Sensor.JointStateMsg();

        // Header
        message.header = new RosMessageTypes.Std.HeaderMsg(ROSUtils.ROSTimeNow(), "");

        // Data
        message.name = new string[_retrivedJoints.Count];
        if (includePosition)
            message.position = new double[_retrivedJoints.Count];
        if (includeVelocity)
            message.velocity = new double[_retrivedJoints.Count];
        if (includeEffort)
            message.effort = new double[_retrivedJoints.Count];

        for (int i = 0; i < _retrivedJoints.Count; i++)
        {
            message.name[i] = _retrivedJoints[i].joint.jointName;
            
            if (includePosition)
                message.position[i] = _retrivedJoints[i].articulationBody.jointPosition[0];
            if (includeVelocity)
                message.velocity[i] = _retrivedJoints[i].articulationBody.jointVelocity[0];
            if (includeEffort)
                message.effort[i] = _retrivedJoints[i].articulationBody.jointForce[0];
        }

        return message;
    }
}
