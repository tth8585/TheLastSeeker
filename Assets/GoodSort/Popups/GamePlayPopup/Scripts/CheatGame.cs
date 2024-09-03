using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatGame : MonoBehaviour
{
    public void OnCheat()
    {
        foreach(var item in MyItemAbility.Instance.DicItemRewardInfo)
        {
            MyUserData.Instance.UpdateItemInfo(item.Key, 1);
        }
    }
    public void OnNext()
    {
        MyUserData.Instance.UpdateCurrentLevel(MyUserData.Instance.GetCurrentUserLevel()+1, 0);
        MyGame.Instance.RestartGame();
    }
    public void OnBack()
    {
        MyUserData.Instance.UpdateCurrentLevel(MyUserData.Instance.GetCurrentUserLevel() - 1, 0);
        MyGame.Instance.RestartGame();
    }
}
