using Imba.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ADSRewardPopup : UIPopup
{
    [SerializeField] private TMP_Text _itemName, _itemDesc, _itemPriceTxt;
    [SerializeField] private Transform _itemContainerTrans;
    [SerializeField] private GameObject _rewardItemPrefab;
    [SerializeField] private ADSRewardSO _adsRewardConfig;

    private List<GameObject> _itemObjs = new List<GameObject>();
    private int _itemPrice;
    private List<ITEM_TYPE> _items;

    #region Popup control
    protected override void OnShowing()
    {
        ADSRewardInfo rewardInfo = GetADSRewardInfo();

        if (rewardInfo == null) 
        {
            Debug.Log($"<color=red>---Reward Info null---</color>");
            UIManager.Instance.PopupManager.HidePopup(UIPopupName.ADSRewardPopup);
            return;
        }

        _items = rewardInfo.Types;
        InitRewardIcon();
        InitRewardInfo(rewardInfo);

       
    }

    protected override void OnShown()
    {
        if (MyGame.Instance != null && MyGame.Instance.CurrentState == GAMEPLAY_STATE.DRAG_AND_DROP)
            MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.PAUSE);
    }

    protected override void OnHidden()
    {
        if (MyGame.Instance != null && MyGame.Instance.CurrentState == GAMEPLAY_STATE.PAUSE)
            MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.RESUME);

        foreach (var item in _itemObjs) item.SetActive(false);
    }

    #endregion

    #region Handle Method
    private ADSRewardInfo GetADSRewardInfo()
    {
        ADSRewardInfo rewardInfo = null;
        if (Parameter.GetType() == typeof(ADSRewardType))
        {
            var adsRewardType = (ADSRewardType)Parameter;
            rewardInfo = _adsRewardConfig.AdsRewards.Where(a => a.ADSRewardType == adsRewardType).FirstOrDefault();
        }
        else
        {
            var adsRewardType = (ITEM_TYPE)Parameter;
            rewardInfo = _adsRewardConfig.AdsRewards.Where(a => a.Types[0] == adsRewardType).FirstOrDefault();
        }
        return rewardInfo;
    }

    private void InitRewardInfo(ADSRewardInfo adsRewardInfo)
    {
        _itemName.text = adsRewardInfo.RewardTitle;
        _itemDesc.text = adsRewardInfo.RewardDescription;

        _itemPrice = adsRewardInfo.Price;
        _itemPriceTxt.text = $"{_itemPrice}";
    }

    private void InitRewardIcon()
    {
        if (_items.Count == 1)
        {
            InitRewardIconByIndex(0);
        }
        else
        {
            for (int i = 0; i < _items.Count; i++)
            {
                InitRewardIconByIndex(i);
            }
        }
    }

    private void InitRewardIconByIndex(int index)
    {
        if(_itemObjs.Count > index)
        {
            var itemInfo = MyItemAbility.Instance.GetItemInfoByType(_items[index]);
            _itemObjs[index]?.SetActive(true);
            _itemObjs[index].GetComponent<Image>().sprite = itemInfo.Sprite;
        }
        else
        {
            var itemInfo = MyItemAbility.Instance.GetItemInfoByType(_items[index]);
            var itemInstant = Instantiate(_rewardItemPrefab, _itemContainerTrans);
            itemInstant.GetComponent<Image>().sprite = itemInfo.Sprite;

            _itemObjs.Add(itemInstant);
        }
    }

    private void OnBoughtItem()
    {
        foreach (var item in _items)
        {
            MyUserData.Instance.UpdateItemInfo(item, 1);
        }
        MyEvent.Instance.GameEventManager.OnClaimedSomeStuff();
        OnClickCloseAdsRewardPopup();
    }
    #endregion

    #region OnClick
    public void OnClickPurchaseByCoin()
    {
        var currentCoin = MyUserData.Instance.UserDataSave.Coin;
        if(currentCoin < _itemPrice)
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.ShopPopup, true);
        }
        else
        {
            MyUserData.Instance.UpdateUserCurrency(-_itemPrice);
            OnBoughtItem();
        }
    }

    public void OnClickAds()
    {
        // call to watch ads and assign OnBoughtItem() to action completed watch ads
        Debug.Log($"<color=red>---Watch Ads to get free item---</color>");
        OnBoughtItem();
    }

    public void OnClickCloseAdsRewardPopup()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.ADSRewardPopup);
    }
    #endregion
}
