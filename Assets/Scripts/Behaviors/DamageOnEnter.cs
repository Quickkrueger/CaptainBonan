using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnEnter : MonoBehaviour
{
    [SerializeField]
    float damageValue = 1;
    
    public void DamageOther(Collider other)
    {
        if(TryGetComponent(out IDamageable component))
        {
            component.TakeDamage(damageValue);
        }
    }
}
