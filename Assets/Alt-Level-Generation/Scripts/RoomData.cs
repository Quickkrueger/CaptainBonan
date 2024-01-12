using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class RoomData
{
    private Vector2Int gridCoordinate;
    private bool occupied = false;
    private Neighbors neighbors;
    private int weight = 0;
    private exRoom roomType = exRoom.Room;
    private GameObject roomObject;


    public Vector2Int GridCoordinate
    {
        get { return gridCoordinate; }
    }
    public bool IsOccupied { get { return occupied; } }
    public int Weight { get { return weight; } }
    public exRoom RoomType { get { return roomType; } }

    public RoomData(int xCoord, int zCoord)
    {
        weight = 0;
        occupied = false;
        neighbors = new Neighbors();
        gridCoordinate = new Vector2Int(xCoord, zCoord);
    }

    public bool SetNeighbors(RoomData north, RoomData south, RoomData east, RoomData west, RoomData northWest, RoomData northEast, RoomData southWest, RoomData southEast)
    {
        neighbors.SetNeighbors(north, south, east, west, northWest, northEast, southWest, southEast);

        return false;
    }

    public bool OccupyRoom(GameObject gameObject, exRoom roomType, float roomSpacing, float floorX, float floorZ)
    {
        this.roomType = roomType;
        occupied = true;
        weight = 0;
        roomObject = GameObject.Instantiate(gameObject, new Vector3(((float)gridCoordinate.x * roomSpacing) - (floorX / 2f), 0f, (float)gridCoordinate.y * roomSpacing - (floorZ / 2f)), Quaternion.identity);
        UpdateNeighbors();


        return false;
    }

    public bool SwapRoom(GameObject newGameObject, exRoom roomType)
    {
        GameObject temp = roomObject;
        roomObject = GameObject.Instantiate(newGameObject, temp.transform.position, temp.transform.rotation);
        GameObject.Destroy(temp);
        return false;
    }



    public bool UpdateWeight()
    {
        int occupiedNeighbors = neighbors.GetNumberOfOccupiedNeighbors();

        if(occupiedNeighbors > 0 && !occupied)
        {
            weight = (int)Mathf.Pow(neighbors.size - (float)Mathf.Clamp((occupiedNeighbors), 0, neighbors.size), 2);
            return true;
        }
        else
        {
            weight = 0;
            return true;
        }

        return false;
    }

    public bool UpdateNeighbors()
    {
        for(int i = 0; i < neighbors.size / 2; i++)
        {
            if (neighbors.GetNeighborByIndex(i) != null)
            {
                neighbors.GetNeighborByIndex(i).UpdateWeight();
            }
        }

        return false;
    }

    public bool CheckForNeighbor(int index)
    {
        if(occupied && neighbors.GetNeighborByIndex(index) != null && neighbors.GetNeighborByIndex(index).IsOccupied)
        {
            return true;
        }
        return false;
    }

    public RoomData GetNeighbor(int index)
    {
        return neighbors.GetNeighborByIndex(index);
    }

    public int GetNumNeighbors()
    {
        return neighbors.size;
    }

    public int GetNumOccupiedNeighbors()
    {
        return neighbors.GetNumberOfOccupiedNeighbors();
    }


}

public struct Neighbors
{
    private RoomData north;
    private RoomData south;
    private RoomData east;
    private RoomData west;
    private RoomData northWest;
    private RoomData southEast;
    private RoomData northEast;
    private RoomData southWest;
    public int size;

    public bool SetNeighbors(RoomData north, RoomData south, RoomData east, RoomData west, RoomData northWest, RoomData northEast, RoomData southWest, RoomData southEast)
    {
        this.north = north;
        this.south = south;
        this.east = east;
        this.west = west;
        this.northEast = northEast;
        this.northWest = northWest;
        this.southEast = southEast;
        this.southWest = southWest;

        size = 8;

        return false;
    }

    public int GetNumberOfOccupiedNeighbors()
    {
        int numOccupied = 0;

        for(int i = 0; i < size; i++)
        {
            if (GetNeighborByIndex(i) != null && GetNeighborByIndex(i).IsOccupied)
            {
                numOccupied++;
            }
        }

        return numOccupied;
    }

    public RoomData GetNeighborByIndex(int index)
    {
        switch (index)
        {
            case 0: return north;
            case 1: return south;
            case 2: return east;
            case 3: return west;
            case 4: return northEast;
            case 5: return northWest;
            case 6: return southEast;
            case 7: return southWest;
            default: return null;
        }
    }
}
