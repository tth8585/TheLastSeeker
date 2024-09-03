using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemPackShop", menuName = "ScriptableObjects/ItemShop/PackItem")]
public class ItemPackShopSO : ItemShopSO
{
    public Sprite Title;
    public Sprite Icon;

    public List<PackReward> Rewards;
}

[Serializable]
public class PackReward
{
    public ITEM_TYPE Type;
    public int Quantity;
}

