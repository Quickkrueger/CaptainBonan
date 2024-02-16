using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    bool randomizable = false;
    [SerializeField]
    GameObject[] spawnSet;
    [SerializeField]
    SpawnGroup spawnGroup;

    public SpawnGroup p_SpawnGroup { get{ return spawnGroup; } }

    public bool Randomizable { get { return randomizable; } }
    // Start is called before the first frame update
    void Start()
    {
        if (!randomizable)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        if (spawnSet.Length > 0)
        {
            int rand = UnityEngine.Random.Range(0, spawnSet.Length);
            Instantiate(spawnSet[rand], transform.position, transform.rotation);
        }
        //Destroy(gameObject);
    }
}

[Flags]
public enum SpawnGroup
{
    None = 0x0,
    Enemies = 0x1,
    Loot = 0x2,
    Traps = 0x4
}
