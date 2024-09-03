using DG.Tweening;
using Imba.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPopup : UIPopup
{
    [SerializeField] private Image _bg;
    [SerializeField] private float _radiusChangeSpeed;
    [SerializeField] private float _maxShaderRadius;

    [SerializeField] private RectTransform _gameIconRT;
    private bool _iconShown = false;

    private Material _copyMat;

    protected override void OnShowing()
    {
        _gameIconRT.localScale = Vector3.one * 0.1f;
    }

    protected override void OnShown()
    {
        if(_copyMat == null)
            _copyMat = new Material(_bg.material);

        _bg.material = _copyMat;

        ShowLoadingPopupFX();
    }

    protected override void OnHidden()
    {
        _bg.materialForRendering.SetFloat("_Radius", 0.5f);
        _gameIconRT.SetActive(false);
    }

    private void ShowLoadingPopupFX()
    {
        StartCoroutine(ShowFX());
    }

    private IEnumerator ShowFX()
    {
        float changeValue = _bg.materialForRendering.GetFloat("_Radius");
        while (changeValue < _maxShaderRadius)
        {
            changeValue += Time.deltaTime * _radiusChangeSpeed;
            _bg.materialForRendering.SetFloat("_Radius", changeValue);

            if(changeValue >= 120 && !_iconShown)
            {
                _gameIconRT.SetActive(true);
                ShowGameIcon();
            }
            yield return null;
        }

        yield return null;
    }

    private void ShowGameIcon()
    {
        _gameIconRT.DOScale(Vector3.one * 1f, 1f).SetEase(Ease.OutBack).OnComplete(
            () => 
            {
                //LoadScene();
                Debug.Log("Do scale icon done");
                MyEvent.Instance.GameEventManager.OnLoadingAnimDone();
                _iconShown = false;
            });
        _iconShown = true;
    }
}
