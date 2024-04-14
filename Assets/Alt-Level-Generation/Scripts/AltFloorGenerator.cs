using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
public class AltFloorGenerator : MonoBehaviour
{
    public Vector2Int floorGridSize;
    public int maxRooms;
    public float roomSpacing = 2;
    private int numRooms = 0;
    public RoomSetSO roomSet;
    public NavMeshSurface navMeshSurface;
    public GameObject[] tileAtlasList;
    //public GameObject hallPrefab;

    private FloorGrid floorGrid;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Clear()
    {
        if (floorGrid != null)
        {
            floorGrid.ClearFloor();
        }
    }



    public void Generate()
    {
        Clear();
        numRooms = 0;
        Random.InitState((int)System.DateTime.Now.Ticks);
        floorGrid = new FloorGrid(floorGridSize.x, floorGridSize.y);
        GenerateFloor();
    }

#if UNITY_EDITOR
    public void GenerateFloorEditor()
    {
        Clear();
        numRooms = 0;
        Random.InitState((int)System.DateTime.Now.Ticks);
        floorGrid = new FloorGrid(floorGridSize.x, floorGridSize.y);
        EditorCoroutineUtility.StartCoroutine(RunStartDelayed(), this);
    }

    IEnumerator RunStartDelayed()
    {
        ChooseStartingRoom();
        yield return new WaitForSeconds(0.1f);
        EditorCoroutineUtility.StartCoroutine(RunRoomDelayed(), this);
    }

    private IEnumerator RunRoomDelayed()
    {
        Vector2Int roomCoord = floorGrid.SelectRoomByWeight();

        if (roomCoord.x != -1 && roomCoord.y != -1)
        {
            int randAtlas = Random.Range(0, tileAtlasList.Length);
            floorGrid.FillRoom(roomCoord.x, roomCoord.y, exRoom.Room, roomSet.rooms.GetRandomObject(), tileAtlasList[randAtlas], roomSpacing);

            numRooms++;

            floorGrid.UpdateWeightedRooms(roomCoord);

            yield return new WaitForSeconds(0.1f);

            if (numRooms < maxRooms)
            {
                EditorCoroutineUtility.StartCoroutine(RunRoomDelayed(), this);
            }
            else
            {
                FinalizeRooms();
            }
        }
    }
#endif

    private void GenerateFloor()
    {
        ChooseStartingRoom();
        ChooseRoom();
        ChooseEndRoom();
        FinalizeRooms();
        GenerateNavmesh();
    }

    

    private void ChooseStartingRoom()
    {
        Vector2Int startCoord = floorGrid.SelectRoomByRandom();

        if (startCoord.x != -1 && startCoord.y != -1)
        {
            int randAtlas = Random.Range(0, tileAtlasList.Length);
            floorGrid.FillRoom(startCoord.x, startCoord.y, exRoom.StartRoom, roomSet.startRooms.GetRandomObject(), tileAtlasList[randAtlas], roomSpacing);
            numRooms++;

            floorGrid.UpdateWeightedRooms(startCoord);
        }
        
    }

    private void ChooseRoom()
    {
        Vector2Int roomCoord = floorGrid.SelectRoomByWeight();

        if (roomCoord.x != -1 && roomCoord.y != -1)
        {
            int randAtlas = Random.Range(0, tileAtlasList.Length);
            floorGrid.FillRoom(roomCoord.x, roomCoord.y, exRoom.Room, roomSet.rooms.GetRandomObject(), tileAtlasList[randAtlas], roomSpacing);

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
        int randAtlas = Random.Range(0, tileAtlasList.Length);
        floorGrid.ReplaceRoom(endCoord, roomSet.endRooms.GetRandomObject(), tileAtlasList[randAtlas], exRoom.EndRoom);
    }

    private void FinalizeRooms()
    {
        floorGrid.FinalizeRooms();
    }

    private void GenerateNavmesh()
    {
        navMeshSurface.BuildNavMesh();
    }


}
