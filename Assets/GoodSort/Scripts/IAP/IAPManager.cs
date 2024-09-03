using Imba.UI;
using Imba.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

public class IAPManager : ManualSingletonMono<IAPManager>, IDetailedStoreListener/*, IAppsFlyerValidateReceipt*/
{
    private IStoreController _storeController;

    public Action OnPurchaseCompleted;
    public Action OnPurchaseFailedResult;

    public IStoreController StoreController => _storeController;

    public bool iapInited = false;

    public override void Awake()
    {
        base.Awake();
        IAPInitialize();
    }

    //private void Update()
    //{
    //    //for test
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        //UIManager.Instance.PopupManager.ShowPopup(UIPopupName.SampleShop);
    //        //ScreenCapture.CaptureScreenshot("HinhNoiBat");
    //    }
    //}

    private async void IAPInitialize()
    {
        InitializationOptions options = new InitializationOptions()
            //#if UNITY_EDITOR || DEVELOPMENT_BUILD
            //            .SetEnvironmentName("test");
            //#else
            .SetEnvironmentName("production");
        //#endif
        await UnityServices.InitializeAsync(options);
        ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
        operation.completed += HandleIAPCatalogLoader;
    }

    private void HandleIAPCatalogLoader(AsyncOperation Operation)
    {
        ResourceRequest request = Operation as ResourceRequest;

        Debug.Log($"Loaded Asset: {request.asset}");
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");

#if UNITY_ANDROID
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.GooglePlay));
#elif UNITY_IOS
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.AppleAppStore));
#else
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.NotSpecified));
             Debug.Log("Hello No Store");
#endif
        foreach (ProductCatalogItem item in catalog.allProducts)
        {
            var payoutDefinitions = new List<PayoutDefinition>();
            foreach (var payout in item.Payouts)
            {
                payoutDefinitions.Add(new PayoutDefinition(payout.typeString,
                                                            payout.subtype,
                                                            payout.quantity,
                                                            payout.data));
            }
            builder.AddProduct(item.id, item.type, null, payoutDefinitions);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _storeController = controller;

        // Check receipt
        CheckNonConsumableProducts();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"<color=red>Initialize failed</color>");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"Error initializing IAP because of {error}." +
            $"\r\nShow a message to the player depending on the error.{message}");

        iapInited = true;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Failed to purchase {product.definition.id} because {failureReason}");
        OnPurchaseFailedResult?.Invoke();
        OnPurchaseFailedResult = null;
        //_loadingOverlay.SetActive(false);
    }
    #region VALIDATE PURCHASE
    //do not delete old logic Khanh
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        bool validPurchase = true;

#if UNITY_EDITOR
        validPurchase = true;
#elif UNITY_ANDROID || UNITY_IOS || PLATFORM_ANDROID
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
        AppleTangle.Data(), Application.identifier);

        try
        {
            var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
        }
        catch (IAPSecurityException e)
        {
            Debug.LogError("Security Exception when purchase: " + e);
            validPurchase = false;
        }
#endif

        if (validPurchase)
        {
            Debug.Log($"Successfully purchased {purchaseEvent.purchasedProduct.definition.id}");
            Debug.Log("Transaction id of purchase: " + purchaseEvent.purchasedProduct.transactionID);
            OnPurchaseCompleted?.Invoke();
            OnPurchaseCompleted = null;
            //_loadingOverlay.SetActive(false);

            //send event to app flyer
            TrackPurchaseEvent(purchaseEvent);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void TrackPurchaseEvent(PurchaseEventArgs purchaseEvent)
    {
        //NOTE IMPORTANT: 
        //chua handle Verify purchase de loai bo purchase hack dan den sai data 
        //Dictionary<string, string> eventValues = new Dictionary<string, string>();
        //eventValues.Add(AFInAppEvents.CURRENCY, "USD");
        //eventValues.Add(AFInAppEvents.REVENUE, purchaseEvent.purchasedProduct.metadata.localizedPrice.ToString()); ;
        //eventValues.Add("af_purchase ", (purchaseEvent.purchasedProduct.metadata.localizedPrice * (decimal)0.8f).ToString());
        //AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, eventValues);
    }

    public void didFinishValidateReceipt(string result)
    {
        throw new NotImplementedException();
    }

    public void didFinishValidateReceiptWithError(string error)
    {
        throw new NotImplementedException();
    }

    #endregion

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Failed to purchase {product.definition.id} because {failureDescription}");
        OnPurchaseFailedResult?.Invoke();
        OnPurchaseFailedResult = null;
        //_loadingOverlay.SetActive(false);
    }

    #region Check receipt
    private void CheckNonConsumableProducts()
    {
        Debug.Log("Checking");
        List<Product> sortedProducts = _storeController.products.all
           .Where(it => it.hasReceipt)
           .ToList();

        foreach (Product product in sortedProducts)
        {
            CheckPrefKeys(product.definition.id);
        }
    }
    private void CheckPrefKeys(string key)
    {
        if(!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetString(key, "Purchased");
            PlayerPrefs.Save();
        }

        iapInited = true;
    }
    #endregion
}
