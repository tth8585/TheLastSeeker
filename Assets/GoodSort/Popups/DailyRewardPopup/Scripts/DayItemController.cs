using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayItemController : MonoBehaviour
{
    [SerializeField] Image _icon,_bg;
    [SerializeField] TMP_Text _valueText, _valueDecoText,_dayText;
    [SerializeField] Sprite _selectedSprite;
    [SerializeField] GameObject _completeObj;
    [SerializeField] Color _selectedColor;

    private DailyRewardDataConfig _data;

    public void InitData(DailyRewardDataConfig data)
    {
        _data = data;
        _valueText.text = _data.Value.ToString();
        _valueDecoText.text = _data.Value.ToString();
        _icon.sprite = MyAtlas.Instance.GetCommonSprite(data.Type);
    }

    public void Complete()
    {
        _completeObj.SetActive(true);
    }

    public void Unlock()
    {
        _completeObj.SetActive(false);
        _bg.sprite = _selectedSprite;
        _dayText.color = _selectedColor;
    }

    public void Lock()
    {
        _completeObj.SetActive(false);
    }

    public Image GetIcon()
    {
        return _icon;
    }
}
