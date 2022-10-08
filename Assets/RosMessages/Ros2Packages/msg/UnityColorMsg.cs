//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Ros2Packages
{
    [Serializable]
    public class UnityColorMsg : Message
    {
        public const string k_RosMessageName = "ros2_packages/UnityColor";
        public override string RosMessageName => k_RosMessageName;

        public int r;
        public int g;
        public int b;
        public int a;

        public UnityColorMsg()
        {
            this.r = 0;
            this.g = 0;
            this.b = 0;
            this.a = 0;
        }

        public UnityColorMsg(int r, int g, int b, int a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static UnityColorMsg Deserialize(MessageDeserializer deserializer) => new UnityColorMsg(deserializer);

        private UnityColorMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.r);
            deserializer.Read(out this.g);
            deserializer.Read(out this.b);
            deserializer.Read(out this.a);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.r);
            serializer.Write(this.g);
            serializer.Write(this.b);
            serializer.Write(this.a);
        }

        public override string ToString()
        {
            return "UnityColorMsg: " +
            "\nr: " + r.ToString() +
            "\ng: " + g.ToString() +
            "\nb: " + b.ToString() +
            "\na: " + a.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
