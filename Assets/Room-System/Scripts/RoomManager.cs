using Cinemachine;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    private bool _isStartRoom;

    private bool activated = false;
    private TileChanger[] _tileChangers;
    private Spawner[] _spawners;
    private Trap[] _traps;
    private MeshFilter[] _tiles;
    private SpawnGroup _roomGroup;

    private void Awake()
    {
        _tileChangers = GetComponentsInChildren<TileChanger>();
        _spawners = GetComponentsInChildren<Spawner>();

        GameObject tileParent = null;

       for(int i = 0; i < transform.childCount; i++)
        {
            tileParent = transform.GetChild(i).gameObject;

            if(tileParent.gameObject.name == "Tiles")
            {
                _tiles = tileParent.GetComponentsInChildren<MeshFilter>();
            }
        }

        System.Array spawnGroups = System.Enum.GetValues(typeof(SpawnGroup));
        for(int i = 0; i < spawnGroups.Length; i++)
        {
            if ((SpawnGroup)spawnGroups.GetValue(i) != SpawnGroup.None) 
            {
                _roomGroup |= (SpawnGroup)spawnGroups.GetValue(i);
            }
        }

        int roomGroupValue = (int) _roomGroup;
        Random.InitState(System.DateTime.Now.Millisecond);
        roomGroupValue = Random.Range(0, roomGroupValue + 1);
        _roomGroup = (SpawnGroup)roomGroupValue;
    }

    private void Start()
    {
        if( _isStartRoom)
        {
            _virtualCamera.MoveToTopOfPrioritySubqueue();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ActivateCamera();

            if (!activated)
            {
                activated = true;
                ActivateRoom();
            }
        }
    }

    private void ActivateCamera()
    {
        _virtualCamera.MoveToTopOfPrioritySubqueue();
    }

    private void ActivateRoom()
    {
        

        if(_traps == null || _traps.Length <= 0 )
        {
            _traps = GetComponentsInChildren<Trap>();
        }

        for(int i = 0; i < _traps.Length; i++)
        {
            if (_traps[i] != null)
            {
                _traps[i].EnableTrap();
            }
        }

    }

    public void SetUpRoom(RoomData roomData)
    {
        SetEntrances(roomData);
        SetSpawners();
        SetTileMeshes(roomData);
        GenerateMeshCollider();
    }

    private void SetEntrances(RoomData roomData)
    {
        for (int i = _tileChangers.Length - 1; i >= 0; i--)
        {
            if (_tileChangers[i].direction != Direction.None && !roomData.CheckForNeighbor((int)_tileChangers[i].direction))
            {
                _tileChangers[i].SwapToAlternate();
            }
        }
    }

    private void SetSpawners()
    {
        foreach(Spawner spawner in _spawners)
        {
            if(spawner.Randomizable && _roomGroup.HasFlag(spawner.p_SpawnGroup))
            {
                spawner.Spawn();
            }
            else if (spawner.Randomizable)
            {
                spawner.gameObject.SetActive(false);
            }
        }
    }

    private void SetTileMeshes(RoomData roomData)
    {
        string[] tileName;
        string tileNameJoined;

        string[] meshName;
        string meshNameJoined;

        MeshFilter[] meshFilters = roomData.MeshAtlas.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] == null)
            {
                continue;
            }

            tileNameJoined = "";
            tileName = _tiles[i].sharedMesh.name.Split('_');
            tileName[0] = "";
            tileNameJoined = System.String.Join('_', tileName);

            for(int j = 0; j < meshFilters.Length; j++)
            {
                meshNameJoined = "";
                meshName = meshFilters[j].sharedMesh.name.Split('_');
                meshName[0] = "";
                meshNameJoined = System.String.Join('_', meshName);
                if (meshNameJoined.Equals(tileNameJoined))
                {
                    _tiles[i].sharedMesh = meshFilters[j].sharedMesh;
                    break;
                }
            }
            
        }
    }

    private void GenerateMeshCollider()
    {

        MeshFilter mFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mRenderer = gameObject.AddComponent<MeshRenderer>();

        CombineInstance[] combineInstance = new CombineInstance[_tiles.Length];

        MeshFilter activeFilter = _tiles[0];

        for(int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] != null)
            {
                activeFilter = _tiles[i];
                break;
            }
        }

        mRenderer.material = activeFilter.GetComponent<MeshRenderer>().material;

        Vector3 tempPosition = transform.position;

        transform.position = Vector3.zero;

        for(int i = 0; i < _tiles.Length; ++i)
        {
            if (_tiles[i] == null || _tiles[i].sharedMesh == null)
            {
                continue;
            }
            combineInstance[i].mesh = _tiles[i].sharedMesh;
            combineInstance[i].transform = _tiles[i].transform.localToWorldMatrix;
            Destroy(_tiles[i].gameObject);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstance);
        mFilter.mesh = combinedMesh;

        MeshCollider mCollider = gameObject.AddComponent<MeshCollider>();
        mCollider.sharedMesh = combinedMesh;

        transform.position = tempPosition;
    }


}
