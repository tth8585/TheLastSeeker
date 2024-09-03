using Imba.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShopPopup : UIPopup
{
    [SerializeField] private TMP_Text _coinText;

    [Header("Item")]
    [SerializeField] private List<ItemShop> _shopItems;

    [Header("Special Offer")]
    [SerializeField] private TMP_Text _time;
    private const string SPECIAL_OFFER_TIME_KEY = "DayStartSpecialOffer";
    private int _maxSpecialTime;
    private int _minutes;

    // FOR PACK
    public static Action ON_OFFER_TIME_CHANGE;
    private static int _sMinutes;

    // IAP STORE
    private IStoreController _storeController;

    private bool _isUICreated = false;

    // FOR NOT ENOUGH COIN
    [SerializeField] private Button _closeShopBtn;
    [SerializeField] private Image _notiNotEnoughCoin;
    private bool _isNotEnoughCoin = false;

    private void OnEnable()
    {
        StartCoroutine(AddEventListener());
    }

    private void OnDisable()
    {
        MyEvent.Instance.UserDataManagerEvent.onLoadedUserData -= UpdateUI;
    }
    IEnumerator AddEventListener()
    {
        yield return new WaitUntil(() => MyEvent.Instance != null);
        MyEvent.Instance.UserDataManagerEvent.onLoadedUserData += UpdateUI;
    }
    private void UpdateUI()
    {
        _coinText.text = FormatText.FormatTextCount(MyUserData.Instance.UserDataSave.Coin);
    }
    protected override void OnInit()
    {
        //OrderInParent = 0;
        base.OnInit();

        _storeController = IAPManager.Instance.StoreController;

        if(_storeController != null)
            HandleAllIconsLoaded();

        _maxSpecialTime = 7 * 24 * 60;// 7day 24hour 60minute
    }

    protected override void OnShown()
    {
        //OrderInParent = 0;
        base.OnShown();
        Debug.Log("Shop view showing");
        if(Parameter != null)
        {
            _isNotEnoughCoin = (bool)Parameter;
        }
        OnNotEnoughCoinShow(_isNotEnoughCoin);

        InitSpecialOfferTime();
        UpdateUI();
    }


    protected override void OnHidden()
    {
        base.OnHidden();
        StopCoroutine(CountDownTime());

        if(_isNotEnoughCoin) _isNotEnoughCoin = false;
    }

    private void OnNotEnoughCoinShow(bool isShow)
    {
        _closeShopBtn.gameObject.SetActive(isShow);
        _notiNotEnoughCoin.gameObject.SetActive(isShow);
        if(isShow)
        {
            StartCoroutine(HideNotiNotEnoughCoin());
        }
    }

    private IEnumerator HideNotiNotEnoughCoin()
    {
        yield return new WaitForSeconds(2f);
        _notiNotEnoughCoin.gameObject.SetActive(false);
    }

    #region IAP Handle
    private void HandleAllIconsLoaded()
    {
        StartCoroutine(LoadItem());
    }

    private IEnumerator LoadItem()
    {
        if (_isUICreated) yield break;

        List<Product> sortedProducts = _storeController.products.all
            .OrderBy(it => it.metadata.localizedPrice)
            .ToList();

        foreach (var item in _shopItems)
        {
            if (item.ItemSO == null) continue;
            var product = sortedProducts.Where(p => p.definition.id == item.ItemSO.ItemID).FirstOrDefault();
            if (product == null) continue;
            if (IsNonComsumbleProductHasBought(product))
            {
                item.gameObject.SetActive(false);
                continue;
            }

            item.InitData(product);
            item.OnPurchase += HandlePurchase;
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

    #region Get Datetime for special offer
    private int GetRemainingTime()
    {
        if(PlayerPrefs.HasKey(SPECIAL_OFFER_TIME_KEY))
        {
            string dateTimeString = PlayerPrefs.GetString(SPECIAL_OFFER_TIME_KEY);
            var startTime = DateTime.Parse(dateTimeString);
            var currentTime = GetWorldTime();
            var remainingTime = currentTime - startTime;
            return _maxSpecialTime - remainingTime.Minutes;
        }
        else
        {
            CreateStartTime();
            return _maxSpecialTime; 
        }
    }

    private void CreateStartTime()
    {
        var startTime = GetWorldTime();
        PlayerPrefs.SetString(SPECIAL_OFFER_TIME_KEY, startTime.ToString());
    }

    private DateTime GetWorldTime()
    {
        //try// too slow
        //{
        //    using (var response = WebRequest.Create("http://www.google.com").GetResponse())
        //    return DateTime.ParseExact(response.Headers["date"],
        //            "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
        //            CultureInfo.InvariantCulture.DateTimeFormat,
        //            DateTimeStyles.AssumeUniversal);
        //}
        //catch(WebException ex)
        //{
        //    Debug.LogError($"<color=red>---</color>" + ex.Message);
        //    return DateTime.Now;
        //}
        return DateTime.Now;
    }

    private void InitSpecialOfferTime()
    {
        _minutes = GetRemainingTime();

        if(_minutes <= 0)
        {
            CreateStartTime();
            _minutes = GetRemainingTime();
        }

        OnRemainingTimeChange();

        _time.text = ConvertMinuteToTime(_minutes);
        StartCoroutine(CountDownTime());
    }

    private IEnumerator CountDownTime()
    {
        while(_minutes > 0)
        {
            yield return new WaitForSeconds(60);
            _minutes -= 1;
            _time.text = ConvertMinuteToTime(_minutes);

            
            if(_minutes == 0)
            {
                yield return new WaitForSeconds(60 * 5);
                CreateStartTime();
                _minutes = GetRemainingTime();
            }

            OnRemainingTimeChange();
        }
    }

    private string ConvertMinuteToTime(int minute)
    {
        int hour = minute / 60;
        int remainingMinute = minute - (hour * 60);
        return $"{hour}h {remainingMinute}m";
    }

    // static method using for pack has special offer
    public static string GetRemainingSpecialOfferTime()
    {
        int hour = _sMinutes / 60;
        int remainingMinute = _sMinutes - (hour * 60);
        return $"{hour}h {remainingMinute}m";
    }

    public void SetStaticMinute() => _sMinutes = _minutes;

    private void OnRemainingTimeChange()
    {
        SetStaticMinute();
        ON_OFFER_TIME_CHANGE?.Invoke();
    }
    #endregion

    #region
    public void OnClickCloseShopPopup()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.ShopPopup);
    }
    #endregion
}
