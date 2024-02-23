using UnityEngine;

public class AltFloorGenerator : MonoBehaviour
{
    public Vector2Int floorGridSize;
    public int maxRooms;
    public float roomSpacing = 2;
    private int numRooms = 0;
    public RoomSetSO roomSet;
    //public GameObject hallPrefab;

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
        ChooseEndRoom();
        FinalizeRooms();
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

    private void FinalizeRooms()
    {
        floorGrid.FinalizeRooms();
    }


}
