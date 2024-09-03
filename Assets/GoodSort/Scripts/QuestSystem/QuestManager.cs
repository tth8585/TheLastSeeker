using System.Collections.Generic;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> dic_dailyQuest;

    private int _currentPlayerLevel;
    private QuestInfoSO[] allQuestSO;
    private bool _startUpdateQuest = false;
    private List<QuestRandom> questRandoms;
    public List<ItemInfoSO> ListItemReward;
    private List<string> _listQuestDone = new();

    private bool _questInited = false;

    private void OnEnable()
    {
        MyEvent.Instance.QuestEvents.onStartQuest += StartQuest;
        MyEvent.Instance.QuestEvents.onAdvanceQuest += AdvanceQuest;
        MyEvent.Instance.QuestEvents.onDoneQuest += DoneQuest;
        MyEvent.Instance.QuestEvents.onQuestTaskStateChange += QuestTaskStateChanged;
        MyEvent.Instance.QuestEvents.onClaimedReward += ClaimReward;
    }

    private void OnDisable()
    {
        MyEvent.Instance.QuestEvents.onStartQuest -= StartQuest;
        MyEvent.Instance.QuestEvents.onAdvanceQuest -= AdvanceQuest;
        MyEvent.Instance.QuestEvents.onDoneQuest -= DoneQuest;
        MyEvent.Instance.QuestEvents.onQuestTaskStateChange -= QuestTaskStateChanged;
        MyEvent.Instance.QuestEvents.onClaimedReward -= ClaimReward;
    }

    private void ClaimReward(string idQuest)
    {
        Quest quest = GetDailyQuestById(idQuest);
        quest.ClaimedReward = true;
        SaveQuestData();
    }

    //private void Start()
    //{
    //    CheckDailyLogin();
    //    InitQuestData();
    //}

    public void InitQuest()
    {
        CheckDailyLogin();
        InitQuestData();
    }

    public void InitQuestData()
    {
        if(_questInited) return;

        if (dic_dailyQuest != null)
            dic_dailyQuest.Clear();

        dic_dailyQuest = CreateDailyQuest();
        //load progress from playerpref
        LoadQuestData();
        _startUpdateQuest = true;

        //broadcast the state of all quest in main quest
        foreach (Quest quest in dic_dailyQuest.Values)
        {
            if (quest.State == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentTask(this.transform);
            }

            MyEvent.Instance.QuestEvents.QuestStateChange(quest);
        }

        _questInited = true;
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetDailyQuestById(id);
        quest.State = state;
        MyEvent.Instance.QuestEvents.QuestStateChange(quest);

        //save to prefeb progress
        SaveQuestData();

        if (state == QuestState.DONE && !_listQuestDone.Contains(id))
        {
            MyEvent.Instance.QuestEvents.DoneDailyQuest(id);
            _listQuestDone.Add(id);
        }
    }

    private void StartQuest(string id)
    {
        Quest quest = GetDailyQuestById(id);
        quest.InstantiateCurrentTask(this.transform);
        ChangeQuestState(id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetDailyQuestById(id);
        quest.NextTask();

        if (quest.CanGoNextTask())
        {
            quest.InstantiateCurrentTask(this.transform);
        }
        else
        {
            //ChangeQuestState(id, QuestState.CAN_FINISH);
            ChangeQuestState(id, QuestState.DONE);
        }
    }

    private void DoneQuest(string id)
    {
        //Quest quest= GetMainQuestById(id);
        //ClaimRewardQuest(quest);
        ChangeQuestState(id, QuestState.DONE);
    }

    private void QuestTaskStateChanged(string id, int taskIndex, QuestTaskState questTaskState)
    {
        Quest quest = GetDailyQuestById(id);
        quest.StoreQuestTaskState(questTaskState, taskIndex);
        ChangeQuestState(id, quest.State);
    }

    private bool CheckRequirementMet(Quest quest)
    {
        bool metRequirements = true;

        //check level
        if (_currentPlayerLevel < quest.Info.LevelRequired) metRequirements = false;
        //check quest
        foreach (var questRequire in quest.Info.QuestsRequire)
        {
            if (GetDailyQuestById(questRequire.Id).State != QuestState.DONE)
            {
                metRequirements = false;
            }
        }

        return metRequirements;
    }

    private Dictionary<string, Quest> CreateDailyQuest()
    {
        Dictionary<string, Quest> dic_idToQuest = new Dictionary<string, Quest>();

        QuestInfoSO[] quests = Resources.LoadAll<QuestInfoSO>("DailyQuestConfigs");

        if (questRandoms == null || questRandoms.Count == 0)
        {
            questRandoms = GetQuestRandom();
        }

        foreach (var data in quests)
        {
            if (dic_idToQuest.ContainsKey(data.Id))
            {
                Debug.LogWarning("Duplicate ID Quest found when create main quest: " + data.Id);
            }
            else
            {
                try
                {
                    ItemInfoSO item = MyItemAbility.Instance.GetItemInfoByName(questRandoms.Find(x => x.questId == data.Id).rewardType);
                    if (item != null)
                    {
                        data.ItemReward = item;
                    }
                }
                catch
                {

                }

                Quest quest = new Quest(data);

                if (quest == null)
                {
                    Debug.LogError("Quest id: " + data.Id + " has no data in SO");
                }
                else
                {
                    if (CheckQuestInRandomList(data.Id))
                    {
                        dic_idToQuest.Add(data.Id, quest);
                    }
                }
            }
        }

        return dic_idToQuest;
    }

    private List<QuestRandom> GetQuestRandom()
    {
        if (PlayerPrefs.HasKey(selectedQuestsKey))
        {
            QuestRandomList questRandomList = JsonUtility.FromJson<QuestRandomList>(PlayerPrefs.GetString(selectedQuestsKey));
            return questRandomList.questRandom;
        }
        else
        {
            Debug.LogError("cant find key: " + selectedQuestsKey + " in PlayerPrefs to create random quest");
            return null;
        }
    }

    private bool CheckQuestInRandomList(string idQuest)
    {
        if (questRandoms == null || questRandoms.Count == 0)
        {
            questRandoms = GetQuestRandom();
        }

        foreach (var data in questRandoms)
        {
            if (data.questId == idQuest)
            {
                return true;
            }
        }

        return false;
    }

    public int GetMaxCountForTaskQuest(string questId)
    {
        foreach (var data in questRandoms)
        {
            if (data.questId == questId)
            {
                return data.indexRandomMaxCount;
            }
        }
        return 0;
    }

    public Quest GetDailyQuestById(string id)
    {
        Quest quest = dic_dailyQuest[id];
        if (quest == null)
        {
            Debug.LogError("Cant find quest has ID: " + id + " in daily quest");
        }

        return quest;
    }

    public Dictionary<string, Quest> GetAllDailyQuest()
    {
        return dic_dailyQuest;
    }

    public int CalculateFinalReward(string questId)
    {
        for (int i = 0; i < questRandoms.Count; i++)
        {
            if (questRandoms[i].questId == questId)
            {
                return i + 1;
            }
        }

        return 1;
    }

    private void LateUpdate()
    {
        if (dic_dailyQuest != null && _startUpdateQuest)
        {
            foreach (Quest quest in dic_dailyQuest.Values)
            {
                //if quest meet reuqirement change state to ready to take
                if (quest.State == QuestState.REQUIRE_NOT_MET && CheckRequirementMet(quest))
                {
                    ChangeQuestState(quest.Info.Id, QuestState.CAN_START);
                    //auto assign quest the 1st and 2nd tutorial quest only
                    if (quest.State == QuestState.CAN_START)
                    {
                        MyEvent.Instance.QuestEvents.StartQuest(quest.Info.Id);
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            MyEvent.Instance.QuestEvents.GetComboQuest(5);
            MyEvent.Instance.QuestEvents.GetComboQuest(7);
            MyEvent.Instance.QuestEvents.UseItemToQuest(ITEM_TYPE.Frozen);
            MyEvent.Instance.QuestEvents.UseItemToQuest(ITEM_TYPE.MagicStick);
            MyEvent.Instance.QuestEvents.UseItemToQuest(ITEM_TYPE.Hit);
            MyEvent.Instance.QuestEvents.UseItemToQuest(ITEM_TYPE.Refesh);
            MyEvent.Instance.QuestEvents.StartALevelQuest();
            MyEvent.Instance.QuestEvents.CompleteLevelQuest();
            MyEvent.Instance.QuestEvents.CompleteLevelQuest();
        }
    }
#endif

    public QuestInfoSO LoadQuestSO(string idQuest)
    {
        if (allQuestSO == null || allQuestSO.Length == 0)
        {
            allQuestSO = Resources.LoadAll<QuestInfoSO>("DailyQuestConfigs");
        }

        foreach (QuestInfoSO infoSO in allQuestSO)
        {
            if (infoSO.Id == idQuest)
            {
                return infoSO;
            }
        }

        return null;
    }

    public bool CheckRewardQuestNotClaimed()
    {
        foreach (Quest quest in dic_dailyQuest.Values)
        {
            if (quest.ClaimedReward || quest.State != QuestState.DONE && quest.State != QuestState.CAN_FINISH)
            {
                continue;
            }
            return true;
        }
        return false;
    }

    #region SAVE LOAD
    private const string questDataKey = "questData";

    // Save quest data to player prefs
    private void SaveQuestData()
    {
        if (dic_dailyQuest.Count == 0)
        {
            Debug.LogWarning("dic_dailyQuest is empty. No data to save.");
            return;
        }

        foreach (var quest in dic_dailyQuest)
        {
            if (quest.Value.ClaimedReward && quest.Value.State == QuestState.DONE)
                MyEvent.Instance.QuestEvents.QuestNoti(false);
            else if (!quest.Value.ClaimedReward && quest.Value.State == QuestState.DONE)
            {
                MyEvent.Instance.QuestEvents.QuestNoti(true);
                break;
            }
        }

        // Convert the dictionary to a list of KeyValuePair for serialization
        List<QuestDataSave> newList = GetListQuestForSave();

        MyQuestClassArrayWrapper wrapper = new MyQuestClassArrayWrapper(newList.ToArray());
        // Convert the list to JSON
        string json = JsonUtility.ToJson(wrapper);

        // Save the JSON string to PlayerPrefs
        PlayerPrefs.SetString(questDataKey, json);
        PlayerPrefs.Save();
    }

    private List<QuestDataSave> GetListQuestForSave()
    {
        List<QuestDataSave> newList = new List<QuestDataSave>();
        foreach (var item in dic_dailyQuest)
        {
            newList.Add(new QuestDataSave(item.Value));
        }
        return newList;
    }

    // Load quest data from player prefs
    private void LoadQuestData()
    {
        if (PlayerPrefs.HasKey(questDataKey))
        {
            string json = PlayerPrefs.GetString(questDataKey);

            MyQuestClassArrayWrapper wrapper = JsonUtility.FromJson<MyQuestClassArrayWrapper>(json);
            if (wrapper == null || wrapper.array == null)
            {
                Debug.LogError("Failed to deserialize JSON data.");
                return;
            }

            Debug.Log("Loaded JSON data: " + json);
            for (int i = 0; i < wrapper.array.Length; i++)
            {
                if (dic_dailyQuest.ContainsKey(wrapper.array[i].IdQuest))
                {
                    QuestDataSave saveData = wrapper.array[i];
                    dic_dailyQuest[saveData.IdQuest] = new Quest(dic_dailyQuest[saveData.IdQuest], saveData.State, saveData.ClaimedReward, saveData.TaskState);
                    if (saveData.State == QuestState.DONE && !saveData.ClaimedReward)
                    {
                        MyEvent.Instance.QuestEvents.DoneQuest(saveData.IdQuest);
                    }
                }
            }
        }
        else
        {
            Debug.Log("xx No saved quest data found.");
        }
    }
    #endregion

    #region RESET DAILY QUEST

    private const string selectedQuestsKey = "SelectedQuests";
    private const string _excludedQuestId = "Q_00";
    private const int _numSelectedQuests = 4;

    private void CheckDailyLogin()
    {
        if (MyDaily.Instance.IsNewDailyForQuest())
        {
            //Debug.Log("xx CheckDailyLogin quest: "+ MyDaily.Instance.IsNewDailyForQuest());
            //clear all save for old day
            if (PlayerPrefs.HasKey(questDataKey))
            {
                PlayerPrefs.DeleteKey(questDataKey);
            }

            if (PlayerPrefs.HasKey(selectedQuestsKey))
            {
                PlayerPrefs.DeleteKey(selectedQuestsKey);
            }
            _listQuestDone.Clear();
            MyDaily.Instance.SetCacheForQuestDailyReset();
            RandomQuestNewDay();
        }
    }
    public ITEM_TYPE RandomReward()
    {
        return ListItemReward[Random.Range(0, MyQuest.Instance.ListItemReward.Count)].ItemType;
    }
    public void RandomQuestNewDay()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("DailyQuestConfigs");
        // Add all quest IDs to the list
        List<string> allQuestIds = GetValidQuests(allQuests);

        if (!allQuestIds.Contains(_excludedQuestId))
        {
            Debug.LogWarning("Quest with ID 'Q_00' not found in available quests.");
            return; // Abort if "Q_00" is not found
        }

        allQuestIds.Remove(_excludedQuestId);

        // Randomly select 5 quest IDs from the list
        List<string> selectedQuestIds = new List<string>();
        for (int i = 0; i < _numSelectedQuests && allQuestIds.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, allQuestIds.Count);
            selectedQuestIds.Add(allQuestIds[randomIndex]);
            allQuestIds.RemoveAt(randomIndex);
        }

        // Add "Q_00" to the list of selected quests
        selectedQuestIds.Add(_excludedQuestId);

        List<QuestRandom> newList = new List<QuestRandom>();
        for (int i = 0; i < selectedQuestIds.Count; i++)
        {
            if(selectedQuestIds[i] == _excludedQuestId)
            {
                newList.Add(new QuestRandom(selectedQuestIds[i], GetRandomIndexAvailable(allQuests, selectedQuestIds[i]), ITEM_TYPE.GreenChest.ToString()));
            }
            else
            {
                newList.Add(new QuestRandom(selectedQuestIds[i], GetRandomIndexAvailable(allQuests, selectedQuestIds[i]), RandomReward().ToString()));
            }
        }

        string json = JsonUtility.ToJson(new QuestRandomList(newList));

        // Save the JSON string to PlayerPrefs
        PlayerPrefs.SetString(selectedQuestsKey, json);
        PlayerPrefs.Save();

        Debug.Log("xx RandomQuestNewDay: " + json);
    }

    public List<string> GetValidQuests(QuestInfoSO[] rawQuests)
    {
        List<string> allQuestIds = new List<string>();
       
        foreach (QuestInfoSO quest in rawQuests)
        {
            allQuestIds.Add(quest.Id);
        }

        return allQuestIds;
    }

    private int GetRandomIndexAvailable(QuestInfoSO[] allQuestSO, string id)
    {
        for (int i = 0; i < allQuestSO.Length; i++)
        {
            if (allQuestSO[i].Id == id)
            {
                QuestTask questTask = allQuestSO[i].TaskPrefabs[0].GetComponent<QuestTask>();
                int[] randomArr = questTask.GetArrMaxCount();

                return randomArr[UnityEngine.Random.Range(0, randomArr.Length)];
            }
        }
        return 0;
    }

    #endregion
}

public class MyQuest : SingletonMonoBehaviour<QuestManager> { }

[System.Serializable]
public class QuestDataSave
{
    public string IdQuest;
    public QuestState State;
    public bool ClaimedReward;
    public string TaskState;
    public ITEM_TYPE RewardType;

    public QuestDataSave(Quest quest)
    {
        RewardType = quest.Info.ItemReward.ItemType;
        IdQuest = quest.Info.Id;
        State = quest.State;
        ClaimedReward = quest.ClaimedReward;
        TaskState = quest.GetCurrentQuestTaskState();
    }
}
[System.Serializable]
public class MyQuestClassArrayWrapper
{
    public QuestDataSave[] array;

    // Constructor to initialize the wrapper with an array
    public MyQuestClassArrayWrapper(QuestDataSave[] array)
    {
        this.array = array;
    }
}
[System.Serializable]
public class QuestRandomList
{
    public List<QuestRandom> questRandom;

    public QuestRandomList(List<QuestRandom> quests)
    {
        this.questRandom = quests;
    }
}

[System.Serializable]
public class QuestRandom
{
    public string questId;
    public int indexRandomMaxCount;
    public string rewardType;

    public QuestRandom(string questId, int indexRandomMaxCount, string rewardType)
    {
        this.questId = questId;
        this.indexRandomMaxCount = indexRandomMaxCount;
        this.rewardType = rewardType;
    }
}