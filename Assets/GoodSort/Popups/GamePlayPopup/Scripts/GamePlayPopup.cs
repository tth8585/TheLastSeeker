using DG.Tweening;
using Imba.UI;
using System.Collections;
using System.Linq;
using TMPro;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayPopup : UIPopup
{
    [SerializeField] private Image _avt;
    [SerializeField] private ComboBarController _comboBarController;
    [SerializeField] private ItemBarController _itemBar;
    //[SerializeField] private Slider _timeSlider;
    [SerializeField] private TMP_Text _timeTxt;
    [SerializeField] private TMP_Text _starCountTxt;
    [SerializeField] private TMP_Text _levelTxt;

    [Header("TextFx")]
    [SerializeField] private UIEffectPool _effectPool;
    [SerializeField] private float _textFXDuration = .5f;
    [SerializeField] private float _shakeStreng = 20f;
    [SerializeField] private float _shakeDuration = .3f;
    [SerializeField] private Ease _textFXEase = Ease.OutBack;

    [Header("Magic & Refresh Anim")]
    [SerializeField] private Image _magicTick;
    [SerializeField] private Transform _magicEndPos;

    [Header("Screen Effect")]
    [SerializeField] private Image _screenEffectImg;
    [SerializeField] [Range(0, 1)] private float _minAlphaValue = -.5f;
    [SerializeField] private int _effectCycle = 5;
    [SerializeField] private float _screenEffectSpeed = 10f;
    [SerializeField] private ScreenEffectSO _effectDatas;
    private OverlayEffectType _currentOverlayEffectType = OverlayEffectType.None;
    private Coroutine _currentEffect;

    public ComboBarController ComboBarController => _comboBarController;
    public TMP_Text TimeTxt => _timeTxt;
    public TMP_Text StarCountTxt => _starCountTxt;

    [SerializeField] GameObject[] _hideObjWhenInTutorial;

    [SerializeField] private UseAbilityEffectController _useAbilityEffectController;
    private void OnEnable()
    {
        MyEvent.Instance.GameEventManager.onStartNewLevel += ResetUI;
    }
    private void OnDisable()
    {
        MyEvent.Instance.GameEventManager.onStartNewLevel -= ResetUI;
    }
    protected override void OnShowing()
    {
        base.OnShowing();
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.BottomTabs);
        ResetUI();
    }

    protected override void OnHiding()
    {
        base.OnHiding();
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.BottomTabs);
    }

    public void ResetUI()
    {
        _starCountTxt.text = "0";
        _levelTxt.text = "Lv. " + MyUserData.Instance.UserDataSave.CurrentLevelData;

        var currentPlayerAvt = MyUserData.Instance.UserDataSave.CurrentAvatarId;
        _avt.sprite = Resources.Load<Sprite>($"Icons/Avatars/{currentPlayerAvt}");

        MyItemAbility.Instance.SetBarItemController(_itemBar);
    }

    public void HandleForTutorialUI()
    {
        int currentLevel = MyUserData.Instance.GetCurrentUserLevel();
        bool isTutorial = currentLevel == 0?true:false;

        for (int i = 0; i < _hideObjWhenInTutorial.Length; i++)
        {
            _hideObjWhenInTutorial[i].SetActive(!isTutorial);
        }
    }
    public void DoDoubleStarEffect()
    {
        _useAbilityEffectController.DoDoubleStarEffect();
    }

    public RectTransform GetRectAbility(int index)
    {
        if (index == 0 || index > 4)
        {
            Debug.LogError("cant get rect ability has index: " + index);
            return null;
        }

        return _itemBar.GetRectAbility(index - 1);
    }

    public void DoMagicWandAnim(TweenCallback callback)
    {
        _useAbilityEffectController.DoMagicEffect(callback);
    }

    public void DoRefreshEffect(TweenCallback callback)
    {
        _useAbilityEffectController.DoRefreshEffect(callback);
    }
    public void DoHitEffect()
    {
        _useAbilityEffectController.DoHitEffect();
    }

    public void DoBoomAnimEffect()
    {
        _useAbilityEffectController.DoBoomEffect();
    }
    public void DoTimeAnimEffect()
    {
        _useAbilityEffectController.DoTimeEffect();
    }

    public void DoTextAnim(Vector3 start, string content, Color color)
    {
        var txtObj = _effectPool.GetObject();
        try
        {
            var fxTxt = txtObj.GetComponent<TMP_Text>(); // switch to get image
            fxTxt.text = content; // text switch to sprite, with sprite get from config 
            fxTxt.color = color;

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(start);

            fxTxt.rectTransform.position = new Vector3(screenPosition.x + Random.Range(-100, 100), screenPosition.y + Random.Range(-100, 100), screenPosition.z);
            fxTxt.SetActive(true);
            fxTxt.gameObject.transform.localScale = Vector3.zero;
            fxTxt.gameObject.transform.DOScale(Vector3.one, _textFXDuration).SetEase(_textFXEase).OnComplete(() =>
            {
                fxTxt.gameObject.transform.DOShakeRotation(_shakeDuration, _shakeStreng).OnComplete(() =>
                {
                    _effectPool.ReturnStarToPool(txtObj);
                });
            });
        }
        catch 
        {
            Debug.Log("Can't do anim");
        }
    }

    #region Time out and frozen bg effect
    public void ForceStopCurrentEffect()
    {
        StopCoroutine(_currentEffect);
    }

    public void ShowOverlayEffect(OverlayEffectType type, float time)
    {
        if (_currentOverlayEffectType == type || type == OverlayEffectType.None) return;
        if (_currentEffect != null) ForceStopCurrentEffect();

        _currentOverlayEffectType = type;
        Debug.Log("Use item ::"+ type.ToString());
        if (type == OverlayEffectType.Frozen)
        {
            _useAbilityEffectController.DoFrozenEffect();
            _currentOverlayEffectType = OverlayEffectType.None;
            _currentEffect = null;
            _screenEffectImg.gameObject.SetActive(false);
            return;
        }
        _screenEffectImg.sprite = _effectDatas.ScreenEffects.Where(e => e.EffectType== type).FirstOrDefault().EffectSprite;
        _screenEffectImg.gameObject.SetActive(true);
        _currentEffect = StartCoroutine(ApplyScreenEffect(time));
    }

    private IEnumerator ApplyScreenEffect(float time)
    {
        float a = _screenEffectImg.color.a;
        float targetA = _minAlphaValue;
        float effectCycleTime = time / _effectCycle;
        float currentTime = 0;

        while (time > 0)
        {
            if(currentTime > 0)
            {
                if (a <= _minAlphaValue)
                {
                    targetA = 1;
                    currentTime = 0;
                }
                if (a >= 1)
                {
                    targetA = _minAlphaValue;
                    currentTime = 0;
                }
            }

            // lerp value alpha of the screen effect img color
            currentTime += Time.deltaTime * _screenEffectSpeed;
            a = Mathf.Lerp(a, targetA, currentTime / effectCycleTime);
            // apply changed alpha
            _screenEffectImg.SetAlpha(a);
            yield return null;

            time -= Time.deltaTime;
        }

        _currentOverlayEffectType = OverlayEffectType.None;
        _currentEffect = null;
        _screenEffectImg.gameObject.SetActive(false);
        _screenEffectImg.SetAlpha(1f);
    }
    #endregion

    #region OnClick
    public void OnClickPauseGame()
    {
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PausePopup);
    }
    #endregion
}

public enum OverlayEffectType
{
    None,
    TimeOut,
    Frozen
}