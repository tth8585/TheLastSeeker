using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeOfferController : MonoBehaviour
{
    [SerializeField] TMP_Text _timeText;
    [SerializeField] RectTransform _rebuildRect;

    public void InitUI()
    {
        _timeText.text = GetTimeText();
        if(_rebuildRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rebuildRect);
        }
    }

    private string GetTimeText()
    {
        DateTime currentTime = DateTime.Now;

        DateTime midnight = currentTime.Date.AddDays(1);

        TimeSpan timeUntilMidnight = midnight - currentTime;

        string formattedTime = $"{(int)timeUntilMidnight.TotalHours:00}h {(int)timeUntilMidnight.TotalMinutes % 60:00}m";

        return formattedTime;
    }
}
