using Imba.UI;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardPopupController : UIPopup
{
    [SerializeField] TimeOfferController _timeOfferController;
    [SerializeField] DaySliderController _daySliderController;
    [SerializeField] DayCountController _dayCountController;

    private bool _canClaim = true;

    protected override void OnShowing()
    {
        base.OnShowing();

        Debug.Log("xx show DailyRewardPopup");
        _timeOfferController.InitUI();
        _daySliderController.InitData(MyDailyReward.Instance.GetTotalDayLogin(), MyDailyReward.Instance.GetRewardMonthData());
        _dayCountController.InitData(MyDailyReward.Instance.GetTotalDayLogin(), MyDailyReward.Instance.GetRewardData());
    }

    public void OnClickClaimReward()
    {
        if (!_canClaim) return;
        _canClaim = false;
        //claim reward
        DailyRewardDataConfig reward = _dayCountController.GetRewardToday();

        //code logic add reward here
        _dayCountController.UpdateUIWhenClaim();
        MyClaimReward.Instance.Claim(GetIconReward(), ()=>OnClaim(reward));
    }

    private Image GetIconReward()
    {
        return _dayCountController.GetImageReward();
    }

    private void OnClaim(DailyRewardDataConfig config)
    {
        //Const.OnClaim(config.Type, config.Value);

        OnClickClosePopup();
    }

    public void OnClickWatchAds()
    {
#if UNITY_EDITOR
        //UIManager.Instance.PopupManager.ShowPopup(UIPopupName.MessagePopup, "Ads is not Available now");
        //MyEvent.Instance.AdsEvents.WatchAdsForDoubleRewardDaily(_dayCountController.GetRewardToday());
        MyDailyReward.Instance.SetHasGotDailyReward();
        OnClickClosePopup();
        //#else
        //        if (MyAds.Instance.IsRewardAdsAvailable())
        //        {
        //            MyAds.Instance.ShowRewardAds();
        //            MyEvent.Instance.AdsEvents.WatchAdsForDoubleRewardDaily(_dayCountController.GetRewardToday());
        //            AnalyticsManager.LogHotelEvent(HotelAnalyticsEventName.daily_reward_claim_x2_video);
        //            OnClickClosePopup();
        //        }
        //        else
        //        {
        //            //ads is not available
        //            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.MessageBox, "Ads is not Available now");
        //        }
#endif
    }

    public void OnClickClosePopup()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.DailyRewardPopup);
    }
}
