using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipController : MonoBehaviour
{
    [SerializeField] private List<TooltipItemDetail> _listItemDetails;
    [SerializeField] RectTransform _rectTransform;
    [SerializeField] BattlePassDataDetail _battlePassDataDetail;
    public void ShowTooltipAtPos(BattlePassDataDetail Data, ToggleTooltip toggleTooltip)
    {
        ItemInfoSO dataSO = MyItemAbility.Instance.GetItemInfoByName(Data.RewardType);
        _battlePassDataDetail = Data;
        CopyRectTransformValues(toggleTooltip.RectTransform);
        for (int i = 0; i < _listItemDetails.Count; i++)
        {
            if (i < dataSO.ChestItems.Count)
            {
                _listItemDetails[i].SetData(dataSO.ChestItems[i], dataSO.ChestItemQuantity[i]);
                _listItemDetails[i].gameObject.SetActive(true);
            }
            else
            {
                _listItemDetails[i].gameObject.SetActive(false);
            }
        }
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        else
        {
            if(_battlePassDataDetail == Data)
            {
                gameObject.SetActive(false);
            }
        }
        CopyRectTransformValues(toggleTooltip.RectTransform);
    }
    void CopyRectTransformValues(RectTransform source)
    {
        // Convert world position to local position relative to the parent
        Vector3 localPosition = _rectTransform.parent.InverseTransformPoint(source.position);
        // Set the local position
        _rectTransform.localPosition = localPosition;
    }
}
