using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    [SerializeField] uint _slotIndex;
    [SerializeField] CharacterController _character;
    [SerializeField] GameObject _nailObject;

    internal void InitData(SlotData slotData, bool isTeamA)
    {
        bool isInEditTeam = false;

        _nailObject.SetActive(isInEditTeam);

        if(slotData.Empty)
        {
            _character.gameObject.SetActive(false);
        }
        else
        {
            _character.gameObject.SetActive(true);
            //for testing now
            _character.InitMat(isTeamA);
        }
    }

    public uint GetSlotId() { return _slotIndex; }
}
