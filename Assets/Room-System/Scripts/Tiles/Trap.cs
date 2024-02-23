using UnityEngine;
using UnityEngine.Events;

public class Trap : MonoBehaviour
{
    [SerializeField]
    UnityEvent<Collider> trapTriggered;

    bool trapEnabled = false;


    public void EnableTrap()
    {
        trapEnabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (trapTriggered != null)
        {
            trapTriggered.Invoke(other);
        }
    }
}
