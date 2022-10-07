using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

/// <summary>
/// Class which can be used to publish ros messages. The <c> onNewMessage </c> event can be used
/// to register callbacks for Unity objects, to avoid subscribing to a topics through ROS.
/// </summary>
[System.Serializable]
public class ROSPublisher<T> where T : Unity.Robotics.ROSTCPConnector.MessageGeneration.Message, new()
{
    [System.Serializable]
    public class MessageEvent : UnityEngine.Events.UnityEvent<T> {}
    
    /// <summary>
    /// If set to false, the messages will not be published on ROS, only the <c> onNewMessage </c>
    /// event will be invoked.
    /// </summary>
    public bool publishMessages;

    /// <summary>
    /// Topic on which the data will be published.
    /// </summary>
    [field: SerializeField]
    public string Topic { get; private set; }

    /// <summary>
    /// Unity subscribers can add callbacks here instead of on the ROS topic which is sent over TCP.
    /// </summary>
    public MessageEvent onNewMessage;

    private ROSConnection _ros;

    public ROSPublisher(string topic = "", bool publishOnROSTopic = true)
    {
        Topic = topic;
        publishMessages = publishOnROSTopic;

        // start the ROS connection
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.RegisterPublisher<T>(Topic);
    }

    public void Publish(T message)
    {
        if (publishMessages && Topic != "")
        {
            _ros.Publish(Topic, message);
            ListAllTopicsWindow.PublisherTopics.Add(new ListAllTopicsWindow.TopicInfo(Topic, typeof(T)));
        }

        onNewMessage.Invoke(message);
    }
}
