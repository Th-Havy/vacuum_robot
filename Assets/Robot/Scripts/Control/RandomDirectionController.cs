using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LowLevelController))]
public class RandomDirectionController : MonoBehaviour
{
    public float linearVelocity = 0.4f;
    public float angularVelocity = 1.0f;
    
    private LowLevelController _controller;

    void Start()
    {
        _controller = GetComponent<LowLevelController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
