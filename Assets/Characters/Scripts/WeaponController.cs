using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    // Start is called before the first frame update
    public UnityEvent WeaponAction;

    public void UseWeapon()
    {
        if (WeaponAction != null)
        {
            WeaponAction.Invoke();
        }
    }
}
