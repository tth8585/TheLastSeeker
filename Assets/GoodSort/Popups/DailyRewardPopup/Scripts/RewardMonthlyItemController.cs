using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardMonthlyItemController : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] TMP_Text _valueText, _decoValueText;

    public void InitData(string spriteName, int value)
    {
        _icon.sprite = MyAtlas.Instance.GetCommonSprite(spriteName);
        _valueText.text = value.ToString();
        _decoValueText.text = value.ToString();
    }

    public Image GetIcon() { return _icon; }
}
