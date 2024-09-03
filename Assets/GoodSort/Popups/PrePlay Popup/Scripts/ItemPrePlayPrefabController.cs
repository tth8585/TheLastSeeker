using Imba.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPrePlayPrefabController : MonoBehaviour
{
    [SerializeField] Image _icon, _bg;
    [SerializeField] TMP_Text _countText;
    [SerializeField] GameObject _emptyObj,_selectedObj;
    [SerializeField] ITEM_TYPE _type;
    [SerializeField] Sprite[] _selectedSprites;

    [SerializeField] private int _count = 0;
    private bool _isSelected = false;

    private ItemInfoSO _itemInfo;

    public ITEM_TYPE Type => _type;

    private void OnEnable()
    {
        _isSelected = false;
    }

    public void InitData(ItemInfoSO itemInfo,int count)
    {
        _itemInfo = itemInfo;
        _count = count;
        UpdateUI();
    }

    public void InitDataDefault()
    {
        ItemInfoSO defaultItem = MyItemAbility.Instance.DicItemRewardInfo[ITEM_TYPE.Coin];
        InitData(defaultItem, 0);
    }

    private void UpdateUI()
    {
        _icon.sprite = _itemInfo.Sprite;
        if (_count <= 0)
        {
            _emptyObj.SetActive(true);
            _countText.SetActive(false);
        }
        else
        {
            _emptyObj.SetActive(false);
            _countText.SetActive(true);
            _countText.text = _count.ToString();
        }
    }

    public void OnClickItem()
    {
        if (_count <= 0)
        {
            UIManager.Instance.PopupManager.HidePopup(UIPopupName.PrePlayPopup);
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.ShopPopup);

            MyEvent.Instance.MainMenuEventManager.ChangeBottomTab(0);
        }
        else
        {
            //switch to select- deselect item
            _isSelected= !_isSelected;
            if(!_isSelected)
            {
                MyItemAbility.Instance.RemoveItemPreplay(_type);
                _bg.sprite = _selectedSprites[0];
                _selectedObj.SetActive(false);
                _countText.SetActive(true);
            }
            else
            {
                MyItemAbility.Instance.AddItemPreplay(_type);
                _bg.sprite = _selectedSprites[1];
                _selectedObj.SetActive(true);
                _countText.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        _selectedObj.SetActive(false);
        _bg.sprite = _selectedSprites[0];
    }
}
