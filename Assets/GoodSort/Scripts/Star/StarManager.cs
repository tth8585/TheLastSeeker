using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class StarManager : MonoBehaviour
{
    [SerializeField] private UIEffectPool _startFXPool;
    [SerializeField] private Vector3 _start, _end;
    private TMP_Text _starCountTxt;

    [SerializeField] float _moveDuration = .3f;
    [SerializeField] float _scaleDuration = .2f;
    [SerializeField] Ease _moveEase = Ease.Linear;
    [SerializeField] Ease _scaleEase = Ease.OutQuad;
    [SerializeField] float _delaySpawnTime = 0.1f;

    int _startAnimCount = 0;

    public Action<int> ON_STAR_COUNT_CHANGE;

    private void OnEnable()
    {
        MyEvent.Instance.GameEventManager.onStarAdded += StarAdded;
    }

    private void OnDisable()
    {
        MyEvent.Instance.GameEventManager.onStarAdded -= StarAdded;
    }

    public void SetStarUI(TMP_Text starCountTxt)
    {
        _starCountTxt = starCountTxt;
    }

    private void StarAdded(int starCount) => _starCountTxt.text = starCount.ToString();

    public void ShowStarEffect(Transform parent, int starCount, Vector3 start, Vector3 end)
    {
        _startAnimCount = starCount;

        if (_startAnimCount == 0) return;

        StartCoroutine(DelaySpawnModel(parent, start, end));
    }
    IEnumerator DelaySpawnModel(Transform parent,Vector3 start,Vector3 endPos)
    {
        while(_startAnimCount > 0)
        {
            var star = _startFXPool.GetObject(parent).transform;
            star.localScale = Vector3.zero;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(start);
            screenPosition += new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
            star.GetComponent<RectTransform>().position = screenPosition;

            star.DOScale(1.2f, _scaleDuration).SetEase(_scaleEase).OnComplete(() =>
            {
                star.DOMove(endPos, _moveDuration).SetEase(_moveEase).SetDelay(.5f).OnComplete(() =>
                {
                    star.localScale = Vector3.zero;
                    _startFXPool.ReturnStarToPool(star.gameObject);
                });
                star.DOScale(.5f, _moveDuration).SetDelay(.5f);
            });
            _startAnimCount--;
            yield return new WaitForSeconds(_delaySpawnTime);

            StartCoroutine(DelaySpawnModel(parent, start, endPos));
        }
    }
}
