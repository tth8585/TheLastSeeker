using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipItemDetail : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] TMP_Text _quantity;
    public void SetData(ItemInfoSO data, int quantity)
    {
        _image.sprite = data.Sprite;
        _quantity.text = quantity.ToString();
    }
}
