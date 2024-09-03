using System.Collections;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Slider _timeSlider;
    [SerializeField] TMP_Text _timeTxt;
    [SerializeField] float _timeCounting = .1f;
    [SerializeField] float _timeDelayCounting = 1.5f;

    private float decreaseRate = 1f;
    private float currentValue;
    private bool _isPauseSlider = false;
    private float _timePlay = 0f;

    private bool _isNearTimedOut = false;
    [SerializeField] private bool _hasCoutingEffect = false;
    private float _itemTimeValue = 60;

    //temp
    internal bool isTimeout = false;

    private bool IsPauseTime
    {
        get { return _isPauseSlider; }
    }

    private bool _isTutorial = true;

    public void SetTimeSlider(UnityEngine.UI.Slider slider) => _timeSlider = slider;
    public void SetTimeTxt(TMP_Text timeTxt) => _timeTxt = timeTxt;

    public void PauseTime(float pauseTime)
    {
        StartCoroutine(COPauseSlider(pauseTime));
    }

    public void SetUpTimePlay(float value, bool isTutorial, bool hasEffect)
    {
        _hasCoutingEffect = hasEffect;

        if (_hasCoutingEffect)
        {
            UpdateTimeWithCountingEffect(value);
        }
        else
        {
            _isTutorial = isTutorial;
            _timePlay = value;
            currentValue = _timePlay;
            if(_timeTxt!= null) 
                UpdateSliderValue();
        }
    }

    private IEnumerator COPauseSlider(float time)
    {
        _isPauseSlider = true;
        yield return new WaitForSeconds(time);
        _isPauseSlider = false;
    }

    private void UpdateSliderValue()
    {
        UpdateTimerUI(currentValue);
        if (currentValue <= 0f)
        {
            isTimeout = true;
            MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.END);
        }
    }

    public void UpdateTimeWithCountingEffect(float value)
    {
        StartCoroutine(UpdateTimeAsync(value));
    }

    IEnumerator UpdateTimeAsync(float value)
    {
        _timePlay = value;
        float targetTime = _timePlay + _itemTimeValue;
        yield return new WaitForSeconds(_timeDelayCounting);

        while (_timePlay < targetTime)
        {
            _hasCoutingEffect = true;
            yield return new WaitForSeconds(_timeCounting);
            _timePlay += 4;
            UpdateTimerUI(_timePlay);
        }

        currentValue = targetTime;
        if (_timeTxt != null)
            UpdateSliderValue();

        _hasCoutingEffect = false;
    }

    private void UpdateTimerUI(float timeValue)
    {
        var value = timeValue;
        var minute = (int)value / 60;
        var seconds = (int)value - minute * 60;

        string minuteText = minute.ToString();
        if (minute < 10)
        {
            minuteText = $"0{minute}";
        }
        string secondsText = seconds.ToString();

        if (seconds < 10)
        {
            secondsText = $"0{seconds}";
        }
        _timeTxt.text = $"{minuteText}:{secondsText}";
    }

    private void Update()
    {
        if (_isTutorial) return;

        if (_hasCoutingEffect) return;

        if (MyGame.Instance.CurrentState != GAMEPLAY_STATE.END && !IsPauseTime)
        {
            currentValue -= decreaseRate * Time.deltaTime;
            currentValue = Mathf.Max(currentValue, 0f); // Ensure currentValue does not go below 0
            UpdateSliderValue();

            if(currentValue <= 10)
            {
                MyGame.Instance.ShowScreenEffect(OverlayEffectType.TimeOut, currentValue); 
                _isNearTimedOut = true;
            }
            if(currentValue > 10 && _isNearTimedOut)
            {
                MyGame.Instance.ForceStopScreenEffect();
                _isNearTimedOut = false;
            }
        }
    }
}
