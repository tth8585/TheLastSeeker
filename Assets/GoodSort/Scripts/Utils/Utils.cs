using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static void LoadLevel()
    {
        
    }

    public static List<T> Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    public static void ClearContent(Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static GameObject LoadPrefab(string objName)
    {
        GameObject go = (GameObject)Resources.Load(Const.prefabPath + objName, typeof(GameObject));
        if (go != null)
        {
            return go;
        }
        else
        {
            Debug.Log(objName + " path is incorect or no prefab in resource");
            return null;
        }
    }
}
