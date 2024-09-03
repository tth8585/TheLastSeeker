using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CanBeDestroySlot : SlotController
{
    private float _timeAnim = 0.5f;
    private float _spawnPosY = 5f;
    private bool _canRefill = true;

    public override void InitData(List<int> dataItem)
    {
        _data = dataItem;

        _numberLayer = 1;

        InitLayer();
    }

    private void InitNewLayer(List<int> dataItem)
    {
        _data = dataItem;

        int value1 = -1, value2 = -1, value3 = -1;
        List<int> newList = new List<int>();
        newList.Add(value1);
        newList.Add(value2);
        newList.Add(value3);
        if (_data.Count>0) newList[0] = _data[0];
        if (_data.Count>1) newList[1] = _data[1];
        if (_data.Count>2) newList[2] = _data[2];
        newList = Utils.Shuffle(newList);

        _listLayer[0].InitRespawnData(newList[0], newList[1], newList[2]);
    }

    public override void CheckLayer()
    {
        bool isMatch = _listLayer[0].CheckMatch();
        bool isEmpty = _listLayer[0].CheckEmpty();

        if (!_canRefill)
        {
            SendEventCount();
            return;
        }

        if (isMatch)
        {
            //destroy anim and set to start spawn if there is Item to fill
            _listLayer[0].HideItem();
            DoDestroyAnim();
            if (CanFillItem())
            {
                DoRespawn();
            }
            else
            {
                _canRefill = false;
                isMatch = false;
            }
            var matchPos = Camera.main.WorldToScreenPoint(this.transform.position);
            MyEvent.Instance.GameEventManager.MatchItem(matchPos);
        }

        if (isEmpty)
        {
            //destroy anim and set to start spawn if there is Item to fill
            DoDestroyAnim();

            if (CanFillItem())
            {
                DoRespawn();
            }
            else
            {
                _canRefill = false;
                isEmpty = false;
            }
        }

        if (!isMatch && !isEmpty)
        {
            SendEventCount();
        }
        CheckShowHidePanel();
    }

    private bool CanFillItem()
    {
        return MySpawn.Instance.CanGetNextItemToFill();
    }

    public bool CanRefill()
    {
        return _canRefill;
    }

    private void DoDestroyAnim()
    {
        List<DestroyModeSlotList> list = MySpawn.Instance.GetAllDestroySlot();

        for(int i=0;i<list.Count; i++)
        {
            List<CanBeDestroySlot> newList = list[i].ListSlot;
            if (newList.Contains(this))
            {
                List<CanBeDestroySlot> listDoAnim = GetListSlotDoAnim(newList);

                for(int j = 0; j < listDoAnim.Count; j++)
                {
                    if (listDoAnim[j].CanRefill()) listDoAnim[j].MoveDown();
                }

                ActiveDestroyAnim();
                //set pos Spawn
                SetPosAboveCamera();
                return;
            }
        }
    }

    private void DoRespawn()
    {
        //fill item
        List<int> newData = MySpawn.Instance.GetNextFillId();
        Debug.Log("xx DoRespawn: "+newData.Count);
        InitNewLayer(newData);
        //done rewpawn
        CheckLayer();
    }

    private List<CanBeDestroySlot> GetListSlotDoAnim(List<CanBeDestroySlot> list)
    {
        float yPos= transform.localPosition.y;
        List<CanBeDestroySlot> newList= new List<CanBeDestroySlot>();
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].transform.localPosition.y > yPos)
            {
                newList.Add(list[i]);
            }
        }
        return newList;
    }

    #region ANIM
    private void MoveDown()
    {
        transform.DOLocalMoveY(transform.localPosition.y - 1, _timeAnim).SetEase(Ease.OutBounce);
    }

    private void ActiveDestroyAnim()
    {
        Debug.LogError("destroy slot anim hot handle");
    }

    private void SetPosAboveCamera()
    {
        transform.localPosition= new Vector3(transform.position.x, _spawnPosY, transform.position.z);
    }
    #endregion
}
