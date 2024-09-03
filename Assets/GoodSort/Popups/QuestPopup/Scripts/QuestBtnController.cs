using Imba.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class QuestBtnController : MonoBehaviour
{
    [SerializeField] GameObject _notiObj;
    [SerializeField] private TMP_Text _notiCountText;
    [SerializeField] private int _questDoneCount;

    private void Start()
    {
        _notiObj.SetActive(false);
        MyEvent.Instance.QuestEvents.onDoneQuest += OnDoneQuest;
        MyEvent.Instance.QuestEvents.onClaimedReward += OnClaimQuestRewardCount;
        AnimateButton();
    }
    private void OnDestroy()
    {
        MyEvent.Instance.QuestEvents.onDoneQuest -= OnDoneQuest;
        MyEvent.Instance.QuestEvents.onClaimedReward -= OnClaimQuestRewardCount;
    }

    private void OnClaimQuestRewardCount(string id)
    {
        _questDoneCount -= 1;
        if(_questDoneCount < 0)
        {
            _questDoneCount = 0;
        }
        UpdateQuestNoti();
    }

    private void UpdateQuestNoti()
    {
        if (_questDoneCount > 0)
        {
            _notiObj.SetActive(true);
            _notiCountText.text = _questDoneCount.ToString();
        }
        else
        {
            _notiObj.SetActive(false);
        }
    }

    private void OnDoneQuest(string obj)
    {
        _questDoneCount += 1;
        UpdateQuestNoti();
    }

    public void AnimateButton()
    {
        // Sequence to create a scale animation
        Sequence sequence = DOTween.Sequence();

        // Add a scaling up animation
        sequence.Append(_notiObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f));

        // Add a scaling down animation
        sequence.Append(_notiObj.transform.DOScale(Vector3.one, 0.2f));

        // Optionally, you can loop or restart the sequence
        sequence.SetLoops(-1);

        // Play the sequence
        sequence.Play();
    }
}
