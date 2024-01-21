using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

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
}
