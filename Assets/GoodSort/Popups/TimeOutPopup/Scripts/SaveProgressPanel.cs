using Imba.UI;
using TMPro;
using UnityEngine;

public class SaveProgressPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentLvl;
    [SerializeField] private int _saveProgressPrice = 800;
    [SerializeField] internal GameObject[] _barFills;

    bool isHasAds = false;

    void OnEnable()
    {
        var currentLvl = MyUserData.Instance.UserDataSave.CurrentLevelData;
        _currentLvl.text = $"Level {currentLvl}";
        InitProgress();
    }

    private void InitProgress()
    {
        int progressStreak = MyUserData.Instance.UserDataSave.ProgressStreak;

        foreach (var item in _barFills)
        {
            item.SetActive(false);
        }

        if (progressStreak > 0)
        {
            for (int i = 0; i < progressStreak; i++)
            {
                _barFills[i].SetActive(true);
            }
        }
    }

    private void SaveWinStreakProgress()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.TimeOutPopup);
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.RePlayPopup);
    }

    #region OnClick
    public void OnClickUseCoinSaveProgress()
    {
        var currentPlayerCoin = MyUserData.Instance.UserDataSave.Coin;
        if (currentPlayerCoin < _saveProgressPrice)
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.ShopPopup, true);
            var shopPopup = (ShopPopup)UIManager.Instance.PopupManager.GetPopup(UIPopupName.ShopPopup);
            shopPopup.transform.SetAsLastSibling();
            return;
        }

        SaveWinStreakProgress();
    }

    public void OnClickWatchAdsSaveProgress()
    {
        Debug.Log($"</color=red>----Watch Ads---- Progress saving...</color>");
        if (!isHasAds) return; // remove this and condition variable when has ads feature

        SaveWinStreakProgress(); // assign this as callback when player has been watched the ads
    }
    #endregion
}
