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

    public bool Randomizable { get { return randomizable; } }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Spawn()
    {
        int rand = UnityEngine.Random.Range(0, spawnSet.Length);
        Instantiate(spawnSet[rand], transform.position, transform.rotation);
    }
}

[Flags]
public enum SpawnGroup
{
    None = 0,
    Enemies = 1 << 0,
    Loot = 1 << 1,
    Traps = 1 << 2
}
