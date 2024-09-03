using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayTotalItemController : MonoBehaviour
{
    [SerializeField] GameObject _completeObj;
    [SerializeField] int _dayToUnlock = 1;
    [SerializeField] RewardMonthlyItemController[] _rewards;
    [SerializeField] TMP_Text _dayText;

    private MonthlyRewardDataConfig _rewardConfig;

    public void InitData(MonthlyRewardDataConfig reward)
    {
        _rewardConfig = reward;
        if (CheckIsClaimed())
        {
            Complete(true);
        }
        else
        {
            Complete(false);
        }

        _dayText.text = _dayToUnlock.ToString();

        UpdateUIReward(reward);
    }

    public int GetDayCount()
    {
        return _dayToUnlock;
    }

    private void UpdateUIReward(MonthlyRewardDataConfig reward)
    {
        for(int i=0;i< _rewards.Length; i++)
        {
            _rewards[i].InitData(reward.Details[i].TypeReward, reward.Details[i].Value);
        }
    }

    public void OnClickClaimReward()
    {
        if (MyDailyReward.Instance.CheckCanClaimMonthlyReward(_dayToUnlock))
        {
            //claim
            for(int i=0;i< _rewards.Length; i++)
            {
                MyClaimReward.Instance.Claim(_rewards[i].GetIcon(), () => OnClaim(_rewardConfig.Details[i]));
            }

            Complete(true);
            //add reward
            MyDailyReward.Instance.OnClaimedMonthlyReward(_dayToUnlock);
            //show effect ?
        }
    }

    private void OnClaim(MonthlyRewardDetailData data)
    {
        //Const.OnClaim(data.TypeReward, data.Value);
    }

    private bool CheckIsClaimed()
    {
        return MyDailyReward.Instance.CheckClaimedMonthlyReward(_dayToUnlock);
    }

    private void Complete(bool isComplete)
    {
        _completeObj.SetActive(isComplete);
    }
}
