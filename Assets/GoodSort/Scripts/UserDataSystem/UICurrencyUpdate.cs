using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICurrencyUpdate : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _coinText;
    private void OnEnable()
    {
        UpdateCoin(MyUserData.Instance.UserDataSave.Coin);
        MyEvent.Instance.UserDataManagerEvent.onClaimedItem += OnClaimedCoin;
    }
    private void OnDisable()
    {
        MyEvent.Instance.UserDataManagerEvent.onClaimedItem -= OnClaimedCoin;
    }
    private void OnClaimedCoin(ITEM_TYPE type, int q)
    {
        if(type == ITEM_TYPE.Coin)
        {
            UpdateCoin(MyUserData.Instance.UserDataSave.Coin);
        }
    }
    private void UpdateCoin(int coin)
    {
        _coinText.text = coin.ToString();
    }
}
