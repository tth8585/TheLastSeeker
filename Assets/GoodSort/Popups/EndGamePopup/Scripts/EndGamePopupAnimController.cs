using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGamePopupAnimController : MonoBehaviour
{
    [SerializeField] ParticleSystem _exploFX;
    [SerializeField] Transform _banner;
    [SerializeField] Transform _bunnyicon;
    [SerializeField] Transform _star;

    [SerializeField] float _scaleDuration = .5f;
    [SerializeField] float _delayAnim = .2f;
    [SerializeField] Ease _scaleEase = Ease.OutBack;
    [SerializeField] Vector3 _bunnyMoveOffset = new Vector3(0, 120, 0);

    private void Awake()
    {
        HideUIAnim();
    }
    private void Update()
    {
        //test
        if (Input.GetKeyDown(KeyCode.K))
        {
            DoAnim();
        }
    }
    public void DoAnim()
    {
        _exploFX.Play();

        //Reset start value
        _banner.localScale = Vector3.zero;
        _bunnyicon.localScale = Vector3.zero;
        _bunnyicon.position -= _bunnyMoveOffset;
        _star.localScale = Vector3.zero;

        _banner.gameObject.SetActive(true);
        _bunnyicon.gameObject.SetActive(true);
        _star.gameObject.SetActive(true);

        StartCoroutine(DelayDoAnim());
    }
    IEnumerator DelayDoAnim()
    {
        _banner.DOScale(1f, _scaleDuration).SetEase(_scaleEase);

        yield return new WaitForSeconds(_delayAnim);
        _star.DOScale(1f, _scaleDuration).SetEase(_scaleEase).OnComplete(() =>{
            AnimateStar();
        });

        yield return new WaitForSeconds(_delayAnim);
        _bunnyicon.DOScale(1f, _scaleDuration).SetEase(_scaleEase).OnPlay(() =>
        {
            _bunnyicon.DOMove(_bunnyicon.position + _bunnyMoveOffset, _scaleDuration).SetEase(Ease.OutBack);
        });
    }

    public void HideUIAnim()
    {
        _banner.gameObject.SetActive(false);
        _bunnyicon.gameObject.SetActive(false);
        _star.gameObject.SetActive(false);
    }

    public void AnimateStar()
    {
        Sequence starSequence = DOTween.Sequence();

        // Add a scaling up animation
        starSequence.Append(_star.transform.DOScale(new Vector3(1.1f, 1.1f, 1.2f), 0.2f));

        // Add a scaling down animation
        starSequence.Append(_star.transform.DOScale(Vector3.one, 0.2f));

        // Optionally, you can loop or restart the sequence
        starSequence.SetLoops(-1);

        // Play the sequence
        starSequence.Play();
    }
}
