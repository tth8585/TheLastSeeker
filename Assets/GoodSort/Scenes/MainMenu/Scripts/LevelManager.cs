using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TTHUnityBase.Base.DesignPattern;
using UnityEditor;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Dictionary<int, LevelData> _dicLevelDatas;

    public void InitData()
    {
        var files = Resources.LoadAll<TextAsset>("LevelData");
//#if UNITY_EDITOR
//        string path = Path.Combine(Application.streamingAssetsPath, "LevelData");
//#elif UNITY_ANDROID
//        string path = "jar:file://" + Application.dataPath + "!/assets/LevelData";
//        Debug.LogError("pathhhhhhhhh: " + path);
//#endif
        //if (Directory.Exists(path))
        if (files.Length > 0)
        {
            // Get all text files in the directory
            //string[] files = Directory.GetFiles(path);
            _dicLevelDatas = new Dictionary<int, LevelData>();
            foreach (var item in files)
            {
                //if (item.EndsWith(".meta")) continue;
                string fileContents = item.text;

                LevelData _levelData = JsonUtility.FromJson<LevelData>(fileContents);
                if (_dicLevelDatas.ContainsKey(_levelData.Level))
                {
                    _dicLevelDatas[_levelData.Level] = _levelData;
                }
                else
                {
                    _dicLevelDatas.Add(_levelData.Level, _levelData);
                }
            }
        }
        else
        {
            Debug.LogError("StreamingAssets directory does not exist.");
        }
    }

    

    public int GetMaxLevelCount() { return _dicLevelDatas.Count; }

    public LevelData GetLevel(int level)
    {
        return _dicLevelDatas[level];
    }
}

public class MyLevel : SingletonMonoBehaviour<LevelManager> { }
