using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerComponent : MonoBehaviour
{
    public GameObject objectToSpawn;
    public void SpawnObject(Vector3 position)
    {
        if(objectToSpawn == null)
        {
            Debug.Log($"No Object Set to Spawn on {gameObject.name}");
            return;
        }

            Instantiate(objectToSpawn, position, transform.rotation);
        
    }

    public void SpawnObject()
    {
        if (objectToSpawn == null)
        {
            Debug.Log($"No Object Set to Spawn on {gameObject.name}");
            return;
        }

        Instantiate(objectToSpawn, transform.position, transform.rotation);

    }
}
