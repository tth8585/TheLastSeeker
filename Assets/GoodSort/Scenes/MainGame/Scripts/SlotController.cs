using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

[System.Serializable]
public class SlotController : MonoBehaviour
{
    [SerializeField] LayerController[] _layerController;
    [SerializeField] GameObject _lockObj;
    [SerializeField] TMP_Text _lockTurnText;

    private SlotData _slotData;

    protected List<int> _data;
    protected LayerController _currentLayerController;
    protected int _numberLayer;
    protected List<LayerController> _listLayer = new List<LayerController>();

    public int GetIdSlot()
    {
        Match match = Regex.Match(transform.name, @"\((\d+)\)");
        if (match.Success)
        {
            string numberString = match.Groups[1].Value;
            if (int.TryParse(numberString, out int number))
            {
                return number;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }

    private void Start()
    {
        _gdText.gameObject.SetActive(false);
    }

    public virtual void CheckLayer()
    {
        bool isMatch = _listLayer[0].CheckMatch();
        bool isEmpty = _listLayer[0].CheckEmpty();

        if (isMatch)
        {
            _listLayer[0].HideItem();
            RemoveLayer();
          
            var matchPos = Camera.main.ScreenToViewportPoint(new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 5f));
            //var matchPos = Camera.main.ScreenToViewportPoint(matchPos1);
            Debug.Log($"<color=grey>{matchPos} </color>");
            MyEvent.Instance.GameEventManager.MatchItem(this.transform.position);// get position in this
        }

        if (isEmpty)
        {
            if(CanGoNextLayer()) RemoveLayer();
            else isEmpty = false;
        }

        if (!isMatch && !isEmpty)
        {
            Debug.Log("xx SendEventCount");
            SendEventCount();
        }

        CheckShowHidePanel();
    }

    public bool CheckDone()
    {
        for(int i=0;i< _listLayer.Count; i++)
        {         
            if (!_listLayer[i].CheckEmpty())
            {
                return false;
            }
        }
        return true;
    }

    public void CheckShowHidePanel()
    {
        Debug.Log("Check CheckShowHidePanel");
        for (int i = 0; i < _listLayer.Count; i++)
        {
            if (!_listLayer[i].CheckEmpty())
            {
                if (i+1 < _listLayer.Count)
                {
                    if (_listLayer[i + 1].CheckEmpty())
                    {
                        _listLayer[i].ShowHidePlane(false);
                    }
                    else
                    {
                        _listLayer[i].ShowHidePlane(true);
                    }
                }
                else
                {
                    _listLayer[i].ShowHidePlane(false);
                }
            }
            else
            {
                _listLayer[i].ShowHidePlane(false);
            }
        }
    }

    protected void SendEventCount()
    {
        MyEvent.Instance.GameEventManager.SlotDoneProgress();
    }

    public void SetUp(SlotData data)
    {
        _slotData = data;
        if (_slotData.isSpecial)
        {
            _currentLayerController = _layerController[0];
        }
        else
        {
            _currentLayerController = _layerController[1];
        }

        _normalDeco.SetActive(!_slotData.isSpecial);
        _specialDeco.SetActive(_slotData.isSpecial);

        GetComponent<SlotMove>().InitDataForMove(data.moveType);

        LockSlot(_slotData.isLocked);
    }

    public virtual void InitData(List<int> dataItem)
    {
        //Debug.Log("init layer number: "+dataItem.Count);
        _data = dataItem;

        int countItemPerlayer = _currentLayerController.GetNumberSlot();

        if (_data.Count % countItemPerlayer == 0)
        {
            _numberLayer = _data.Count / countItemPerlayer;
        }
        else
        {
            _numberLayer = _data.Count / countItemPerlayer + 1;
        }

        InitLayer();

        //Debug.Log("layer init: "+transform.name+"/"+dataItem.Count+"/"+_numberLayer);
    }

    protected virtual void InitLayer()
    {
        for(int i = 0; i < _numberLayer; i++)
        {
            GameObject obj= Instantiate(_currentLayerController.gameObject,this.transform);
            LayerController layer= obj.GetComponent<LayerController>();

            if (layer.GetNumberSlot()==1)
            {
                layer.InitData(_data[i]);
            }
            else
            {
                int value1 = -1, value2 = -1, value3 = -1;
                List<int> newList = new List<int>();
                newList.Add(value1);
                newList.Add(value2);
                newList.Add(value3);
                if (i * 3 < _data.Count) newList[0] = _data[i * 3];
                if (i * 3 + 1 < _data.Count) newList[1] = _data[i * 3 + 1];
                if (i * 3 + 2 < _data.Count) newList[2] = _data[i * 3 + 2];
                newList = Utils.Shuffle(newList);

                layer.InitData(newList[0], newList[1], newList[2]);
            }

            if (i == 0) 
            {
                layer.OnEnableCollider();
                //layer.ShowHidePlane(false);
            }
            else
            {
               // layer.ShowHidePlane(true);
            }

            layer.SetLocalPos(i * 0.5f);
            _listLayer.Add(layer);

            CheckShowHidePanel();
        }
    }

    private void RemoveLayer()
    {
        if(_listLayer.Count == 1)
        {
            CheckLayer();
        }
        else
        {
            //move forward all layer
            for (int i = 0; i < _listLayer.Count; i++)
            {
                if (i + 1 < _listLayer.Count)
                {
                    if (i == 0)
                    {
                        _listLayer[i].MoveForWard(_listLayer[i + 1], CheckLayer);
                    }
                    else
                    {
                        _listLayer[i].MoveForWard(_listLayer[i + 1], null);
                    }
                }
                else
                {
                    _listLayer[i].MoveForWard(null, null);
                }
            }
        }
    }

    private bool CanGoNextLayer()
    {
        for (int i = 0; i < _listLayer.Count; i++)
        {
            if (!_listLayer[i].CheckEmpty()) return true;
        }

        return false;
    }

    public int GetItemCountToFillLayer()
    {
        GameObject obj = Instantiate(_currentLayerController.gameObject);
        LayerController layer = obj.GetComponent<LayerController>();
        int count= layer.GetNumberSlot();
        Destroy(obj);
        return count;
    }

    public List<int> GetListIdOfLayer(int layerIndex)
    {
        if (layerIndex > _listLayer.Count - 1) 
        {
            InitExtraLayer();
        }

        return _listLayer[layerIndex].GetListId();
    }

    public List<int> GetListIdOfLayer(int layerIndex, int excludeId)
    {
        if (layerIndex > _listLayer.Count - 1)
        {
            InitExtraLayer();
        }

        return _listLayer[layerIndex].GetListId(excludeId);
    }

    private void InitExtraLayer()
    {
        GameObject obj = Instantiate(_currentLayerController.gameObject, this.transform);
        LayerController layer = obj.GetComponent<LayerController>();

        if (layer.GetNumberSlot() == 1)
        {
            layer.InitData(-1);
        }
        else
        {
            List<int> newList = new List<int>();
            newList.Add(-1);
            newList.Add(-1);
            newList.Add(-1);

            newList = Utils.Shuffle(newList);

            layer.InitData(newList[0], newList[1], newList[2]);
        }

        int currentIndex = _listLayer.Count;
        layer.SetLocalPos(currentIndex * 0.5f);
        _listLayer.Add(layer);
    }

    public void InitReplaceNewListIndex(int layerIndex, List<int> newIdList) 
    {
        if(GetItemCountToFillLayer() == 1)
        {
            _listLayer[layerIndex].InitRespawnData(newIdList[0]);
        }
        else
        {
            _listLayer[layerIndex].InitRespawnData(newIdList[0], newIdList[1], newIdList[2]);
        }
    }

    #region ITEM ABILITY
    public List<ItemSlot> GetSlot(int id)
    {
        List<ItemSlot> newList= new List<ItemSlot>();

        for (int i = 0; i < _listLayer.Count; i++)
        {
            LayerController layer = _listLayer[i];
            List<ItemSlot> slotList = layer.GetSlotWithId(id);
            if(slotList != null) newList.AddRange(layer.GetSlotWithId(id));
        }
        return newList;
    }

    public List<ItemSlot> GetSurfaceSlot()
    {
        List<ItemSlot> newList = new List<ItemSlot>();

        ItemSlot[] slotArr = _listLayer[0].GetSlot();
        if (slotArr != null) 
        {
            for(int i = 0; i < slotArr.Length; i++)
            {
                newList.Add(slotArr[i]);
            }
        }

        return newList;
    }
    #endregion

    #region LOCK
    public void CountToUnlock()
    {
        _slotData.turnCount--;
        _lockTurnText.text = _slotData.turnCount.ToString();
        if (_slotData.turnCount <= 0 && _slotData.isLocked)
        {
            LockSlot(false);
            MyGameFx.Instance.PlayUnLockSlotFxAtPos(transform.position);
        }

        if (_slotData.turnCount == 1)
        {
            //anim spcial here ?
        }
    }

    public int GetCountToUnlock()
    {
        return _slotData.turnCount;
    }

    private void LockSlot(bool isLock)
    {
        _slotData.isLocked = isLock;
        _lockObj.SetActive(isLock);
        _lockTurnText.text = _slotData.turnCount.ToString();

        if (isLock)
        {
            Debug.Log("Lock slot ::" + _slotData.turnCount);
        }
    }

    public bool IsLocked()
    {
        return _slotData.isLocked;
    }

    #endregion

    #region FOR GD
    [SerializeField] TMP_Text _gdText, _gdLock, _gdLockCount;
    [SerializeField] GameObject _gdMoveObj;
    [SerializeField] GameObject[] _arrows;
    [SerializeField] GameObject _normalDeco, _specialDeco;

    public void ShowForGD(bool isShow)
    {
        _gdText.gameObject.SetActive(isShow);
        _gdText.text = GetIdSlot().ToString();
    }

    public void ShowSpecial(bool isSpecial)
    {
        _normalDeco.SetActive(!isSpecial);
        _specialDeco.SetActive(isSpecial);
    }

    public void ShowLockOptions(int countLock)
    {
        if (countLock == 0)
        {
            _gdLock.gameObject.SetActive(false);
            _gdLockCount.text = countLock.ToString();
        }
        else
        {
            _gdLock.gameObject.SetActive(true);
            _gdLockCount.text = countLock.ToString();
        }
    }

    public void ShowMove(int moveType) 
    {
        MOVE_SLOT_TYPE type = (MOVE_SLOT_TYPE)moveType;
        if(type== MOVE_SLOT_TYPE.STAY) _gdMoveObj.gameObject.SetActive(false);
        else
        {
            _gdMoveObj.gameObject.SetActive(true);
            for (int i = 0; i < _arrows.Length; i++)
            {
                if(i== moveType-1) _arrows[i].gameObject.SetActive(true);
                else _arrows[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion
}
