using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ItemAnimation : MonoBehaviour
{
    [SerializeField] Transform _model;
    [SerializeField] GameObject _meshObj;
    [SerializeField] ParticleSystem _vfx;

    private float _timeAnim = 0f;
    private float _timeWaitAnim = 0.2f;
    private float _downScaleDuration = 0.2f;
    private float _returnDuration = 0.1f;

    public void DoAnimAndDestroy(TweenCallback callback)
    {
        if (_timeAnim == 0f) _timeAnim = MySpawn.Instance.GetTimeAnim();

        //_model.DOMoveZ(-0.8f, _timeAnim / 5f).OnComplete(() => 
        //{
        //    AnimShrinkObject(callback);
        //});
        AnimShrinkObject(callback);
    }

    public void AnimShrinkOnly()
    {
        Vector3 originalScale = _model.localScale;

        Vector3 downScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 0.8f, originalScale.z * 0.8f);

        _model.DOScale(downScale, _downScaleDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            _model.DOScale(originalScale, _returnDuration).SetEase(Ease.OutQuint).OnComplete(() =>
            {

            });
        });
    }

    private void AnimShrinkObject(TweenCallback callback)
    {
        Vector3 originalScale = _model.localScale;

        Vector3 downScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 0.8f, originalScale.z*0.8f);

        _model.DOScale(downScale, _downScaleDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            _model.DOScale(originalScale, _returnDuration).SetEase(Ease.OutQuint).OnComplete(() =>
            {
                OffMeshModel();
                StartCoroutine(COWaitAnim(_timeWaitAnim, callback));
                StartCoroutine(COWaitAndDoVfx(0.05f));
                VibratePhone();
            });
        });
    }

    private void OffMeshModel()
    {
        _meshObj.SetActive(false);
    }

    IEnumerator COWaitAnim(float timeWait, TweenCallback callback)
    {
        yield return new WaitForSeconds(timeWait);
        _meshObj.SetActive(true);
        callback?.Invoke();
    }

    IEnumerator COWaitAndDoVfx(float timeWait)
    {
        yield return new WaitForSeconds(timeWait);
        _vfx.Play();
    }

    private void VibratePhone()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }
}
