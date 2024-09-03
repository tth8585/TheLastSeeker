using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEventManager
{
    public event Action<int> onChangeBottomTab;
    public void ChangeBottomTab(int indexTab)
    {
        if (onChangeBottomTab != null) onChangeBottomTab(indexTab);
    }
}
