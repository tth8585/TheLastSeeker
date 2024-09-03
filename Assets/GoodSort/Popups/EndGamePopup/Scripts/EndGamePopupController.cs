using Imba.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePopupController : UIPopup
{
    [SerializeField] private TMP_Text _earnStarTxt;
    [SerializeField] private TMP_Text _earnStarDoubleTxt;

    [SerializeField] private Button _normalClaimBtn;
    [SerializeField] private float _timeToShowNormalClaimBtn;
    [SerializeField] private EndGamePopupAnimController _endGamePopupAnimController;

    private int _earnStars = 0;
    #region Popup Method
    protected override void OnShown()
    {
        base.OnShown();
        _endGamePopupAnimController.DoAnim();
    }
    protected override void OnHidden()
    {
        base.OnHidden();
        _endGamePopupAnimController.HideUIAnim();
    }
    protected override void OnShowing()
    {
        if (Parameter != null)
            _earnStars = (int)Parameter;

        _earnStarTxt.text = $"x{_earnStars}";
        _earnStarDoubleTxt.text = $"x{_earnStars * 2}";

        StartCoroutine(WaitToShowNormalClaimBtn());
    }
    #endregion

    #region Handle Method
    private void ClaimHandle(int boost)
    {
        var finalClaimValue = _earnStars * boost;
        Debug.Log($"<color=yellow>Final Claim Stars Value: {finalClaimValue}</color>");
        MyUserData.Instance.UpdateItemInfo(ITEM_TYPE.Star, finalClaimValue);
        MyUserData.Instance.UpdateProgressStreak(1);
    }
    
    private void ClaimedHandle()
    {
        MyGame.Instance.RestartGame();
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.EndGamePopup);
    }

    private IEnumerator WaitToShowNormalClaimBtn()
    {
        _normalClaimBtn.SetActive(false);
        yield return new WaitForSeconds(_timeToShowNormalClaimBtn);
        _normalClaimBtn.SetActive(true);
    }
    #endregion

    #region OnClick
    public void OnClickDoubleClaim()
    {
        ClaimHandle(2);
        ClaimedHandle();
    }

    public void OnClickClaim()
    {
        ClaimHandle(1);
        ClaimedHandle();
    }
    #endregion
}
