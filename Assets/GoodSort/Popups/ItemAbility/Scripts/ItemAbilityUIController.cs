using Imba.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemAbilityUIController : MonoBehaviour
{
    [SerializeField] ItemBarController _controller;
    [SerializeField] Image _abilityIcon;
    [SerializeField] TMP_Text _countText;
    [SerializeField] ITEM_TYPE _type;
    [SerializeField] Image _cooldownImage;

    private float _timeCoolDown = 3f;
    private float _currentTime = 0f;

    private ItemInfoSO _itemInfo;

    private void OnEnable()
    {
        _currentTime = 0;
        UpdateCoolDownImage();
        GetComponent<UIButton>().Interactable = true;

        MyEvent.Instance.GameEventManager.onUseItem += UpdateCooldown;
    }

    private void OnDisable()
    {
        MyEvent.Instance.GameEventManager.onUseItem -= UpdateCooldown;
    }

    private void UpdateCooldown(ItemAbilityUIController controller)
    {
        _currentTime = _timeCoolDown;
        GetComponent<UIButton>().Interactable = false;
    }

    public void InitData(int count, ItemInfoSO itemInfo)
    {
        _itemInfo = itemInfo;
        _countText.text = count.ToString();
        _abilityIcon.sprite = _itemInfo.Sprite;
    }

    public ITEM_TYPE Type
    {
        get { return _type; }
    }

    public void OnClickItem()
    {
        if(_currentTime>0)
        {
            //not cooldown yet
            return;
        }

        if(MyItemAbility.Instance.CanActiveItem(_type))
        {
            MyItemAbility.Instance.ActiveItem(_type);
            MyEvent.Instance.GameEventManager.UseItem(this);
            _currentTime = _timeCoolDown;
            GetComponent<UIButton>().Interactable = false;
        }
    }

    #region COOL DOWN
    private void UpdateCoolDownImage()
    {
        _cooldownImage.fillAmount= _currentTime/_timeCoolDown;
    }

    private void Update()
    {
        if (_currentTime > 0)
        {
            _currentTime -= Time.deltaTime;
            if(_currentTime<0 )
            {
                _currentTime = 0;
                GetComponent<UIButton>().Interactable = true;
            }

            UpdateCoolDownImage();
        }
    }
    #endregion
}
