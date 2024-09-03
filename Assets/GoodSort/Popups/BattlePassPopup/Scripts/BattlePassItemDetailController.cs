using DG.Tweening;
using Imba.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public enum PASS_ITEM_TYPE
{
    FREE,
    GODEN,
}

public class BattlePassItemDetailController : MonoBehaviour
{
    [SerializeField] PASS_ITEM_TYPE _type = PASS_ITEM_TYPE.FREE;
    [SerializeField] TMP_Text _valueText,_levelText;
    [SerializeField] Image _icon,_bg;
    [SerializeField] GameObject _claimBtn;
    [SerializeField] GameObject _lockIcon;
    [SerializeField] GameObject _completeObj;
    [SerializeField] Slider _progressSlider;
    [SerializeField] Image _levelBg;
    [SerializeField] Sprite _activeSprite, _inactiveSprite, _defaultIconSprite;
    [SerializeField] Color _activeLevelColor, _inactiveLevelColor;
    [SerializeField] Sprite _freeBg, _proBg;
    [SerializeField] CanvasGroup rewardAnim;
    [SerializeField] ToggleTooltip _toggleTooltip;

    private BattlePassDataDetail _data;
    [SerializeField] float animDuration;
    bool isInit;

    public void InitData(BattlePassDataDetail data, bool isFillSlider)
    {
        if (!isInit)
        {
            isInit = true;
            rewardAnim.transform.SetParent(UIManager.Instance.CanvasRect);
        }
        _data = data;
        ResetUI();
        UpdateUI();

        UpdateState(isFillSlider);
        UpdateSlider();
        CheckIfClaimedReward();
        _toggleTooltip.SetData(data);
    }

    private void CheckIfClaimedReward()
    {
        if (_data.IsClaimed)
        {
            _lockIcon.SetActive(false);
            _completeObj.SetActive(true);
            _icon.SetActive(false);
            _claimBtn.SetActive(false);

            //if(_type == GSPASS_ITEM_TYPE.FREE)
            //    AnalyticsManager.EventLevelGSPass(_data.Level);
        }
    }

    private void UpdateState(bool isReachLevel)
    {
        if (isReachLevel)
        {
            bool canClaimOnPurchasePack = CheckCanClaimPurchasePack();

            if (!canClaimOnPurchasePack)
            {
                //lock when not reach 
                _lockIcon.SetActive(true);
                _icon.SetActive(true);
                _completeObj.SetActive(false);
                _claimBtn.SetActive(false);
            }
            else
            {
                bool canClaimOnLevelReach = CheckCanClaimOnExpReach();
                if (canClaimOnLevelReach)
                {
                    //show btn claim
                    _lockIcon.SetActive(false);
                    _icon.SetActive(true);

                    _completeObj.SetActive(false);
                    _claimBtn.SetActive(true);
                }
                else
                {
                    //lock when not reach 
                    _lockIcon.SetActive(true);
                    _completeObj.SetActive(false);
                    _claimBtn.SetActive(false);
                    _icon.SetActive(true);

                }
            }

        }
        else
        {
            //lock when not reach 
            _lockIcon.SetActive(true);
            _completeObj.SetActive(false);
            _icon.SetActive(true);

            _claimBtn.SetActive(false);
        }

    }

    private bool CheckCanClaimPurchasePack()
    {
        //check player purchase right pass or not
        if (_type == PASS_ITEM_TYPE.GODEN)
        {
            if (!MyBattlePass.Instance.IsUnlockProPass()) return false;
        }

        return true;
    }

    private bool CheckCanClaimOnExpReach()
    {
        if(MyBattlePass.Instance.CanLevelUp(_data.Level)) return true;

        return false;
    }

    private void UpdateSlider()
    {
        if (_data.IsClaimed)
        {
            _progressSlider.value = 1;
            _levelBg.sprite = _activeSprite;
            _levelText.color = _activeLevelColor;
        }
        else
        {
            _progressSlider.value = 0;
            _levelBg.sprite = _inactiveSprite;
            _levelText.color = _inactiveLevelColor;
        }
    }

    private void ResetUI()
    {
        //slider
        _progressSlider.value = 0;
        //bg
        if(_type== PASS_ITEM_TYPE.FREE)
        {
            _bg.sprite = _freeBg;
        }
        else if(_type== PASS_ITEM_TYPE.GODEN)
        {
            _bg.sprite = _proBg;
        }

        _claimBtn.SetActive(false);
        _levelBg.sprite = _inactiveSprite;
        _levelText.color= _inactiveLevelColor;
        _completeObj.SetActive(false);
        _icon.SetActive(true);

    }

    private void UpdateUI()
    {
        _valueText.text = _data.Amount.ToString();
        _levelText.text= _data.Level.ToString();
        _icon.sprite = MyItemAbility.Instance.GetItemInfoByName(_data.RewardType).Sprite;
    }

    public void OnClaimButtonClick()
    {
        //anim
        _data.IsClaimed = true;
        rewardAnim.GetComponent<Image>().sprite = _icon.sprite;
        rewardAnim.transform.position = _icon.transform.position;
        rewardAnim.transform.SetAsLastSibling();
        rewardAnim.alpha = 1;
        rewardAnim.transform.DOMove(rewardAnim.transform.position + Vector3.up * 500, animDuration);
        rewardAnim.DOFade(0, animDuration);
        //show complete obj
        _lockIcon.SetActive(false);
        _completeObj.SetActive(true);
        _claimBtn.SetActive(false);
        _icon.SetActive(false);
        MyEvent.Instance.BattlePassEvents.ClaimedReward(_data.Level, _type);
        MyEvent.Instance.UserDataManagerEvent.ClaimItem(MyItemAbility.Instance.GetItemInfoByName( _data.RewardType).ItemType, _data.Amount);

        UpdateSlider();
        //// analytics
        //AnalyticsManager.EventLevelGSPass(_data.Level);
    }
}
