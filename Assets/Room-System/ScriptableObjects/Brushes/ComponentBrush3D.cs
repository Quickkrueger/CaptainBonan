#if UNITY_EDITOR
//Created by: Marshall Krueger
//Last edited by: Marshall Krueger 02/09/23
//Purpose: A 3D Tile brush for our 3D tile system
using UnityEditor.Tilemaps;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;
using System.Reflection;
using UnityEngine.Timeline;

namespace RoomTools.Brushes
{



    [CreateAssetMenu(fileName = "New ComponentBrush3D", menuName = "3D/Tilemap/ComponentBrush3D", order = 0)]
    [CustomGridBrush(false, true, false, "ComponentBrush3D")]
    public class ComponentBrush3D : GameObjectBrush
    {

        public List<MonoScript> componentCells;
        public MonoScript activeComponentCell;

        private void OnEnable()
        {

            if (componentCells == null)
            {
                componentCells = new List<MonoScript> ();
            }

            if (activeComponentCell == null)
            {
                activeComponentCell = new MonoScript();
            }

            LoadScripts();
        }

        private void OnValidate()
        {

            if (componentCells == null)
            {
                componentCells = new List<MonoScript>();
            }

            if (activeComponentCell == null)
            {
                activeComponentCell = new MonoScript();
            }


            LoadScripts();
                
        }

        private void LoadScripts()
        {
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            componentCells.Clear();
            foreach (string assetPath in assetPaths)
            {
                if (assetPath.EndsWith(".cs") && assetPath.StartsWith("Assets")) // or .js if you want
                {
                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                    if (CheckForMonobehaviorBase(script))
                    {
                        componentCells.Add(script);
                    }
                }
            }
        }

        private bool CheckForMonobehaviorBase(MonoScript script)
        {
            System.Type type = script.GetClass();
            while(type != null)
            {

                if (type == typeof(MonoBehaviour))
                {
                    return true;
                }

                type = type.BaseType;

            }
            return false;
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

        }

        private void CallErase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget.layer == 31)
            {
                return;
            }

            Transform erased = brushTarget.GetComponent<TileTracker>().GetObjectInCell(gridLayout, brushTarget.transform, new Vector3Int(position.x, position.y, 0), m_Anchor);
            if (erased != null && erased.gameObject.TryGetComponent(activeComponentCell.GetClass(), out Component script))
            {
                Undo.DestroyObjectImmediate(script);
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

            CallBoxFill(gridLayout, brushTarget, bounds);
        }

        /// <summary>
        /// Purpose: Prepare tiles to be painted
        /// </summary>
        /// <param name="grid">a valid grid or child of a grid</param>
        /// <param name="position">the position in the grid</param>
        /// <param name="parent">parent object to the new tile</param>
        /// <param name="cell">the active brush cell</param>
        private void PaintCell(GridLayout grid, Vector3Int position, Transform parent, MonoScript cell)
        {
            if (cell == null)
                return;

            Transform existingGO = parent.GetComponent<TileTracker>().GetObjectInCell(grid, parent, position, m_Anchor);

            Component script;

            if (existingGO != null && existingGO.TryGetComponent(cell.GetClass(), out script))
            {
                CallErase(grid, parent.gameObject, position);
            }

            if (existingGO != null && !existingGO.TryGetComponent(cell.GetClass(), out script))
            {
                Component newScript = existingGO.gameObject.AddComponent(cell.GetClass());
                Undo.RegisterCreatedObjectUndo(newScript, $"Paint {cell.name}");
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
        }

        private void CallBoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            GetGrid(ref gridLayout, ref brushTarget);

            foreach (Vector3Int location in position.allPositionsWithin)
            {
                Vector3Int local = location - position.min;
                Transform parent = brushTarget != null ? brushTarget.transform : null;
                Transform containsObject = brushTarget.GetComponent<TileTracker>().GetObjectInCell(gridLayout, parent, location, m_Anchor);

                if (containsObject != null)
                {
                    MonoScript cell = activeComponentCell;
                    PaintCell(gridLayout, location, parent, cell);
                }
            }
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


    }


    [CustomEditor(typeof(ComponentBrush3D)), CanEditMultipleObjects]
    public class ComponentBrush3DEditor : Editor
    {
        Vector2 scrollPosition = Vector2.zero;
        ComponentBrush3D _instance;
        SerializedObject _target;
        List<MonoScript> _componentCells;
        SerializedProperty _sCurrentCell;
        [SerializeField]
        List<SerializedProperty> _componentProperties;

        string _search;
        //BrushCell3D _currentCell;

        /// <summary>
        /// Purpose: Sets up the GUI for the TileBrush3D interface
        /// </summary>
        /// 
        private void OnEnable()
        {
            _search = string.Empty;
            _instance = target as ComponentBrush3D;
            _target = new SerializedObject(_instance);
            _componentCells = _instance.componentCells;
            _sCurrentCell = _target.FindProperty("activeComponentCell");

            _componentProperties = new List<SerializedProperty>();
        }

        public override void OnInspectorGUI()
        {
            GUIStyle listStyle = new GUIStyle();
            GUIStyle brushTexttStyle = new GUIStyle();
            Color[] pixels;


            listStyle.normal.background = Texture2D.blackTexture;
            brushTexttStyle.fixedWidth = 300f;
            brushTexttStyle.stretchWidth = true;

            _search = EditorGUILayout.TextField("Search", _search);

            _target.Update();

            pixels = listStyle.normal.background.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color(0.25f, 0.25f, 0.25f, 1f);
            }

            listStyle.normal.background.SetPixels(pixels);
            listStyle.normal.background.Apply();

            EditorGUILayout.PropertyField(_sCurrentCell, true);

            foreach(SerializedProperty property in _componentProperties)
            {
                EditorGUILayout.PropertyField(property, true);
            }



            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, listStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth - 10), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));


            for (int i = 0; i < _componentCells.Count; i++)
            {
                if (_componentCells[i] != null && (_componentCells[i].name.Contains(_search) || _search == string.Empty))
                {

                    if (GUILayout.Button(_componentCells[i].name))
                    {
                        _instance.activeComponentCell = _componentCells[i];
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }

}
#endif
