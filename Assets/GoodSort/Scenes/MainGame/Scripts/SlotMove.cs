using UnityEngine;

public class SlotMove : MonoBehaviour
{
    private float _moveSpeed = 0f;
    private float _smoothDelta = 0f;
    private bool isMoving = false;
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private MOVE_SLOT_TYPE _type;

    public void InitDataForMove(MOVE_SLOT_TYPE type)
    {
        _type = type;
        switch (type)
        {
            case MOVE_SLOT_TYPE.MOVE_RIGHT:
                _startPoint = new Vector3(MyMovePos.Instance.GetXPosForMoveLeft(), transform.position.y,transform.position.z);
                _endPoint= new Vector3(MyMovePos.Instance.GetXPosForMoveRight(), transform.position.y, transform.position.z);
                break;
            case MOVE_SLOT_TYPE.MOVE_LEFT:
                _startPoint = new Vector3(MyMovePos.Instance.GetXPosForMoveRight(), transform.position.y, transform.position.z);
                _endPoint = new Vector3(MyMovePos.Instance.GetXPosForMoveLeft(), transform.position.y, transform.position.z);
                break;
            case MOVE_SLOT_TYPE.MOVE_UP:
                _startPoint = new Vector3(transform.position.x, MyMovePos.Instance.GetYPosForMoveDown(), transform.position.z);
                _endPoint = new Vector3(transform.position.x, MyMovePos.Instance.GetYPosForMoveUp(), transform.position.z);
                break;
            case MOVE_SLOT_TYPE.MOVE_DOWN:
                _startPoint = new Vector3(transform.position.x, MyMovePos.Instance.GetYPosForMoveUp(), transform.position.z);
                _endPoint = new Vector3(transform.position.x, MyMovePos.Instance.GetYPosForMoveDown(), transform.position.z);
                break;
            default:
                break;
        }

        isMoving = true;
        _smoothDelta = MyGame.Instance.GetMoveSmooth();
        _moveSpeed = MyGame.Instance.GetMoveSpeed();

    }

    void Update()
    {
        if (isMoving && _type!= MOVE_SLOT_TYPE.STAY)
        {
            float adjustedDeltaTime = Time.deltaTime * _smoothDelta; // Slightly higher deltaTime (adjust as needed)
            float movementAmount = _moveSpeed * adjustedDeltaTime;

            transform.position = Vector3.MoveTowards(transform.position, _endPoint, movementAmount);
            if (Vector3.Distance(transform.position, _endPoint) < 0.1f)
            {
                transform.position = _startPoint; // Immediately reset position to A
            }
        }
    }
}
