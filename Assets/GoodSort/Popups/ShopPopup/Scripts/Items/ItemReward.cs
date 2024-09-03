using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemReward : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TMP_Text _itemQuantity;

    public void InitItem(PackReward info)
    {
        ItemInfoSO itemInfo = MyItemAbility.Instance.GetItemInfoByType(info.Type);
        _itemIcon.sprite = itemInfo.Sprite;
        _itemQuantity.text = $"x{info.Quantity}";
    }
}
