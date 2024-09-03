using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class GoodenChestItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private List<TooltipItemDetail> _listItemDetails;
    [SerializeField] private Transform _tooltip;
    [SerializeField] private GameObject _claimButton, _claimedIcon;
    [SerializeField] private ITEM_TYPE _chestType = ITEM_TYPE.GoldenChest;
    private void OnEnable()
    {
        if(_chestType == ITEM_TYPE.GoldenChest)
        {
            CheckIfClaimedChest();
            MyEvent.Instance.BattlePassEvents.onClaimedReward += OnClaimedRewardCheck;

        }
    }
    private void OnDisable()
    {
        if (_chestType == ITEM_TYPE.GoldenChest)
        {
            MyEvent.Instance.BattlePassEvents.onClaimedReward -= OnClaimedRewardCheck;
        }
    }
    private void OnClaimedRewardCheck(int level, PASS_ITEM_TYPE type)
    {
        if(level == 30)
        {
            UnLockGoldenChest();
        }
    }
    private void UnLockGoldenChest()
    {
        _claimButton.SetActive(true);
    }
    public void OnClaimChest()
    {
        if(_chestType == ITEM_TYPE.GoldenChest)
        {
            MyBattlePass.Instance.UnlockFinalChest();
        }
        _claimedIcon.SetActive(true);
        _claimButton.SetActive(false);
    }
    private void CheckIfClaimedChest()
    {
        if (MyBattlePass.Instance.GetFinalChestStatus())
        {
            bool isClaimed;
            isClaimed = MyBattlePass.Instance.GetFinalChestStatus();

            _claimedIcon.SetActive(isClaimed);
            _claimButton.SetActive(!isClaimed);
        }
        else
        {
            _claimedIcon.SetActive(false);
            _claimButton.SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ItemInfoSO dataSO = MyItemAbility.Instance.DicItemRewardInfo[_chestType];
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
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _tooltip.SetActive(!_tooltip.gameObject.activeSelf);
    }
}
