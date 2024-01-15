//Created by: Marshall Krueger
//Last edited by: Marshall Krueger 02/09/23
//Purpose: A 3D Tile brush for our 3D tile system
using UnityEngine.SceneManagement;
using UnityEditor.Tilemaps;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace RoomTools.Brushes
{

[CreateAssetMenu(fileName = "New TileBrush3D", menuName = "3D/Tilemap/TileBrush3D", order = 0)]
[CustomGridBrush(false, true, false, "TileBrush3D")]
public class TileBrush3D : GameObjectBrush 
{

    private BrushCell currentCell;
    [HideInInspector]
    public bool replaceTiles = false;


    /// <summary>
    /// Purpose: Get the current brush cell
    /// </summary>
    /// <returns>the current brush cell</returns>
    public BrushCell GetCurrentCell( )
    {
        return currentCell;
    }

    /// <summary>
    /// Purpose: Set the current brush cell
    /// </summary>
    /// <param name="newCell">a valid brush cell</param>
    public void SetCurrentCell(BrushCell newCell)
    {
        currentCell = newCell;
    }

    /// <summary>
    /// Purpose: Erase tiles from the tilemap
    /// </summary>
    /// <param name="gridLayout">a valid grid or child of a grid</param>
    /// <param name="brushTarget">the gameobject represented location in the grid</param>
    /// <param name="position">the position in the grid</param>
    public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        if(brushTarget.layer == 31)
        {
            return;
        }

        Transform erased = GetObjectInCell(gridLayout, brushTarget.transform, new Vector3Int(position.x, position.y, 0));
        if(erased != null)
        {
            Undo.DestroyObjectImmediate(erased.gameObject);
        }
    }

    
    /// <summary>
    /// Purpose: Begin the "paint" process
    /// </summary>
    /// <param name="gridLayout">a valid grid or child of a grid</param>
    /// <param name="brushTarget">the gameobject represented location in the grid</param>
    /// <param name="position">the position in the grid</param>
    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            Vector3Int min = position - pivot;
            BoundsInt bounds = new BoundsInt(min, size);

            BoxFill(gridLayout, brushTarget, bounds);
        }

        /// <summary>
        /// Purpose: Prepare tiles to be painted
        /// </summary>
        /// <param name="grid">a valid grid or child of a grid</param>
        /// <param name="position">the position in the grid</param>
        /// <param name="parent">parent object to the new tile</param>
        /// <param name="cell">the active brush cell</param>
        private void PaintCell(GridLayout grid, Vector3Int position, Transform parent, BrushCell cell)
        {
            if (cell.gameObject == null)
                return;

            Transform existingGO = GetObjectInCell(grid, parent, position);

            if(replaceTiles && existingGO != null && existingGO.gameObject.name != cell.gameObject.name)
            {
                Erase(grid, parent.gameObject, position);
            }

            if (existingGO == null)
            {
                SetSceneCell(grid, parent, position, cell.gameObject, cell.offset, cell.scale, cell.orientation, m_Anchor);
            }
        }

        /// <summary>
        /// Purpose: specify paint locations
        /// </summary>
        /// <param name="gridLayout">a valid grid or child of a grid</param>
        /// <param name="brushTarget">the gameobject represented location in the grid</param>
        /// <param name="position">the position in the grid</param>
        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            GetGrid(ref gridLayout, ref brushTarget);
            
            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                BrushCell cell = currentCell;
                PaintCell(gridLayout, location, brushTarget != null ? brushTarget.transform : null, cell);
            }
        }

        /// <summary>
        /// Purpose: paint tiles to tilemap
        /// </summary>
        /// <param name="grid">a valid grid or child of a grid</param>
        /// <param name="parent">parent object to the new tile</param>
        /// <param name="position">the position in the grid</param>
        /// <param name="go">gameObject of the active brush cell</param>
        /// <param name="offset">offset as specified in active brush cell</param>
        /// <param name="scale">scale as specified in active brush cell</param>
        /// <param name="orientation">orientation as specified in active brush cell</param>
        /// <param name="anchor">anchor point specified in the brush</param>
        private static void SetSceneCell(GridLayout grid, Transform parent, Vector3Int position, GameObject go, Vector3 offset, Vector3 scale, Quaternion orientation, Vector3 anchor)
        {
            if (go == null)
                return;

            GameObject instance;
            if (PrefabUtility.IsPartOfPrefabAsset(go))
            {
                instance = (GameObject) PrefabUtility.InstantiatePrefab(go, parent != null ? parent.root.gameObject.scene : SceneManager.GetActiveScene());
                instance.transform.parent = parent;
            }
            else
            {
                instance = Instantiate(go, parent);
                instance.name = go.name;
                instance.SetActive(true);
                foreach (var renderer in instance.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = true;
                }
            }

            Undo.RegisterCreatedObjectUndo(instance, "Paint GameObject");

            var cellSize = grid.cellSize;
            var cellStride = cellSize + grid.cellGap;
            cellStride.x = Mathf.Approximately(0f, cellStride.x) ? 1f : cellStride.x;
            cellStride.y = Mathf.Approximately(0f, cellStride.y) ? 1f : cellStride.y;
            cellStride.z = Mathf.Approximately(0f, cellStride.z) ? 1f : cellStride.z;
            var anchorRatio = new Vector3(
                anchor.x * cellSize.x / cellStride.x,
                anchor.y * cellSize.y / cellStride.y,
                anchor.z * cellSize.z / cellStride.z
            );
            instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(position + anchorRatio));
            instance.transform.localRotation = orientation;
            instance.transform.localScale = scale;
            instance.transform.Translate(offset);
        }
        
        /// <summary>
        /// Purpose: Get active grid
        /// </summary>
        /// <returns>the active grid</returns>
        private void GetGrid(ref GridLayout gridLayout, ref GameObject brushTarget)
        {
            if (brushTarget == hiddenGrid)
                brushTarget = null;
            if (brushTarget != null)
            {
                var targetGridLayout = brushTarget.GetComponent<GridLayout>();
                if (targetGridLayout != null)
                    gridLayout = targetGridLayout;
            }
        }

    /// <summary>
    /// Purpose: Find any existing objects in a tile location
    /// </summary>
    /// <param name="gridLayout">a valid grid or child of a grid</param>
    /// <param name="brushTarget">the gameobject represented location in the grid</param>
    /// <param name="position">the position in the grid</param>
    private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
    {
        int childCount = parent.childCount;
        Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position - Vector3Int.one / 2)) + Vector3.down * 3;
        Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3Int.one + Vector3Int.one / 2)) + Vector3.up * 6;
        Bounds bounds = new Bounds((max + min) * 0.5f, max - min);

        for(int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if(bounds.Contains(child.position))
            {
                return child;
            }
        }

        return null;
    }

}



#if UNITY_EDITOR

[CustomEditor(typeof(TileBrush3D))]
public class TileBrush3DEditor : Editor
    {
        Vector2 scrollPosition = Vector2.zero;

        /// <summary>
        /// Purpose: Sets up the GUI for the TileBrush3D interface
        /// </summary>
        public override void OnInspectorGUI()
        {
            TileBrush3D tileBrush3DInstance = (TileBrush3D)target;
            List<GameObjectBrush.BrushCell> validCells = new List<GameObjectBrush.BrushCell>(tileBrush3DInstance.cells);
            GameObjectBrush.BrushCell currentCell = tileBrush3DInstance.GetCurrentCell();
            GUIStyle listStyle = new GUIStyle();
            Color[] pixels;

            for(int i = validCells.Count - 1; i >= 0; i--)
            {
                if (validCells[i].gameObject == null)
                {
                    validCells.Remove(validCells[i]);
                }
            }

            listStyle.normal.background = Texture2D.blackTexture;

            pixels = listStyle.normal.background.GetPixels();

            for(int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color(0.25f, 0.25f, 0.25f, 1f);
            }

            listStyle.normal.background.SetPixels(pixels);
            listStyle.normal.background.Apply();

            string currentCellName = "";
            

            if(currentCell != null && currentCell.gameObject != null)
            {
                currentCellName = currentCell.gameObject.name;
            }

            EditorGUILayout.PrefixLabel($"Selected Brush: {currentCellName.ToUpper()}");
            EditorGUILayout.Space();
            tileBrush3DInstance.replaceTiles = EditorGUILayout.ToggleLeft("Replace tiles with current brush.", tileBrush3DInstance.replaceTiles);
            EditorGUILayout.Space();


            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, listStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth - 10), GUILayout.Height(300), GUILayout.ExpandWidth(true));

            
            for(int i = 0; i < validCells.Count; i++)
            {
                int widthMod = 1;
                if(tileBrush3DInstance.cells[i].gameObject != null)
                {
                    
                    Texture2D assetPreview = AssetPreview.GetAssetPreview(validCells[i].gameObject);
                    GUIContent content = new GUIContent((assetPreview), validCells[i].gameObject.name);
                    

                    if(assetPreview != null && (int)assetPreview.width > 0)
                    {
                        widthMod = (int)EditorGUIUtility.currentViewWidth / (int)assetPreview.width;
                    }

                    if(widthMod < 1)
                    {
                        widthMod = 1;
                    }

                    if(i % widthMod == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }

                    if(GUILayout.Button(content, GUILayout.Width(assetPreview != null ? assetPreview.width : 150)))
                    {
                        tileBrush3DInstance.SetCurrentCell(tileBrush3DInstance.cells[i]);
                    }

                    if(i % widthMod == widthMod - 1 || i == validCells.Count - 1 )
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            DrawDefaultInspector();
        }
    }
#endif
}