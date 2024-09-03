using Imba.UI;
using UnityEngine;

public class RePlayPopup : PrePlayPopup
{
    protected override void OnShown()
    {
        _adsBtn.SetActive(true);
        _currentLevel = MyUserData.Instance.UserDataSave.CurrentLevelData;

        if (_currentLevel >= 0)
        {
            _levelText.text = "Level " + _currentLevel.ToString();
            InitItemBar();
        }
        else
        {
            Debug.LogError("pre Load level failed");
        }
    }

    private void RePlayLevel()
    {
        MyEvent.Instance.GameEventManager.onLoadingAnimDone -= RePlayLevel;
        MyGame.Instance.RestartGame();
    }

    #region OnClick
    public override void OnClickPlayBtn()
    {
        if (MyHeart.Instance.CurrentHeartCount > 0)
        {
            MyEvent.Instance.GameEventManager.ClearLoadingAnimAction();
            MyEvent.Instance.GameEventManager.onLoadingAnimDone += RePlayLevel;
            MyHeart.Instance.UpdateCurrentHeart(-1);

            UIManager.Instance.PopupManager.HidePopup(UIPopupName.RePlayPopup);
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.LoadingPopup);
        }
        else
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.NoHeartPopup);
        }
    }

    public void OnClickClosePopup()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.RePlayPopup);
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.LoadingPopup);
    }
    #endregion
}
