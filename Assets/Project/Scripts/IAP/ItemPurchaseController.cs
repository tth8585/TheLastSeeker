using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ItemPurchaseController : MonoBehaviour
{
    internal string _idPack;

    [Header("IAP Control")]
    [SerializeField] internal Button _purchaseBtn;

    public delegate void PurchaseEvent(Product model, Action OnComplete, Action OnFail);
    public event PurchaseEvent OnPurchase;

    internal Product _model;

    public virtual void InitData(Product product)
    {
        // init visual data to shop in-app purchase item
    }

    public virtual void InitData(Product product, bool isNonPurchase) { }

    #region On Click
    public void OnClickWatchAdsForReward()
    {
        Debug.Log("Play Ads");
    }

    public virtual void OnClickPurchase()
    {
        _purchaseBtn.enabled = false;
        OnPurchase?.Invoke(_model, HandlePurchaseComplete, HandlePurchaseFailed);
    }
    #endregion

    internal virtual void HandlePurchaseComplete()
    {
        _purchaseBtn.enabled = true;

        if (_model.definition.type == ProductType.NonConsumable)
            this.gameObject.SetActive(false);

        PlayerPrefs.SetString(_model.definition.id, "Purchased");
    }

    internal virtual void HandlePurchaseFailed()
    {
        _purchaseBtn.enabled = true;

        Debug.Log("Purchase Failed");
    }

    public bool IsAssignPurchaseHandle()
    {
        return OnPurchase == null ? false : true;
    }

}
