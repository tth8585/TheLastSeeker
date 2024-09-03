using Imba.UI;
using UnityEngine;

public class TimeOutPanel : MonoBehaviour
{
    [SerializeField] private int _timePrice = 600;
    [SerializeField] private float _timeBuyValue = 60f;

    bool isHasAds = false;

    private void UseCoinGetTime()
    {
        MyUserData.Instance.UpdateUserCurrency(-_timePrice);

        MyGame.Instance.SetUpTimePlay(_timeBuyValue);
        MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.DRAG_AND_DROP);
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.TimeOutPopup);
    }

    #region OnClick
    public void OnClickUseCoinGetTime()
    {
        var currentPlayerCoin = MyUserData.Instance.UserDataSave.Coin;
        if (currentPlayerCoin < _timePrice)
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.ShopPopup, true);
            var shopPopup = (ShopPopup) UIManager.Instance.PopupManager.GetPopup(UIPopupName.ShopPopup);
            shopPopup.transform.SetAsLastSibling(); // not use alwaysOnTop because of prevent over the tab popup
            return;
        }

        UseCoinGetTime();
    }   
    
    public void OnClickWatchAdsGetTime()
    {
        Debug.Log($"</color=red>----Watch Ads---- Continue play with 60 seconds...</color>");
        if (!isHasAds) return; // remove this and condition variable when has ads feature

        UseCoinGetTime(); // assign this as callback when player has been watched the ads
    }
    #endregion
}
