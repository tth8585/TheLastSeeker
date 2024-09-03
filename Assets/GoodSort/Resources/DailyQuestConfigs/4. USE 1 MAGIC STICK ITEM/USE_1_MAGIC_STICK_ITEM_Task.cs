using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USE_1_MAGIC_STICK_ITEM_Task : QuestTask
{
    protected override void SetQuestTaskState(string state)
    {
        ChangeState(state);
    }

    private void OnEnable()
    {
        MyEvent.Instance.QuestEvents.onUseItem += UpdateTask;
    }

    private void OnDisable()
    {
        MyEvent.Instance.QuestEvents.onUseItem -= UpdateTask;
    }

    private void UpdateTask(ITEM_TYPE type)
    {
        if (type != ITEM_TYPE.MagicStick) return;
        _count++;
        SetQuestTaskState(_count.ToString());

        if (_count >= _maxCount)
        {
            FinishTask();
        }
    }
}
