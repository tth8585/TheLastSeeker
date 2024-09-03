using System;
using System.Collections;
using System.Collections.Generic;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private UserDataSave _userDataSave;

    private const string _userDataKey = "UserData";
    private const string _defaultAvatarId = "101";
    private const string _defaultPlayerName = "User Name";

    private Dictionary<ITEM_TYPE, int> _dicItemDatas = new();

    public UserDataSave UserDataSave => _userDataSave;

    public Dictionary<ITEM_TYPE, int> DicItemDatas => _dicItemDatas;

    private void Start()
    {
        StartCoroutine(AddEventListeners());
    }

    IEnumerator AddEventListeners()
    {
        yield return new WaitUntil(() => MyEvent.Instance != null);

        MyEvent.Instance.UserDataManagerEvent.onClaimedItem += UpdateItemInfo;
    }

    public void InitData()
    {
        LoadUserData();
    }

    public int GetCurrentUserLevel()
    {
        return UserDataSave.CurrentLevelData;
    }

    #region SET USER DATA

    public void UpdateCurrentLevel(int curLevel, int progress)
    {
        _userDataSave.CurrentLevelData = curLevel;
        //_dicLevelDatas[curLevel] = progress;

        SaveInfo();
        LoadUserData();
    }
    public void SetItemInfoData(ITEM_TYPE itemType, int quantity)
    {
        _dicItemDatas[itemType] = quantity;
        MyEvent.Instance.UserDataManagerEvent.ItemAbilityChanged();

        SaveInfo();
        LoadUserData();
    }
    public void UpdateItemInfo(ITEM_TYPE type, int quantity)
    {
        //+quantity to add, -quantity to reduce
        if(type == ITEM_TYPE.Coin)
        {
            UpdateUserCurrency(quantity);
        }
        else
        {
            _dicItemDatas[type] += quantity;
            
        }
        
        if (type.ToString().Contains("Chest"))
        {
            ItemInfoSO item = MyItemAbility.Instance.GetItemInfoByType(type);

            for (int i = 0; i < item.ChestItemQuantity.Count; i++)
            {
                _dicItemDatas[item.ChestItems[i].ItemType]+= item.ChestItemQuantity[i];
            }
        }

        MyEvent.Instance.UserDataManagerEvent.ItemAbilityChanged();

        SaveInfo();
        LoadUserData();
    }

    public void UpdateUserName(string name)
    {
        _userDataSave.UserName = name;
        SaveInfo();
        LoadUserData();
    }

    public void UpdateUserCurrency(int quantity)
    {
        _userDataSave.Coin += quantity;
        
        SaveInfo();
        LoadUserData();
    }

    public void UpdateProgressStreak(int progress)
    {
        int rs = _userDataSave.ProgressStreak;
        if (progress == -1) rs = 0;
        else rs += progress;

        if (rs > 3) rs = 3;

        _userDataSave.ProgressStreak = rs;

        SaveInfo();
        LoadUserData();
    }

    public void UpdatePlayerAvatar(string avatarID)
    {
        if (!_userDataSave.UserAvatarsOwned.Contains(avatarID))
        {
            _userDataSave.UserAvatarsOwned.Add(avatarID);

        }
        _userDataSave.CurrentAvatarId = avatarID;

        SaveInfo();
        LoadUserData();
    }

    public void UpdatePlayerSkin(string skinID)
    {
        if (!_userDataSave.UserSkinsOwned.Contains(skinID))
            _userDataSave.UserSkinsOwned.Add(skinID);
        else
            _userDataSave.CurrentSkinId = skinID;

        SaveInfo();
        LoadUserData();
    }

    #endregion SET USER DATA

    #region SAVE LOAD DATA

    private void SaveInfo()
    {
        GetItemsDataForSave();
        //GetLevelsDataForSave();
        string json = JsonUtility.ToJson(_userDataSave);

        //Debug.Log("SAVE USER DATA :::: " + json);
        PlayerPrefs.SetString(_userDataKey, json);
        PlayerPrefs.Save();
    }

    private void LoadUserData()
    {
        if (!PlayerPrefs.HasKey(_userDataKey))
        {
            InitDefaultUserData();
            SaveInfo();
        }
        else
        {
            string json = PlayerPrefs.GetString(_userDataKey);
            _userDataSave = JsonUtility.FromJson<UserDataSave>(json);
            //Debug.Log("LOAD USER DATA ::::: " + json);

        }

        LoadItemDatas();
        //LoadProgressStreak();
        MyEvent.Instance.UserDataManagerEvent.OnLoadedUserData();
    }

    private void GetItemsDataForSave()
    {
        ItemInfoDataSave[] itemArr = new ItemInfoDataSave[_dicItemDatas.Count];
        int index = 0;
        foreach (var item in _dicItemDatas)
        {
            ItemInfoDataSave itemData = new ItemInfoDataSave() {ItemType =item.Key, ItemQuantity = item.Value };
            itemArr[index] = itemData;
            index++;
        }
        _userDataSave.ItemsDataArr = itemArr;
    }

    private void LoadItemDatas()
    {
        InitItemTypes();

        foreach (ItemInfoDataSave item in _userDataSave.ItemsDataArr)
        {
            _dicItemDatas[item.ItemType] = item.ItemQuantity;
        }
    }

    private void InitItemTypes()
    {
        
        foreach (var item in MyItemAbility.Instance.DicItemRewardInfo)
        {
            if (!_dicItemDatas.ContainsKey(item.Key))
            {
                _dicItemDatas.Add(item.Key, 0);
            }
        }
    }
    //private void GetLevelsDataForSave()
    //{
    //    LevelDataSave[] itemArr = new LevelDataSave[_dicLevelDatas.Count];
    //    int index = 0;
    //    foreach (var item in _dicLevelDatas)
    //    {
    //        LevelDataSave itemData = new LevelDataSave() { Level = item.Key, Progress = item.Value};
    //        itemArr[index] = itemData;
    //        index++;
    //    }
    //    _userDataSave.LevelsDataArr = itemArr;
    //}

    //private void InitAllLevel()
    //{
    //    //create dic level default data
    //    _dicLevelDatas.Clear();

    //    for (int i = 0; i <= MyLevel.Instance.GetMaxLevelCount(); i++)
    //    {
    //        if (!_dicLevelDatas.ContainsKey(i))
    //        {
    //            _dicLevelDatas.Add(i, 0);
    //        }
    //    }
    //}

    private void InitDefaultUserData()
    {
        _userDataSave = new UserDataSave()
        {
            UserName = _defaultPlayerName,
            CurrentAvatarId = _defaultAvatarId,
            Coin = 0,
            CurrentLevelData = 0,
            ItemsDataArr = new ItemInfoDataSave[0],
            LevelsDataArr = new LevelDataSave[0],
        };
    }
    #endregion SAVE LOAD DATA

    private void OnDisable()
    {
        MyEvent.Instance.UserDataManagerEvent.onClaimedItem -= UpdateItemInfo;
    }
}

public class MyUserData : SingletonMonoBehaviour<UserDataManager> { }


[Serializable]
public class UserDataSave
{
    public string UserName;
    public int Coin;
    public string CurrentAvatarId;
    public string CurrentSkinId;
    public int CurrentLevelData;
    public int ProgressStreak;
    public ItemInfoDataSave[] ItemsDataArr;
    public LevelDataSave[] LevelsDataArr;
    public List<string> UserAvatarsOwned;
    public List<string> UserSkinsOwned;
}

[Serializable]
public class LevelDataSave
{
    public int Level;
    public int Progress;
}

[Serializable]
public class ItemInfoDataSave
{
    public ITEM_TYPE ItemType;
    public int ItemQuantity;
}
