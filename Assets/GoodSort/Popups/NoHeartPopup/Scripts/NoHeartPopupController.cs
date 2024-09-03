using System.Collections;
using System.Collections.Generic;
using Imba.UI;
using UnityEngine;
using TMPro;

public class NoHeartPopupController : UIPopup
{
    [SerializeField] TextMeshProUGUI _timerText,_heartPriceText;
    IEnumerator _counterAsync;
    int _heartPrice = 600;
    protected override void OnShown()
    {
        SetUpData();
        base.OnShown();
    }
    private void SetUpData()
    {
        _heartPriceText.text = _heartPrice.ToString();
        float seconds = MyHeart.Instance.TimeRecoverRemain;
        if(_counterAsync != null)
        {
            StopCoroutine(_counterAsync);
            _counterAsync = null;
        }
        _counterAsync = HeartRecoverCountDown(seconds);
        StartCoroutine(_counterAsync);
    }

    IEnumerator HeartRecoverCountDown(float seconds)
    {
        float timeRemain = seconds;
        while (timeRemain > 0)
        {
            _timerText.text = TimeFormatter.FormatSecondsToMinutesandSeconds(timeRemain);
            yield return new WaitForSeconds(1);
            timeRemain -= 1;
        }
        OnClickBack();
    }

    public void OnClickBuyFullHeart()
    {
        if(MyUserData.Instance.UserDataSave.Coin >= _heartPrice)
        {
            MyHeart.Instance.UpdateCurrentHeart(MyHeart.Instance.MaxHeart);
            MyUserData.Instance.UpdateUserCurrency(-_heartPrice);
            OnClickBack();
        }
        else
        {
            MyEvent.Instance.MainMenuEventManager.ChangeBottomTab(0);
            OnClickBack();
        }
    }

    public void OnClickAds()
    {
#if UNITY_EDITOR
        OnClickBack();
        MyHeart.Instance.UpdateCurrentHeart(1);
#endif
    }

    public void OnClickBack()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.NoHeartPopup);
    }
}
