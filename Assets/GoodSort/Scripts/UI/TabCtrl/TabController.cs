using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabController : MonoBehaviour
{
    [SerializeField] private List<TabButton> _tabButtons;

    [SerializeField] private int _currentShowTabIndex;

    private void Start()
    {
        AutoSetTabIndex();
        ShowMainTab();
    }

    #region PUBLIC METHOD
    public void SwitchTab(int index)
    {
        _tabButtons[_currentShowTabIndex].InternalHide();
        _tabButtons[index].InternalShow(out _currentShowTabIndex);
    }
    #endregion

    #region PRIVATE METHOD
    private void AutoSetTabIndex()
    {
        for(int i = 0; i < _tabButtons.Count; i++)
        {
            _tabButtons[i].index = i;
        }
    }

    private void ShowMainTab()
    {
        DisableAllTab();// disable all before show 

        var tabBtn = _tabButtons.Where(t => t.HomeTab == true).FirstOrDefault();
        if (tabBtn == null) return;
        tabBtn.InternalShow(out _currentShowTabIndex);
    }

    private void DisableAllTab()
    {
        foreach(var tab in _tabButtons)
        {
            tab.InternalHide();
        }
    }
    #endregion
}

[Serializable]
public class TabButton
{
    public int index;
    public GameObject SelectBG;
    public GameObject Icon;
    public bool HomeTab;

    internal void InternalShow(out int currentIndex)
    {
        SelectBG.gameObject.SetActive(true);
        currentIndex = index;
    }

    internal void InternalHide() => SelectBG.gameObject.SetActive(false);
}
