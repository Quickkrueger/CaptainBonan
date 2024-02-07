using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChanger : MonoBehaviour
{

    private MeshFilter _tileMesh;
    private MeshCollider _tileCollider;
    [SerializeField]
    private Mesh alternateTile;

    [SerializeField]
    private Direction direction;

    public Direction Direction {  get { return direction; } }

    private void Awake()
    {
        _tileMesh = GetComponent<MeshFilter>();
        _tileCollider = GetComponent<MeshCollider>();
        if(direction == Direction.None)
        {
            SwapToAlternate();
        }
    }


    public void SwapToAlternate()
    {
        if (alternateTile != null)
        {
            _tileMesh.mesh = alternateTile;
            _tileCollider.sharedMesh = alternateTile;
        }
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