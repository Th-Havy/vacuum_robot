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

    private bool _collisionOccured = false;
    private float _collisionTime = 0f;
    private float _collisionAngle = 0f;
    private float _turnTime = 0f;
    private float _targetAngle = 0f;

    [Header("ROS settings")]
    private ROSConnection _ros;
    [SerializeField]
    private string baseCollisionTopicName = "base_collision";

    // Necessary to have, otherwise the component cannot be disabled in the inspector
    void Start()
    {
        // start the ROS connection
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.Subscribe<RosMessageTypes.Std.Float32Msg>(baseCollisionTopicName, HandleBaseCollision);

        _controller = GetComponent<LowLevelController>();
    }

    // Update the state machine
    void Update() 
    {
        switch (_state)
        {
            case State.Forward:
                if (_collisionOccured)
                {
                    _collisionOccured = false;
                    _state = State.Backward;
                }
                break;
            case State.Backward:
                // Switch to turn states
                if ((Time.time - _collisionTime) * linearVelocity > backwardDistance)
                {
                    _targetAngle = Random.Range(-Mathf.PI, Mathf.PI);

                    // Switch to turn state
                    _turnTime = Time.time;
                    _state = State.Turn;
                }
                break;
            case State.Turn:
                // Switch back to forward state
                Debug.Log("Conditions " + ((Time.time - _turnTime) * angularVelocity).ToString() + " " + _targetAngle);
                if ((Time.time - _turnTime) * angularVelocity > Mathf.Abs(_targetAngle))
                {
                    Debug.Log("Switching to forward state");
                    _state = State.Forward;
                }
                break;
        }

        UpdateLowLevelCommands();
    }

    void UpdateLowLevelCommands()
    {
        // TODO: read fall prevention sensors to stop forward/backward motion
        // or keep turning until there is no more fall in front

        Debug.Log(_state.ToString() + _targetAngle.ToString());
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
                _controller.AngularVelocity = Mathf.Sign(_targetAngle) * angularVelocity;
                break;
            default:
                break;
        }
    }

    void HandleBaseCollision(RosMessageTypes.Std.Float32Msg message)
    {
        if (_state == State.Forward)
        {
            _collisionOccured = true;
            _collisionTime = Time.time;
            _collisionAngle = message.data;
        }
    }
}
