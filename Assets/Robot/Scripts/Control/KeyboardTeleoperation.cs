using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LowLevelController))]
public class KeyboardTeleoperation : MonoBehaviour
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
    private BaseCollisionSensor _collisionSensor;

    private State _state = State.Forward;
    private float _turnAngle = 0f;

    void Start()
    {
        _controller = GetComponent<LowLevelController>();
        _collisionSensor = GetComponentInChildren<BaseCollisionSensor>(true);
        _collisionSensor.publisher.onNewMessage.AddListener(HandleBaseCollision);
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

    }
}
