using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USE_3_IN_GAME_ITEMS_Task : QuestTask
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
        switch (type)
        {
            case ITEM_TYPE.None:
                break;
            case ITEM_TYPE.DoubleStar:
                break;
            case ITEM_TYPE.Time:
                break;
            case ITEM_TYPE.Hit:
                UpdateQuestCount();
                break;
            case ITEM_TYPE.Coin:
                break;
            case ITEM_TYPE.BlueChest:
                break;
            case ITEM_TYPE.Refesh:
                UpdateQuestCount();
                break;
            case ITEM_TYPE.MagicStick:
                UpdateQuestCount();
                break;
            case ITEM_TYPE.Frozen:
                UpdateQuestCount();
                break;
            case ITEM_TYPE.Heart:
                break;
            case ITEM_TYPE.GreenChest:
                break;
            case ITEM_TYPE.VioletChest:
                break;
            case ITEM_TYPE.HeartUnlimited:
                break;
            case ITEM_TYPE.RubyChest:
                break;
            case ITEM_TYPE.GoldenChest:
                break;
            case ITEM_TYPE.Boom:
                break;
            case ITEM_TYPE.Star:
                break;
        }
    }
    private void UpdateQuestCount()
    {
        _count++;
        SetQuestTaskState(_count.ToString());

        if (_count >= _maxCount)
        {
            FinishTask();
        }
    }
}
