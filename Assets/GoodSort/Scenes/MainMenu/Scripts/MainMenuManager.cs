using Imba.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] UserDataManager _userData;
    [SerializeField] ItemAbilityManager _abilityManager;
    [SerializeField] LevelManager _levelManager;

    private void Start()
    {
        _levelManager.InitData();
        _abilityManager.InitData();
        _userData.InitData();
        MyMainView.Instance.UpdateUI();

        if(_userData.GetCurrentUserLevel()== 0)
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.LoadingPopup);
        }
    }
}
