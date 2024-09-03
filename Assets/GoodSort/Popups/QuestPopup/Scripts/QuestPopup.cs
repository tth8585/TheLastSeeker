using Imba.UI;
using SuperScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPopup : UIPopup
{
    [SerializeField] LoopGridView mLoopGridViewListQuest;
    [SerializeField] TimeOfferController _timeOfferController;
    [SerializeField] ItemQuestController _questCount;

    private List<Quest> _listQuest= new List<Quest>();
    private string _idCountQuest = "Q_00";
    int questItemInitCount;

    protected override void OnShowing()
    {
        base.OnShowing();

        _timeOfferController.InitUI();

        GetListQuest();
        InitQuestGridView();

        Quest questCount = MyQuest.Instance.GetDailyQuestById(_idCountQuest);
        _questCount.InitData(questCount);
        StartCoroutine(DoPopupAnim());

        // Analytics
        //AnalyticsManager.LogHotelEvent(HotelAnalyticsEventName.open_daily_quest);
    }

    IEnumerator DoPopupAnim()
    {
        yield return new WaitUntil(FinishInitQuestItem);
        GetComponent<QuestPopupAnimController>().DoAnim();
    }
    bool FinishInitQuestItem() => questItemInitCount == _listQuest.Count;

    private void GetListQuest()
    {
        _listQuest.Clear();
        Dictionary<string, Quest> _dic_Quest = MyQuest.Instance.GetAllDailyQuest();

        //need to remove the count quest from data SO
        foreach (var item in _dic_Quest)
        {
            if(item.Key!= _idCountQuest)
            {
                _listQuest.Add(item.Value);
            }
        }
    }

    public void OnClickBackBtn()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.QuestPopup);
    }

    #region LIST LOOP VIEW

    private void InitQuestGridView()
    {
        if (_listQuest != null || _listQuest.Count > 0)
        {
            if (!mLoopGridViewListQuest.mListViewInited)
            {
                mLoopGridViewListQuest.InitGridView(_listQuest.Count, OnGetItemQuest);
            }
            else
            {
                RefreshQuestList();
            }
        }
    }

    protected LoopGridViewItem OnGetItemQuest(LoopGridView gridView, int itemIndex, int row, int column)
    {
        LoopGridViewItem item = null;
        if (_listQuest == null) return null;
        if (_listQuest.Count == 0)
        {
            Debug.LogError("quest list count =0");
        }
        else
        {
            item = gridView.NewListViewItem("ItemQuest");
            ItemQuestController questItem = item.GetComponent<ItemQuestController>();
            questItem.InitData(_listQuest[itemIndex]);
            GetComponent<QuestPopupAnimController>().AddQuestItem(item.transform);
            questItemInitCount++;
        }

        return item;
    }

    protected void RefreshQuestList()
    {
        if (_listQuest == null)
        {
            mLoopGridViewListQuest.RecycleAllItem();
        }
        else
        {
            if (_listQuest != null && mLoopGridViewListQuest != null)
            {
                try
                {
                    mLoopGridViewListQuest.SetListItemCount(_listQuest.Count);
                }
                catch (Exception ex)
                {
                    Debug.LogError("some error unknown: " + ex);
                    return;
                }
            }

            if (_listQuest.Count > 0)
            {
                for (int i = 0; i < _listQuest.Count; i++)
                {
                    if (mLoopGridViewListQuest.GetShownItemByItemIndex(i) != null)
                    {
                        ItemQuestController questItem = mLoopGridViewListQuest.GetShownItemByItemIndex(i).GetComponent<ItemQuestController>();
                        questItem.InitData(_listQuest[i]);
                    }
                }
            }
        }
    }

    #endregion
}
