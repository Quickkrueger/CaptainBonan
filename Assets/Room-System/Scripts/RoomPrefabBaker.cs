#if UNITY_EDITOR
//Created by: Marshall Krueger
//Last edited by: marshall Krueger 02/13/2023
//Purpose: This script generates a prefab from a newly made room layout

using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Autodesk.Fbx;
using Unity.VisualScripting;
using Cinemachine;

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

        if(transform.childCount != 0)
        {

            for(int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.childCount >= 2 && child.GetChild(0).childCount > 0)
                {
                    //ModelExporter.ExportObject(folderPath + "/" + child.gameObject.name + ".fbx", child.gameObject);
                    PrefabUtility.SaveAsPrefabAssetAndConnect(child.gameObject, folderPath + "/" + child.gameObject.name + ".prefab", InteractionMode.UserAction);

                    success = true;
                }
            }

            for(int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }

        }

        GameObject newRoom = new GameObject();
        newRoom.transform.parent = transform;
        newRoom.transform.localPosition = Vector3.zero;
        newRoom.name = "New Room";

        RoomManager roomManager = newRoom.AddComponent<RoomManager>();
       

        GameObject tiles = new GameObject();
        tiles.transform.parent = newRoom.transform;
        tiles.transform.localPosition = Vector3.zero;
        tiles.name = "Tiles";

        GameObject spawners = new GameObject();
        spawners.transform.parent = newRoom.transform;
        spawners.transform.localPosition = Vector3.zero;
        spawners.name = "Spawners";

        GameObject camera = new GameObject();
        camera.transform.parent = newRoom.transform;
        camera.transform.localPosition = new Vector3(-0.5f, 6, -5);
        camera.transform.localEulerAngles = Vector3.right * 60;
        camera.name = "CM vcam1";

        CinemachineVirtualCamera virtCam = camera.AddComponent<CinemachineVirtualCamera>();
        virtCam.m_Lens.FieldOfView = 60;

        roomManager._virtualCamera = virtCam;


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

            if(GUILayout.Button("Generate Prefab from Tilemaps", GUILayout.Height(50)))
            {
                if(roomPrefabBakerInstance.GeneratePrefab())
                {
                    Debug.Log("Prefab Successfully Generated");
                }
                else
                {
                    Debug.LogError("Prefab could not be generated");
                }
            }
        }
    }
#endif