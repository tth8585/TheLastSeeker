using Imba.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.GSPassPopup);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
