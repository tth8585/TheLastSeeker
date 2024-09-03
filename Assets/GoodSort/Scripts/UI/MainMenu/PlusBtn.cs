using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusBtn : MonoBehaviour
{
    [SerializeField] ITEM_TYPE _type;
    public void OnClickPlusBtn()
    {      
        MyEvent.Instance.MainMenuEventManager.ChangeBottomTab(0);
    }
}
