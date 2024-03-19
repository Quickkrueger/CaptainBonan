#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.Tilemaps.GameObjectBrush;
namespace RoomTools.Brushes
{
    [System.Serializable]
    public class RuleBrush3D : BrushCell
    {
        public RuleMap3D ruleMap;

        public RuleBrush3D() : base()
        {

            ruleMap = new RuleMap3D();
            ruleMap.InitializeMap();
            scale = Vector3.one;
            orientation = Quaternion.identity;
            offset = Vector3.zero; 
            gameObject = null;

        }

        
    }

    [System.Serializable]
    public struct RuleMap3D
    {
        public Rule3D north;
        public Rule3D south;
        public Rule3D east;
        public Rule3D west;
        public Rule3D southwest;
        public Rule3D southeast;
        public Rule3D northwest;
        public Rule3D northeast;

        public void InitializeMap()
        {
            north = new Rule3D(new Vector3Int(0, 1, 0));
            south = new Rule3D(new Vector3Int(0, -1, 0));
            east = new Rule3D(new Vector3Int(1, 0, 0));
            west = new Rule3D(new Vector3Int(-1, 0, 0));
            northeast = new Rule3D(new Vector3Int(1, 1, 0));
            northwest = new Rule3D(new Vector3Int(-1, 1, 0));
            southeast = new Rule3D(new Vector3Int(1, -1, 0));
            southwest = new Rule3D(new Vector3Int(-1, -1, 0));
        }

        public int Length()
        {
            return 8;
        }

        public Rule3D GetRuleByIndex(int i)
        {
            switch(i)
            {
                case 0: 
                    return north;
                case 1: 
                    return south;
                case 2: 
                    return east;
                case 3: 
                    return west;
                case 4: 
                    return southwest;
                case 5:
                    return northeast;
                case 6:
                    return northwest;
                case 7:
                    return southeast;
                default:
                    throw new System.ArgumentOutOfRangeException();
                    
            }
        }

        public void AssignRuleByIndex(int i, RuleType type)
        {
            switch (i)
            {
                case 0:
                    north.type = type;
                    break;
                case 1:
                    south.type = type;
                    break;
                case 2:
                    east.type = type;
                    break;
                case 3:
                    west.type = type;
                    break;
                case 4:
                    southwest.type = type;
                    break;
                case 5:
                    northeast.type = type;
                    break;
                case 6:
                    northwest.type = type;
                    break;
                case 7:
                    southeast.type = type;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();

            }
        }

        public bool CompareRuleMap(RuleMap3D other)
        {
            bool result = true;

            for (int i = 0; i < Length(); i++)
            {
                if(other.GetRuleByIndex(i).type != GetRuleByIndex(i).type & other.GetRuleByIndex(i).type != RuleType.None)
                {
                    result = false; 
                    break;
                }
            }

            return result;
        }


    }

    [System.Serializable]
    public struct Rule3D
    {
        public Vector3Int position;
        public RuleType type;

        public Rule3D(Vector3Int newPosition)
        {
            position = newPosition;
            type = RuleType.None;
        }

    }

    [System.Serializable]
    public enum RuleType
    {
        None,
        Occupied,
        Vacant
    }

    [CustomPropertyDrawer(typeof(RuleMap3D))]
    public class Map3DDrawer : PropertyDrawer
    {
        Texture2D occupiedTexture;
        Texture2D noneTexture;
        Texture2D vacantTexture;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 60f;
        }

        private void SetupButton(Rect position, string ruleName, SerializedProperty property)
        {
            Color[] pixels;
            if (occupiedTexture == null)
            {
                occupiedTexture = new Texture2D((int)position.width / 3, (int)position.height / 3);
                pixels = Enumerable.Repeat(Color.green, ((int)position.width / 3) * ((int)position.height / 3)).ToArray();
                occupiedTexture.SetPixels(pixels);
                occupiedTexture.Apply();
            }
            if (noneTexture == null)
            {
                noneTexture = new Texture2D((int)position.width / 3, (int)position.height / 3);
                pixels = Enumerable.Repeat(Color.gray, ((int)position.width / 3) * ((int)position.height / 3)).ToArray();
                noneTexture.SetPixels(pixels);
                noneTexture.Apply();
            }
            if (vacantTexture == null){
                vacantTexture = new Texture2D((int)position.width / 3, (int)position.height / 3);
                pixels = Enumerable.Repeat(Color.red, ((int)position.width / 3) * ((int)position.height / 3)).ToArray();
                vacantTexture.SetPixels(pixels);
                vacantTexture.Apply();
            }

            SerializedProperty ruleProperty = property.FindPropertyRelative(ruleName);
            SerializedProperty enumproperty = ruleProperty.FindPropertyRelative("type");
            int enumValue = enumproperty.enumValueIndex;

            GUIStyle style = new GUIStyle();
            
            if((RuleType)enumValue == RuleType.None)
            {
                style.normal.background = noneTexture;
                style.active.background = noneTexture;
                style.hover.background = noneTexture;
            }
            else if((RuleType)enumValue == RuleType.Occupied)
            {
                style.normal.background = occupiedTexture;
                style.active.background = occupiedTexture;
                style.hover.background = occupiedTexture;
            }
            else
            {
                style.normal.background = vacantTexture;
                style.active.background = vacantTexture;
                style.hover.background = vacantTexture;
            }

            if(GUI.Button(position, "", style))
            {
                if(enumValue >= 2)
                {
                    enumproperty.enumValueIndex = 0;
                }
                else
                {
                    enumproperty.enumValueIndex++;
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            float buttonHeight = position.height / 3;
            float buttonWidth = buttonHeight;

            Rect northRect = new Rect(position.x + buttonWidth, position.y, buttonWidth - 1, buttonHeight - 1);
            Rect northwestRect = new Rect(position.x, position.y, buttonWidth - 1, buttonHeight - 1);
            Rect northeastRect = new Rect(position.x + buttonWidth * 2, position.y, buttonWidth - 1, buttonHeight - 1);

            Rect centerRect = new Rect(position.x + buttonWidth, position.y + buttonHeight, buttonWidth - 1, buttonHeight - 1);
            Rect westRect = new Rect(position.x, position.y + buttonHeight, buttonWidth - 1, buttonHeight - 1);
            Rect eastRect = new Rect(position.x + buttonWidth * 2, position.y + buttonHeight, buttonWidth - 1, buttonHeight - 1);

            Rect southRect = new Rect(position.x + buttonWidth, position.y + buttonHeight * 2, buttonWidth - 1, buttonHeight - 1);
            Rect southwestRect = new Rect(position.x, position.y + buttonHeight * 2, buttonWidth - 1, buttonHeight - 1);
            Rect southeastRect = new Rect(position.x + buttonWidth * 2, position.y + buttonHeight * 2, buttonWidth - 1, buttonHeight - 1);

            

            SetupButton(northRect, "north", property);
            SetupButton(northeastRect, "northeast", property);
            SetupButton(northwestRect, "northwest", property);

            GUI.Button(centerRect, "X");
            SetupButton(eastRect, "east", property);
            SetupButton(westRect, "west", property);

            SetupButton(southRect, "south", property);
            SetupButton(southeastRect, "southeast", property);
            SetupButton(southwestRect, "southwest", property);



            EditorGUI.EndProperty();
        }
    }
}
#endif