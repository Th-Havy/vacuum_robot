using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vacuum : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Area where forces are applied to attract objects.")]
    private Collider attractionArea;
    [SerializeField]
    [Tooltip("Area where sucked objects are destroyed.")]
    private Collider hole;

    [Tooltip("Object with this tag will be attracted/destroyed.")]
    public string attractedObjectsTag;

    public float attractionForce = 10f;

    void Start()
    {
        ReportCollision areaReporter = attractionArea.gameObject.AddComponent<ReportCollision>();
        areaReporter.onTriggerEnterEvent.AddListener(AddAttractionForce);
        areaReporter.onTriggerStayEvent.AddListener(AddAttractionForce);

        ReportCollision holeReporter = hole.gameObject.AddComponent<ReportCollision>();
        holeReporter.onTriggerEnterEvent.AddListener(DestroyObject);
    }

    private void AddAttractionForce(Collider other)
    {
        if (other.CompareTag(attractedObjectsTag))
        {
            Vector3 direction = (attractionArea.transform.position - other.transform.position).normalized;
            // Since the object to which this script is attached has no rigidbody, then
            // the triggering object has to have one attached, hence no null checks are done.
            other.attachedRigidbody.AddForce(attractionForce * direction, ForceMode.Acceleration);
        }
    }

    private void DestroyObject(Collider other)
    {
        if (other.CompareTag(attractedObjectsTag))
        {
            Destroy(other.gameObject);
        }
    }
}
