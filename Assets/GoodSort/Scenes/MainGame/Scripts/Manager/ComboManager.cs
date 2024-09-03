using Imba.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [SerializeField] ComboCSVReader _comboConfig;
    [SerializeField] ComboBarController _comboBarController;

    private Dictionary<int, ComboDataConfig> _dicCombo = new Dictionary<int, ComboDataConfig>();

    [SerializeField] private Vector3 _fxPosition;
    private float _currentTimeCombo = 0f;
    private float _totalTimeCombo = 0f;
    private int _comboCount = 0;
    private int _comboStar = 1;
    private string _floatText = string.Empty;

    private void OnEnable()
    {
        MyEvent.Instance.GameEventManager.onMatchItem += CheckCombo;
        MyEvent.Instance.GameEventManager.onSetMatchPosition += SetFXPosition;

        //ResetCombo();
    }

    private void OnDisable()
    {
        MyEvent.Instance.GameEventManager.onMatchItem -= CheckCombo;
        MyEvent.Instance.GameEventManager.onSetMatchPosition -= SetFXPosition;
    }

    private void Update()
    {
        if(_currentTimeCombo > 0f)
        {
            _currentTimeCombo-= Time.deltaTime;
            _comboBarController.UpdateTimeLeft(_currentTimeCombo, _totalTimeCombo);

            if (_currentTimeCombo <= 0f ) 
            {
                ResetCombo();
            }
        }
    }

    public void SetComboBarController(ComboBarController comboBar) => _comboBarController = comboBar;

    private void SetFXPosition(Vector3 position)
    {
        _fxPosition = position;
    }

    public void CheckCombo()
    {
        if(_comboCount == 0)
        {
            //not in combo
            MyGame.Instance.AddStar(_comboStar);
            _comboCount++;
            ComboDataConfig data = GetComboConfig(_comboCount);
            _currentTimeCombo = data.TimeValue;
            _totalTimeCombo = data.TimeValue;
            _comboStar = data.StarBonusValue;
            _floatText = data.FloatingText;
        }
        else
        {
            if (_currentTimeCombo > 0)
            {
                MyGame.Instance.AddStar(_comboStar);
                _comboCount++;
                ComboDataConfig data = GetComboConfig(_comboCount);
                _currentTimeCombo = data.TimeValue;
                _totalTimeCombo = data.TimeValue;
                _comboStar = data.StarBonusValue;
                _floatText = data.FloatingText;

                if(_comboCount == 5)
                {
                    MyEvent.Instance.QuestEvents.GetComboQuest(5);
                }

                if(_comboCount == 7)
                {
                    MyEvent.Instance.QuestEvents.GetComboQuest(7);
                }
            }
            else
            {
                //not need handle reset combo here
            }
        }

        //update combo star UI
        _comboBarController.UpdateStarBonusValue(_comboStar);
        _comboBarController.UpdateComboFloatingText(_floatText);
        _comboBarController.UpdateComboValue(_comboCount);

        // play fx
        //var startPos = MyCam.Instance.GetMiddleCamObjectPos();

        GamePlayPopup gamePlayPopup = (GamePlayPopup)UIManager.Instance.PopupManager.GetPopup(UIPopupName.GamePlayPopup);
        gamePlayPopup.DoTextAnim(_fxPosition, _floatText, Color.yellow);

        MyEvent.Instance.GameEventManager.StartStarFly(_comboBarController.transform, _comboStar, _fxPosition, _comboBarController.StarImgTrans.position);
    }

    private void ResetCombo()
    {
        //reset combo
        _comboCount = 0;
        //add combo star to bonus star
        //MyGame.Instance.AddStar(_comboStar);
        _comboStar = 0;
        //anim reset combo ?
        _comboBarController.ResetUI();
    }

    private ComboDataConfig GetComboConfig(int count)
    {
        //load config combo
        if (_dicCombo.Count == 0)
        {
            _dicCombo = _comboConfig.LoadConfig();
        }

        if(_dicCombo.ContainsKey(count))
        {
            return _dicCombo[count];
        }
        else
        {
            if(count> _dicCombo.Count && _dicCombo.Count > 0)
            {
                //over max combo
                return _dicCombo[_dicCombo.Count-1];
            }
            else
            {
                Debug.LogError("Something error in get value from combo config, need to check");
            }
        }

        return null;
    }
}
