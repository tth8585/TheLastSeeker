using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DaySliderController : MonoBehaviour
{
    [SerializeField] Slider _slider;
    [SerializeField] TMP_Text _totalDayText;
    [SerializeField] DayTotalItemController[] _dayItem;

    public void InitData(int totalDay, Dictionary<int, MonthlyRewardDataConfig> reward)
    {
        _totalDayText.text = (totalDay).ToString();
        _slider.value = (float)(totalDay)/30;

        UpdateReward(reward);
    }

    private void UpdateReward(Dictionary<int, MonthlyRewardDataConfig> reward)
    {
        for(int i=0;i< _dayItem.Length;i++)
        {
            if (reward.ContainsKey(_dayItem[i].GetDayCount()))
            {
                _dayItem[i].InitData(reward[_dayItem[i].GetDayCount()]);
            }
        }
    }
}
