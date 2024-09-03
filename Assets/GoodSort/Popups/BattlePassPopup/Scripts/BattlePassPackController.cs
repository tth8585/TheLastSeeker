using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassPackController : MonoBehaviour
{
    [SerializeField] GameObject _purchaseBtn, _completeObj;
    [SerializeField] private BattlePassPurchaseController _purchaseController;
    [SerializeField] PASS_ITEM_TYPE _type = PASS_ITEM_TYPE.FREE;

    [Header("PassInfo")]
    [SerializeField] private BattlePassSO _battlePassInfo;


    private void OnEnable()
    {
        if (_type == PASS_ITEM_TYPE.GODEN)
        {
            if (MyBattlePass.Instance.IsUnlockProPass())
            {
                _purchaseBtn.SetActive(false);
                _completeObj.SetActive(true);
            }
            else
            {
                _purchaseBtn.SetActive(true);
                _completeObj.SetActive(false);
            }
        }
        else
        {
            _purchaseBtn.SetActive(false);
            _completeObj.SetActive(true);
        }

        if(_type== PASS_ITEM_TYPE.GODEN)
        {
            MyEvent.Instance.BattlePassEvents.onPurchaseProPack += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if (_type == PASS_ITEM_TYPE.GODEN)
        {
            MyEvent.Instance.BattlePassEvents.onPurchaseProPack -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        _purchaseBtn.SetActive(false);
        _completeObj.SetActive(true);
    }

    public void OnClickPurchasePack()
    {
        if (_type == PASS_ITEM_TYPE.FREE) return;

        _purchaseController.BattlePassInfo = _battlePassInfo;
        _purchaseController.gameObject.SetActive(true);
        _purchaseController.BattlePassType = _type;

        //AnalyticsManager.LogGSEvent(type);
    }
    public void OnClickClosePurchasePack()
    {
        _purchaseController.gameObject.SetActive(false);

        //AnalyticsManager.LogGSEvent(type);
    }
}
