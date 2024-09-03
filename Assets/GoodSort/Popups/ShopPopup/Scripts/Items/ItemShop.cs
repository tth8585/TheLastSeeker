using System;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class ItemShop : ItemPurchaseController
{
    [SerializeField] protected TMP_Text _price;
    [SerializeField] protected ItemShopSO _configItem;

    [Header("Discount")]
    [SerializeField] protected TMP_Text _discountValue;
    [SerializeField] protected TMP_Text _defautPrice; // this is default value in IAP catalog

    public ItemShopSO ItemSO => _configItem;
    public Action ON_PLAYER_PURCHASE;
    #region
    public override void InitData(Product product)
    {
        _model = product;

        if (_configItem.DiscountValue > 0)
        {
            ShowDiscount();
        }
        else
        {
            _price.SetText($"{product.metadata.localizedPriceString}");
        }
    }
    #endregion

    internal override void HandlePurchaseComplete()
    {
        base.HandlePurchaseComplete();
        ON_PLAYER_PURCHASE?.Invoke();
        // Handle for player reward
        // Save purchase data and UI changed
        //_packItem.isPurchased = true;

        //SaveBuyPackID(_packItem.packID);


        // Analytics
    }

    internal override void HandlePurchaseFailed()
    {
        base.HandlePurchaseFailed();

        // Show notification purchase failed
    }

    #region
    private void ShowDiscount()
    {
        if (_discountValue == null) return;

        _discountValue.gameObject.transform.parent.gameObject.SetActive(true);
        _discountValue.text = $"{_configItem.DiscountValue}%";

        var price = (float) _model.metadata.localizedPrice;
        var discountPrice = price - (price * (_configItem.DiscountValue / 100));
        _price.text = discountPrice.ToString();

        if (_defautPrice == null) return;
        _defautPrice.gameObject.SetActive(true);
        _defautPrice.text = _model.metadata.localizedPriceString;
    }
    #endregion
}
