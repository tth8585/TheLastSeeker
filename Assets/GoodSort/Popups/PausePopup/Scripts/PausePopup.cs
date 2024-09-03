using Imba.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PausePopup : SettingsPopup
{
    [Header("Pause")]
    [SerializeField] private TMP_Text _popupTitle;
    [SerializeField] private GameObject _pausePanel;

    [Header("Confirm Quit")]
    [SerializeField] private GameObject _confirmQuitPanel;
    [SerializeField] private TMP_Text _loseStar;

    #region Popup show/hide
    protected override void OnShown()
    {
        // Pause game here
        MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.PAUSE);

        _popupTitle.text = "Pause";
        _confirmQuitPanel.SetActive(false);
        _pausePanel.SetActive(true);
    }

    protected override void OnHidden()
    {
        // Resume game here
        if(MyGame.Instance.CurrentState == GAMEPLAY_STATE.PAUSE)
            MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.RESUME);

        _confirmQuitPanel.SetActive(false);
    }
    #endregion

    #region OnClick
    public override void OnClickHideSetting()
    {
        Debug.Log($"<color=purple>Hide Pause Popup Button Clicked</color>");
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.PausePopup);
    }

    public void OnClickQuitGame()
    {
        _popupTitle.text = "Exit the level";
        _pausePanel.SetActive(false);
        _confirmQuitPanel.SetActive(true);

        _loseStar.text = MyGame.Instance.CurrentStar.ToString();
    }

    public void OnClickConfirmQuitGame()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.PausePopup);
        if (Time.timeScale == 0) Time.timeScale = 1;

        MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.STOP);
    }
    #endregion
}
