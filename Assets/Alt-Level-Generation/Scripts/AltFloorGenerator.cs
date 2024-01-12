using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltFloorGenerator : MonoBehaviour
{
    public Vector2Int floorGridSize;
    public int maxRooms;
    public float roomSpacing = 2;
    private int numRooms = 0;
    public RoomSetSO roomSet;
    public GameObject hallPrefab;

    private FloorGrid floorGrid;
    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        floorGrid = new FloorGrid(floorGridSize.x, floorGridSize.y);
        GenerateFloor();
    }

    private void GenerateFloor()
    {
        ChooseStartingRoom();
        ChooseRoom();
        MakeHalls();
        ChooseEndRoom();
    }

    private void ChooseStartingRoom()
    {
        Vector2Int startCoord = floorGrid.SelectRoomByRandom();

        if (startCoord.x != -1 && startCoord.y != -1)
        {
            floorGrid.FillRoom(startCoord.x, startCoord.y, exRoom.StartRoom, roomSet.startRooms.GetRandomObject(), roomSpacing);
            numRooms++;

            floorGrid.UpdateWeightedRooms(startCoord);
        }
        
    }

    private void ChooseRoom()
    {
        Vector2Int roomCoord = floorGrid.SelectRoomByWeight();

        if (roomCoord.x != -1 && roomCoord.y != -1)
        {
            floorGrid.FillRoom(roomCoord.x, roomCoord.y, exRoom.Room, roomSet.rooms.GetRandomObject(), roomSpacing);

            numRooms++;

            floorGrid.UpdateWeightedRooms(roomCoord);

            if (numRooms < maxRooms)
            {
                ChooseRoom();
            }
        }
    }

    private void ChooseEndRoom()
    {
        Vector2Int endCoord = floorGrid.SelectEndRoomByRandom();
        floorGrid.ReplaceRoom(endCoord, roomSet.endRooms.GetRandomObject(), exRoom.EndRoom);
    }

    private void MakeHalls()
    {
        for(int i = 0; i < floorGridSize.x; i++)
        {
            for(int j = 0; j < floorGridSize.y; j++)
            {

                if(floorGrid.CheckForTileNeighbor(i, j, 1))
                {
                    Instantiate(hallPrefab, new Vector3(i * roomSpacing - ((float)floorGridSize.x / 2f), 0, (j * roomSpacing) + (roomSpacing / 2) - ((float)floorGridSize.y / 2f)), Quaternion.Euler(new Vector3(90, 0, 0)));
                }

                if (floorGrid.CheckForTileNeighbor(i, j, 3))
                {
                    Instantiate(hallPrefab, new Vector3((i * roomSpacing) + (roomSpacing / 2) - ((float)floorGridSize.x / 2f), 0, j * roomSpacing - ((float)floorGridSize.y / 2f)), Quaternion.Euler(new Vector3(0, 0, 90)));
                }

            }
        }
    }

}
