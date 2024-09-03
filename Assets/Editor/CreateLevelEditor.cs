using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateLevelController))]
public class CreateLevelEditor : Editor
{
    CreateLevelController controller;
    private List<SlotData> _slotData = new List<SlotData>();
    private string[] CameraOptions = new string[] { "Main", "LeftSmall", "DestroyMode"};
    private int previousCamereaOptionIndex = 0;

    public override void OnInspectorGUI()
    {
        controller= (CreateLevelController)target;

        controller.CameraModeIndex = EditorGUILayout.Popup("Camera SetUp", controller.CameraModeIndex, CameraOptions);
        if (controller.CameraModeIndex != previousCamereaOptionIndex)
        {
            // Call a method or perform actions when the selection changes
            SelectionChanged(controller.CameraModeIndex);
            // Update the previous option index
            previousCamereaOptionIndex = controller.CameraModeIndex;
        }

        if (controller.CameraModeIndex == 2)
        {
            controller.IsDestroyMode = true;
        }
        else
        {
            controller.IsDestroyMode = false;
        }

        EditorGUILayout.Separator();
        controller.Level= EditorGUILayout.IntField("Level", controller.Level);
        EditorGUILayout.Separator();
        controller.CountSpace= EditorGUILayout.IntField("Count Space", controller.CountSpace);
        EditorGUILayout.Separator();
        controller.TotalIdSpawn=EditorGUILayout.IntField("Total Id Spawn", controller.TotalIdSpawn);
        EditorGUILayout.Separator();
        controller.MoveSpeed = EditorGUILayout.FloatField("Move Speed", controller.MoveSpeed);
        controller.MoveSmooth = EditorGUILayout.FloatField("Move Smooth", controller.MoveSmooth);
        EditorGUILayout.Separator();
        controller.TimeForLevel= EditorGUILayout.FloatField("Time Play", controller.TimeForLevel);
        EditorGUILayout.Separator();

        if (GUILayout.Button("Open Level Window"))
        {
            OpenLevelWindow();
        }

        if (GUILayout.Button("Create Level"))
        {
            CreateLevel();
        }

        if(GUILayout.Button("Load Level"))
        {
            LoadLevel();
        }
    }

    private void SelectionChanged(int newIndex)
    {
        // Implement your code to handle the selection change here
        Debug.Log("Selected option changed to: " + CameraOptions[newIndex]);

        MyCam.Instance.SetUpCamera(newIndex);
    }

    private void OpenLevelWindow()
    {
        LevelWindow.Open(OnValueSelected);
        //LevelWindow.ShowWindow();
    }

    void OnValueSelected(List<SlotData> value)
    {
        _slotData = value;
    }

    private string CreateLevelData()
    {
        LevelData levelData = new LevelData();

        levelData.IsDestroyMode = controller.IsDestroyMode;
        levelData.CountSpace = controller.CountSpace;
        levelData.TotalIdSpawn = controller.TotalIdSpawn;
        levelData.Level = controller.Level;
        levelData.MoveSpeed = controller.MoveSpeed;
        levelData.MoveSmooth = controller.MoveSmooth;
        levelData.TimePlay = controller.TimeForLevel;
        levelData.SlotData = _slotData.ToArray();
        

        string json = JsonConvert.SerializeObject(levelData);

        return json;
    }

    private void CreateLevel()
    {
        string filePath = Path.Combine(Path.Combine(Application.streamingAssetsPath, "LevelData/"),controller.Level.ToString());

        string dataString = CreateLevelData();

        if (File.Exists(filePath))
        {
            Debug.LogWarning("File already exists: " + filePath);
        }

        Debug.Log("Create Level: "+ dataString);
        File.WriteAllText(filePath, dataString);
    }

    private void LoadLevel()
    {
        string filePath = Path.Combine(Path.Combine(Application.streamingAssetsPath, "LevelData/"), controller.Level.ToString());

        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            //JsonConvert.DeserializeObject<LevelData>(jsonString);
            Debug.Log("Load Level: "+jsonString);
            //LevelWindow.Open(OnValueSelected,jsonString);
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
}
