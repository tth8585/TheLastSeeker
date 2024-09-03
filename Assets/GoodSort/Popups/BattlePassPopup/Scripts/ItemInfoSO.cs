using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfoData", menuName = "ScriptableObjects/ItemInfoSO")]
public class ItemInfoSO : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public ITEM_TYPE ItemType;
    public List<ItemInfoSO> ChestItems;
    public List<int> ChestItemQuantity;
}
