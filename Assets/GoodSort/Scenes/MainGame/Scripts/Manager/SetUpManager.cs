using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpManager : MonoBehaviour
{
    private float _moveSpeed = 0f;
    private float _moveSmooth = 0f;

    public float GetMoveSpeed()
    {
        return _moveSpeed;
    }

    public float GetMoveSmooth() { return _moveSmooth; }

    public void SetMoveSpeed(float speed)
    {
        _moveSpeed = speed;
    }

    public void SetMoveSmooth(float smooth)
    {
        _moveSmooth = smooth;
    }
}
