using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBarController : MonoBehaviour
{
    [SerializeField] ItemAbilityUIController[] _arrItem;

    private void OnEnable()
    {
        MyEvent.Instance.UserDataManagerEvent.onItemChanged += UpdateUI;
    }

    private void OnDisable()
    {
        MyEvent.Instance.UserDataManagerEvent.onItemChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        InitData();
    }

    public void InitData()
    {
        for(int i=0;i< _arrItem.Length;i++)
        {
            ItemInfoSO itemInfo = MyItemAbility.Instance.GetItemInfoByType(_arrItem[i].Type);
            int quantity = MyUserData.Instance.DicItemDatas[itemInfo.ItemType];
            _arrItem[i].InitData(quantity, itemInfo);
        }
    }

    public RectTransform GetRectAbility(int index)
    {
        return _arrItem[index].GetComponent<RectTransform>();
    }
}
