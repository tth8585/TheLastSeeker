using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GSPassData", menuName = "ScriptableObjects/GSPass")]
public class BattlePassSO : ScriptableObject
{
    [Header("Reward")]
    public int Gem;
    public int Cash;
    public int Token;//2
    public bool RemoveADs;
    public bool RandomSkin;

    [Header("RewardUI")]
    public Sprite GemIcon;
    public Sprite CashIcon;
    public Sprite TokenIcon;//7
    public Sprite RemoveADSIcon;
    public Sprite SkinIcon;

    [Header("BG")]
    public Sprite GSPassBG;

    //public int GetSkin() => SkinItemController.GetRandomSkin();
    
}
