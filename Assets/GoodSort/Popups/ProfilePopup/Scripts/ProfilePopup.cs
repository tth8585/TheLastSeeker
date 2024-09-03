using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Imba.UI;
using SuperScrollView;
using System;
using TMPro;

public class ProfilePopup : UIPopup
{
    [Header("Content")]
    [SerializeField] private Button _avatarBtn;
    [SerializeField] private Button _skinBtn;
    [SerializeField] private Sprite _choosedBtn;
    [SerializeField] private Sprite _notChooseBtn;
    [SerializeField] private Image _choosedItemBG;

    [Space(5)]
    [SerializeField] private LoopGridView mLoopGridViewAvatar;
    [SerializeField] private LoopGridView mLoopGridViewSkin;
    [SerializeField] private TextAsset _profileConfigCsv;

    [Space(10)]
    [Header("Player Info")]
    [SerializeField] private Image _currentPlayerAvt;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private TMP_Text _currentPlayerCoin;


    private int _itemCount;
    private ProfileItemType _currentProfileType;
    private List<ProfileItemInfo> _avatarsInfo;
    private List<ProfileItemInfo> _skinsInfo;
    private string _currentChoosedItemID;
    

    #region
    protected override void OnShowing()
    {
        UpdateUI();

        _playerNameInputField.text = MyUserData.Instance.UserDataSave.UserName;
        //_currentPlayerCoin.text = FormatText.FormatTextCount(MyUserData.Instance.UserDataSave.Coin);

        if (_avatarsInfo == null || _skinsInfo == null)
        {
            _avatarsInfo = new List<ProfileItemInfo>();
            _skinsInfo = new List<ProfileItemInfo>();
            ParseProfileCSVData.GetProfileItemInfoByCSVFile(_profileConfigCsv.text, out _avatarsInfo, out _skinsInfo);
        }

        _currentProfileType = ProfileItemType.Avatar;
        InitProfileItem(_currentProfileType);

        mLoopGridViewAvatar.SetActive(true);
        mLoopGridViewSkin.SetActive(false);
    }

    protected override void OnHidden()
    {
        _choosedItemBG.SetActive(false);
        _currentChoosedItemID = string.Empty;
    }
    #endregion


    private void InitProfileItem(ProfileItemType itemType)
    {
        _choosedItemBG.SetActive(false);
        _currentChoosedItemID = string.Empty;

        if(itemType == ProfileItemType.Avatar)
        {
            InitProfileGridView(mLoopGridViewAvatar, _avatarsInfo, OnGetAvatarItem, RefreshAvatarList);
        }
        else if(itemType == ProfileItemType.Skin)
        {
            InitProfileGridView(mLoopGridViewSkin, _skinsInfo, OnGetSkinItem, RefreshSkinList);
        }
    }

    private void OnOneItemChoosed(ProfileItem item, string itemID, bool isPurchase)
    {
        _choosedItemBG.transform.SetParent(item.transform);
        _choosedItemBG.SetActive(true);
        _choosedItemBG.rectTransform.SetAsFirstSibling();
        _choosedItemBG.rectTransform.anchoredPosition = Vector2.zero;

        _currentChoosedItemID = itemID;
      
        //_changeBtn.SetActive(true);

        if (_currentProfileType == ProfileItemType.Avatar)
        {
            _currentPlayerAvt.sprite = Resources.Load<Sprite>($"Icons/Avatars/{_currentChoosedItemID}");

            if (!isPurchase) return;

            MyUserData.Instance.UpdatePlayerAvatar(_currentChoosedItemID);
            MyEvent.Instance.UserDataManagerEvent.OnLoadedUserData();
        }

        if (_currentProfileType == ProfileItemType.Skin)
        {
            MyUserData.Instance.UpdatePlayerSkin(_currentChoosedItemID);
        }
    }

    private void OnItemDisable()
    {
        _choosedItemBG.SetActive(false);
    }

    private void UpdateUI()
    {
        UpdateAvt();
        UpdateCoin();
    }

    private void UpdateAvt()
    {
        var currentPlayerAvt = MyUserData.Instance.UserDataSave.CurrentAvatarId;
        _currentPlayerAvt.sprite = Resources.Load<Sprite>($"Icons/Avatars/{currentPlayerAvt}");
    }

    private void UpdateCoin() =>
        _currentPlayerCoin.text = FormatText.FormatTextCount(MyUserData.Instance.UserDataSave.Coin);

    private bool CheckItemHasBought(string itemID, string itemType)
    {
        if(itemType == "Avatar")
        { 
            return MyUserData.Instance.UserDataSave.UserAvatarsOwned.Contains(itemID);
        }
        else
        {
            return MyUserData.Instance.UserDataSave.UserSkinsOwned.Contains(itemID);
        }
      
    }

    #region LIST LOOP VIEW

    private void InitProfileGridView(LoopGridView loopGridView, List<ProfileItemInfo> itemInfos, Func<LoopGridView, int, int , int, LoopGridViewItem> func, Action refresh)
    {
        _itemCount = 0;
        if (itemInfos != null || itemInfos.Count > 0)
        {
            if (!loopGridView.mListViewInited)
            {
                loopGridView.InitGridView(itemInfos.Count, func);
            }
            else
            {
                refresh?.Invoke();
            }
        }
    }

    protected LoopGridViewItem OnGetAvatarItem(LoopGridView gridView, int itemIndex, int row, int column)
    {
        _itemCount++;

        LoopGridViewItem item = null;
        if (_avatarsInfo == null) return null;
        if (_avatarsInfo.Count == 0)
        {
            Debug.LogError("quest list count =0");
        }
        else
        {
            item = gridView.NewListViewItem("ProfileItem");
            ProfileItem profileItem = item.GetComponent<ProfileItem>();

            var itemInfo = _avatarsInfo[itemIndex];
            var isPurchased = CheckItemHasBought(itemInfo.ID, itemInfo.Type);

            profileItem.InitProfileItem(itemInfo, isPurchased);

            profileItem.OnClickItemBtn += OnOneItemChoosed;
            profileItem.OnItemDisable += OnItemDisable;
            profileItem.OnItemHasBought += UpdateUI;
        }

        return item;
    }

    protected void RefreshAvatarList()
    {
        if (_avatarsInfo == null)
        {
            mLoopGridViewAvatar.RecycleAllItem();
        }
        else
        {
            if (_avatarsInfo != null && mLoopGridViewAvatar != null)
            {
                try
                {
                    mLoopGridViewAvatar.SetListItemCount(_avatarsInfo.Count);
                }
                catch (Exception ex)
                {
                    Debug.LogError("some error unknown: " + ex);
                    return;
                }
            }
        }
    }

    // Skin Loop grid view
    protected LoopGridViewItem OnGetSkinItem(LoopGridView gridView, int itemIndex, int row, int column)
    {
        _itemCount++;
        LoopGridViewItem item = null;
        if (_skinsInfo == null) return null;
        if (_skinsInfo.Count == 0)
        {
            Debug.LogError("quest list count =0");
        }
        else
        {
            item = gridView.NewListViewItem("ProfileItem");
            ProfileItem profileItem = item.GetComponent<ProfileItem>();

            var itemInfo = _skinsInfo[itemIndex];
            var isPurchased = CheckItemHasBought(itemInfo.ID, itemInfo.Type);
            profileItem.InitProfileItem(itemInfo, isPurchased);

            profileItem.OnClickItemBtn += OnOneItemChoosed;
            profileItem.OnItemDisable += OnItemDisable;
        }

        return item;
    }

    protected void RefreshSkinList()
    {
        if (_skinsInfo == null)
        {
            mLoopGridViewSkin.RecycleAllItem();
        }
        else
        {
            if (_skinsInfo != null && mLoopGridViewSkin != null)
            {
                try
                {
                    mLoopGridViewSkin.SetListItemCount(_skinsInfo.Count);
                }
                catch (Exception ex)
                {
                    Debug.LogError("some error unknown: " + ex);
                    return;
                }
            }
        }
    }

    #endregion

    #region OnClick
    public void OnClickClose()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.ProfilePopup);
    }

    public void OnClickChange()
    {
        string newPlayerName = _playerNameInputField.text;
        MyUserData.Instance.UpdateUserName(newPlayerName);
    }

    public void OnClickAvatarBtn()
    {
        _avatarBtn.GetComponent<Image>().sprite = _choosedBtn;
        _skinBtn.GetComponent<Image>().sprite = _notChooseBtn;

        _currentProfileType = ProfileItemType.Avatar;
        InitProfileItem(ProfileItemType.Avatar);

        mLoopGridViewAvatar.SetActive(true);
        mLoopGridViewSkin.SetActive(false);
    }

    public void OnClickSkinBtn()
    {
        _skinBtn.GetComponent<Image>().sprite = _choosedBtn;
        _avatarBtn.GetComponent<Image>().sprite = _notChooseBtn;

        _currentProfileType = ProfileItemType.Skin;
        InitProfileItem(ProfileItemType.Skin);

        mLoopGridViewAvatar.SetActive(false);
        mLoopGridViewSkin.SetActive(true);
    }

    public void OnClickSaveUserName()
    {
        // click to show keyboard
        _playerNameInputField.ActivateInputField();
    }
    #endregion
}

public static class ParseProfileCSVData
{
    public static void GetProfileItemInfoByCSVFile(string profileCSV, out List<ProfileItemInfo> avatars, out List<ProfileItemInfo> skins)
    {
       
        //List<ProfileItemInfo> profileItemInfos = new List<ProfileItemInfo>();
        avatars = new List<ProfileItemInfo>();
        skins = new List<ProfileItemInfo>();

        // Split the CSV text into lines
        string[] lines = profileCSV.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Trim().Split(',');

            ProfileItemInfo reward = new ProfileItemInfo
            {
                ID = fields[0],
                Price = fields[1],
                Type = fields[2]
            };

            if (reward.Type == ProfileItemType.Avatar.ToString())
                avatars.Add(reward);
            else
                skins.Add(reward);

            // Add the reward to the list
            //profileItemInfos.Add(reward);
        }

        
        
    }
}
