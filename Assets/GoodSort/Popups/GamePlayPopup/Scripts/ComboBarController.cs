using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboBarController : MonoBehaviour
{
    [SerializeField] TMP_Text _comboValueText, _comboFloatingText,_comboStarBonusText;
    [SerializeField] TMP_Text _timeComboLeft;
    [SerializeField] Slider _timeComboLeftSlider;

    [Header("ComboFX")]
    [SerializeField] Image _starImg;
    [SerializeField] TMP_Text _currentStar;

    public Transform StarImgTrans => _starImg.transform;

    private void Start()
    {
        ResetUI();
    }

    public void UpdateTimeLeft(float timeValue, float totalTime)
    {
        if(timeValue>0)
        {
            _timeComboLeftSlider.value = timeValue/totalTime;
        }
        else
        {
            _timeComboLeftSlider.value = 0;
        }
    }

    public void UpdateComboValue(int comboCount)
    {
        _comboValueText.text = "Combo x" + comboCount;
    }

    public void UpdateComboFloatingText(string stringValue)
    {
        _comboFloatingText.text = stringValue;
    }

    public void UpdateStarBonusValue(int starValue)
    {
        _comboStarBonusText.text = "+" + starValue + " Star";
    }

    public void ResetUI()
    {
        UpdateComboValue(0);
        UpdateComboFloatingText(string.Empty);
        UpdateStarBonusValue(0);
        UpdateTimeLeft(0, 1);
    }
}
