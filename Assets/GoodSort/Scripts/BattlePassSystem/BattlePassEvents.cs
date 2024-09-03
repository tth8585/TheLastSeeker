using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassEvents 
{
    public event Action<int, PASS_ITEM_TYPE> onClaimedReward;
    public void ClaimedReward(int level, PASS_ITEM_TYPE type)
    {
        if (onClaimedReward != null) onClaimedReward(level, type);
    }

    public event Action onExpChanged;
    public void ExpChanged()
    {
        if (onExpChanged != null) onExpChanged();
    }

    public event Action onLevelChanged;
    public void LevelChanged()
    {
        if (onLevelChanged != null) onLevelChanged();
    }

    public event Action onPurchaseProPack;
    public void PurchaseProPack()
    {
        if (onPurchaseProPack != null) onPurchaseProPack();
    }
}
