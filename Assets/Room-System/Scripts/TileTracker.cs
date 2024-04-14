#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
public class TileTracker : MonoBehaviour
{
    private Dictionary<Vector3Int, Transform> childPositions = new Dictionary<Vector3Int, Transform>();

    private Vector3 cellSize;
    private Vector3 cellStride;
    private Vector3 anchorRatio;


    private void OnValidate()
    {
        childPositions.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            childPositions.Add(Vector3Int.FloorToInt(child.position), child);
        }
    }

    public Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position, Vector3 anchor)
    {

        Vector3Int iWorldPos = SetPositionData(grid, anchor, position);

        if (childPositions.ContainsKey(iWorldPos) && childPositions[iWorldPos] != null)
        {
            return childPositions[iWorldPos];
        }
        else if (childPositions.ContainsKey(iWorldPos))
        {
            childPositions.Remove(iWorldPos);
        }

        return null;
    }

    public bool AddChild(GridLayout grid, Transform child, Vector3Int position, Vector3 anchor)
    {
        Vector3Int iWorldPos = SetPositionData(grid, anchor, position);

        childPositions.Add(iWorldPos, child);
        return true;
    }

    public bool RemoveChild(GridLayout grid, Vector3Int position, Vector3 anchor)
    {
        Vector3Int iWorldPos = SetPositionData(grid, anchor, position);

        childPositions.Remove(iWorldPos);
        return true;
    }

    private Vector3Int SetPositionData(GridLayout grid, Vector3 anchor, Vector3Int position)
    {
        cellSize = grid.cellSize;
        cellStride = cellSize + grid.cellGap;

        cellStride.x = Mathf.Approximately(0f, cellStride.x) ? 1f : cellStride.x;
        cellStride.y = Mathf.Approximately(0f, cellStride.y) ? 1f : cellStride.y;
        cellStride.z = Mathf.Approximately(0f, cellStride.z) ? 1f : cellStride.z;
        anchorRatio = new Vector3(
            anchor.x * cellSize.x / cellStride.x,
            anchor.y * cellSize.y / cellStride.y,
            anchor.z * cellSize.z / cellStride.z
        );

        Vector3 worldPos = grid.LocalToWorld(grid.CellToLocalInterpolated(position + anchorRatio));

        Vector3Int iWorldPos = Vector3Int.FloorToInt(worldPos);

        return iWorldPos;
    }
}

#endif
