using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
    public UnityEvent<Collider> colliderDetected;
    private void OnTriggerEnter(Collider other)
    {
        colliderDetected.Invoke(other);
    }
}
