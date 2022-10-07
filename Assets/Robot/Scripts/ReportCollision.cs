using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that can be used to report collision/trigger events.
/// Add functions that subscribe to the desired collision/trigger event of this component to be called.
/// </summary>
public class ReportCollision : MonoBehaviour
{
    [System.Serializable]
    public class CollisionEvent : UnityEngine.Events.UnityEvent<Collision> {}
    [System.Serializable]
    public class TriggerEvent : UnityEngine.Events.UnityEvent<Collider> {}

    public CollisionEvent onCollisionEnterEvent = new CollisionEvent();
    public CollisionEvent onCollisionStayEvent = new CollisionEvent();
    public CollisionEvent onCollisionExitEvent = new CollisionEvent();

    public TriggerEvent onTriggerEnterEvent = new TriggerEvent();
    public TriggerEvent onTriggerStayEvent = new TriggerEvent();
    public TriggerEvent onTriggerExitEvent = new TriggerEvent();

    private void OnCollisionEnter(Collision other)
    {
        onCollisionEnterEvent.Invoke(other);
    }

    private void OnCollisionStay(Collision other)
    {
        onCollisionStayEvent.Invoke(other);
    }

    private void OnCollisionExit(Collision other)
    {
        onCollisionExitEvent.Invoke(other);
    }

    void OnTriggerEnter(Collider other)
    {
        onTriggerEnterEvent.Invoke(other);
    }

    void OnTriggerStay(Collider other)
    {
        onTriggerStayEvent.Invoke(other);
    }

    void OnTriggerExit(Collider other)
    {
        onTriggerExitEvent.Invoke(other);
    }
}
