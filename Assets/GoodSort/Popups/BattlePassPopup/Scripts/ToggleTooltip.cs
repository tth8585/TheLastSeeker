using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleTooltip : MonoBehaviour, IPointerDownHandler
{
    BattlePassDataDetail _data;
    [SerializeField] Transform _tooltipPos;
    public RectTransform RectTransform;
    public void SetData(BattlePassDataDetail data)
    {
        _data = data;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_data.RewardType.Contains("Chest"))
        {
            MyBattlePass.Instance.ToolTipController.ShowTooltipAtPos(_data, this);

        }
    }
}
