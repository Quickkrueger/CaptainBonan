using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomSet", menuName = "ScriptableObjects/LevelGenerator/RoomSet", order = 1)]
public class RoomSetSO : ScriptableObject
{
    public AssetList startRooms;
    public AssetList rooms;
    public AssetList endRooms;
}
