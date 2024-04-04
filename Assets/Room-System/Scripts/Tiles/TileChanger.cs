using UnityEngine;

public class TileChanger : MonoBehaviour
{

    private MeshFilter _tileMesh;
    [SerializeField]
    public Mesh alternateTile;
    [SerializeField]
    public Direction direction;
    [SerializeField]
    public Vector3 newRotation;


    private void Awake()
    {
        
        if(direction == Direction.None)
        {
            _tileMesh = GetComponent<MeshFilter>();
            SwapToAlternate();
        }
    }


    public void SwapToAlternate()
    {
        _tileMesh = GetComponent<MeshFilter>();

        if (alternateTile != null)
        {
            _tileMesh.mesh = alternateTile;
            transform.eulerAngles += newRotation;
        }
        else
        {
            DestroyImmediate(gameObject);
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