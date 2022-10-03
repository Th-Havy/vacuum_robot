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
                AddLoger<RosMessageTypes.Std.BoolMsg>();
                break;
            case MessageType.Byte:
                AddLoger<RosMessageTypes.Std.ByteMsg>();
                break;
            case MessageType.ByteMultiArray:
                AddLoger<RosMessageTypes.Std.ByteMultiArrayMsg>();
                break;
            case MessageType.Char:
                AddLoger<RosMessageTypes.Std.CharMsg>();
                break;
            case MessageType.ColorRGBA:
                AddLoger<RosMessageTypes.Std.ColorRGBAMsg>();
                break;
            // case MessageType.Duration:
            //     // TODO: correct namespace once Unity has implemented the fix
            //     AddLoger<Unity.Robotics.ROSTCPConnector.MessageGeneration.timeMsg>();
            //     break;
            case MessageType.Empty:
                AddLoger<RosMessageTypes.Std.EmptyMsg>();
                break;
            case MessageType.Float32:
                AddLoger<RosMessageTypes.Std.Float32Msg>();
                break;
            case MessageType.Float32MultiArray:
                AddLoger<RosMessageTypes.Std.Float32MultiArrayMsg>();
                break;
            case MessageType.Float64:
                AddLoger<RosMessageTypes.Std.Float64Msg>();
                break;
            case MessageType.Float64MultiArray:
                AddLoger<RosMessageTypes.Std.Float64MultiArrayMsg>();
                break;
            case MessageType.Header:
                AddLoger<RosMessageTypes.Std.HeaderMsg>();
                break;
            case MessageType.Int16:
                AddLoger<RosMessageTypes.Std.Int16Msg>();
                break;
            case MessageType.Int16MultiArray:
                AddLoger<RosMessageTypes.Std.Int16MultiArrayMsg>();
                break;
            case MessageType.Int32:
                AddLoger<RosMessageTypes.Std.Int32Msg>();
                break;
            case MessageType.Int32MultiArray:
                AddLoger<RosMessageTypes.Std.Int32MultiArrayMsg>();
                break;
            case MessageType.Int64:
                AddLoger<RosMessageTypes.Std.Int64Msg>();
                break;
            case MessageType.Int64MultiArray:
                AddLoger<RosMessageTypes.Std.Int64MultiArrayMsg>();
                break;
            case MessageType.Int8:
                AddLoger<RosMessageTypes.Std.Int8Msg>();
                break;
            case MessageType.Int8MultiArray:
                AddLoger<RosMessageTypes.Std.Int8MultiArrayMsg>();
                break;
            case MessageType.MultiArrayDimension:
                AddLoger<RosMessageTypes.Std.MultiArrayDimensionMsg>();
                break;
            case MessageType.MultiArrayLayout:
                AddLoger<RosMessageTypes.Std.MultiArrayLayoutMsg>();
                break;
            case MessageType.String:
                AddLoger<RosMessageTypes.Std.StringMsg>();
                break;
            // case MessageType.Time:
            //     AddLoger<RosMessageTypes.Std.TimeMsg>();
            //     break;
            case MessageType.UInt16:
                AddLoger<RosMessageTypes.Std.UInt16Msg>();
                break;
            case MessageType.UInt16MultiArray:
                AddLoger<RosMessageTypes.Std.UInt16MultiArrayMsg>();
                break;
            case MessageType.UInt32:
                AddLoger<RosMessageTypes.Std.UInt32Msg>();
                break;
            case MessageType.UInt32MultiArray:
                AddLoger<RosMessageTypes.Std.UInt32MultiArrayMsg>();
                break;
            case MessageType.UInt64:
                AddLoger<RosMessageTypes.Std.UInt64Msg>();
                break;
            case MessageType.UInt64MultiArray:
                AddLoger<RosMessageTypes.Std.UInt64MultiArrayMsg>();
                break;
            case MessageType.UInt8:
                AddLoger<RosMessageTypes.Std.UInt8Msg>();
                break;
            case MessageType.UInt8MultiArray:
                AddLoger<RosMessageTypes.Std.UInt8MultiArrayMsg>();
                break;
            default:
                break;
        }
    }

    private void AddLoger<T>() where T : Unity.Robotics.ROSTCPConnector.MessageGeneration.Message, new()
    {
        Debug.Log("Subscribed to: " + Topic);
        ROSConnection.GetOrCreateInstance().Subscribe<T>(Topic, LogCallback);
        ListAllTopicsWindow.SubscriberTopics.Add(new ListAllTopicsWindow.TopicInfo(Topic, typeof(T)));
    }

    private void LogCallback<T>(T message) where T : Unity.Robotics.ROSTCPConnector.MessageGeneration.Message, new()
    {
        Debug.Log(message);
    }
}
