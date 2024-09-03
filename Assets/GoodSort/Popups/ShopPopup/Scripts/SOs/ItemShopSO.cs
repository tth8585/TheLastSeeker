using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewItemPackShop", menuName = "ScriptableObjects/ItemShop/ShopItem")]
public class ItemShopSO : ScriptableObject
{
    public ItemShopType ItemType;
    public string ItemID;
    public float DiscountValue;
}

public enum ItemShopType
{
    None,
    GamePass,
    NoAds,
    Pack,
    Coin
}