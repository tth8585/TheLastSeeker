using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using DG.Tweening;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;
using UnityEngine.UI;

public enum ITEM_TYPE
{
    None,
    DoubleStar,
    Time,
    Hit,
    Coin,
    BlueChest,
    Refesh,
    MagicStick,
    Frozen,
    Heart,
    GreenChest,
    VioletChest,
    HeartUnlimited,
    RubyChest,
    GoldenChest,
    Boom,
    Star
}

public class ItemAbilityManager : MonoBehaviour
{
    [SerializeField] ItemBarController _itemBarController;

    [SerializeField] private List<ITEM_TYPE> _itemPreplay = new();
    public List<ITEM_TYPE> ItemPreplay => _itemPreplay;
    public Dictionary<ITEM_TYPE, ItemInfoSO> DicItemRewardInfo = new Dictionary<ITEM_TYPE, ItemInfoSO>();
    public bool HasLoadItemInfo = false;

    [SerializeField] Image _claimItemImage;
    private ObjectPoolsManager _claimItemPool;
    private void Start()
    {
        _claimItemPool = new();
        _claimItemPool.InitPoolObjects(6, _claimItemImage.transform, this.transform);
    }
    public void InitData()
    {
        HasLoadItemInfo = false;
        LoadBattlePassItemInfo();
    }
    public void OnClaimItemForcusCenterScreen(ITEM_TYPE itemType, Transform parentUI, Vector3 startPos, RectTransform endTarget, int quantity, TweenCallback callback)
    {
        Sprite sprite = GetItemInfoByType(itemType).Sprite;
        Transform item = _claimItemPool.GetObject();
        item.GetComponent<Image>().sprite = sprite;
        item.GetComponent<Image>().SetAlpha(1);
        item.parent = parentUI;
        item.localPosition = startPos;
        item.localScale = Vector3.one;

        item.DOShakeScale(.8f,.2f).OnComplete(()=>
        {
            _claimItemPool.ReturnObjToPools(item);
            OnClaimItemAnim(itemType, parentUI, startPos, endTarget, quantity, callback);
        });

    }
    public void OnClaimItemAnim(ITEM_TYPE itemType,Transform parentUI, Vector3 startPos, RectTransform endTarget, int quantity, TweenCallback callback)
    {
        Sprite sprite = GetItemInfoByType(itemType).Sprite;
        List<Transform> _listItemToAnim = new();
        for(int i=0; i< quantity;i++)
        {
            Transform item = _claimItemPool.GetObject();
            item.GetComponent<Image>().sprite = sprite;
            item.GetComponent<Image>().SetAlpha(1);
            item.parent = parentUI;
            item.localPosition = startPos;
            item.localScale = Vector3.one;
            _listItemToAnim.Add(item);
        }
        StartCoroutine(DoClaimItemAnimByQuantity(_listItemToAnim, callback, endTarget));
    }
    IEnumerator DoClaimItemAnimByQuantity(List<Transform> listItems, TweenCallback callback, RectTransform endrect)
    {
        int i = 0;
        int count = listItems.Count;
        while (i < count) {
            Transform item = listItems[i];
            Vector2 endPos = SwitchToRectTransform(endrect,listItems[i].GetComponent<RectTransform>());

            item.GetComponent<RectTransform>().DOAnchorPos(endPos, 1).SetEase(Ease.OutQuad);
            item.DOScale(.2f, 1).OnComplete(() =>
            {
                _claimItemPool.ReturnObjToPools(item);
            });
            item.GetComponent<Image>().DOFade(.2f, 1);

            i++;
            yield return new WaitForSeconds(.1f);
        }
        callback?.Invoke();
    }
    public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
    {
        Vector2 localPoint;
        Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * 0.5f + from.rect.xMin, from.rect.height * 0.5f + from.rect.yMin);
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
        screenP += fromPivotDerivedOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
        Vector2 pivotDerivedOffset = new Vector2(to.rect.width * 0.5f + to.rect.xMin, to.rect.height * 0.5f + to.rect.yMin);
        return to.anchoredPosition + localPoint - pivotDerivedOffset;
    }
    public void SetBarItemController(ItemBarController itemBarController)
    {
        _itemBarController = itemBarController;
        _itemBarController.InitData();
    }

    public bool CanActiveItem(ITEM_TYPE type)
    {
        if(MyGame.Instance.CurrentState== GAMEPLAY_STATE.END)
        {
            Debug.LogWarning("cant use item because its in end State");
            return false;
        }

        int itemQuantity = GetItemAbilityQuantity(type);

        if (itemQuantity > 0)
        {
            return true;
        }

        return false;
    }

    public void ActiveItem(ITEM_TYPE type)
    {
        MyGame.Instance.ActiveItem(type);

        ItemInfoSO itemInfo = GetItemInfoByType(type);
        MyEvent.Instance.QuestEvents.UseItemToQuest(itemInfo.ItemType);
    }

    private int GetItemAbilityQuantity(ITEM_TYPE type)
    {
        ItemInfoSO itemInfo = GetItemInfoByType(type);
        return MyUserData.Instance.DicItemDatas[itemInfo.ItemType];
    }

    public void AddItemPreplay(ITEM_TYPE type)
    {
        if (!_itemPreplay.Contains(type))
        {
            _itemPreplay.Add(type);
        }
    }

    public void ClearItemPrePlayOnUsed()
    {
        _itemPreplay.Clear();
    }
    public void RemoveItemPreplay(ITEM_TYPE type)
    {
        if (_itemPreplay.Contains(type))
        {
            _itemPreplay.Remove(type);
        }
    }

    #region SAVE LOAD

    private void LoadBattlePassItemInfo()
    {
        DicItemRewardInfo.Clear();

        ItemInfoSO[] datas = Resources.LoadAll<ItemInfoSO>("SO/ItemInfo/");

        foreach (var data in datas)
        {
            DicItemRewardInfo.Add(data.ItemType, data);
        }

        HasLoadItemInfo = true;


    }
    public ItemInfoSO GetItemInfoByName(string nameType)
    {
        ITEM_TYPE type;
        if (!Enum.TryParse(nameType, true, out type))
        {
            Debug.LogError("Can't Get Item by name: " + nameType);
        }
        return DicItemRewardInfo[type];
    }
    public ItemInfoSO GetItemInfoByType(ITEM_TYPE itemType)
    {
        var item = DicItemRewardInfo.Where(i => i.Value.ItemType.Equals(itemType)).FirstOrDefault();
        return item.Value;
    }
    #endregion

    #region FOR TEST

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            MyUserData.Instance.UpdateItemInfo(ITEM_TYPE.Frozen,1);
            MyUserData.Instance.UpdateItemInfo(ITEM_TYPE.MagicStick,1);
            MyUserData.Instance.UpdateItemInfo(ITEM_TYPE.Hit,1);
            MyUserData.Instance.UpdateItemInfo(ITEM_TYPE.Refesh,1);

            _itemBarController.InitData();
        }
    }

    #endregion
}

public class MyItemAbility : SingletonMonoBehaviour<ItemAbilityManager> { }

[System.Serializable]
public class ItemAbilityDataSave
{
    public int Count;
    public int Type;
}

[System.Serializable]
public class MyClassArrayWrapper
{
    public ItemAbilityDataSave[] array;

    // Constructor to initialize the wrapper with an array
    public MyClassArrayWrapper(ItemAbilityDataSave[] array)
    {
        this.array = array;
    }
}
