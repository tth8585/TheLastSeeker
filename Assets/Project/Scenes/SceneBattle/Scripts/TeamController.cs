using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    [SerializeField] bool _isTeamA = true;
    [SerializeField] SlotController[] _slots;

    internal void InitData(TeamData data) 
    {
        for(int i=0;i<data.SlotDatas.Count;i++)
        {
            for(int j=0;j<_slots.Length;j++)
            {
                if (data.SlotDatas[i].SlotId== _slots[j].GetSlotId())
                {
                    _slots[j].InitData(data.SlotDatas[i], _isTeamA);
                }
            }
        }
    }
}

public class TeamData
{
    public List<SlotData> SlotDatas;
}

public class SlotData
{
    public uint SlotId;
    public bool Empty;
}
