using Imba.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopupController : UIPopup
{
    [SerializeField] RectTransform _focusObj;
    [SerializeField] RectTransform _targetRect;
    [SerializeField] GameObject _darkImage;
    [SerializeField] Animator _handAnimator;
    [SerializeField] GameObject _desObj;
    [SerializeField] TMP_Text _desText;
    [SerializeField] string[] _desString;

    bool _isCloseWhenClick = false;

    protected override void OnShowing()
    {
        base.OnShowing();

        if (Parameter != null)
        {
            RectTransform rect = (RectTransform)Parameter;
            CloneTargetAndFocus(rect);
            _isCloseWhenClick = true;
            _handAnimator.Play("HandAnimationClick");
            SetupDescription();
        }
        else
        {
            //level 0
            _isCloseWhenClick = true;
            ResetTarget();
            _handAnimator.Play("HandAnimationLevel0");
            _desObj.SetActive(false);
        }
    }

    private void ResetTarget()
    {
        _focusObj.anchoredPosition= Vector2.zero;
        EnableDarkImage(true);
    }

    private void SetupDescription()
    {
        _desObj.SetActive(true);
        _desText.text = _desString[MyUserData.Instance.GetCurrentUserLevel() - 1];
    }

    private void EnableDarkImage(bool enable)
    {
        if(enable)
        {
            _darkImage.GetComponent<Image>().color = new Color(0, 0, 0, 0);//.SetActive(false);
            _darkImage.GetComponent<Button>().interactable = true;
        }
        else
        {
            _darkImage.GetComponent<Image>().color = new Color(0, 0, 0, 0.9411765f);//.SetActive(false);
            _darkImage.GetComponent<Button>().interactable = false;
        }
    }

    private void CloneTargetAndFocus(RectTransform rect)
    {
        EnableDarkImage(false);
        RectTransform clonedRect = Instantiate(rect, _targetRect.transform);
        clonedRect.anchoredPosition = Vector2.zero;
        clonedRect.anchorMin = new Vector2(0.5f, 0.5f);
        clonedRect.anchorMax = new Vector2(0.5f, 0.5f);

        UIButton btn = clonedRect.GetComponent<UIButton>();
        btn.OnClick = rect.GetComponent<UIButton>().OnClick;
        btn.GetComponent<Button>().onClick.AddListener(NewButtonClick);

        _targetRect.anchoredPosition = SwitchToRectTransform(rect, _targetRect);
        SetPosFocus(_targetRect.anchoredPosition);
    }

    private void NewButtonClick()
    {
        Debug.Log("xx NewButtonClick");
        //destroy child in _targetRect
        foreach(Transform child in _targetRect.transform)
        {
            Destroy(child.gameObject);
        }
        //hide popup tut
        if(_isCloseWhenClick) UIManager.Instance.PopupManager.HidePopup(UIPopupName.TutorialPopup);
        //EventManager.Instance.QuestEvents.ClickTutorial();
    }

    public void OnButtonCloseClick()
    {
        MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.CHECK_MATCH);
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.TutorialPopup);
    }

    private void SetPosFocus(Vector2 posOnScreen)
    {
        _focusObj.anchoredPosition = posOnScreen;
    }


    /// Converts the anchoredPosition of the first RectTransform to the second RectTransform,
    /// taking into consideration offset, anchors and pivot, and returns the new anchoredPosition
    /// </summary>
    public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
    {
        Vector2 localPoint;
        Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * 0.5f + from.rect.xMin, from.rect.height * 0.5f + from.rect.yMin);
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
        screenP += fromPivotDerivedOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
        Vector2 pivotDerivedOffset = new Vector2(to.rect.width * 0.5f + to.rect.xMin, to.rect.height * 0.5f + to.rect.yMin);
        return to.anchoredPosition + localPoint - pivotDerivedOffset;
    }
}

public class TutorialData
{
    public RectTransform TargetRect;
    public bool CloseAfterClick;
}
