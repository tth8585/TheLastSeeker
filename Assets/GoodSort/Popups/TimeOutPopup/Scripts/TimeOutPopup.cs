using Imba.UI;
using TMPro;
using UnityEngine;

public class TimeOutPopup : UIPopup
{
    [SerializeField] private GameObject _timeOutPanel;
    [SerializeField] private GameObject _saveProgressPanel;

    [SerializeField] private TMP_Text _coin;

    protected override void OnShowing()
    {
        _timeOutPanel.SetActive(true);
        _saveProgressPanel.SetActive(false);

        _coin.text = MyUserData.Instance.UserDataSave.Coin.ToString();
    }

    #region
    public void OnClickCloseTimeOutPanel()
    {
        _timeOutPanel.SetActive(false);
        _saveProgressPanel.SetActive(true);
    }

    public void OnClickCloseSaveProgressPanel()
    {
        _timeOutPanel.SetActive(false);
        _saveProgressPanel.SetActive(false);

        // delete win progress streak
        MyUserData.Instance.UpdateProgressStreak(-1);

        UIManager.Instance.PopupManager.HidePopup(UIPopupName.TimeOutPopup);
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.RePlayPopup);
    }

    //public void OnClickCloseReplayGamePanel()
    //{
    //    Debug.Log($"<color=red>On click close replay game panel -> back to main game</color>");
    //}
    #endregion
}
