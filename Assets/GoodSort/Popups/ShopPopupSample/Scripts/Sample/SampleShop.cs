using Imba.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public class SampleShop : UIPopup
{
    [SerializeField] SampleItem item;
    [SerializeField] SampleItem pack;

    private IStoreController _storeController;

    private bool _isUICreated = false;

    protected override void OnInit()
    {
        base.OnInit();

        _storeController = IAPManager.Instance.StoreController;
        HandleAllIconsLoaded();
    }

    protected override void OnShowing()
    {
        base.OnShowing();
        Debug.Log("Shop popup showing");
    }

    #region IAP Handle
    private void HandleAllIconsLoaded()
    {
        StartCoroutine(CreateGemUI());
    }

    private IEnumerator CreateGemUI()
    {
        if (_isUICreated) yield break;

        List<Product> sortedProducts = _storeController.products.all
            .OrderBy(it => it.metadata.localizedPrice)
            .ToList();

        foreach (Product product in sortedProducts)
        {
            if (product.definition.payout.subtype.Equals("Health"))
            {
                item.OnPurchase += HandlePurchase;
                item.InitData(product);
            }
            if (product.definition.payout.subtype.Equals("Pack"))
            {
                pack.OnPurchase += HandlePurchase;
                pack.InitData(product);
            }
        }

        _isUICreated = true;
    }

    private bool IsNonComsumbleProductHasBought(Product product)
    {
        if (PlayerPrefs.HasKey(product.definition.id) || product.hasReceipt) return true;
        return false;
    }

    private void HandlePurchase(Product product, Action OnPurchaseCompleted, Action OnPurchaseFailed)
    {
        IAPManager.Instance.OnPurchaseCompleted += OnPurchaseCompleted;
        IAPManager.Instance.OnPurchaseFailedResult += OnPurchaseFailed;

        _storeController.InitiatePurchase(product);
    }
    #endregion
}
