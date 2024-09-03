using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;

public class MovePositionManager : MonoBehaviour
{
    [SerializeField] Transform _moveLeftAPos, _moveRightBPos;
    [SerializeField] Transform _moveUpAPos, _moveDownBPos;

    private float CalculateS(MOVE_SLOT_TYPE moveType)
    {
        switch (moveType)
        {
            case MOVE_SLOT_TYPE.MOVE_RIGHT:
            case MOVE_SLOT_TYPE.MOVE_LEFT:
                return Vector3.Distance(new Vector3(_moveLeftAPos.position.x,0,0), new Vector3(_moveRightBPos.position.x,0,0));
            case MOVE_SLOT_TYPE.MOVE_UP:
            case MOVE_SLOT_TYPE.MOVE_DOWN:
                return Vector3.Distance(new Vector3(0, _moveUpAPos.position.y, 0), new Vector3(0, _moveDownBPos.position.y, 0));
            default:
                return 0;
        }
    }

    public float CalculateS(MOVE_SLOT_TYPE moveType, Vector3 startPos)
    {
        switch (moveType)
        {
            case MOVE_SLOT_TYPE.STAY:
                return 0;
            case MOVE_SLOT_TYPE.MOVE_RIGHT:
                return Vector3.Distance(startPos, new Vector3(_moveRightBPos.position.x,startPos.y,startPos.z));
            case MOVE_SLOT_TYPE.MOVE_LEFT:
                return Vector3.Distance(startPos, new Vector3(_moveLeftAPos.position.x, startPos.y, startPos.z));
            case MOVE_SLOT_TYPE.MOVE_UP:
                return Vector3.Distance(startPos, new Vector3(startPos.x, _moveUpAPos.position.y, startPos.z));
            case MOVE_SLOT_TYPE.MOVE_DOWN:
                return Vector3.Distance(startPos, new Vector3(startPos.x, _moveDownBPos.position.y, startPos.z));
            default: 
                return 0;
        }
    }

    public float GetXPosForMoveRight()
    {
        return _moveRightBPos.position.x;
    }

    public float GetXPosForMoveLeft()
    {
        return _moveLeftAPos.position.x;
    }

    public float GetYPosForMoveUp()
    {
        return _moveUpAPos.position.y;
    }

    public float GetYPosForMoveDown()
    {
        return _moveDownBPos.position.y;
    }
}

public class MyMovePos : SingletonMonoBehaviour<MovePositionManager> { }
