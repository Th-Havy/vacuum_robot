using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackRobot : MonoBehaviour
{
    public Transform robot;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(robot);
    }
}
