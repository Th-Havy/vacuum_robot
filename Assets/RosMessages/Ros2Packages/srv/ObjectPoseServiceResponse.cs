//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Ros2Packages
{
    [Serializable]
    public class ObjectPoseServiceResponse : Message
    {
        public const string k_RosMessageName = "ros2_packages/ObjectPoseService";
        public override string RosMessageName => k_RosMessageName;

        public Geometry.PoseMsg object_pose;

        public ObjectPoseServiceResponse()
        {
            this.object_pose = new Geometry.PoseMsg();
        }

        public ObjectPoseServiceResponse(Geometry.PoseMsg object_pose)
        {
            this.object_pose = object_pose;
        }

        public static ObjectPoseServiceResponse Deserialize(MessageDeserializer deserializer) => new ObjectPoseServiceResponse(deserializer);

        private ObjectPoseServiceResponse(MessageDeserializer deserializer)
        {
            this.object_pose = Geometry.PoseMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.object_pose);
        }

        public override string ToString()
        {
            return "ObjectPoseServiceResponse: " +
            "\nobject_pose: " + object_pose.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Response);
        }
    }
}