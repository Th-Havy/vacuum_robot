using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCollisionSensor : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Collider which senses the collisions")]
    private Collider baseCollider;

    [SerializeField]
    [Tooltip("Radius of collider which senses the collisions")]
    private float colliderRadius;

    [Header("ROS settings")]
    public ROSPublisher<RosMessageTypes.Std.Float32Msg> publisher = new ROSPublisher<RosMessageTypes.Std.Float32Msg>("collision", true);

    // Necessary to have, otherwise the component cannot be disabled in the inspector
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        FindCollisionPoint(other);
    }

    void OnCollisionStay(Collision other)
    {
        FindCollisionPoint(other);
    }

    /// <summary>
    /// Average collision points, and find closests on the radius of the robot
    /// </summary>
    void FindCollisionPoint(Collision other)
    {
        Vector3 averageContactPoint = Vector3.zero;
        foreach (ContactPoint contact in other.contacts)
        {
            averageContactPoint += contact.point;
        }
        averageContactPoint /= other.contacts.Length;

        Vector3 averageInBaseFrame = transform.InverseTransformPoint(averageContactPoint);        
        Vector3 averageOnHorizontalPlane = Vector3.ProjectOnPlane(averageInBaseFrame, Vector3.up);

        // Collisions should only be detected on the side of the cylinder collider of the base
        float radiusTolerance = 0.01f;
        if (Mathf.Abs(averageOnHorizontalPlane.magnitude - colliderRadius) < radiusTolerance)
        {
            // Angle between front of base and collision point
            float collisionAngle = -Vector3.SignedAngle(Vector3.forward, averageOnHorizontalPlane, Vector3.up);
            RosMessageTypes.Std.Float32Msg message = CreateMessage(collisionAngle);

            // TODO: Create proper visualization component
            Debug.DrawRay(transform.TransformPoint(averageOnHorizontalPlane.normalized * colliderRadius), transform.TransformDirection(averageOnHorizontalPlane.normalized) * 0.1f, Color.red);

            publisher.Publish(message);
        }
    }

    private RosMessageTypes.Std.Float32Msg CreateMessage(float angle)
    {
        RosMessageTypes.Std.Float32Msg message = new RosMessageTypes.Std.Float32Msg();
        
        message.data = angle;

        return message;        
    }
}
