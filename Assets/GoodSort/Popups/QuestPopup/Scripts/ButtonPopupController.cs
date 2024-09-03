using Imba.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPopupController : MonoBehaviour
{
    [SerializeField] Image _bgBtn;
    [SerializeField] Sprite _activeBg, _inactiveBg;
    [SerializeField] TMP_Text _text;
    [SerializeField] TMP_FontAsset _activeFont, _inactiveFont;

    public void SetButtonSprite(bool active)
    {
        if (active)
        {
            _bgBtn.sprite = _activeBg;
            _text.font = _activeFont;
        }
        else
        {
            _bgBtn.sprite = _inactiveBg;
            _text.font = _inactiveFont;
        }
    }

    public void UpdateUI(bool isActive)
    {
        SetButtonSprite(isActive);
    }

    public void OnClickGoBtn()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.QuestPopup);
    }

    public void SetActive(bool isActive)
    {
        _bgBtn.enabled = isActive;
        _text.enabled = isActive;
    }
}
