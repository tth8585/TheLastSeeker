using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class SampleItem : ItemPurchaseController
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _price;


    public override void InitData(Product product)
    {
        base.InitData(product);

        _model = product;

        _price.SetText($"{product.metadata.localizedPriceString} " +
            $"{product.metadata.isoCurrencyCode}");
        _title.text = product.definition.id;

    }   
    
}
