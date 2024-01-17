using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            _virtualCamera.MoveToTopOfPrioritySubqueue();
        }
    }
}
