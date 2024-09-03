using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Utilities;

public class BattlePassPurchaseController : IAPPopupController
{
    private const int MAX_REWARDS_CAN_REWARD = 5;
    private BattlePassSO _battlePassInfo;

    [Header("UI")]
    //[SerializeField] private TMP_Text _GSPassTitle;
    [SerializeField] private Image _battlePassBGImage;
    [SerializeField] private TMP_Text _battlePassPrice;

    [SerializeField] private GameObject _rewardItemPrefab;
    [SerializeField] private Transform _rewardPanel;

    private PASS_ITEM_TYPE _battlePassType;

    private bool _isUICreated = false;

    private Product _product;

    public PASS_ITEM_TYPE BattlePassType { set => _battlePassType = value; }
    public BattlePassSO BattlePassInfo { set => _battlePassInfo = value; }

    public void InitUI(Product product)
    {
        //_GSPassTitle.text = product.metadata.localizedTitle;
        _battlePassPrice.SetText($"{product.metadata.localizedPriceString} " +
            $"{product.metadata.isoCurrencyCode}");

        _battlePassBGImage.sprite = _battlePassInfo.GSPassBG;

        InitBuyGSPassReward();
    }

    private void InitBuyGSPassReward()
    {
        Type scriptableObjectType = typeof(BattlePassSO);

        FieldInfo[] fieldInfos = scriptableObjectType.GetFields();

        for (int i = 0; i < MAX_REWARDS_CAN_REWARD; i++)
        {
            Sprite itemSprite = fieldInfos[MAX_REWARDS_CAN_REWARD + i].GetValue(_battlePassInfo) as Sprite;

            BattlePassRewardItem rewardItem = Instantiate(_rewardItemPrefab, _rewardPanel).GetComponent<BattlePassRewardItem>();

            if (fieldInfos[i].FieldType == typeof(int))
            {
                var value = MathUtil.NiceCash((int)fieldInfos[i].GetValue(_battlePassInfo));
                rewardItem.InitRewardItem(itemSprite, value.ToString());
            }
            else
            {
                rewardItem.InitRewardItem(itemSprite, "none", (bool)fieldInfos[i].GetValue(_battlePassInfo));
            }
        }
    }

    internal override IEnumerator LoadItemUI(string subType)
    {
        if (subType != "BattlePass" || _isUICreated) yield break;

        _sortedProducts = _storeController.products.all
            .OrderBy(it => it.metadata.localizedPrice)
            .ToList();

        var id = GetProductIDByType();
        var product = _sortedProducts.Where(p => p.definition.id == id).FirstOrDefault();

        if (product != null)
        {
            _product = product;
            InitUI(product);
            Debug.Log($"Init {id}");

            _isUICreated = true;
        }


        yield return null;
    }

    private string GetProductIDByType()
    {
        return "pro_battle_pass";
    }

    #region On Click
    public void OnClickPurchaseBattlePass()
    {
        HandlePurchase(_product, OnPurchaseCompleted, OnPurchaseFailed);
    }

    private void OnPurchaseCompleted()
    {
        this.gameObject.SetActive(false);

        MyBattlePass.Instance.PurchasePack(_battlePassType);

        //ItemPurchaseController.RemoveADS(true);

        // For analytics
        //AnalyticsManager.LogGSEvent(type);

        //int level = MyMainView.Instance.Model.LoadLvl();
        //AnalyticsManager.EventFirstShopping(MyMainView.Instance.Model.GSName, level);
    }

    private void OnPurchaseFailed()
    {
        Debug.Log("Purchase pass failed");
    }

    public void OnClickHidePurchaseBattlePass() => this.gameObject.SetActive(false);
    #endregion
}
