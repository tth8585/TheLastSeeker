using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public enum ItemSubtype
{
    None,
    BattlePass,
    Pack,
    Gem
}

public class IAPPopupController : MonoBehaviour/*, IDetailedStoreListener*/
{
    [Header("IAP Handle")]
    [SerializeField] protected ItemSubtype _selectedSubtype;
    //[SerializeField] private GameObject _loadingOverlay;
    protected List<Product> _sortedProducts;

    protected IStoreController _storeController;

    #region IAP Handle
    private void OnEnable()
    {
        //_storeController = MyIAP.Instance.StoreController;
        HandleAllIconsLoaded(_selectedSubtype.ToString());
    }

    private void HandleAllIconsLoaded(string subtype)
    {
        StartCoroutine(LoadItemUI(subtype));
    }

    internal virtual IEnumerator LoadItemUI(string subtype = null)
    {
        yield return null;
    }

    private bool IsNonComsumbleProductHasBought(string productID)
    {
        if (PlayerPrefs.HasKey(productID)) return true;
        return false;
    }

    internal void HandlePurchase(Product product, Action OnPurchaseCompleted, Action OnPurchaseFailed)
    {
        IAPManager.Instance.OnPurchaseCompleted += OnPurchaseCompleted;
        IAPManager.Instance.OnPurchaseFailedResult += OnPurchaseFailed;

        _storeController.InitiatePurchase(product);
    }
    #endregion
}
