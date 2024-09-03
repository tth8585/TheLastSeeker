using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UseAbilityEffectController : MonoBehaviour
{
    [Header("Anim Values")]
    [SerializeField] float _moveDuration = .3f;
    [SerializeField] float _scaleDownDuration = .2f;
    [SerializeField] Ease _moveEase = Ease.OutBack;
    [SerializeField] Ease _scaleDownEase = Ease.OutQuad;
    [SerializeField] Ease _scaleUpEase = Ease.Linear;

    [Header("Frozen Effect")]
    [SerializeField] Image _frozenEffectImg;
    [SerializeField] Image _frozenEffectBackground;
    float _frozenEffectDuration = 5;
    [SerializeField] Ease _frozenEffectEase = Ease.Linear;
    [SerializeField] Ease _frozenEffectOutEase = Ease.InBack;
    [SerializeField] Image _frozenIcon;
    [SerializeField] ParticleSystem _frostExploFX;

    [Header("Refresh Effect")]
    [SerializeField] Image _refreshIcon;
    [SerializeField] float _refreshScaleUpDuration = .5f;
    [SerializeField] float _refreshRotateDuration = 1f;
    [SerializeField] Vector3 _refreshIconScale = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] ParticleSystem _refreshFX;
    [SerializeField] ParticleSystem _refreshExploFX;

    [Header("Magic Effect")]
    [SerializeField] Transform _magicIcon;
    [SerializeField] Transform _magicIconStartPos;
    [SerializeField] Transform _magicIconEndPos;
    [SerializeField] Vector3 _magicIconScale = new Vector3(.8f, .8f, .8f);

    [Header("Boom Effect")]
    [SerializeField] Transform _boomIcon;
    [SerializeField] Transform _boomIconStartPos;
    [SerializeField] Transform _boomIconEndPos;
    [SerializeField] float _boomShakeDuration = .3f;
    [SerializeField] int _boomShakeStrength = 10;
    [SerializeField] Vector3 _boomIconScale = new Vector3(.8f, .8f, .8f);

    [Header("Time Effect")]
    [SerializeField] Image _timeIcon;
    [SerializeField] Transform _timerCountIcon;
    [SerializeField] Transform _timeObject;
    [SerializeField] Transform _timeStartPos;
    [SerializeField] Transform _clockWise;
    [SerializeField] float _timeRotateDuration = .5f;
    [SerializeField] float _timeMoveDuration = 1f;
    [SerializeField] float _timeScaleDownDelay = 2f;
    [SerializeField] Vector3 _clockWiseRotate = new Vector3(0, 0, -350);
    [SerializeField] ParticleSystem _timeParticleExplo;
    [SerializeField] ParticleSystem _timeSpakleFX;

    [Header("DoubleStar Effect")]
    [SerializeField] Image _doubleStarIcon;
    [SerializeField] Transform _doubleStar;
    [SerializeField] Transform _x2StartIcon;
    [SerializeField] Transform _doubleStarStartPos;
    [SerializeField] Transform _starEndPos;
    [SerializeField] Transform _starParticle;
    [SerializeField] ParticleSystem _starSpakleFX;
    [SerializeField] float _starMoveDuration = 1f;
    [SerializeField] Ease _starMoveEase = Ease.OutQuad;

    [Header("Hit Effect")]
    [SerializeField] Image _hitIcon;
    [SerializeField] float _hitScaleUpDuration;
    [SerializeField] float _hitRotateStartDuration;
    [SerializeField] float _hitRotateEndDuration;
    [SerializeField] int _hitRotateAngle = 120;
    [SerializeField] float _hitFadeDuration = .5f;
    [SerializeField] float _timeDelayHit = .2f;
    [SerializeField] ParticleSystem _hitInFX;
    [SerializeField] ParticleSystem _hitExploFX;
    int count = 3;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DoHitEffect();
        }
    }
    private void OnEnable()
    {
        _magicIcon.gameObject.SetActive(false);
        _boomIcon.gameObject.SetActive(false);
        _x2StartIcon.gameObject.SetActive(false);
        _hitIcon.gameObject.SetActive(false);
        _refreshIcon.gameObject.SetActive(false);
        _frozenIcon.gameObject.SetActive(false);
    }

    public void DoHitEffect()
    {
        //reset value
        _hitIcon.transform.localScale = Vector3.zero;
        _hitIcon.gameObject.SetActive(true);
        _hitIcon.SetAlpha(1);
        _hitIcon.transform.rotation = Quaternion.Euler(0, 0, 0);

        _hitIcon.transform.DOScale(Vector3.one, _hitScaleUpDuration-.1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            count = 3;
            StartCoroutine(LoopingHit());
        });
    }
    IEnumerator LoopingHit()
    {
        if (count <= 0) 
        {
            _hitIcon.DOFade(.2f, _hitFadeDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                _hitIcon.gameObject.SetActive(false);
            });
            yield return null;
        }
        else
        {
            if(count < 3)
            yield return new WaitForSeconds(_timeDelayHit);

            count--;
            _hitIcon.transform.DORotate(new Vector3(0, 0, -_hitRotateAngle), _hitRotateStartDuration).SetEase(Ease.OutQuad).OnPlay(() =>
            {
                _hitInFX.Stop();

                _hitInFX.Play();
            }).OnComplete(() =>
            {
                _hitIcon.transform.DORotate(new Vector3(0, 0, _hitRotateAngle), _hitRotateEndDuration).OnPlay(() =>
                {
                    _hitExploFX.Stop();

                    _hitExploFX.Play();
                }).SetEase(Ease.Linear).OnComplete(() =>
                {
                    StartCoroutine(LoopingHit());
                });
            });
        }
    }
    public void DoRefreshEffect(TweenCallback callback)
    {
        _refreshIcon.transform.localScale = Vector3.one;
        _refreshIcon.transform.rotation = Quaternion.identity;
        _refreshIcon.gameObject.SetActive(true);
        _refreshIcon.SetAlpha(1f);

        _refreshFX.Play();
        _refreshIcon.transform.DOScale(_refreshIconScale, _refreshScaleUpDuration);
        _refreshIcon.transform.DORotate(new Vector3(0,0,180), _refreshRotateDuration/2).SetEase(Ease.Linear).OnComplete(()=>
        {
            _refreshIcon.transform.DORotate(new Vector3(0, 0, 359), _refreshRotateDuration / 2).SetEase(Ease.Linear).OnComplete(() =>
            {
                callback?.Invoke();
                _refreshIcon.DOFade(.2f, .2f).OnComplete(() =>
                {
                    _refreshIcon.gameObject.SetActive(false);
                    StartCoroutine(ExploRefresh());
                });
            });
        });
    }
    IEnumerator ExploRefresh()
    {
        yield return new WaitForSeconds(.5f);
        _refreshExploFX.Play();
    }
    public void DoDoubleStarEffect()
    {
        _doubleStar.position = _doubleStarStartPos.position + new Vector3(-(Screen.width/2 + 100),0,0);

        _starParticle.gameObject.SetActive(true);
        _doubleStar.gameObject.SetActive(true);
        _doubleStarIcon.SetAlpha(1);
        _doubleStar.transform.localScale = Vector3.one;

        _doubleStarIcon.transform.rotation = Quaternion.identity;

        _doubleStar.DOMove(_doubleStarStartPos.position, _starMoveDuration).SetDelay(.4f).SetEase(_moveEase);
        _doubleStarIcon.transform.DORotate(new Vector3(0,0,-180), _starMoveDuration).SetDelay(.4f).OnComplete(()=>
        {
            _starParticle.gameObject.SetActive(false);
            _doubleStar.transform.DOMove(_starEndPos.position, _starMoveDuration).SetEase(Ease.OutQuad);
            _doubleStarIcon.DOFade(0.2f, _starMoveDuration);

            _doubleStar.transform.DOScale(.3f, _starMoveDuration).OnComplete(() =>
            {
                _doubleStar.gameObject.SetActive(false);
                _starSpakleFX.Play();
            });

            _x2StartIcon.DOScale(1.3f, .3f).SetDelay(_starMoveDuration - .2f).OnStart(() =>
            {
            }).SetEase(Ease.OutBack).OnStart(()=>
            {
                _x2StartIcon.gameObject.SetActive(true);
            }).OnComplete(() =>
            {
                _x2StartIcon.DOShakeScale(.5f, .3f).OnComplete(() =>
                {
                    _x2StartIcon.DOScale(1, .3f);
                });
            });
        });
    }
    public void DoTimeEffect()
    {
        //Reset value
        _timeObject.transform.position = _timeStartPos.position;
        _timeObject.gameObject.SetActive(true);
        _timeIcon.SetAlpha(1);
        _timeIcon.transform.localScale = Vector3.one;

        _clockWise.rotation = Quaternion.identity;
        //Rotate half cicle
        _timeSpakleFX.Play();
        _clockWise.DORotate(_clockWiseRotate, _timeRotateDuration/2).SetDelay(.4f).OnComplete(()=>
        {
            _timeSpakleFX.Stop();

            //Rotate half cicle
            _clockWise.DORotate(new Vector3(0,0,2), _timeRotateDuration/2).OnComplete(() =>
            {
                _timeObject.transform.DOMove(_timerCountIcon.position, _timeMoveDuration).SetEase(Ease.OutQuad);
                _timeIcon.DOFade(0.2f, _timeMoveDuration);
                _timeIcon.transform.DOScale(.3f, _timeMoveDuration).OnComplete(() =>
                {
                    _timeObject.gameObject.SetActive(false);
                    _timeParticleExplo.Play();
                });

                //Wait to time FX clock move to Timer icon
                _timerCountIcon.DOScale(1.2f, .5f).SetEase(Ease.OutBack).SetDelay(_timeMoveDuration - .1f).OnComplete(()=>
                {
                    _timerCountIcon.DOScale(1, .3f).SetDelay(_timeScaleDownDelay);
                });
            });
        });
    }

    public void DoBoomEffect()
    {
        _boomIcon.position = _boomIconStartPos.position;
        _boomIcon.gameObject.SetActive(true);

        _boomIcon.DOMove(_boomIconEndPos.position, _moveDuration).SetEase(_moveEase).SetDelay(.2f).OnComplete(() =>
        {
            MyGameFx.Instance.PlayBoomFX();
            _boomIcon.DOShakeRotation(_boomShakeDuration,new Vector3(0, 0, _boomShakeStrength),10,90, false, ShakeRandomnessMode.Harmonic).OnComplete(() =>
            {
                _boomIcon.DOScale(_boomIconScale, _scaleDownDuration).SetEase(_scaleDownEase).SetDelay(.3f).OnComplete(() =>
                {
                    _boomIcon.DOScale(Vector3.one, .1f).SetEase(Ease.OutQuint).OnComplete(() =>
                    {
                        _boomIcon.gameObject.SetActive(false);
                    });
                });

            });
        });
    }

    public void DoFrozenEffect()
    {
        _frozenEffectBackground.DOFade(.05f, _frozenEffectDuration).SetEase(_frozenEffectEase);

        _frozenEffectImg.DOFade(1f, _frozenEffectDuration).SetEase(_frozenEffectEase).OnComplete(() =>
        {
            _frozenEffectImg.DOFade(0, _frozenEffectDuration).SetEase(_frozenEffectEase);
            _frozenEffectBackground.DOFade(0, _frozenEffectDuration).SetEase(_frozenEffectEase);
        });

        _frozenIcon.transform.localScale= Vector3.zero;
        _frozenIcon.gameObject.SetActive(true);
        _frozenIcon.SetAlpha(1f);

        _frozenIcon.transform.DOScale(Vector3.one, _hitScaleUpDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            _frozenIcon.transform.DOShakeRotation(_boomShakeDuration, new Vector3(0, 0, _boomShakeStrength), 10, 90, false, ShakeRandomnessMode.Harmonic).OnComplete(() =>
            {
                _frostExploFX.Play();
                _frozenIcon.DOFade(.2f, _hitFadeDuration).OnComplete(() =>
                {
                    _frozenIcon.gameObject.SetActive(false);
                });
                _frozenIcon.transform.DOScale(4, _hitFadeDuration).SetEase(Ease.Linear);
            });
        });
    }

    public void DoMagicEffect(TweenCallback callback)
    {
        _magicIcon.position = _magicIconStartPos.position;
        _magicIcon.gameObject.SetActive(true);

        _magicIcon.DOMove(_magicIconEndPos.position, _moveDuration).SetEase(_moveEase).OnComplete(()=>
        {
            _magicIcon.DOScale(_magicIconScale, _scaleDownDuration).SetEase(_scaleDownEase).OnComplete(()=>
            {
                _magicIcon.DOScale(Vector3.one, .1f).SetEase(Ease.OutQuint).OnComplete(() =>
                {
                    _magicIcon.gameObject.SetActive(false);
                    MyGameFx.Instance.PlayMagicFX(callback);
                });
            });
        });
    }
}
