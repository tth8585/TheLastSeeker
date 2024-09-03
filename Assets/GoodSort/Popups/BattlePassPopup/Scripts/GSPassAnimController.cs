using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class GSPassAnimController : MonoBehaviour
{
    [SerializeField] Transform _closeBtn, _title, _gridView, _tutorialPanel, _purchasePanel;
    [SerializeField] Transform[] _passItems;
    [SerializeField] float _duration;

    public void DoAnim()
    {
        var closeBtnPos = _closeBtn.position;
        var titlePos = _title.position;
        var gridPos = _gridView.position;

        _closeBtn.position += Vector3.up * 500;
        _closeBtn.DOMove(closeBtnPos, _duration).SetEase(Ease.OutBack);

        _title.position += Vector3.up * 500;
        _title.DOMove(titlePos, _duration).SetEase(Ease.OutBack);

        _gridView.position += Vector3.down * 1000;
        _gridView.DOMove(gridPos, _duration).SetEase(Ease.OutBack);

        foreach (var item in _passItems)
        {
            item.localScale = Vector3.zero;
        }
        StartCoroutine(DoPassItemAnim());
    }

    IEnumerator DoPassItemAnim()
    {
        foreach (var item in _passItems)
        {
            item.DOScale(1, _duration).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(.1f);
        }
    }
    public void OnOpenPanel(Transform ui)
    {
        ui.GetChild(0).localScale = Vector3.zero;
        ui.SetActive(true);
        ui.GetChild(0).DOScale(1, _duration*0.5f).SetEase(Ease.OutBack);
    }
    public void OnCLosePanel(Transform ui)
    {
        ui.GetChild(0).DOScale(0, _duration*0.5f).SetEase(Ease.InBack).onComplete = ()=> { ui.SetActive(false); };
    }
}
