using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Imba.UI;

public class ProfileItem : MonoBehaviour
{
    [SerializeField] private Image _avt;
    [SerializeField] private TMP_Text _priceTxt;
    [SerializeField] private Button _buyItemBtn;

    private ProfileItemPriceType _priceType;
    private ProfileItemInfo _itemInfo;
    private int _price;
    private bool _isPurchased = false;

    internal Action<ProfileItem, string, bool> OnClickItemBtn;
    internal Action OnItemDisable;
    internal Action OnItemHasBought;

    public void InitProfileItem(ProfileItemInfo itemInfo, bool isPurchased)
    {
        _itemInfo = itemInfo;
        _isPurchased = isPurchased;
        try
        {
            var avtSprite = Resources.Load<Sprite>($"Icons/Avatars/{_itemInfo.ID}");
            _avt.sprite = avtSprite;
        }
        catch
        {
            Debug.LogError("Item Img not found!");
        }
       

        InitPrice(_itemInfo.Price);
    }

    private void InitPrice(string price)
    {
        switch(price)
        {
            case "0":
                _priceType = ProfileItemPriceType.None;
                _isPurchased = true;
                break;
            case "ADS":
                _priceType = ProfileItemPriceType.Ads;
                _price = 0;
                break;
            default:
                _priceType = ProfileItemPriceType.Coin;
                int.TryParse(price, out _price);
                break;
        }

        if (_isPurchased)
        {
            _buyItemBtn.SetActive(false);
            return;
        }

        SetPrice(_priceType); 
    }

    private void SetPrice(ProfileItemPriceType type)
    {
        if(type == ProfileItemPriceType.None)
        {
            _buyItemBtn.SetActive(false);
            return;
        }

        _buyItemBtn.SetActive(true);
        if (_price > 0)
            _priceTxt.text = $"<sprite name={type}> <voffset=0.1em> {_price}"; 
        else
            _priceTxt.text = $"<sprite name={type}> <voffset=0.1em> 0/1";
    }

    private void OnBuyByCoin()
    {
        var currentCoin = MyUserData.Instance.UserDataSave.Coin;
        if(currentCoin < _price)
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.ShopPopup, true);
            UIManager.Instance.PopupManager.HidePopup(UIPopupName.ProfilePopup);
            return;
        }

        MyUserData.Instance.UpdateUserCurrency(-_price);

        if(_itemInfo.Type == ProfileItemType.Avatar.ToString())
        {
            MyUserData.Instance.UpdatePlayerAvatar(_itemInfo.ID);
            OnItemHasBought?.Invoke();
        }
        else
        {
            MyUserData.Instance.UpdatePlayerSkin(_itemInfo.ID);
        }
       
        _buyItemBtn.SetActive(false);
        _isPurchased = true;
    }

    private void OnDisable()
    {
        OnItemDisable?.Invoke();
    }

    #region On Click
    public void OnClickBuy()
    {
        if(_priceType == ProfileItemPriceType.Ads)
        {
            //Launch Ads
            _isPurchased = true;
        }
        else
        {
            OnBuyByCoin();
        }
    }

    public void OnClickToChange()
    {
        
        OnClickItemBtn?.Invoke(this, _itemInfo.ID, _isPurchased);
        
    }
    #endregion
}

public class ProfileItemInfo
{
    public string ID;
    public string Price;
    public string Type;
}

public enum ProfileItemType
{
    None,
    Avatar,
    Skin
}

public enum ProfileItemPriceType
{
    None,
    Ads,
    Coin
}
