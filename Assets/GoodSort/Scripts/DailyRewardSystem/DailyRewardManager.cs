using Imba.UI;
using System;
using System.Collections.Generic;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;

public class DailyRewardManager : MonoBehaviour
{
    [SerializeField] bool ForTest = true;
    private const string dailyRewardKey = "dailyReward";

    private DailyRewardData _data;

    public void InitReward()
    {
        CheckDailyLogin();
    }

    private System.Collections.IEnumerator CheckPopupNotNull()
    {
        while (UIManager.Instance.PopupManager == null)
        {
            Debug.Log("Object is null. Waiting for it to be instantiated...");
            yield return null; // Wait for one frame
        }

        //UIManager.Instance.PopupManager.ShowPopup(UIPopupName.DailyRewardPopup);
    }

    public void CheckDailyLogin()
    {
        bool isNewDay = false;

        if (!PlayerPrefs.HasKey(dailyRewardKey))
        {
            isNewDay = true;
        }
        else
        {
            LoadData();

            DateTime lastDayClaimed = DateTime.Now;
            if (DateTime.TryParse(_data.ClaimDayString, out lastDayClaimed))
            {
                if(DateTime.Now.Year> lastDayClaimed.Year)
                {
                    _data.IsClaimedRewardToday = false;
                    _data.ClaimDayString = DateTime.Today.ToString();
                    SaveData();
                    isNewDay = true;
                }
                else if (DateTime.Now.DayOfYear > lastDayClaimed.DayOfYear)
                {
                    _data.IsClaimedRewardToday = false;
                    _data.ClaimDayString= DateTime.Today.ToString();
                    SaveData();
                    isNewDay = true;
                }
                else
                {
                    if (!_data.IsClaimedRewardToday) isNewDay = true;
                    else isNewDay = false;
                }
            }
            else
            {
                Debug.LogError("Cant parse last day claimed reward from playpref");
                isNewDay = true;
            }
        }

        if (isNewDay)
        {
            if (!PlayerPrefs.HasKey(dailyRewardKey))
            {
                //new install logic
                DailyRewardData data = new DailyRewardData();
                data.TotalCountLoginDay = 1;
                data.IsClaimedRewardToday = false;
                data.MonthlyClaimed = new int[1];
                Array.Clear(data.MonthlyClaimed, 0, data.MonthlyClaimed.Length);
                _data = data;
                SaveData();
            }
            else
            {
                LoadData();
            }
        }
        else
        {
            LoadData();
        }

        if (!_data.IsClaimedRewardToday)
        {
            StartCoroutine(CheckPopupNotNull());
        }
    }

    public int GetTotalDayLogin()
    {
        return _data.TotalCountLoginDay;
    }

    public void OnClaimedMonthlyReward(int dayValue)
    {
        List<int> newList = new List<int>();
        for(int i=0;i<_data.MonthlyClaimed.Length;i++)
        {
            newList.Add(_data.MonthlyClaimed[i]);
        }

        newList.Add(dayValue);

        _data.MonthlyClaimed = newList.ToArray();
        Debug.Log("xx claim monthly reward: "+dayValue);
        SaveData();
    }

    public Dictionary<int, DailyRewardDataConfig> GetRewardData()
    {
        return GetComponent<DailyRewardCSVReader>().GetReward();
    }

    public Dictionary<int, MonthlyRewardDataConfig> GetRewardMonthData()
    {
        return GetComponent<MonthlyRewardCSVReader>().GetReward();
    }

    public bool CheckCanClaimMonthlyReward(int dayToUnlock)
    {
        if (CheckClaimedMonthlyReward(dayToUnlock)) return false;

        if (!GetRewardMonthData().ContainsKey(dayToUnlock)) return false;

        if(_data.TotalCountLoginDay < dayToUnlock) return false;

        return true;
    }

    public bool CheckClaimedMonthlyReward(int dayToUnlock)
    {
        return Array.Exists(_data.MonthlyClaimed, element => element == dayToUnlock);
    }

    #region SAVE LOAD DATA

    private void SaveData()
    {
        string json = JsonUtility.ToJson(_data);
        PlayerPrefs.SetString(dailyRewardKey, json);
        PlayerPrefs.Save();

        Debug.Log("xx SaveData daily reward: "+ json);
    }

    private void LoadData()
    {
        Debug.Log("xx LoadData daily reward");
        string json = PlayerPrefs.GetString(dailyRewardKey);
        _data = JsonUtility.FromJson<DailyRewardData>(json);
    }
    #endregion

    public void SetHasGotDailyReward()
    {
        Debug.Log("xx SetHasGotDailyReward");
        _data.IsClaimedRewardToday = true;
        _data.TotalCountLoginDay++;
        _data.ClaimDayString= DateTime.Today.ToString();

        if (_data.TotalCountLoginDay > 30)
        {
            //reset monthly reward
            _data.TotalCountLoginDay = 1;
            List<int> newList = new List<int>();
            _data.MonthlyClaimed = newList.ToArray();
        }

        SaveData();
    }
}

public class MyDailyReward : SingletonMonoBehaviour<DailyRewardManager> { }

[System.Serializable]
public class DailyRewardData
{
    public int TotalCountLoginDay;
    public bool IsClaimedRewardToday;
    public int[] MonthlyClaimed;
    public string ClaimDayString;
}
