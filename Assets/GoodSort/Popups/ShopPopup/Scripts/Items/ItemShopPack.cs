using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using System;

public class ItemShopPack : ItemShop
{
    [Header("UI")]
    [SerializeField] private Image _packTitle;
    [SerializeField] private Image _packIcon;
    [SerializeField] private List<PackReward> _packReward;
    [SerializeField] private TMP_Text _newOfferTimeTxt;

    [Header("Item Reward")]
    [SerializeField] private TMP_Text _coinQuantity;
    [SerializeField] private ItemReward _itemReward;
    [SerializeField] private Transform _itemRewardContainer;
    bool isInitReward = false;

    public override void InitData(Product product)
    {
        base.InitData(product);

        ItemPackShopSO packSO = (ItemPackShopSO)_configItem;
        _packTitle.sprite = packSO.Title;
        _packIcon.sprite = packSO.Icon;
        _packReward = packSO.Rewards;

        if (!isInitReward)
        {
            InitPackRewardItem();
        }

        if (packSO.DiscountValue > 0)
        {
            ShopPopup.ON_OFFER_TIME_CHANGE += ShowSpecialOfferCountdownTime;
        }
    }

    private void InitPackRewardItem()
    {
        isInitReward = true;
        foreach (var item in _packReward)
        {
            if (item.Type == ITEM_TYPE.Coin)
            {
                _coinQuantity.text = $"x{item.Quantity}";
                continue;
            }

            var itemR = Instantiate(_itemReward, _itemRewardContainer);
            itemR.InitItem(item);
        }
    }

    internal override void HandlePurchaseComplete()
    {
        base.HandlePurchaseComplete();
        ShopPopup.ON_OFFER_TIME_CHANGE -= ShowSpecialOfferCountdownTime;

        GetItemOnPurchased();

        // Analytics

    }

    private void GetItemOnPurchased()
    {
        foreach (PackReward item in _packReward)
        {
            MyUserData.Instance.UpdateItemInfo(item.Type, item.Quantity);
        }
    }

    internal override void HandlePurchaseFailed()
    {
        base.HandlePurchaseFailed();
        // Show notification purchase failed
#if UNITY_EDITOR
        GetItemOnPurchased();
        return;
#endif

    }

    #region Show count down time to new special offer
    private void ShowSpecialOfferCountdownTime()
    {
        _newOfferTimeTxt.gameObject.transform.parent.gameObject.SetActive(true);
        _newOfferTimeTxt.text = ShopPopup.GetRemainingSpecialOfferTime();
    }
    #endregion
}
