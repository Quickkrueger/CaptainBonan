using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChanger : MonoBehaviour
{
    [SerializeField]
    private GameObject alternateTile;
    
    public void SwapToAlternate()
    {
        Instantiate(alternateTile, transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }
}
