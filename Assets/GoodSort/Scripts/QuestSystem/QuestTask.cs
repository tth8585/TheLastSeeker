using UnityEngine;

public abstract class QuestTask : MonoBehaviour
{
    [SerializeField] int[] _maxCountArr;

    protected int _maxCount=0;
    protected int _count;
    protected string _questId;

    private bool _isDone = false;
    private int _taskIndex;

    public void InitTask(string questId,int index, string questTaskState)
    {
        int.TryParse(questTaskState, out _count);
        _maxCount= MyQuest.Instance.GetMaxCountForTaskQuest(questId);
        this._questId = questId;
        _taskIndex = index;
        if(questTaskState != null&& questTaskState!=string.Empty)
        {
            SetQuestTaskState(questTaskState);
        }
    }

    protected void FinishTask()
    {
        if(!_isDone)
        {
            _isDone = true;
            //Do Stuff
            MyEvent.Instance.QuestEvents.AdvanceQuest(_questId);
            MyEvent.Instance.QuestEvents.DoneQuest(_questId);
            Destroy(gameObject);
        }
    }

    protected void ChangeState(string newState)
    {
        MyEvent.Instance.QuestEvents.QuestTaskStateChange(_questId,_taskIndex,new QuestTaskState(newState));
    }

    public string GetQuestId()
    {
        return this._questId;
    }

    public int GetCurrentCount()
    {
        return _count;
    }

    public int[] GetArrMaxCount()
    {
        return _maxCountArr;
    }

    protected abstract void SetQuestTaskState(string state);
}
