using Imba.UI;
using System.Collections;
using TMPro;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : UIView
{
    [SerializeField] TMP_Text _userNameText, _userNameShadowText, _coinText, _starText, _levelText;
    [SerializeField] private Image _avtImg;

    private int _currentlevel = 1;

    private void OnEnable()
    {
        StartCoroutine(AddEventListener());
        Debug.Log("Enableeeeee first");
        MyEvent.Instance.GameEventManager.onLoadingAnimDone += LoadScene;
    }

    private void OnDisable()
    {
        MyEvent.Instance.UserDataManagerEvent.onLoadedUserData -= UpdateUI;
        //MyEvent.Instance.GameEventManager.onLoadingAnimDone -= LoadScene;
    }

    protected override void OnShowing()
    {
        Debug.Log("Showwww first");
        MyEvent.Instance.GameEventManager.ClearLoadingAnimAction();
        MyEvent.Instance.GameEventManager.onLoadingAnimDone += LoadScene;
    }    

    //protected override void OnHiding()
    //{
    //    MyEvent.Instance.GameEventManager.onLoadingAnimDone -= LoadScene;
    //

    public void LoadScene()
    {
        MyEvent.Instance.GameEventManager.onLoadingAnimDone -= LoadScene;
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }

    IEnumerator AddEventListener()
    {
        yield return new WaitUntil(() => MyEvent.Instance != null);
        //yield return null;

        MyEvent.Instance.UserDataManagerEvent.onLoadedUserData += UpdateUI;
    }


    public void UpdateUI()
    {
        LoadUserDataUI();
    }

    #region LOAD UI
    private void LoadUserDataUI()
    {
        UserDataSave userData = MyUserData.Instance.UserDataSave;

        var avtId = userData.CurrentAvatarId;
        _avtImg.sprite = Resources.Load<Sprite>($"Icons/Avatars/{avtId}");
        _userNameText.text = userData.UserName;
        _userNameShadowText.text = userData.UserName;
        _coinText.text = FormatText.FormatTextCount(userData.Coin);
        _starText.text = MyUserData.Instance.DicItemDatas[ITEM_TYPE.Star].ToString();
        _levelText.text = "LEVEL "+userData.CurrentLevelData;
    }
    private string FormatTextCount(int count)
    {
        if(count > 1000)
        {
            count = count / 1000;
            return count + "k";
        }
        return count.ToString();
    }

    #endregion LOAD UI

    #region BUTTON CLICK
    public void OnClickBattlePass()
    {
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.GSPassPopup);
    }
    public void OnClickQuestPass()
    {
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.QuestPopup);
    }  

    public void OnClickPlayGame()
    {
        _currentlevel = MyUserData.Instance.UserDataSave.CurrentLevelData;
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PrePlayPopup, _currentlevel);
    }

    public void OnClickSetting()
    {
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.SettingsPopup);
    }

    public void OnClickProfile()
    {
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.ProfilePopup);
    }
    #endregion
}

public class MyMainView : SingletonMonoBehaviour<MainMenuView> { }

public static class FormatText
{
    public static string FormatTextCount(int count)
    {
        if (count > 1000)
        {
            count = count / 1000;
            return count + "k";
        }
        return count.ToString();
    }
}