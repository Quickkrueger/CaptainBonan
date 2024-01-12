using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class AssetList : ScriptableObject
{
    public GameObject[] assetList;

    public GameObject GetRandomObject()
    {
        return assetList[Random.Range(0, assetList.Length)];
    }

    public GameObject GetObjectAtIndex(int index)
    {
        return assetList[index];
    }
        
    public GameObject[] GetAssetList()
    {
        return assetList;
    }
    
    
    
}
