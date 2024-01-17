using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnStartup : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject);
    }
}
