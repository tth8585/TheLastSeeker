using Imba.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrePlayPopup : UIPopup
{
    [SerializeField] ItemPrePlayPrefabController[] _itemPrePlay;
    [SerializeField] internal GameObject[] _barFills;
    [SerializeField] internal GameObject _mainBg, _tutorialBg, _adsBtn;
    [SerializeField] internal TMP_Text _levelText;

    internal int _currentLevel = 0;

    protected override void OnShown()
    {
        base.OnShown();
        _adsBtn.SetActive(true);
        _currentLevel = (int) Parameter;

        if (_currentLevel >= 0)
        {
            _levelText.text= "Level "+_currentLevel.ToString();
            InitProgress();
            InitItemBar();
        }
        else
        {
            Debug.LogError("pre Load level failed");
        }
    }

    #region INIT DATA

    private void InitProgress()
    {
        int progressStreak = GetProgress();

        foreach (var item in _barFills)
        {
            item.SetActive(false);
        }

        if(progressStreak > 0)
        {
            for (int i = 0; i < progressStreak; i++)
            {
                _barFills[i].SetActive(true);
            }
        }
    }

    internal void InitItemBar()
    {
        foreach (var item in _itemPrePlay)
        {
            ItemInfoSO itemData = MyItemAbility.Instance.GetItemInfoByType(item.Type);
            item.InitData(itemData,GetItemPrePlayQuantity(itemData));
        }
    }

    private int GetItemPrePlayQuantity(ItemInfoSO itemData)
    {
        return MyUserData.Instance.DicItemDatas[itemData.ItemType];
    }

    #endregion

    #region Handle Method
    private void OnClaimedGift()
    {
        _adsBtn.SetActive(false);
        InitItemBar();

        MyEvent.Instance.GameEventManager.onClaimedSomeStuff -= OnClaimedGift;
    }
    #endregion

    #region ON CLICK BTN

    public virtual void OnClickPlayBtn()
    {
        Debug.LogError("change scene and apply using item preplay not handle");
        ClosePopup();
        if(MyHeart.Instance.CurrentHeartCount > 0)
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.LoadingPopup);
            MyHeart.Instance.UpdateCurrentHeart(-1);
        }
        else
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.NoHeartPopup);
        }
    }

    public void OnClickFreeGiftBtn()
    {
        Debug.LogError("watch ads and get free item (one per item) not handle");
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.ADSRewardPopup, ADSRewardType.FreeGift);
        //MyUserData.Instance.UpdateItemInfo(ITEM_TYPE.Boom, 1);
        //MyUserData.Instance.UpdateItemInfo(ITEM_TYPE.DoubleStar, 1);
        //MyUserData.Instance.UpdateItemInfo(ITEM_TYPE.Time, 1);
        MyEvent.Instance.GameEventManager.onClaimedSomeStuff += OnClaimedGift;
    }

    public void OnClickContinueBtn()
    {
        _mainBg.SetActive(true);
        _tutorialBg.SetActive(false);
    }

    public void OnClickCloseBtn()
    {
        _mainBg.SetActive(true); 
        _tutorialBg.SetActive(false);
        ClosePopup();
    }

    private void ClosePopup()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.PrePlayPopup);
    }

    public void OnClickTutBtn()
    {
        _mainBg.SetActive(false);
        _tutorialBg.SetActive(true);
    }

    private int GetProgress()
    {
        return MyUserData.Instance.UserDataSave.ProgressStreak;
    }

    #endregion
}

public class LevelProgressData
{
    public int Progress { get; set; }
}
