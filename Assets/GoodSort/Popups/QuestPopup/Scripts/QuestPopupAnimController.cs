using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Imba.UI;

public class QuestPopupAnimController : MonoBehaviour
{
    [SerializeField] Transform questCountItem;
    [SerializeField] float distance;
    [SerializeField] float duration;
    [SerializeField] float questItemAnimDuration;
    [SerializeField] float questItemAnimDeltaTime;
    [SerializeField] Transform questCountItemParent;

    List<Transform> QuestItems = new();
    List<Vector3> questItemPositions = new();

    public void DoAnim()
    {
        foreach (var item in QuestItems)
        {
            item.transform.localScale = Vector3.zero;
            questItemPositions.Add(item.transform.position);
        }
        StartCoroutine(DoQuestItemAnim());

        //questCountItem.transform.SetParent(UIManager.Instance.CanvasRect);
        //questCountItem.transform.SetAsLastSibling();
        //questCountItem.transform.position += Vector3.up * distance;
        //questCountItem.transform.DOMove(questCountItem.transform.position + Vector3.down * distance, duration).SetEase(Ease.OutBack);
            //OnComplete(() => { questCountItem.transform.SetParent(questCountItemParent); questCountItem.transform.SetAsLastSibling(); });

        transform.position += Vector3.down * distance;
        transform.DOMove(transform.position + Vector3.up * distance, duration).SetEase(Ease.OutBack);
    }

    IEnumerator DoQuestItemAnim()
    {
        int index = 0;
        while (index < QuestItems.Count)
        {
            var offsetY = (QuestItems.Count - index) * distance;
            QuestItems[index].transform.position += Vector3.down * offsetY;
            QuestItems[index].transform.DOMove(questItemPositions[index], questItemAnimDuration * (1 - (float)index / QuestItems.Count));
            QuestItems[index].transform.DOScale(Vector3.one, questItemAnimDuration).SetEase(Ease.OutBack);
            index++; 
            yield return new WaitForSeconds(questItemAnimDeltaTime);
        }
    }

    public void AddQuestItem(Transform questItem) => QuestItems.Add(questItem);
}
