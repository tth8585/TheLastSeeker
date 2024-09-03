using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer _skin;
    [SerializeField] Animator _animator;
    [SerializeField] Material[] _materials;

    internal void InitMat(bool isTeamA)
    {
        if (isTeamA)
        {
            transform.localEulerAngles= Vector3.zero;
            _skin.material= _materials[0];
        }
        else
        {
            transform.localEulerAngles = new Vector3(0,-180,0);
            _skin.material = _materials[1];
        }
    }
}
