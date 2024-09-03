using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipManager : MonoBehaviour
{
    public void CheckCanMoveAndShowTip(SlotController[] allSlot)
    {
        List<int> listId = new List<int>();
        for (int i = 0; i < allSlot.Length; i++)
        {
            List<ItemSlot> surfaceSlot = allSlot[i].GetSurfaceSlot();
            foreach (ItemSlot slot in surfaceSlot)
            {
                listId.Add(slot.GetItemIndex());
            }
        }

        bool canMove = CheckCanMove(listId);

        if (!canMove)
        {
            //show tip use item
            Debug.LogError("Need show tip use item not handle");

            //need show 1 tip only
            return;
        }


        bool hasMatchItemOnSurface = CheckAnyMatchItemOnSurface(listId);
        if (!hasMatchItemOnSurface)
        {
            //show tip use item 
            Debug.LogError("Need show tip use item not handle");
        }
    }

    private bool CheckCanMove(List<int> listId)
    {
        for (int i = 0; i < listId.Count; i++)
        {
            if (listId[i] == 0) return true;
        }

        return false;
    }

    private bool CheckAnyMatchItemOnSurface(List<int> listId)
    {
        Dictionary<int, int> frequencyMap = new Dictionary<int, int>();

        foreach (int value in listId)
        {
            if (value == 0)
            {
                continue; // Skip value 0
            }

            if (frequencyMap.ContainsKey(value))
            {
                frequencyMap[value]++;
                if (frequencyMap[value] == 3)
                {
                    return true; // Return true if any element appears exactly three times
                }
            }
            else
            {
                frequencyMap[value] = 1;
            }
        }

        return false;
    }

}
