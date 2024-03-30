using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnEnter : MonoBehaviour
{
    [SerializeField]
    int damageValue = 1;
    [SerializeField]
    LayerMask validLayers;
    [SerializeField]
    bool captureTriggerEvents;

    private List<Collider> targets;

    private void Awake()
    {
        targets = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!targets.Contains(other) && captureTriggerEvents)
        {
            targets.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targets.Contains(other) && captureTriggerEvents)
        {
            targets.Remove(other);
        }
    }

    public void InitiateDamage()
    {
        DamageOther();
    }

    public void DamageOther(Collider other = null)
    {
        IDamageable component;
        for (int i = 0; i < targets.Count; i++)
        {
            if (validLayers == (validLayers | 1 << targets[i].gameObject.layer) && targets[i].TryGetComponent(out component))
            {
                component.TakeDamage(damageValue);
            }
        }

        if(other != null &&!targets.Contains(other) && validLayers == (validLayers | 1 << other.gameObject.layer) && other.TryGetComponent(out component))
        {
            component.TakeDamage(damageValue);
        }
    }
}
