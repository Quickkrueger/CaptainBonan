using System.Collections.Generic;
using UnityEngine;

public class FloorGrid
{
    private RoomData[,] floorRooms;
    List<RoomData> weightedRooms;
    List <RoomData> occupiedRooms;

    public FloorGrid(int gridWidth, int gridHeight)
    {
        floorRooms = new RoomData[gridWidth, gridHeight];

        weightedRooms = new List<RoomData>();
        occupiedRooms = new List<RoomData>();

        SetupRooms();

        EstablishNeighbors();
        
    }

    private void SetupRooms()
    {
        for (int i = 0; i < floorRooms.GetLength(0); i++)
        {
            for (int j = 0; j < floorRooms.GetLength(1); j++)
            {
                floorRooms[i, j] = new RoomData(i, j);
            }
        }
    }

    private void EstablishNeighbors()
    {
        for (int i = 0; i < floorRooms.GetLength(0); i++)
        {
            for (int j = 0; j < floorRooms.GetLength(1); j++)
            {
                RoomData north = null;
                RoomData south = null;
                RoomData east = null;
                RoomData west = null;
                RoomData northEast = null;
                RoomData northWest = null;
                RoomData southEast = null;
                RoomData southWest = null;

                if (j > 0)
                {
                    north = floorRooms[i, j - 1];

                    if (i < floorRooms.GetLength(0) - 1)
                    {
                        northWest = floorRooms[i + 1, j - 1];
                    }

                    if (i > 0)
                    {
                        northEast = floorRooms[i - 1, j - 1];
                    }
                }

                if (j < floorRooms.GetLength(1) - 1)
                {
                    south = floorRooms[i, j + 1];

                    if (i < floorRooms.GetLength(0) - 1)
                    {
                        southWest = floorRooms[i + 1, j + 1];
                    }

                    if (i > 0)
                    {
                        southEast = floorRooms[i - 1, j + 1];
                    }
                }

                if (i > 0)
                {
                    east = floorRooms[i - 1, j];
                }

                if (i < floorRooms.GetLength(0) - 1)
                {
                    west = floorRooms[i + 1, j];
                }

                floorRooms[i, j].SetNeighbors(north, south, east, west, northWest, northEast, southWest, southEast);

            }
        }
    }

    public void ClearFloor()
    {
        SetupRooms();
        EstablishNeighbors();
    }

    public RoomData GetRoom(int xCoord, int zCoord)
    {
        return floorRooms[xCoord, zCoord];
    }

    public void FillRoom(int xCoord, int zCoord, exRoom roomType, GameObject gameObject, float roomSpacing)
    {
        floorRooms[xCoord, zCoord].OccupyRoom(gameObject, roomType, roomSpacing, (float)floorRooms.GetLength(0), (float)floorRooms.GetLength(1));
        occupiedRooms.Add(floorRooms[xCoord, zCoord]);
    }

    public void UpdateWeightedRooms(Vector2Int activeCoord)
    {
        RoomData activeRoom = floorRooms[activeCoord.x, activeCoord.y];
        for (int i = 0; i < activeRoom.GetNumNeighbors(); i++)
        {
            if (!weightedRooms.Contains(activeRoom.GetNeighbor(i)) && activeRoom.GetNeighbor(i) != null && activeRoom.GetNeighbor(i).Weight != 0)
            {
                weightedRooms.Add(activeRoom.GetNeighbor(i));
            }

            if (weightedRooms.Contains(activeRoom))
            {
                weightedRooms.Remove(activeRoom);
            }
        }
    }

    public Vector2Int SelectRoomByRandom()
    {
        int randX = Random.Range(floorRooms.GetLength(0) / 4, (floorRooms.GetLength(0) * 3) / 4);
        int randZ = Random.Range(floorRooms.GetLength(1) / 4, (floorRooms.GetLength(1) * 3) / 4);

        if(!floorRooms[randX, randZ].IsOccupied)
        {
            return new Vector2Int(randX, randZ);
        }

        return new Vector2Int(-1, -1);
    }

    public Vector2Int SelectRoomByWeight()
    {
        float floorWeight = 0;
        float currentWeight = 0;
        float rand = Random.Range(0, 1.0f);

        
        for(int i = 0; i < weightedRooms.Count; i++)
        {
            floorWeight += (float)weightedRooms[i].Weight;
        }

        for (int i = 0; i < weightedRooms.Count; i++)
        {
            currentWeight += (float)weightedRooms[i].Weight;

            if (currentWeight/floorWeight >= rand)
            {
                return weightedRooms[i].GridCoordinate;
            }
         }

        return new Vector2Int(-1, -1);

    }

    public Vector2Int SelectEndRoomByRandom()
    {
        List<RoomData> deadEnds = new List<RoomData>();

        foreach(RoomData room in occupiedRooms)
        {
            if (room.GetNumOccupiedNeighbors() <= 2)
            {
                deadEnds.Add(room);
            }
        }

        while (true)
        {
            int randIndex = Random.Range(0, deadEnds.Count);

            if (deadEnds[randIndex].RoomType != exRoom.StartRoom && Vector2Int.Distance(deadEnds[randIndex].GridCoordinate, GetStartRoom()) >= Mathf.Sqrt(occupiedRooms.Count) / 2)
            {
                return deadEnds[randIndex].GridCoordinate;
            }
        }
        return new Vector2Int(-1, -1);
    }

    public void ReplaceRoom(Vector2Int roomCoord, GameObject newRoomObject, exRoom roomType)
    {
        floorRooms[roomCoord.x, roomCoord.y].SwapRoom(newRoomObject, roomType);
    }

    public Vector2Int GetStartRoom()
    {
        foreach(RoomData room in occupiedRooms)
        {
            if(room.RoomType == exRoom.StartRoom)
            {
                return room.GridCoordinate;
            }
        }

        return new Vector2Int(-1, -1);
    }

    public bool CheckForTileNeighbor(int xCoord, int zCoord, int index)
    {
        return floorRooms[xCoord, zCoord].CheckForNeighbor(index);
    }
    
}
