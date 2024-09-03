using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Imba.UI;

public class BottomNavBtnInfo:MonoBehaviour
{
    public UIPopupName UIPopupName;
    public string Name;
    public RectTransform Icon;
    public GameObject Selected;

    public void SelectItem(bool isSelect)
    {
        Selected.SetActive(isSelect);
    }
}
