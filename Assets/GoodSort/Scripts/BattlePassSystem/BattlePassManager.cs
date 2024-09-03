using System;
using System.Collections.Generic;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;
using UnityEngine.UI;

public class BattlePassManager : MonoBehaviour
{
    const string BATTLEPASS_KEY = "BattlePass";
    const string PROPASS = "BattlePass_pro";
    [SerializeField] TextAsset csvFile;
    [SerializeField] Sprite _GSTokenIcon;

    private Dictionary<int, BattlePassData> _dicPasslevel= new Dictionary<int, BattlePassData>();
    private int _currentLevel = -1;
    private int _currentExp = 0;

    public ToolTipController ToolTipController;

    private bool _isPurchaseProPass = false;

    private void OnEnable()
    {
        MyEvent.Instance.BattlePassEvents.onClaimedReward += ClaimedReward;
    }

    private void OnDisable()
    {
        MyEvent.Instance.BattlePassEvents.onClaimedReward -= ClaimedReward;
    }

    public Dictionary<int, BattlePassData> GetPassDic()
    {
        return _dicPasslevel;
    }

    private void ClaimedReward(int level, PASS_ITEM_TYPE type)
    {
        if (_dicPasslevel.ContainsKey(level))
        {
            switch (type)
            {
                case PASS_ITEM_TYPE.FREE:
                    _dicPasslevel[level].isClaimedFreeReward = true;
                    break;
                case PASS_ITEM_TYPE.GODEN:
                    _dicPasslevel[level].isClaimedProReward = true;
                    break;
            }
        }
        SaveData();
        if(type == PASS_ITEM_TYPE.FREE)
        {
            LevelUp(level);
        }
    }

    private void Start()
    {
        _dicPasslevel.Clear();
        _dicPasslevel= ParseRewardsData(csvFile.text);
        LoadData();
    }

    public void AddExpFX(Vector3 position)
    {
        MyClaimReward.Instance.Claim(REWARD_TYPE.GSPassToken, _GSTokenIcon, position, new Vector2(100, 100), () => { Debug.Log("Claim GSPass token"); });
    }

    public void AddExp(int token)
    {
        _currentExp += token;
        SaveData();
        MyEvent.Instance.BattlePassEvents.ExpChanged();
    }
    private void LevelUp(int level)
    {
        int requiredTokensForNextLevel = GetRequiredTokenForLevel(level);

        //if can go next level
        if (requiredTokensForNextLevel != -1)
        {
            if (_currentExp >= requiredTokensForNextLevel)
            {
                _currentLevel = level;
                _currentExp -= requiredTokensForNextLevel;
                //ban event + level
            }
        }
        else
        {
            _currentLevel = level;
            _currentExp = GetRequiredTokenForLevel(_currentLevel);
            if(_currentExp == -1)
            {
                _currentExp = 0;
            }
        }
        MyEvent.Instance.BattlePassEvents.ExpChanged();
        MyEvent.Instance.BattlePassEvents.LevelChanged();
        SaveData();
    }
    public bool CanLevelUp(int level)
    {
        int requiredTokensForLevel = GetRequiredTokenForLevel(level);

        //if can go next level
        if (requiredTokensForLevel > -1)
        {
            if (_currentExp >= requiredTokensForLevel && IsClaimedLastLevel(level))
            {
                return true;
            }
        }
        else
        {
            return true;
        }
        return false;
    }
    private bool IsClaimedLastLevel(int curLevel)
    {
        if(curLevel == 0 || curLevel == -1) return true;
        if(_dicPasslevel.ContainsKey(curLevel-1) && _dicPasslevel[curLevel-1].isClaimedFreeReward)
        {
            return true;
        }
        return false;
    }
    public int GetCurrentLevel()
    {
        return _currentLevel;
    }

    public int GetCurrentExp()
    {
        return _currentExp;
    }

    public int GetCurrentRequireToken()
    {
        if (_currentLevel == -1) return 0;
        return GetRequiredTokenForLevel(_currentLevel+1);
    }

    public bool IsUnlockProPass()
    {
        _isPurchaseProPass = PlayerPrefs.HasKey(PROPASS);
        return _isPurchaseProPass;
    }

    private int GetRequiredTokenForLevel(int level)
    {
        if (_currentLevel == -1) return -1;
        if (_dicPasslevel.ContainsKey(level))
        {
            return _dicPasslevel[level].exp;
        }

        return -1;
    }

    private Dictionary<int, BattlePassData> ParseRewardsData(string csvText)
    {
        Dictionary<int, BattlePassData> level = new Dictionary<int, BattlePassData>();

        // Split the CSV text into lines
        string[] lines = csvText.Split('\n');

        //bo di phan tu start va end tren UI
        for (int i = 2; i < lines.Length; i++)
        {
            string[] fields = lines[i].Trim().Split(',');
            level.Add(int.Parse(fields[0]), new BattlePassData(int.Parse(fields[1]), false, false));
        }

        return level;
    }

    #region SAVE LOAD DATA

    private void SaveData()
    {
        if (_dicPasslevel.Count == 0)
        {
            Debug.LogWarning("dic_pass is empty. No data to save.");
            return;
        }

        string json = JsonUtility.ToJson(GetDataForSave());

        // Save the JSON string to PlayerPrefs
        PlayerPrefs.SetString(BATTLEPASS_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("xx SaveData Pass "+json);
    }

    private BattlePassSaveData GetDataForSave()
    {
        BattlePassSaveData data = new BattlePassSaveData();
        data.currentExp = _currentExp;
        data.currentLevel = _currentLevel;

        List<bool> newList = new List<bool>();
        foreach(var item in _dicPasslevel)
        {
            newList.Add(item.Value.isClaimedFreeReward);
        }
        data.claimedFreeRewardArr = newList.ToArray();

        List<bool> newList2 = new List<bool>();
        foreach (var item in _dicPasslevel)
        {
            newList2.Add(item.Value.isClaimedProReward);
        }
        data.claimedProRewardArr = newList2.ToArray();

        return data;
    }

    private void LoadData()
    {
        if (PlayerPrefs.HasKey(BATTLEPASS_KEY))
        {
            Debug.Log("xx LoadData: " + PlayerPrefs.GetString(BATTLEPASS_KEY));
            BattlePassSaveData data = JsonUtility.FromJson<BattlePassSaveData>(PlayerPrefs.GetString(BATTLEPASS_KEY));
            
            _currentExp = data.currentExp;
            _currentLevel = data.currentLevel;

            for (int i = 0; i < data.claimedFreeRewardArr.Length; i++)
            {
                if (_dicPasslevel.ContainsKey(i))
                {
                    _dicPasslevel[i].isClaimedFreeReward= data.claimedFreeRewardArr[i];
                    _dicPasslevel[i].isClaimedProReward = data.claimedProRewardArr[i];
                }
            }
        }
        else
        {
            _currentLevel = -1;
            _currentExp = 0;
        }
    }

    #endregion

    #region PURCHASE PACK

    public void PurchasePack(PASS_ITEM_TYPE purchasetype)
    {
        Debug.Log("xx Purchase battle pass Pack: "+purchasetype);
        //send request to sever and update purchase state

        if(purchasetype== PASS_ITEM_TYPE.GODEN)
        {
            _isPurchaseProPass = true;
            PlayerPrefs.SetString(PROPASS, "Has Purchased");

            MyEvent.Instance.BattlePassEvents.PurchaseProPack();
        }
    }


    public void CheckPurchased(string GSPassType)
    {
        //GSPASS_ITEM_TYPE type;
        //if (GSPassType.Equals("pro_battle_pass"))
        //    type = GSPASS_ITEM_TYPE.PRO;
        //else
        //    return;

        //MyBattlePass.Instance.PurchasePack(type);
        //ItemPurchaseController.RemoveADS(true);
    }
    #endregion

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.E)) 
        {
            AddExp(5);
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            PurchasePack(PASS_ITEM_TYPE.GODEN);
        }
    }
    public bool GetFinalChestStatus()
    {
        return _dicPasslevel[30].isClaimedFreeReward;
    }
    public void UnlockFinalChest()
    {
        //Final = 32 with 31 lv
        MyUserData.Instance.UpdateItemInfo(ITEM_TYPE.GoldenChest, 1);
        _dicPasslevel[99].isClaimedFreeReward = true;
        _dicPasslevel[99].isClaimedProReward = true;
    }
}

public class MyBattlePass : SingletonMonoBehaviour<BattlePassManager> { }

[System.Serializable]
public class BattlePassSaveData
{
    public int currentLevel;
    public int currentExp;
    public bool[] claimedFreeRewardArr;
    public bool[] claimedProRewardArr;
}

public class BattlePassData
{
    public int exp;
    public bool isClaimedFreeReward;
    public bool isClaimedProReward;

    public BattlePassData(int expValue, bool isClaimFree, bool isClaimPro) 
    {
        exp = expValue;
        isClaimedFreeReward = isClaimFree;
        isClaimedProReward = isClaimPro;
    }
}
