#if UNITY_EDITOR
//Created by: Marshall Krueger
//Last edited by: Marshall Krueger 02/09/23
//Purpose: A 3D Tile brush for our 3D tile system
using UnityEngine.SceneManagement;
using UnityEditor.Tilemaps;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RoomTools.Brushes
{

    [CreateAssetMenu(fileName = "New RuleBrush3D", menuName = "3D/Tilemap/RuleBrush3D", order = 0)]
    [CustomGridBrush(false, true, false, "RuleBrush3D")]
    public class RuleBrush3D : GameObjectBrush
    {

        [SerializeField]
        public RuleTile3D[] cells3D;
        private bool doubleChecking = false;

        private void OnEnable()
        {

            if (cells3D == null)
            {
                cells3D = new RuleTile3D[0];
            }
        }

        private void OnValidate()
        {

            if (cells3D == null)
            {
                cells3D = new RuleTile3D[0];
            }
        }

        /// <summary>
        /// Purpose: Erase tiles from the tilemap
        /// </summary>
        /// <param name="gridLayout">a valid grid or child of a grid</param>
        /// <param name="brushTarget">the gameobject represented location in the grid</param>
        /// <param name="position">the position in the grid</param>
        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            CallErase(gridLayout, brushTarget, position);

            SelectRuleCell(gridLayout, brushTarget.transform, position);

            if (!doubleChecking)
            {
                doubleChecking = true;
                Erase(gridLayout, brushTarget, position);
            }
            else
            {
                doubleChecking = false;
            }
            
        }

        private void CallErase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget.layer == 31)
            {
                return;
            }

            Transform erased = GetObjectInCell(gridLayout, brushTarget.transform, new Vector3Int(position.x, position.y, 0));
            if (erased != null)
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

            if(!doubleChecking)
            {
                doubleChecking = true;
                Paint(gridLayout, brushTarget, position);
            }
            else
            {
                doubleChecking = false;
            }
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

            if (existingGO != null && existingGO.gameObject.name != cell.gameObject.name)
            {
                CallErase(grid, parent.gameObject, position);
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
            CallBoxFill(gridLayout, brushTarget, position);

            if (!doubleChecking)
            {
                doubleChecking = true;
                BoxFill(gridLayout, brushTarget, position);
            }
            else
            {
                doubleChecking= false;
            }
        }

        private void CallBoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            GetGrid(ref gridLayout, ref brushTarget);

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                Transform parent = brushTarget != null ? brushTarget.transform : null;
                BrushCell cell = SelectRuleCell(gridLayout, parent, location);
                PaintCell(gridLayout, location, parent, cell);
            }
        }

        private BrushCell SelectRuleCell(GridLayout grid, Transform parent, Vector3Int position, bool isNeighbor = false)
        {
            RuleTile3D cell = null;
            Transform current;
            RuleMap3D tileLayoutMap = new RuleMap3D();
            tileLayoutMap.InitializeMap();

            for (int j = 0; j < tileLayoutMap.Length(); j++)
            {

                current = GetObjectInCell(grid, parent, position + tileLayoutMap.GetRuleByIndex(j).position);
                tileLayoutMap.AssignRuleByIndex(j,GetRuleTypeFromTransform(current));

                if (current != null && !isNeighbor)
                {
                    BrushCell neighborCell = SelectRuleCell(grid, parent, position + tileLayoutMap.GetRuleByIndex(j).position, true);
                    PaintCell(grid, position + tileLayoutMap.GetRuleByIndex(j).position, parent, neighborCell);
                }
            }

            for (int i = 0; i < cells3D.Length; i++)
            {
                cell = cells3D[i];

                if (tileLayoutMap.CompareRuleMap(cell.ruleMap))
                {
                    break;
                }
            }

            return cell;
        }

        private RuleType GetRuleTypeFromTransform(Transform neighbor)
        {
            if(neighbor == null)
            {
                return RuleType.Vacant;
            }
            else
            {
                return RuleType.Occupied;
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
                instance = (GameObject)PrefabUtility.InstantiatePrefab(go, parent != null ? parent.root.gameObject.scene : SceneManager.GetActiveScene());
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

            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (bounds.Contains(child.position))
                {
                    return child;
                }
            }

            return null;
        }

    }


    [CustomEditor(typeof(RuleBrush3D)), CanEditMultipleObjects]
    public class RuleTile3DEditor : Editor
    {
        Vector2 scrollPosition = Vector2.zero;
        RuleBrush3D _instance;
        //List<BrushCell3D> _validCells;
        SerializedObject _target;
        SerializedProperty _cells3D;
        //BrushCell3D _currentCell;

        /// <summary>
        /// Purpose: Sets up the GUI for the TileBrush3D interface
        /// </summary>
        /// 
        private void OnEnable()
        {
            _instance = target as RuleBrush3D;
            //_validCells = new List<BrushCell3D>(_instance.cells3D);
            //_currentCell = _instance.GetCurrentCell();
            _target = new SerializedObject(_instance);
            _cells3D = _target.FindProperty("cells3D");
        }

        public override void OnInspectorGUI()
        {
            GUIStyle listStyle = new GUIStyle();
            GUIStyle brushTexttStyle = new GUIStyle();
            Color[] pixels;
            _target.Update();

            listStyle.normal.background = Texture2D.blackTexture;
            brushTexttStyle.fixedWidth = 300f;
            brushTexttStyle.stretchWidth = true;

            pixels = listStyle.normal.background.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color(0.25f, 0.25f, 0.25f, 1f);
            }

            listStyle.normal.background.SetPixels(pixels);
            listStyle.normal.background.Apply();


            EditorGUILayout.PropertyField(_cells3D, true);

            _target.ApplyModifiedProperties();
        }
    }

}
#endif