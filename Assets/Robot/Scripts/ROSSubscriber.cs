using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

/// <summary>
/// Class which can be used to subscribe to ROS messages. The <c> onNewMessage </c> event can be used
/// to register callbacks for Unity objects.
/// </summary>
[System.Serializable]
public class ROSSubscriber<T> where T : Unity.Robotics.ROSTCPConnector.MessageGeneration.Message, new()
{
    [System.Serializable]
    public class MessageEvent : UnityEngine.Events.UnityEvent<T> {}

    /// <summary>
    /// Topic on which the data will be published. Changing the topic after the constructor has been called
    /// will not produce any change.
    /// </summary>
    [field: SerializeField]
    public string Topic { get; private set; }

    /// <summary>
    /// Add callbacks to this event which will be executed everytime a new message is received.
    /// </summary>
    public MessageEvent onNewMessage;

    public ROSSubscriber(string topic = "")
    {
        Topic = topic;
        ROSConnection.GetOrCreateInstance().Subscribe<T>(topic, Callback);
        ListAllTopicsWindow.SubscriberTopics.Add(new ListAllTopicsWindow.TopicInfo(Topic, typeof(T)));
    }

    private void Callback(T message)
    {
        onNewMessage.Invoke(message);
    }
}
