using System;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;
using System.Collections;

public class DailyManager : MonoBehaviour
{
    private const string lastLoginDateKey = "LastLoginDate";

    private bool _newDailyForQuest = false;
    private bool _newDailyForSpin = false;
    private void Start()
    {
        CheckIfNewDay();
        StartCoroutine(WaitToSetUpDaily());
    }
    IEnumerator WaitToSetUpDaily()
    {
        yield return new WaitUntil(() => MyQuest.Instance != null);
        yield return new WaitUntil(() => MyDailyReward.Instance != null);
        yield return new WaitUntil(() => MyItemAbility.Instance != null && MyItemAbility.Instance.HasLoadItemInfo);
        MyQuest.Instance.InitQuest();
        MyDailyReward.Instance.InitReward();
    }
    public void CheckIfNewDay()
    {
        if (PlayerPrefs.HasKey(lastLoginDateKey))
        {
            //neu co key, xu ly logic
            HanleCheckLastLogin();
        }
        else
        {
            //neu chua co key, may' lan dau tien install game
            HandleNewInstall();
        }
    }

    private void HanleCheckLastLogin()
    {
        string lastLoginDateString = PlayerPrefs.GetString(lastLoginDateKey);
        DateTime lastLoginDate;

        if (DateTime.TryParse(lastLoginDateString, out lastLoginDate))
        {
            if (DateTime.Today > lastLoginDate)
            {
                PlayerPrefs.SetString(lastLoginDateKey, DateTime.Today.ToString());
                PlayerPrefs.Save();
                _newDailyForQuest = true;
                _newDailyForSpin = true;
                //MyBuff.Instance.ResetCashBuffUsedCount();
            }
            else
            {
                _newDailyForQuest = false;
                _newDailyForSpin = false;
            }
        }
    }

    private  void HandleNewInstall()
    {
        PlayerPrefs.SetString(lastLoginDateKey, DateTime.Today.ToString());
        _newDailyForQuest = true;
        _newDailyForSpin = true;
    }

    public bool IsNewDailyForQuest()
    {
        //return true;
        return _newDailyForQuest;
    }

    public bool IsNewDailyForSpin()
    {
        //return true;
        return _newDailyForSpin;
    }

    public void SetCacheForQuestDailyReset()
    {
        _newDailyForQuest = false;
    }

    public bool IsNewDailyLogin()
    {
        if (PlayerPrefs.HasKey(lastLoginDateKey))
        {
            string lastLoginDateString = PlayerPrefs.GetString(lastLoginDateKey, string.Empty);
            DateTime lastLoginDate;

            if (DateTime.TryParse(lastLoginDateString, out lastLoginDate))
            {
                if (DateTime.Today > lastLoginDate)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

public class MyDaily : SingletonMonoBehaviour<DailyManager> { }
