using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    private bool _isStartRoom;

    private TileChanger[] tileChangers;

    private void Awake()
    {
        tileChangers = GetComponentsInChildren<TileChanger>();
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
    }

    private void SetEntrances(RoomData roomData)
    {
        for (int i = 0; i < tileChangers.Length; i++)
        {
            if (tileChangers[i].Direction != Direction.None && !roomData.CheckForNeighbor((int)tileChangers[i].Direction))
            {
                tileChangers[i].SwapToAlternate();
            }
        }
    }


}
