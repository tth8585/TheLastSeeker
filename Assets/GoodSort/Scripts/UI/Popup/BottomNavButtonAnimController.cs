using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Imba.UI;

public class BottomNavButtonAnimController : MonoBehaviour
{
    [SerializeField] List<BottomNavBtnInfo> _listItems;
    [SerializeField] Vector3 _scaleSize = new Vector3(1.2f,1.2f,1.2f);
    [SerializeField] int _iconUpOffset = 5;
    [SerializeField] float _duration = 0.4f;
    [SerializeField] Ease _iconEase = Ease.OutBack;
    [SerializeField] Ease _itemEase = Ease.OutBack;

    private void OnEnable()
    {
        MyEvent.Instance.MainMenuEventManager.onChangeBottomTab += MainMenuEventManager_onChangeBottomTab;
    }

    private void OnDisable()
    {
        MyEvent.Instance.MainMenuEventManager.onChangeBottomTab -= MainMenuEventManager_onChangeBottomTab;
    }

    private void MainMenuEventManager_onChangeBottomTab(int indexTab)
    {
        OnClickItem(_listItems[indexTab]);
    }

    private void Start()
    {
        foreach(var item in _listItems)
        {
            if (item.Selected.gameObject.activeSelf)
            {
                item.Selected.transform.DOScale(_scaleSize, _duration).SetEase(Ease.Linear);
                item.Icon.DOAnchorPos(new Vector2(1, _iconUpOffset), _duration).SetEase(Ease.OutBack);
                item.Icon.transform.DOScale(_scaleSize, _duration).SetEase(_iconEase);
                item.transform.SetAsLastSibling();
            }
        }
    }
    public void OnClickItem(BottomNavBtnInfo itemNav)
    {
        //Do Anim btn
        for(int i = 0; i < _listItems.Count; i++)
        {
            if(_listItems[i].Name == itemNav.Name)
            {
                _listItems[i].Selected.transform.DOScale(_scaleSize, _duration).SetEase(_itemEase);
                _listItems[i].Icon.DOAnchorPos(new Vector2(1,_iconUpOffset), _duration).SetEase(_iconEase);
                _listItems[i].Icon.transform.DOScale(_scaleSize, _duration).SetEase(_iconEase);
                _listItems[i].transform.SetAsLastSibling();
                _listItems[i].SelectItem(true);
            }
            else
            {
                _listItems[i].Selected.transform.localScale = Vector3.one;
                _listItems[i].Icon.anchoredPosition = Vector2.zero;
                _listItems[i].Icon.transform.localScale = Vector3.one;
                _listItems[i].SelectItem(false);
            }
        }

        OnChangeTab(itemNav.UIPopupName);
    }
    public void OnChangeTab(UIPopupName uIPopupName)
    {
        //Hide all tabs
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.ShopPopup);
        //UIManager.Instance.PopupManager.HidePopup(UIPopupName.Arrchi);

        switch (uIPopupName)
        {
            case UIPopupName.Sample:
                break;
            case UIPopupName.MessageBox:
                break;
            case UIPopupName.MessageBoxUnclosePopup:
                break;
            case UIPopupName.SampleShop:
                break;
            case UIPopupName.GSPassPopup:
                break;
            case UIPopupName.ShopPopup:
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.ShopPopup);
                var shopPopup = UIManager.Instance.PopupManager.GetPopup(UIPopupName.ShopPopup);
                shopPopup.transform.SetAsFirstSibling();
                break;
            case UIPopupName.DailyRewardPopup:
                break;
            case UIPopupName.PrePlayPopup:
                break;
            case UIPopupName.LoadingPopup:
                break;
            case UIPopupName.GamePlayPopup:
                break;
            case UIPopupName.QuestPopup:
                break;
            case UIPopupName.BottomTabs:
                break;
        }
    }
}
