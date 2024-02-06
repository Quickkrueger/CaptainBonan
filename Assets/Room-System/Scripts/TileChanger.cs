using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChanger : MonoBehaviour
{

    private MeshFilter _tileMesh;
    [SerializeField]
    private Mesh alternateTile;

    [SerializeField]
    private Direction direction;

    public Direction Direction {  get { return direction; } }

    private void Start()
    {
        _tileMesh = GetComponent<MeshFilter>();
        if(direction == Direction.None)
        {
            SwapToAlternate();
        }
    }


    public void SwapToAlternate()
    {
        _tileMesh.mesh = alternateTile;
    }
}

public enum Direction
{
    North,
    South,
    East,
    West,
    None
}