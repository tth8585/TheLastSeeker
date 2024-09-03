using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public QuestState State;
    public int QuestTaskIndex;
    public QuestTaskState[] QuestTaskStates;
    public bool ClaimedReward;

    public QuestData(QuestState state, int questTaskIndex, QuestTaskState[] questTaskStates, bool claimedReward)
    {
        //Quest Progress 
        State = state;
        //task index hien tai cua player
        QuestTaskIndex = questTaskIndex;
        //state cua Task hien tai la 1 mang string
        QuestTaskStates = questTaskStates;
        //check xem player da nhan reward chua
        ClaimedReward = claimedReward;
    }
}

