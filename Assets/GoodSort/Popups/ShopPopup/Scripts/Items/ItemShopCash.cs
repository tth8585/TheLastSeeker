using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ItemShopCash : ItemShop
{
    [Header("UI")]
    [SerializeField] private TMP_Text _packValue;
    [SerializeField] private Image _packIcon;

    [SerializeField] private Image _popularFlag;

    public override void InitData(Product product)
    {
        base.InitData(product);

        ItemCashShopSO cashSO = (ItemCashShopSO)_configItem;
        _packValue.text = $"+{cashSO.Quantity}";
        _packIcon.sprite = cashSO.Icon;

        if(cashSO.IsPopular) _popularFlag.gameObject.SetActive(cashSO.IsPopular);
    }

    internal override void HandlePurchaseComplete()
    {
        // Handle for player reward
        // Save purchase data and UI changed
        _purchaseBtn.enabled = true;
        // Analytics

#if UNITY_EDITOR
        GetItemOnPurchased();
        return;
#endif
    }

    internal override void HandlePurchaseFailed()
    {
        base.HandlePurchaseFailed();
#if UNITY_EDITOR
        GetItemOnPurchased();
        return;
#endif
    }

    private void GetItemOnPurchased()
    {
        ItemCashShopSO cashSO = (ItemCashShopSO)_configItem;
        MyUserData.Instance.UpdateItemInfo(ITEM_TYPE.Coin, int.Parse(cashSO.Quantity));
    }
}
