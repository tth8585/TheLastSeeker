using Imba.UI;
using SuperScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GSPassPopupController : UIPopup
{
    [SerializeField] LoopGridView mLoopGridViewGSPass;
    [SerializeField] CSVReader _csvReader;
    [SerializeField] Slider _passSlider;
    [SerializeField] TMP_Text _passLevelText, _passExpText;

    private List<RewardBattlePass> _rewards;

    public ToolTipController ToolTipController;

    private int _itemCount = 0;

    private void OnEnable()
    {
        _itemCount = 0;
        MyBattlePass.Instance.ToolTipController = ToolTipController;
        MyEvent.Instance.BattlePassEvents.onPurchaseProPack += UpdateUI;
        MyEvent.Instance.BattlePassEvents.onExpChanged += UpdateUI;
        MyEvent.Instance.BattlePassEvents.onLevelChanged += UpdateUI;
        MyEvent.Instance.BattlePassEvents.onClaimedReward += UpdateUIClaimed;
        mLoopGridViewGSPass.mOnBeginDragAction += DisableTooltip;
    }
    IEnumerator DoAnimUI()
    {
        yield return new WaitUntil(() => _itemCount >= 3);
        GetComponent<GSPassAnimController>().DoAnim();
    }
    private void OnDisable()
    {
        MyEvent.Instance.BattlePassEvents.onExpChanged -= UpdateUI;
        MyEvent.Instance.BattlePassEvents.onPurchaseProPack -= UpdateUI;
        MyEvent.Instance.BattlePassEvents.onLevelChanged -= UpdateUI;
        MyEvent.Instance.BattlePassEvents.onClaimedReward -= UpdateUIClaimed;
        mLoopGridViewGSPass.mOnBeginDragAction -= DisableTooltip;
    }
    private void DisableTooltip(PointerEventData pointerEventData)
    {
        ToolTipController.gameObject.SetActive(false);
    }

    private void UpdateUIClaimed(int arg1, PASS_ITEM_TYPE tYPE)
    {
        GetGSPassReward();
        UpdateUI();
    }

    private void UpdateUI()
    {
        InitSliderPassLevel();

        GetGSPassReward();

        InitQuestGridView();
    }

    protected override void OnShowing()
    {
        base.OnShowing();

        StartCoroutine(DoAnimUI());

        InitSliderPassLevel();

        GetGSPassReward();

        InitQuestGridView();

    }

    private void InitSliderPassLevel()
    {
        int level = MyBattlePass.Instance.GetCurrentLevel();
        if(level < 0)
        {
            level = 0;
        }
        int exp= MyBattlePass.Instance.GetCurrentExp();
        int maxExp = MyBattlePass.Instance.GetCurrentRequireToken();
        _passLevelText.text = level.ToString();
        _passExpText.text= exp + "/"+maxExp;
        if(maxExp == 0)
        {
            _passSlider.value = 1;
        }
        else
        {
            _passSlider.value = exp/(float)maxExp;
        }
    }

    public void OnClickClosePopup()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.GSPassPopup);
    }

    public void GetGSPassReward()
    {
        _rewards = _csvReader.GetBattlePassReward();

        Dictionary<int, BattlePassData> dicPass = MyBattlePass.Instance.GetPassDic();

        //update state reward (claimed or not)
        for(int i=0;i< _rewards.Count-1; i++)
        {
            if (dicPass.ContainsKey(i))
            {
                _rewards[i+1].isClaimedFree = dicPass[i].isClaimedFreeReward;
                _rewards[i+1].isClaimedPro = dicPass[i].isClaimedProReward;
            }
        }
    }

    #region LIST LOOP VIEW

    private void InitQuestGridView()
    {
        _itemCount = 0;
        if (_rewards != null || _rewards.Count > 0)
        {
            if (!mLoopGridViewGSPass.mListViewInited)
            {
                mLoopGridViewGSPass.InitGridView(_rewards.Count, OnGetItemReward);
            }
            else
            {
                RefreshFriendList();
            }
        }
    }

    protected LoopGridViewItem OnGetItemReward(LoopGridView gridView, int itemIndex, int row, int column)
    {
        _itemCount++;
        LoopGridViewItem item = null;
        if (_rewards == null) return null;
        if (_rewards.Count == 0)
        {
            Debug.LogError("quest list count =0");
        }
        else
        {
            if(itemIndex == 0)
            {
                item = gridView.NewListViewItem("HeadItem");

            }else if(itemIndex == _rewards.Count - 1)
            {
                item = gridView.NewListViewItem("BottomItem");
            }
            else
            {
                item = gridView.NewListViewItem("Pass item");
                BattlePassItemController questItem = item.GetComponent<BattlePassItemController>();
                questItem.InitData(_rewards[itemIndex]);    
            }
        }

        return item;
    }

    protected void RefreshFriendList()
    {
        if (_rewards == null)
        {
            mLoopGridViewGSPass.RecycleAllItem();
        }
        else
        {
            if (_rewards != null && mLoopGridViewGSPass != null)
            {
                try
                {
                    mLoopGridViewGSPass.SetListItemCount(_rewards.Count);
                }
                catch (Exception ex)
                {
                    Debug.LogError("some error unknown: " + ex);
                    return;
                }
            }

            if (_rewards.Count > 0)
            {
                //Debug.Log("xx RefreshFriendList");
                for (int i = 0; i < _rewards.Count; i++)
                {
                    if(_rewards[i].Level != -99 && _rewards[i].Level != 99)
                    {
                        if (mLoopGridViewGSPass.GetShownItemByItemIndex(i) != null)
                        {
                            BattlePassItemController questItem = mLoopGridViewGSPass.GetShownItemByItemIndex(i).GetComponent<BattlePassItemController>();
                            questItem.InitData(_rewards[i]);
                            _itemCount++;
                        }
                    }
                }
            }
        }
    }

    #endregion
}
