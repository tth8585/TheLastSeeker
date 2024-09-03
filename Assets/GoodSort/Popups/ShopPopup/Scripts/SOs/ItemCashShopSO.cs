using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCashItemShop", menuName = "ScriptableObjects/ItemShop/CashItem")]
public class ItemCashShopSO : ItemShopSO
{
    public string Quantity;
    public Sprite Icon;
    public bool IsPopular;
}


