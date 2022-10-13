using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LowLevelController))]
public class KeyboardTeleoperation : MonoBehaviour
{
    public float linearVelocity = 1.0f;
    public float angularVelocity = 3.0f;
    
    private LowLevelController _controller;

    void Start()
    {
        _controller = GetComponent<LowLevelController>();
    }

    void FixedUpdate()
    {
        
        _controller.LinearVelocity = linearVelocity * Input.GetAxis("Vertical");
        _controller.AngularVelocity = -angularVelocity * Input.GetAxis("Horizontal");
    }
}
