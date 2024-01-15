//Created by: Marshall Krueger
//Last edited by: marshall Krueger 02/13/2023
//Purpose: This script generates a prefab from a newly made room layout

using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Autodesk.Fbx;

[RequireComponent(typeof(Grid))]
public class RoomPrefabBaker : MonoBehaviour
{

    private string folderPath = "Assets";


    /// <summary>
    /// Purpose: Generate a prefab
    /// </summary>
    /// <returns>true on success</returns>
    public bool GeneratePrefab()
    {
        
        bool success = false;

        if(!AssetDatabase.IsValidFolder(folderPath))
        {
            Object activeFolder = Selection.activeObject;

            folderPath = "Assets";
        }

        if(transform.childCount > 0)
        {

            for(int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if(child.childCount != 0)
                {
                    using (FbxManager fbxManager = FbxManager.Create())
                    {
                        // configure IO settings.
                        fbxManager.SetIOSettings(FbxIOSettings.Create(fbxManager, Globals.IOSROOT));

                        // Export the scene
                        using (FbxExporter exporter = FbxExporter.Create(fbxManager, "myExporter"))
                        {

                            // Initialize the exporter.
                            bool status = exporter.Initialize(folderPath, -1, fbxManager.GetIOSettings());

                            // Create a new scene to export
                            FbxScene scene = FbxScene.Create(fbxManager, "myScene");

                            // Export the scene to the file.
                            exporter.Export(scene);
                        }
                    }

                    //ModelExporter.ExportObject(folderPath + "/" + child.gameObject.name + ".fbx", child.gameObject);
                    //PrefabUtility.SaveAsPrefabAssetAndConnect(child.gameObject, folderPath + "/" + child.gameObject.name + ".prefab", InteractionMode.UserAction);
                    
                    success = true;
                }
            }

            for(int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }

        }

        GameObject temp = new GameObject();
        temp.transform.parent = transform;
        temp.transform.localPosition = Vector3.zero;
        temp.name = "New Room";

        Grid grd = temp.AddComponent<Grid>();
        grd.cellSwizzle = GridLayout.CellSwizzle.XZY;

        return success;
    }

    /// <summary>
    /// Purpose: set the current file path
    /// </summary>
    /// <param name="newFolderPath">a valid file path</param>
    public void SetFolderPath(string newFolderPath)
    {
        folderPath = newFolderPath;
    }

    /// <summary>
    /// Purpose: get the current file path
    /// </summary>
    /// <returns>the current file path</returns>
    public string GetFolderPath()
    {
        return folderPath;
    }
    
}




#if UNITY_EDITOR
[CustomEditor(typeof(RoomPrefabBaker))]
public class RoomPrefabBakerEditor : Editor
    {

        /// <summary>
        /// Purpose: Sets up the GUI for the prefab generator
        /// </summary>
        public override void OnInspectorGUI()
        {
            RoomPrefabBaker roomPrefabBakerInstance = (RoomPrefabBaker)target;

            EditorGUILayout.BeginHorizontal();

            roomPrefabBakerInstance.SetFolderPath(GUILayout.TextField(roomPrefabBakerInstance.GetFolderPath()));

            if(GUILayout.Button("Use Open Folder", GUILayout.Height(20), GUILayout.Width(110)))
            {
                System.Type projectWindowUtilType = typeof(ProjectWindowUtil);
                MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
                object obj = getActiveFolderPath.Invoke(null, new object[0]);
                string pathToCurrentFolder = obj.ToString();

                roomPrefabBakerInstance.SetFolderPath(pathToCurrentFolder);

            }
            EditorGUILayout.EndHorizontal();

            if(GUILayout.Button("Generate FBX from Tilemaps", GUILayout.Height(50)))
            {
                if(roomPrefabBakerInstance.GeneratePrefab())
                {
                    Debug.Log("FBX Successfully Generated");
                }
                else
                {
                    Debug.LogError("FBX could not be generated");
                }
            }
        }
    }
#endif