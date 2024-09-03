using System.Collections.Generic;
using UnityEngine;

public class ItemShopContainer : MonoBehaviour
{
    [SerializeField] private List<ItemShop> _items;

    private void OnEnable()
    {
        StillHaveItemToPurchase();
        if (!this.gameObject.activeSelf) return;

        foreach (var item in _items)
        {
            item.ON_PLAYER_PURCHASE += StillHaveItemToPurchase;
        }
    }

    private void OnDisable()
    {
        foreach (var item in _items)
        {
            item.ON_PLAYER_PURCHASE -= StillHaveItemToPurchase;
        }
    }

    private void StillHaveItemToPurchase()
    {
        foreach (var item in _items)
        {
            if (item.gameObject.activeSelf) return;
        }
        this.gameObject.SetActive(false);
    }    
}
