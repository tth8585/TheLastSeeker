using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class TitlePopupController : MonoBehaviour
{
    public string _title;

    private TMP_Text _mainText,_bgText;

    public void SetText()
    {
        if (_mainText == null) _mainText = transform.GetChild(1).GetComponent<TMP_Text>();
        if(_bgText == null) _bgText = transform.GetChild(0).GetComponent<TMP_Text>();

        _mainText.text = _title;
        _bgText.text= _title;
    }
}
