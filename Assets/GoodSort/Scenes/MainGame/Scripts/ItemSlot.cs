using DG.Tweening;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] ItemController _itemController;

    private ItemController _currentItem = null;
    private TweenCallback _callbackChangeVisualCache;

    public void InitData(int itemIndex)
    {
        GameObject obj = Instantiate(_itemController.gameObject, this.transform);
        _currentItem = obj.GetComponent<ItemController>();
        _currentItem.InitData(itemIndex);
    }

    public void InitRespawnData(int itemIndex)
    {
        _currentItem.InitRespawnData(itemIndex);
    }

    public void EnableCollider()
    {
        GetComponent<BoxCollider>().enabled = true;

        if(_currentItem != null && !IsEmptyItem())
        {
            _currentItem.EnableDragAndDrop();
        }
    }

    public bool IsEmptyItem()
    {
        return _currentItem.IsEmptyObject();
    }

    public void SwapItemCache(ItemSlot targetSlot, int targetId)
    {
        ItemController targetItem = targetSlot.GetCurrentItem();
        ItemController item = _currentItem;

        int idShining = item.GetIndex();
        int idTarget = targetItem.GetIndex();

        _callbackChangeVisualCache = () => 
        {
            item.InitData(idTarget);
            targetItem.InitData(targetId);
            //do anim
            targetItem.GetComponent<ItemAnimation>().AnimShrinkOnly();
        };
    }

    public void ChangeIdCache(int targetId)
    {
        _callbackChangeVisualCache = () => 
        {
            _currentItem.InitData(targetId);
            //do anim
            _currentItem.GetComponent<ItemAnimation>().AnimShrinkOnly();
        };
    }

    public void ChangeVisualCache()
    {
        _callbackChangeVisualCache?.Invoke();
    }

    public void SwapItem(ItemSlot slot, TweenCallback callback=null)
    {
        ItemController swapItem = slot.GetCurrentItem();
        ItemController item = _currentItem;
        swapItem.transform.parent = this.transform;
        item.transform.parent= slot.transform;
        swapItem.GetComponentInParent<ItemSlot>().GetNewCurrentItem();
        item.GetComponentInParent<ItemSlot>().GetNewCurrentItem();

        swapItem.transform.DOLocalMove(Vector3.zero, 0.2f).OnComplete(() =>
        {
            swapItem.EnableDragAndDrop();
        });

        item.transform.DOLocalMove(Vector3.zero, 0.2f).OnComplete(() =>
        {
            item.EnableDragAndDrop();
            callback?.Invoke();
        });
    }

    public void GetNewCurrentItem()
    {
        _currentItem= transform.GetChild(0).GetComponent<ItemController>();
    }

    public ItemController GetCurrentItem()
    {
        return _currentItem;
    }

    public void HideItem()
    {
        _currentItem.DoAnimAndDestroy();
    }

    public void HideItemByUsingItem(bool finalObj)
    {
        _currentItem.DoAnimAndDestroyWhenUseItem(finalObj);
    }

    public int GetItemIndex()
    {
        return _currentItem.GetIndex();
    }

    public bool CanPlace()
    {
        if (_currentItem.IsEmptyObject()) return true; 
        return false;
    }
}
