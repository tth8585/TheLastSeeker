using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "ScriptableObjects/QuestData", order = 1)]
[System.Serializable]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string Id {  get; private set; }

    public int IdForUI = 0;
    public string QuestName;
    public string QuestDiscription;
    //tutorial cant show on UI popup

    [Header("Requirement")]
    public int LevelRequired;
    public QuestInfoSO[] QuestsRequire;

    [Header("Task")]
    public GameObject[] TaskPrefabs;

    [Header("Reward")]
    public ItemInfoSO ItemReward;

    private void OnValidate()
    {
#if UNITY_EDITOR
        Id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
