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
        _tileMesh = GetComponent<MeshFilter>();
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
            transform.eulerAngles += newRotation;
        }
        else
        {
            Destroy(gameObject);
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