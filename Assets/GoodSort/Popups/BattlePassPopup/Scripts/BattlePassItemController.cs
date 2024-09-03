using SuperScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassItemController : MonoBehaviour
{
    [SerializeField] BattlePassItemDetailController _freeItem, _proItem;

    public void InitData(RewardBattlePass data)
    {
        int currentLevel = MyBattlePass.Instance.GetCurrentLevel();
        bool isFill = false;
        if(data.Level <= currentLevel +1)
        {
            isFill = true;
        }

        _freeItem.InitData(GetFreeData(data), isFill);
        _proItem.InitData(GetProData(data), isFill);
    }

    private BattlePassDataDetail GetFreeData(RewardBattlePass data)
    {
        return new BattlePassDataDetail 
        {
            Level = data.Level,
            Amount= data.FreeAmount,
            RewardType= data.FreeRewardType,
            IsClaimed = data.isClaimedFree
        };
    }

    private BattlePassDataDetail GetProData(RewardBattlePass data)
    {
        return new BattlePassDataDetail
        {
            Level = data.Level,
            Amount = data.ProAmount,
            RewardType = data.ProRewardType,
            IsClaimed = data.isClaimedPro
        };
    }  
}

[Serializable]
public class BattlePassDataDetail
{
    public int Level { get; set; }
    public int Amount { get; set; }
    public string RewardType { get; set; }
    public bool IsClaimed { get; set; }
}
