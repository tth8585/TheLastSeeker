using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public QuestInfoSO Info;
    public QuestState State;
    public bool ClaimedReward = false;
    private int _currentQuestTaskIndex;
    private QuestTaskState[] _questTaskStates;

    public Quest(QuestInfoSO info)
    {
        Info = info;
        State = QuestState.REQUIRE_NOT_MET;
        _currentQuestTaskIndex = 0;
        ClaimedReward = false;

        List<QuestTaskState> newList= new List<QuestTaskState>();
        newList.Add(new QuestTaskState("0"));
        _questTaskStates = newList.ToArray();
    }

    public Quest(Quest currentData, QuestState state, bool isClaimReward, string taskState)
    {
        Info = currentData.Info;
        State = state;
        ClaimedReward = isClaimReward;
        List<QuestTaskState> newList = new List<QuestTaskState>();
        newList.Add(new QuestTaskState(taskState));
        _questTaskStates = newList.ToArray();
    }
    
    public string GetCurrentQuestTaskState()
    {
        if(_currentQuestTaskIndex>= _questTaskStates.Length)
        {
            return _questTaskStates[_questTaskStates.Length - 1].State;
        }

        return _questTaskStates[_currentQuestTaskIndex].State;
    }

    public void NextTask()
    {
        _currentQuestTaskIndex++;
    }

    public bool CanGoNextTask()
    {
        return (_currentQuestTaskIndex < Info.TaskPrefabs.Length);
    }

    public int GetCurrentTaskIndex()
    {
        return _currentQuestTaskIndex;
    }

    public QuestTaskState[] GetQuestTaskStates()
    {
        return _questTaskStates;
    }

    public void InstantiateCurrentTask(Transform parent)
    {
        GameObject task = GetCurrentTaskPrefab();
        if (task!=null)
        {
            QuestTask questTask= Object.Instantiate<GameObject>(task, parent).GetComponent<QuestTask>();

            if (_questTaskStates == null)
            {
                List<QuestTaskState> newList = new List<QuestTaskState>();
                newList.Add(new QuestTaskState("0"));
                _questTaskStates = newList.ToArray();
            }

            if (_questTaskStates.Length > 0 && _currentQuestTaskIndex < _questTaskStates.Length)
            {
                if (_questTaskStates[_currentQuestTaskIndex] == null) _questTaskStates[_currentQuestTaskIndex] = new QuestTaskState("0");
                questTask.InitTask(Info.Id, _currentQuestTaskIndex, _questTaskStates[_currentQuestTaskIndex].State);
            }
            else
            {
                questTask.InitTask(Info.Id, _currentQuestTaskIndex, string.Empty);
            }
        }
    }

    public GameObject GetFirstTaskPrefab()
    {
        return  Info.TaskPrefabs[0];
    }

    public GameObject GetCurrentTaskPrefab()
    {
        GameObject taskPrefab = null;
        if (CanGoNextTask())
        {
            taskPrefab = Info.TaskPrefabs[_currentQuestTaskIndex];
        }
        else
        {
            //Debug.LogError("Quest: "+Info.Id+" has no task index: "+_currentQuestTaskIndex);
        }

        return taskPrefab;
    }

    public void StoreQuestTaskState(QuestTaskState state,int taskIndex)
    {
        //Debug.Log("xx StoreQuestTaskState test: " + state.State+"/"+ _questTaskStates.Length);

        if (_currentQuestTaskIndex < _questTaskStates.Length)
        {
            //Debug.Log("xx StoreQuestTaskState: "+state.State);
            _questTaskStates[_currentQuestTaskIndex].State= state.State;
        }
        else
        {
            Debug.LogError("Task index is out of range: " + "Quest id: " + Info.Id + "Task Index: " + taskIndex);
        }
    }

    public QuestData GetQuestData()
    {
        return new QuestData(State, _currentQuestTaskIndex, _questTaskStates, ClaimedReward);
    }
}
