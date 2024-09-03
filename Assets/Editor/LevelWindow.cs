using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelWindow : EditorWindow
{
    public delegate void ValueCallback(List<SlotData> data);
    public ValueCallback OnValueSelected;

    Dictionary<int, SlotGameDesign> _dic = new Dictionary<int, SlotGameDesign>();
    Dictionary<int, SlotController> _dicSlot = new Dictionary<int, SlotController>();

    private List<SlotData> _listData = new List<SlotData>();

    private Vector2 scrollPosition;
    public string LoadData=string.Empty;

    private void OnEnable()
    {
        _dic.Clear();
        _dicSlot.Clear();
    }

    [MenuItem("Window/CreateLevel")]
    public static void ShowWindow()
    {
        LevelWindow window = (LevelWindow)GetWindow(typeof(LevelWindow));
        window.minSize = new Vector2(300, 500);
        window.maxSize = new Vector2(600, 800);
    }

    public static void Open(ValueCallback callback, string loadData=null)
    {
        ShowWindow();
        LevelWindow window = GetWindow<LevelWindow>();
        window.OnValueSelected = callback;
        if (loadData != null) 
        {
            window.LoadData=loadData;
        }
    }

    private void OnGUI()
    {
        //Debug.Log(LoadData);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        GUILayout.Label("All Slots");

        //load all slot
        AllSlotController allSlot = FindFirstObjectByType<AllSlotController>();

        for (int i=0;i< allSlot.transform.childCount; i++)
        {
            if (allSlot.transform.GetChild(i).GetComponent<SlotController>() == null)
            {
                continue;
            }

            SlotGameDesign slot = new SlotGameDesign();
            slot.id = i;
            if (!_dic.ContainsKey(slot.id)) _dic.Add(slot.id, slot);
            if (!_dicSlot.ContainsKey(slot.id)) 
            {
                SlotController slotController = allSlot.transform.GetChild(i).GetComponent<SlotController>();
                //slotController.SetId(i);
                _dicSlot.Add(slot.id, slotController);
            }
        }

        foreach(var item in _dic)
        {
            CreateSlotGUI(item.Value);
        }
        
        if (GUILayout.Button("Create Value"))
        {
            _listData.Clear();

            if (OnValueSelected != null)
            {
                foreach (var item in _dic)
                {
                    if (item.Value.IsActive)
                    {
                        SlotData data = new SlotData(item.Value.id, item.Value);
                        _listData.Add(data);
                    }
                }

                OnValueSelected(_listData);

                Close();
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Separator();

        //if(LoadData!= string.Empty)
        //{
        //    LevelData levelData = JsonUtility.FromJson<LevelData>(LoadData);
        //    SlotData[] datas = levelData.SlotData;

        //    foreach (var item in _dic) 
        //    {
        //        for(int i=0;i<datas.Length; i++)
        //        {
        //            if (_dic.ContainsKey(datas[i].id))
        //            {
        //                LoadDataForGD(datas[i], _dic[datas[i].id]);
        //            }
        //        }
        //    }
        //}
    }

    private void LoadDataForGD(SlotData data, SlotGameDesign slot)
    {
        slot.IsActive = true;
        _dicSlot[slot.id].ShowForGD(true);
        slot.Options[0] = data.isSpecial;
        slot.Options[1] = data.isLocked;
        slot.CountLock = data.turnCount;
        slot.MoveType = (int)data.moveType;
    }

    private void CreateSlotGUI(SlotGameDesign slot)
    {
        slot.IsActive = EditorGUILayout.BeginToggleGroup("Slot "+slot.id.ToString(), slot.IsActive);
        slot.IsActive = EditorGUILayout.BeginFoldoutHeaderGroup(slot.IsActive, "Options");
        if (slot.IsActive)
        {
            _dicSlot[slot.id].ShowForGD(true);
            slot.Options[0] = EditorGUILayout.Toggle(("Special"), slot.Options[0]);
            if (slot.Options[0])
            {
                _dicSlot[slot.id].ShowSpecial(true);
            }
            else
            {
                _dicSlot[slot.id].ShowSpecial(false);
            }
            slot.Options[1] = EditorGUILayout.Toggle(("Lock"), slot.Options[1]);
            if (slot.Options[1])
            {
                slot.CountLock = EditorGUILayout.IntField("Turn Count", slot.CountLock);
                _dicSlot[slot.id].ShowLockOptions(slot.CountLock);
            }
            else
            {
                _dicSlot[slot.id].ShowLockOptions(0);
            }

            slot.MoveType= EditorGUILayout.Popup("Move Option:", slot.MoveType, slot.MoveOptions);
            _dicSlot[slot.id].ShowMove(slot.MoveType);
        }
        else
        {
            _dicSlot[slot.id].ShowForGD(false);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.Separator();
    }

    private int GetId(string name)
    {
        int result;
        if (int.TryParse(name, out result))
        {
            // Parsing successful, 'result' contains the parsed integer value
            return result;
        }
        else
        {
            // Parsing failed, input string is not a valid integer
            Debug.LogWarning("Failed to parse input string as integer");
            return 0;
        }
    }
}
