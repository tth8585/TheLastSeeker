using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ONE_TIME_COMBO_X7_Task : QuestTask
{
    protected override void SetQuestTaskState(string state)
    {
        ChangeState(state);
    }

    private void OnEnable()
    {
        MyEvent.Instance.QuestEvents.onGetCombo += UpdateTask;
    }

    private void OnDisable()
    {
       MyEvent.Instance.QuestEvents.onGetCombo -= UpdateTask;
    }

    private void UpdateTask(int count)
    {
        if (count != 7) return;
        _count++;
        SetQuestTaskState(_count.ToString());

        if (_count >= _maxCount)
        {
            FinishTask();
        }
    }
}
