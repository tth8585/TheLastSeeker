using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COMPLETE_2_LEVEL_Task : QuestTask
{
    protected override void SetQuestTaskState(string state)
    {
        ChangeState(state);
    }

    private void OnEnable()
    {
        MyEvent.Instance.QuestEvents.onCompleteLevel += UpdateTask;
    }

    private void OnDisable()
    {
        MyEvent.Instance.QuestEvents.onCompleteLevel -= UpdateTask;
    }

    private void UpdateTask()
    {
        _count++;
        SetQuestTaskState(_count.ToString());

        if (_count >= _maxCount)
        {
            FinishTask();
        }
    }
}
