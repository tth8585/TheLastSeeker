using System;

public class QuestEvents
{
    public event Action<int> onGetCombo;
    public void GetComboQuest(int count) => onGetCombo?.Invoke(count);

    public event Action onCompleteLevel;
    public void CompleteLevelQuest() => onCompleteLevel?.Invoke();

    public event Action onStartALevel;
    public void StartALevelQuest() => onStartALevel?.Invoke();

    public event Action<ITEM_TYPE> onUseItem;
    public void UseItemToQuest(ITEM_TYPE type) => onUseItem?.Invoke(type);

    public event Action onCompleteSpecialGuestRequest;
    public void CompleteSpecialGuestRequest() => onCompleteSpecialGuestRequest?.Invoke();

    public event Action onClaimedRewardNoti;
    public void ClaimedRewardNoti()
    {
        if (onClaimedRewardNoti != null) onClaimedRewardNoti();
    }

    public event Action<string> onClaimedReward;
    public void ClaimedReward(string idClaimed)
    {
        if (onClaimedReward != null) onClaimedReward(idClaimed);
    }

    public event Action<string> onStartQuest;
    public void StartQuest(string id)
    {
        if (onStartQuest != null) onStartQuest(id);
    }

    public event Action<string> onAdvanceQuest;
    public void AdvanceQuest(string id)
    {
        if (onAdvanceQuest != null) onAdvanceQuest(id);
    }

    public event Action<string> onDoneQuest;
    public void DoneQuest(string id) => onDoneQuest?.Invoke(id);

    public event Action<bool> questNoti;
    public void QuestNoti(bool isHaveReward) => questNoti?.Invoke(isHaveReward);

    public event Action<string> onDoneDailyQuest;
    public void DoneDailyQuest(string id) => onDoneDailyQuest?.Invoke(id);

    public event Action<Quest> onQuestStateChange;
    public void QuestStateChange(Quest quest)
    {
        if (onQuestStateChange != null) onQuestStateChange(quest);
    }

    public event Action<string,int,QuestTaskState> onQuestTaskStateChange;
    public void QuestTaskStateChange(string id, int step, QuestTaskState state)
    {
        if (onQuestTaskStateChange != null) onQuestTaskStateChange(id,step,state);
    }
}
