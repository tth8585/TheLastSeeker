using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePassRewardItem : MonoBehaviour
{
    [SerializeField] private Image _rewardIcon;
    [SerializeField] private TMP_Text _rewardValue;

    public void InitRewardItem(Sprite icon, string rewardValue, bool isActive = true)
    {
        if(!isActive || rewardValue.Equals("0")) 
        {
            this.gameObject.SetActive(false);
            return; 
        }

        _rewardIcon.sprite = icon;

        if(rewardValue != null && rewardValue != "none") 
            _rewardValue.text = rewardValue;
        else
            _rewardValue.SetActive(false);
    }
}
