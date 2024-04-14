using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnEvent : MonoBehaviour
{
    [SerializeField]
    int damageValue = 1;
    [SerializeField]
    LayerMask validLayers;

    private List<Collider> targets;

    private void Awake()
    {
        targets = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!targets.Contains(other) && validLayers == (validLayers | 1 << other.gameObject.layer))
        {
            targets.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targets.Contains(other))
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
        for (int i = targets.Count - 1; i >= 0; i--)
        {

            if (targets[i] == null)
            {
                targets.RemoveAt(i);
                continue;
            }

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
