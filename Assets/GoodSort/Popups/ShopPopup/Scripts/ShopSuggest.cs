using SuperScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShopSuggest : MonoBehaviour
{
    [SerializeField] private Scrollbar _scrollBar; // in develop
    [SerializeField] private ScrollRect _scrollRect; // in develop
    [SerializeField] private List<ItemShopPack> _items;
    [SerializeField] private List<Image> _dots; // in develop
    private List<Product> _suggestPacks;
    private List<float> _snapValue; // in develop

    // LoopGridView
    public LoopListView2 mLoopListView;
    private int _pageCount = 0;
    // Dots
    public Transform mDotsRootObj;
    List<DotElem> mDotElemList = new List<DotElem>();

    #region Init data for shop suggest
    void Start()
    {
        InitPack();

        InitDots();
        LoopListViewInitParam initParam = LoopListViewInitParam.CopyDefaultInitParam();
        initParam.mSnapVecThreshold = 99999;
        //mLoopListView.mOnBeginDragAction = OnBeginDrag;
        //mLoopListView.mOnDragingAction = OnDraging;
        mLoopListView.mOnEndDragAction = OnEndDrag;
        mLoopListView.mOnSnapNearestChanged = OnSnapNearestChanged;
        mLoopListView.InitListView(_pageCount, OnGetItemByIndex, initParam);

    }

    private void InitPack()
    {
        _suggestPacks = new List<Product>();
        _suggestPacks = GetCanSuggestPack();

        InstantiateSuggestPack(_suggestPacks);
    }    

    private List<Product> GetCanSuggestPack()
    {
        List<Product> result = new List<Product>();
        var store = IAPManager.Instance.StoreController;

        var allProducts = store.products.all;

        result.AddRange(allProducts.Where(p => p.definition.payout.subtype == "Pack")
            .ToList());
        
        return result;
    }

    private void InstantiateSuggestPack(List<Product> suggestPack)
    {
        //int startValue = 0;
        //float snapDistance =(float) 1 / (_items.Count - 1) ;
        //_snapValue = new List<float>();
        foreach(var item in _items)
        {
            var product = suggestPack.Where(p => p.definition.id == item.ItemSO.ItemID).FirstOrDefault();

            if (product != null)
            {
                //item.InitData(product);
                //item.OnPurchase += HandlePurchase;
                _pageCount++;
            }
            else
            {
                item.SetActive(false);
                //var index = _items.IndexOf(item);
                //if(index < _dots.Count) _dots.RemoveAt(index);

                //_items.Remove(item);
            }
            //_snapValue.Add(startValue + (_items.IndexOf(item) * snapDistance));
        }
    }

    private void HandlePurchase(Product product, Action OnPurchaseCompleted, Action OnPurchaseFailed)
    {
        IAPManager.Instance.OnPurchaseCompleted += OnPurchaseCompleted;
        IAPManager.Instance.OnPurchaseFailedResult += OnPurchaseFailed;

        IAPManager.Instance.StoreController.InitiatePurchase(product);
    }
    #endregion

    #region UI Control
    void InitDots()
    {
        int childCount = mDotsRootObj.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            Transform tf = mDotsRootObj.GetChild(i);
            DotElem elem = new DotElem();
            elem.mDotElemRoot = tf.gameObject;
            elem.mDotSmall = tf.Find("dotSmall").gameObject;
            elem.mDotBig = tf.Find("dotBig").gameObject;
            //ClickEventListener listener = ClickEventListener.Get(elem.mDotElemRoot);
            //int index = i;
            //listener.SetClickEventHandler(delegate (GameObject obj) { OnDotClicked(index); });
            mDotElemList.Add(elem);
        }
    }

    void OnSnapNearestChanged(LoopListView2 listView, LoopListViewItem2 item)
    {
        UpdateAllDots();
    }

    void UpdateAllDots()
    {
        int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
        if (curNearestItemIndex < 0 || curNearestItemIndex >= _pageCount)
        {
            return;
        }
        int count = mDotElemList.Count;
        if (curNearestItemIndex >= count)
        {
            return;
        }
        for (int i = 0; i < count; ++i)
        {
            DotElem elem = mDotElemList[i];
            if (i != curNearestItemIndex)
            {
                elem.mDotSmall.SetActive(true);
                elem.mDotBig.SetActive(false);
            }
            else
            {
                elem.mDotSmall.SetActive(false);
                elem.mDotBig.SetActive(true);
            }
        }
    }

    LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= _pageCount)
        {
            return null;
        }

        LoopListViewItem2 item = listView.NewListViewItem($"Pack ({pageIndex})");
        var itemControl = item.GetComponentInChildren<ItemShopPack>();
        var product = _suggestPacks.Where(p => p.definition.id == itemControl.ItemSO.ItemID).FirstOrDefault();
        if(product != null)
        {
            itemControl.InitData(product);
            //if(itemControl.OnPurchase != null)
           
            if(!itemControl.IsAssignPurchaseHandle())
            {
                Debug.Log("Assign On Purchase" + pageIndex);
                itemControl.OnPurchase += HandlePurchase;
            }
        }
        //ListItem14 itemScript = item.GetComponent<ListItem14>();
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            //itemScript.Init();
        }
        return item;
    }

    #endregion

    #region Drag control

    void OnEndDrag()
    {
        float vec = mLoopListView.ScrollRect.velocity.x;
        int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
        LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(curNearestItemIndex);
        if (item == null)
        {
            mLoopListView.ClearSnapData();
            return;
        }
        if (Mathf.Abs(vec) < 50f)
        {
            mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
            return;
        }
        Vector3 pos = mLoopListView.GetItemCornerPosInViewPort(item, ItemCornerEnum.LeftTop);
        if (pos.x > 0)
        {
            if (vec > 0)
            {
                mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex - 1);
            }
            else
            {
                mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
            }
        }
        else if (pos.x < 0)
        {
            if (vec > 0)
            {
                mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
            }
            else
            {
                mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex + 1);
            }
        }
        else
        {
            if (vec > 0)
            {
                mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex - 1);
            }
            else
            {
                mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex + 1);
            }
        }
    }
    #endregion
}
