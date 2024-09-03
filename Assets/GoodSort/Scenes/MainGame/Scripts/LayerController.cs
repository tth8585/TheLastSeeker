using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerController : MonoBehaviour
{
    [SerializeField] ItemSlot[] _itemSlots;
    [SerializeField] GameObject _hidePlane;

    public void InitData(int id1,int id2,int id3)
    {
        _itemSlots[0].InitData(id1);
        _itemSlots[1].InitData(id2);
        _itemSlots[2].InitData(id3);
    }

    public void InitRespawnData(int id1, int id2, int id3)
    {
        _itemSlots[0].InitRespawnData(id1);
        _itemSlots[1].InitRespawnData(id2);
        _itemSlots[2].InitRespawnData(id3);
    }

    public void InitRespawnData(int id)
    {
        _itemSlots[0].InitRespawnData(id);
    }

    public void InitData(int id)
    {
        _itemSlots[0].InitData(id);
    }

    public void ShowHidePlane(bool isShow)
    {
        _hidePlane.SetActive(isShow);
    }

    public int GetNumberSlot()
    {
        return _itemSlots.Length;
    }

    public void OnEnableCollider()
    {
        StartCoroutine(COEnableCollider());
    }

    public bool CheckEmpty()
    {
        for(int i=0; i< _itemSlots.Length; i++)
        {
            if (!_itemSlots[i].IsEmptyItem())
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator COEnableCollider()
    {
        yield return new WaitForEndOfFrame();
        for(int i=0 ; i< _itemSlots.Length; i++)
        {
            _itemSlots[i].EnableCollider();
        }
    }

    public virtual bool CheckMatch()
    {
        if (CheckEmpty()) return false;
        if (_itemSlots.Length == 1) return false;

        int index1 = _itemSlots[0].GetItemIndex();
        int index2= _itemSlots[1].GetItemIndex();
        int index3= _itemSlots[2].GetItemIndex();
        if(index1 == index2 && index1 == index3) return true;

        return false;
    }

    public void HideItem()
    {
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            _itemSlots[i].HideItem();
        }
    }

    public void MoveForWard(LayerController newLayer, TweenCallback callback)
    {
        if(newLayer != null)
        {
            for(int i=0;i<_itemSlots.Length;i++)
            {
                if(i== _itemSlots.Length - 1)
                {
                    _itemSlots[i].SwapItem(newLayer.GetItemSlot(i), callback);
                }
                else
                {
                    _itemSlots[i].SwapItem(newLayer.GetItemSlot(i));
                }
            }
        }
    }

    public ItemSlot GetItemSlot(int index)
    {
        return _itemSlots[index];
    }

    public void SetLocalPos(float localZ)
    {
        transform.localPosition= Vector3.zero+ new Vector3(0,0,localZ);
    }

    public List<int> GetListId()
    {
        List<int> newList= new List<int>();
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            newList.Add(_itemSlots[i].GetItemIndex());
        }
        return newList;
    }

    public List<int> GetListId(int excludeId)
    {
        List<int> newList = new List<int>();
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            if (excludeId == -1) //1st time take id item
            {
                newList.Add(_itemSlots[i].GetItemIndex());
            }
            else
            {
                if (_itemSlots[i].GetItemIndex() != excludeId)
                {
                    newList.Add(_itemSlots[i].GetItemIndex());
                }
            }
        }
        return newList;
    }

    public List<ItemSlot> GetSlotWithId(int id)
    {
        List<ItemSlot> newList= new List<ItemSlot>();
        for(int i = 0; i < _itemSlots.Length; i++)
        {
            if (_itemSlots[i].GetItemIndex() == id) newList.Add(_itemSlots[i]);
        }

        return newList;
    }

    public ItemSlot[] GetSlot()
    {
        return _itemSlots;
    }
}
