using System;
using UnityEngine;

public class GameEventManager
{
    public event Action onSlotDoneProgress;
    public void SlotDoneProgress()
    {
        if (onSlotDoneProgress != null) onSlotDoneProgress();
    }
}
