using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockFPS : MonoBehaviour
{
    [SerializeField] private int _targetFPS = 60;
    void Start()
    {
        Application.targetFrameRate = _targetFPS;
    }
}
