using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "ADSReward", menuName = "ScriptableObjects/ADSReward")]
public class ADSRewardSO : ScriptableObject
{
    public List<ADSRewardInfo> AdsRewards;
}

[Serializable]
public class ADSRewardInfo
{
    public ADSRewardType ADSRewardType;
    public List<ITEM_TYPE> Types;
    public string RewardTitle;
    public string RewardDescription;
    public int Price;
}

public enum ADSRewardType 
{
    None, 
    FreeGift,
    Item
}
