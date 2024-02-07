using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    GameObject toSpawn;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(toSpawn, transform.position, transform.rotation);
    }
}
