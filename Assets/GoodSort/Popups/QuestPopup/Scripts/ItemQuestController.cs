using DG.Tweening;
using Imba.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemQuestController : MonoBehaviour
{
    [SerializeField] TMP_Text _rewardValueText,_questNameText;
    [SerializeField] Color _activeColor, _deactiveColor;
    [SerializeField] Image _rewardIcon;
    [SerializeField] ButtonPopupController _claimBtn;
    [SerializeField] SliderQuestController _slider;
    [SerializeField] GameObject _completeObj;
    [SerializeField] Image rewardAnim;

    private Quest _questData;

    bool isInit;

    int maxQuestProgress;

    public void InitData(Quest quest)
    {
        if (!isInit)
        {
            isInit = true;
            _claimBtn.GetComponent<Button>()?.onClick.AddListener(OnClickClaimButton);
            rewardAnim.transform.SetParent(UIManager.Instance.CanvasRect);
        }
        _questData = quest;
        _questNameText.text = _questData.Info.QuestName;

        InitDataSlider();

        _claimBtn.UpdateUI(CheckCompleteQuest());
        _rewardValueText.text = GetRewardText();
        _rewardIcon.sprite = GetRewardSprite();

        _completeObj.SetActive(_questData.ClaimedReward);
        _claimBtn.gameObject.SetActive(!_questData.ClaimedReward);

        if(_questData.ClaimedReward)
        {
            _questNameText.color = _deactiveColor;
        }
        else
        {
            _questNameText.color = _activeColor;
        }
        
    }

    private void InitDataSlider()
    {
        maxQuestProgress = GetMaxQuestProgress();

        if (_questData.ClaimedReward)
        {
            _slider.UpdateUIClaimed(maxQuestProgress);
        }
        else
        {
            GameObject taskObj = _questData.GetCurrentTaskPrefab();
            if (taskObj == null) _slider.UpdateUI(maxQuestProgress, _questData.ClaimedReward);
            else
            {
                QuestTask task = taskObj.GetComponent<QuestTask>();
                //maxQuestProgress = task.GetMaxRequire();
                _slider.InitUI(int.Parse(_questData.GetCurrentQuestTaskState()), maxQuestProgress);
            }
        }
    }

    private int GetMaxQuestProgress()
    {
        return MyQuest.Instance.GetMaxCountForTaskQuest(_questData.Info.Id);
    }

    private Sprite GetRewardSprite()
    {
        return _questData.Info.ItemReward.Sprite;
    }

    private bool CheckCompleteQuest()
    {
        if(_questData.State== QuestState.DONE)
        {
            return true;
        }

        return false;
    }

    private string GetRewardText()
    {
        //if (_questData.Info.GoldAmount > 0)
        //{
        //    return _questData.Info.GoldAmount.ToString();
        //}
        //else if (_questData.Info.GemAmount > 0)
        //{
        //    return _questData.Info.GemAmount.ToString();
        //}
        //else if (_questData.Info.TokenAmount > 0)
        //{
        //    return MyQuest.Instance.CalculateFinalReward(_questData.Info.Id).ToString(); // _questData.Info.TokenAmount.ToString();
        //}

        return "1";
    }

    void OnClickClaimButton()
    {
        if (!CheckCompleteQuest()) return;
        _questData.ClaimedReward = true;
        MyEvent.Instance.QuestEvents.ClaimedReward(_questData.Info.Id);
        MyEvent.Instance.UserDataManagerEvent.ClaimItem(_questData.Info.ItemReward.ItemType, 1);

        _claimBtn.SetActive(false);
        _completeObj.SetActive(true);
        _slider.UpdateUI(maxQuestProgress, _questData.ClaimedReward);

        rewardAnim.sprite = _rewardIcon.sprite;
        rewardAnim.transform.position = _rewardIcon.transform.position;
        rewardAnim.transform.SetAsLastSibling();
        rewardAnim.enabled = true;
        //rewardAnim.transform.DOMove(rewardAnimEndPos, MyMainView.Instance.GetAnimDuration()).SetEase(Ease.InBack).OnComplete(() => OnAnimComplete());

        // Analytic
        //AnalyticsManager.LogHotelEvent(HotelAnalyticsEventName.daily_quest_claim);

        // check if all quests done -> using DailyQuestStreakClaimAnalytic();
        //if(_questData.State == QuestState.DONE)
        //    AnalyticsManager.LogHotelEvent(HotelAnalyticsEventName.daily_quest_complete);
    }
    void OnAnimComplete() 
    {
        //rewardAnim.GetComponentInChildren<ParticleSystem>().Play();
        rewardAnim.enabled = false;
        //MyEvent.Instance.ClaimQuestReward(rewardType, MyQuest.Instance.CalculateFinalReward(_questData.Info.Id));
    }

}

public enum QuestRewardType
{
    Gold,
    Gem,
    Token,
}
