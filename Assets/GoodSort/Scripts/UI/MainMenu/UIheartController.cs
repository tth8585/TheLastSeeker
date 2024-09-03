using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIheartController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _countText;
    [SerializeField] GameObject _heartUnlimitedIcon, _plusIcon;

    bool isActiveHeartBuff = false;

    private void Awake()
    {
        MyEvent.Instance.HeartEventsManager.onUpdateHeart += OnUpdateHeart;
        MyEvent.Instance.HeartEventsManager.onGetHeartUnlimited += OnGetHeartUnlimited;
        MyEvent.Instance.HeartEventsManager.onExpireHeartUnlimited += OnExpireHeartUnlimited;
    }
    private void Start()
    {
        OnUpdateHeart(MyHeart.Instance.CurrentHeartCount);
    }
    private void OnUpdateHeart(int count)
    {
        if (!isActiveHeartBuff)
        {
            _countText.text = count.ToString();
        }
    }
    private void OnGetHeartUnlimited(double seconds)
    {
        isActiveHeartBuff = true;
        _heartUnlimitedIcon.SetActive(true);
        _plusIcon.SetActive(false);
        _countText.text = TimeFormatter.FormatSecondsToMinutesandSeconds(seconds);
    }

    private void OnExpireHeartUnlimited()
    {
        isActiveHeartBuff = false;
        _heartUnlimitedIcon.SetActive(false);
        _plusIcon.SetActive(true);
        _countText.text = MyHeart.Instance.CurrentHeartCount.ToString();
    }

    private void OnDestroy()
    {
        MyEvent.Instance.HeartEventsManager.onUpdateHeart -= OnUpdateHeart;
        MyEvent.Instance.HeartEventsManager.onGetHeartUnlimited -= OnGetHeartUnlimited;
        MyEvent.Instance.HeartEventsManager.onExpireHeartUnlimited -= OnExpireHeartUnlimited;
    }
}

public class TimeFormatter
{
    public static string FormatSecondsToMinutesandSeconds(double totalSeconds)
    {
        int minutes = (int)(totalSeconds / 60);
        int seconds = (int)(totalSeconds % 60);

        return $"{minutes:D2}:{seconds:D2}";
    }
}


