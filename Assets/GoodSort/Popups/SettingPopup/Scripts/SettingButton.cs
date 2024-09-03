using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    [SerializeField] private Image _disableImg;

    internal void EnableSetting(bool enable)
    {
        _disableImg.gameObject.SetActive(enable);
    }

    internal void DisableSetting()
    {
        _disableImg.gameObject.SetActive(false);
    }
}
