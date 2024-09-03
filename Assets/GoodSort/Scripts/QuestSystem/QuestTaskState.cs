using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestTaskState
{
    public string State;

    //for task like multy step
    public QuestTaskState(string state)
    {
        this.State = state;
    }

    //for task require moving from A to B location
    public QuestTaskState()
    {
        this.State = "";
    }
}
