using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayCountController : MonoBehaviour
{
    [SerializeField] DayItemController[] dayItemControllers;

    private int _currentDay;
    private Dictionary<int, DailyRewardDataConfig> _reward;

    public void InitData(int totalDay, Dictionary<int, DailyRewardDataConfig> dicData)
    {
        _currentDay = (totalDay-1) % 7;
        _reward = dicData;
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < dayItemControllers.Length; i++)
        {
            if (i < _currentDay)
            {
                dayItemControllers[i].Complete();
            }
            else if(i==_currentDay)
            {
                dayItemControllers[i].Unlock();
            }
            else
            {
                dayItemControllers[i].Lock();
            }

            if (_reward.ContainsKey(i + 1))
            {
                dayItemControllers[i].InitData(_reward[i+1]);
            }
        }
    }

    public DailyRewardDataConfig GetRewardToday()
    {
        return _reward[_currentDay+1];
    }

    public Image GetImageReward()
    {
        return dayItemControllers[_currentDay].GetIcon();
    }

    public void UpdateUIWhenClaim()
    {
        dayItemControllers[_currentDay].Complete();
    }
}
