using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    INIT,
    START,
    CALCULATE,
    ENDGAME,
}

public class SceneBattleController : MonoBehaviour
{
    [SerializeField] TeamController _teamA, _teamB;

    private BattleState _currentState = BattleState.INIT;

    private void Start()
    {
        ChangeBattleState(BattleState.INIT);
    }

    private TeamData CreateDefaultData()
    {
        TeamData data = new TeamData();

        data.SlotDatas = new List<SlotData>();

        data.SlotDatas.Add(CreateDefaultSlotData(0));

        SlotData slotData = CreateDefaultSlotData(1);
        slotData.Empty = false;
        data.SlotDatas.Add(slotData);

        data.SlotDatas.Add(CreateDefaultSlotData(2));
        data.SlotDatas.Add(CreateDefaultSlotData(3));
        data.SlotDatas.Add(CreateDefaultSlotData(4));
        data.SlotDatas.Add(CreateDefaultSlotData(5));
        data.SlotDatas.Add(CreateDefaultSlotData(6));
        data.SlotDatas.Add(CreateDefaultSlotData(7));
        data.SlotDatas.Add(CreateDefaultSlotData(8));

        return data;
    }

    private SlotData CreateDefaultSlotData(uint index)
    {
        SlotData data = new SlotData();
        data.SlotId = index;
        data.Empty = true;
        return data;
    }

    private void ChangeBattleState(BattleState state)
    {
        Debug.Log("change game state: "+ _currentState+"==>"+ state);
        _currentState = state;
        switch (state)
        {
            case BattleState.INIT:
                HandleInit();
                break;
            case BattleState.START:
                HandleStart();
                break;
            case BattleState.CALCULATE:
                HandleCalculate();
                break;
            case BattleState.ENDGAME:
                break;
            default:
                Debug.LogError("something happend cant change state: " + state);
                break;
        }
    }

    private void HandleCalculate()
    {
        
    }

    private void HandleStart()
    {
        ChangeBattleState(BattleState.CALCULATE);
    }

    private void HandleInit()
    {
        TeamData dataA = CreateDefaultData();
        _teamA.InitData(dataA);
        _teamB.InitData(dataA);
        ChangeBattleState(BattleState.START);
    }
}
