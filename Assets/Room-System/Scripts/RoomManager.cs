using Cinemachine;
using System;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    private bool _isStartRoom;

    private TileChanger[] _tileChangers;
    private Spawner[] _spawners;
    private SpawnGroup _roomGroup;

    private void Awake()
    {
        _tileChangers = GetComponentsInChildren<TileChanger>();
        _spawners = GetComponentsInChildren<Spawner>();

        Array spawnGroups = Enum.GetValues(typeof(SpawnGroup));
        for(int i = 0; i < spawnGroups.Length; i++)
        {
            if ((SpawnGroup)spawnGroups.GetValue(i) != SpawnGroup.None) 
            {
                _roomGroup |= (SpawnGroup)spawnGroups.GetValue(i);
            }
        }

        int roomGroupValue = (int) _roomGroup;
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        roomGroupValue = UnityEngine.Random.Range(0, roomGroupValue + 1);
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
            _virtualCamera.MoveToTopOfPrioritySubqueue();
        }
    }

    public void SetUpRoom(RoomData roomData)
    {
        SetEntrances(roomData);
        SetSpawners();
    }

    private void SetEntrances(RoomData roomData)
    {
        for (int i = 0; i < _tileChangers.Length; i++)
        {
            if (_tileChangers[i].Direction != Direction.None && !roomData.CheckForNeighbor((int)_tileChangers[i].Direction))
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


}
