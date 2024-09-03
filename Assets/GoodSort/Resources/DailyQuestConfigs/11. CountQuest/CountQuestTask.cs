using UnityEngine;
using System;
using System.Collections.Generic;

public class CountQuestTask : QuestTask
{
    protected override void SetQuestTaskState(string state)
    {
       ChangeState(state);
    }

    private void OnEnable()
    {
        MyEvent.Instance.QuestEvents.onDoneDailyQuest += UpdateTask;
    }

    private void OnDisable()
    {
        MyEvent.Instance.QuestEvents.onDoneDailyQuest -= UpdateTask;
    }

    private void UpdateTask(string idQuest)
    {
        if (idQuest == _questId) return;
        _count++;
        SetQuestTaskState(_count.ToString());

        if(_count >= _maxCount)
        {
            FinishTask();
        }       
    }
}
