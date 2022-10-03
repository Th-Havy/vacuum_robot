using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

public class ROSLoggerSubscriber : MonoBehaviour
{
    public enum MessageType
    {
        Bool,
        Byte,
        ByteMultiArray,
        Char,
        ColorRGBA,
        Duration,
        Empty,
        Float32,
        Float32MultiArray,
        Float64,
        Float64MultiArray,
        Header,
        Int16,
        Int16MultiArray,
        Int32,
        Int32MultiArray,
        Int64,
        Int64MultiArray,
        Int8,
        Int8MultiArray,
        MultiArrayDimension,
        MultiArrayLayout,
        String,
        Time,
        UInt16,
        UInt16MultiArray,
        UInt32,
        UInt32MultiArray,
        UInt64,
        UInt64MultiArray,
        UInt8,
        UInt8MultiArray,
    }

    [field: SerializeField]
    public MessageType TopicType { get; private set; } = MessageType.String;

    [field: SerializeField]
    public string Topic { get; private set; }

    private void Start()
    {
        switch (TopicType)
        {
            case MessageType.Bool:
                AddLoger<RosMessageTypes.Std.Bool>();
                break;
            case MessageType.Byte:
                AddLoger<RosMessageTypes.Std.Byte>();
                break;
            case MessageType.ByteMultiArray:
                AddLoger<RosMessageTypes.Std.ByteMultiArray>();
                break;
            case MessageType.Char:
                AddLoger<RosMessageTypes.Std.Char>();
                break;
            case MessageType.ColorRGBA:
                AddLoger<RosMessageTypes.Std.ColorRGBA>();
                break;
            case MessageType.Duration:
                // TODO: correct namespace once Unity has implemented the fix
                AddLoger<Unity.Robotics.ROSTCPConnector.MessageGeneration.Duration>();
                break;
            case MessageType.Empty:
                AddLoger<RosMessageTypes.Std.Empty>();
                break;
            case MessageType.Float32:
                AddLoger<RosMessageTypes.Std.Float32>();
                break;
            case MessageType.Float32MultiArray:
                AddLoger<RosMessageTypes.Std.Float32MultiArray>();
                break;
            case MessageType.Float64:
                AddLoger<RosMessageTypes.Std.Float64>();
                break;
            case MessageType.Float64MultiArray:
                AddLoger<RosMessageTypes.Std.Float64MultiArray>();
                break;
            case MessageType.Header:
                AddLoger<RosMessageTypes.Std.Header>();
                break;
            case MessageType.Int16:
                AddLoger<RosMessageTypes.Std.Int16>();
                break;
            case MessageType.Int16MultiArray:
                AddLoger<RosMessageTypes.Std.Int16MultiArray>();
                break;
            case MessageType.Int32:
                AddLoger<RosMessageTypes.Std.Int32>();
                break;
            case MessageType.Int32MultiArray:
                AddLoger<RosMessageTypes.Std.Int32MultiArray>();
                break;
            case MessageType.Int64:
                AddLoger<RosMessageTypes.Std.Int64>();
                break;
            case MessageType.Int64MultiArray:
                AddLoger<RosMessageTypes.Std.Int64MultiArray>();
                break;
            case MessageType.Int8:
                AddLoger<RosMessageTypes.Std.Int8>();
                break;
            case MessageType.Int8MultiArray:
                AddLoger<RosMessageTypes.Std.Int8MultiArray>();
                break;
            case MessageType.MultiArrayDimension:
                AddLoger<RosMessageTypes.Std.MultiArrayDimension>();
                break;
            case MessageType.MultiArrayLayout:
                AddLoger<RosMessageTypes.Std.MultiArrayLayout>();
                break;
            case MessageType.String:
                AddLoger<RosMessageTypes.Std.String>();
                break;
            case MessageType.Time:
                AddLoger<RosMessageTypes.Std.Time>();
                break;
            case MessageType.UInt16:
                AddLoger<RosMessageTypes.Std.UInt16>();
                break;
            case MessageType.UInt16MultiArray:
                AddLoger<RosMessageTypes.Std.UInt16MultiArray>();
                break;
            case MessageType.UInt32:
                AddLoger<RosMessageTypes.Std.UInt32>();
                break;
            case MessageType.UInt32MultiArray:
                AddLoger<RosMessageTypes.Std.UInt32MultiArray>();
                break;
            case MessageType.UInt64:
                AddLoger<RosMessageTypes.Std.UInt64>();
                break;
            case MessageType.UInt64MultiArray:
                AddLoger<RosMessageTypes.Std.UInt64MultiArray>();
                break;
            case MessageType.UInt8:
                AddLoger<RosMessageTypes.Std.UInt8>();
                break;
            case MessageType.UInt8MultiArray:
                AddLoger<RosMessageTypes.Std.UInt8MultiArray>();
                break;
            default:
                break;
        }
    }

    private void AddLoger<T>() where T : Unity.Robotics.ROSTCPConnector.MessageGeneration.Message, new()
    {
        Debug.Log("Subscribed to: " + Topic);
        ROSConnection.instance.Subscribe<T>(Topic, LogCallback);
        ListAllTopicsWindow.SubscriberTopics.Add(new ListAllTopicsWindow.TopicInfo(Topic, typeof(T)));
    }

    private void LogCallback<T>(T message) where T : Unity.Robotics.ROSTCPConnector.MessageGeneration.Message, new()
    {
        Debug.Log(message);
    }
}
