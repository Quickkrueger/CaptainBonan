using UnityEngine;

public class SpawnerTile : MonoBehaviour
{
    [SerializeField]
    bool randomizable = false;
    [SerializeField]
    GameObject[] spawnSet;
    [SerializeField]
    SpawnGroup spawnGroup;
    [SerializeField]
    Transform spawnTransform;

    public SpawnGroup p_SpawnGroup { get{ return spawnGroup; } }

    private SpriteRenderer _editorIndicator;

    public bool Randomizable { get { return randomizable; } }

    private void Awake()
    {
        _editorIndicator = GetComponent<SpriteRenderer>();

        Destroy(_editorIndicator);
    }
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
            GameObject spawnedObject = Instantiate(spawnSet[rand], spawnTransform.position, spawnTransform.rotation);
            if (transform.parent != null)
            {
                spawnedObject.transform.parent = transform.parent;
            }
        }
    }
}

[System.Flags]
public enum SpawnGroup
{
    None = 0x0,
    Enemies = 0x1,
    Loot = 0x2,
    Traps = 0x4
}
