using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

[RequireComponent(typeof(LowLevelController))]
public class RandomDirectionController : MonoBehaviour
{
    /// <summary>
    /// The robot goes forward until a collision occurs, then goes backward for a bit
    /// and finally turn at a random angle before going forward again.
    /// </summary>
    enum State
    {
        Forward,
        Backward,
        Turn
    }
    public float linearVelocity = 0.4f;
    public float angularVelocity = 1.0f;
    public float backwardDistance = 0.1f;

    private LowLevelController _controller;

    private State _state = State.Forward;

    [Header("ROS settings")]
    private ROSConnection _ros;
    [SerializeField]
    private string topicName = "base_collision";

    // Necessary to have, otherwise the component cannot be disabled in the inspector
    void Start()
    {
        // start the ROS connection
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.Subscribe<RosMessageTypes.Std.Float32Msg>(topicName, HandleBaseCollision);

        _controller = GetComponent<LowLevelController>();
    }

    void FixedUpdate()
    {
        // TODO: read fall prevention sensors to stop forward/backward motion
        // or keep turning until there is no more fall in front
        switch (_state)
        {
            case State.Forward:
                _controller.LinearVelocity = linearVelocity;
                _controller.AngularVelocity = 0f;
                break;
            case State.Backward:
                _controller.LinearVelocity = -linearVelocity;
                _controller.AngularVelocity = 0f;
                break;
            case State.Turn:
                _controller.LinearVelocity = 0f;
                _controller.AngularVelocity = angularVelocity;
                break;
            default:
                break;
        }
    }

    void HandleBaseCollision(RosMessageTypes.Std.Float32Msg message)
    {
        Debug.Log("Collision angle: " + message.data.ToString());
        _state = State.Backward;
    }
}
