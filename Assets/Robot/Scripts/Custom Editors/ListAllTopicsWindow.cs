using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ListAllTopicsWindow : EditorWindow
{
    private enum Formatting
    {
        Default,
        PythonDictionary,
        TCPServerDictionary,
    }

    private enum ROSObjectType
    {
        Publisher,
        Subsriber
    }

    public struct TopicInfo
    {
        public string topic;
        public System.Type messageType;

        public TopicInfo(string topic, System.Type messageType)
        {
            this.topic = topic;
            this.messageType = messageType;
        }
    }

    private static Formatting _formatting = Formatting.Default;
    private static string _tab ="    ";
    private static char _quote = '\'';
    private static int _queueSize = 10;
    private static string _tcpServer = "tcp_server";
    private static bool _removeROSSharpNamespace = true;
    private static bool _includeRosTCPEndpointPackageName = false;

    /// <summary>
    /// Property keeping a list of all the topics that are being published.
    /// </summary>
    public static HashSet<TopicInfo> PublisherTopics { get; private set; } = new HashSet<TopicInfo>();

    /// <summary>
    /// Property keeping a list of all the topics that are being subscribed to.
    /// </summary>
    public static HashSet<TopicInfo> SubscriberTopics { get; private set; } = new HashSet<TopicInfo>();

    [MenuItem ("Robotics/List all topics")]
    public static void ShowWindow ()
    {
        EditorWindow.GetWindow(typeof(ListAllTopicsWindow));
    }
    
    void OnGUI ()
    {
        // Select formatting type
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Output formatting");
        _formatting = (Formatting)EditorGUILayout.EnumPopup(_formatting);
        GUILayout.EndHorizontal();

        if (_formatting != Formatting.Default)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tabs");
            _tab = GUILayout.TextField(_tab, 8);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Quotes");
            _quote = GUILayout.TextField(_quote.ToString(), 1)[0];
            GUILayout.EndHorizontal();
        }

        if (_formatting == Formatting.TCPServerDictionary)
        {
            _queueSize = EditorGUILayout.IntField("Queue size", _queueSize);
            _tcpServer = EditorGUILayout.TextField("Python TCP server name", _tcpServer);
            _removeROSSharpNamespace = EditorGUILayout.Toggle("Remove ROSSharp namespace", _removeROSSharpNamespace);
            _includeRosTCPEndpointPackageName = EditorGUILayout.Toggle("ros_tcp_endpoint package", _includeRosTCPEndpointPackageName);
        }

        if (!Application.isPlaying)
        {
            GUILayout.Label("Enter playmode to get the list of used topics.", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            return;
        }

        GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea);
        Font monoFont = Font.CreateDynamicFontFromOSFont("Noto Mono", textAreaStyle.fontSize);
        textAreaStyle.font = monoFont;

        // Only in playmode
        GUILayout.Label("Published topics");
        GUILayout.TextArea(GetFormattedSet(PublisherTopics, ROSObjectType.Publisher), textAreaStyle);
        GUILayout.Label("Subscribed topics");
        GUILayout.TextArea(GetFormattedSet(SubscriberTopics, ROSObjectType.Subsriber), textAreaStyle);
    }

    private string GetFormattedSet(HashSet<TopicInfo> set, ROSObjectType rosObjectType)
    {
        string result = "";

        switch (_formatting)
        {
            case Formatting.Default:
                foreach (TopicInfo item in set)
                {
                    result += string.Format("{0} ({1})\n", item.topic, item.messageType);
                }
                break;
            case Formatting.PythonDictionary:
                result = "{\n";
                foreach (TopicInfo item in set)
                {
                    result += string.Format("\t'{0}': {1},\n", item.topic, item.messageType);
                }
                result += "}";
                // Replace tabs, qutoes
                result = result.Replace("\t", _tab);
                result = result.Replace('\'', _quote);
                break;
            case Formatting.TCPServerDictionary:
                result = "{\n";
                foreach (TopicInfo item in set)
                {
                    string messageType = _removeROSSharpNamespace ? RemoveROSSharpNamespace(item.messageType.ToString()) : item.messageType.ToString();
                    string constructorName = "";
                    string constructorParams = "";
                    string packageName = _includeRosTCPEndpointPackageName ? "ros_tcp_endpoint." : "";

                    if (rosObjectType == ROSObjectType.Publisher)
                    {
                        constructorName = "RosPublisher";
                        constructorParams = string.Format("topic='{0}', message_class={1}, queue_size={2}", item.topic, messageType, _queueSize);
                    }
                    else if (rosObjectType == ROSObjectType.Subsriber)
                    {
                        constructorName = "RosSubscriber";
                        constructorParams = string.Format("topic='{0}', message_class={1}, tcp_server={2}, queue_size={3}", item.topic, messageType, _tcpServer, _queueSize);
                    }

                    result += string.Format("\t'{0}': {1}{2}({3}),\n", item.topic, packageName, constructorName, constructorParams);    
                }
                result += "}";
                // Replace tabs, qutoes
                result = result.Replace("\t", _tab);
                result = result.Replace('\'', _quote);
                break;
            default:
                break;
        }

        return result;
    }

    // Remove ROSSharp Namespace (e.g. RosMessageTypes.Std.String => String) 
    private string RemoveROSSharpNamespace(string messageType)
    {
        string[] namespaces = messageType.Split('.');

        if (namespaces.Length > 1 && namespaces[0] == "RosMessageTypes")
            return namespaces[namespaces.Length - 1];
        else
            return messageType;
    }
}
