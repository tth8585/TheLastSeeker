using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SliderQuestController : MonoBehaviour
{
    [SerializeField] GameObject _completeBar;
    [SerializeField] TMP_Text _progressText;
    [SerializeField] GameObject[] _arrActiveObject;
    [SerializeField] UnityEngine.UI.Slider _slider;

    private float _maxValue;

    public void InitUI(int currentValue, int maxValue)
    {
        _maxValue= maxValue;
        _slider.value = currentValue / _maxValue;
        _progressText.text = currentValue + "/" + ((int)_maxValue).ToString();
        CheckDone();
    }

    public void UpdateUI(int currentValue, bool isRewardClainmed)
    {
        _slider.value = currentValue / _maxValue;
        _progressText.text = currentValue + "/" + ((int)_maxValue).ToString();
        CheckDone(isRewardClainmed);
    }

    public void UpdateUIClaimed(int maxValue)
    {
        _progressText.text = maxValue + "/" + ((int)maxValue).ToString();
        CheckDone(true);
    }

    private void CheckDone(bool isRewardClaimed = false)
    {
        //bool isDone = false;

        //if (_slider.value >= _maxValue) isDone = true;

        foreach (var item in _arrActiveObject)
        {
            item.SetActive(!isRewardClaimed);
        }

        _completeBar.SetActive(isRewardClaimed);
    }
}
