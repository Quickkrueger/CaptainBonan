using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnCollide : MonoBehaviour
{
    public UnityEvent<Vector3> collideEvent;

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPosition = collision.GetContact(0).point;

        collideEvent.Invoke(hitPosition);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        RaycastHit hit;
        Vector3 hitPosition = transform.position;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
        {
            hitPosition = hit.point;
        }

        collideEvent.Invoke(hitPosition);
        Destroy(gameObject);

    }
}
