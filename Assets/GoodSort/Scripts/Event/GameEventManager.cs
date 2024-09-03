using System;
using UnityEngine;

public class GameEventManager
{
    public event Action onSlotDoneProgress;
    public void SlotDoneProgress()
    {
        if (onSlotDoneProgress != null) onSlotDoneProgress();
    }

    public event Action onMatchItem;
    public event Action<Vector3> onSetMatchPosition;
    public void MatchItem(Vector3 matchPos)
    {
        onSetMatchPosition?.Invoke(matchPos);

        if (onMatchItem != null) onMatchItem();
    }

    public event Action onDoneAnimHitItem;
    public void DoneAnimHitItem()
    {
        if (onDoneAnimHitItem != null) onDoneAnimHitItem();
    }

    public event Action onDoneMagicItem;
    public void DoneMagicItem()
    {
        if (onDoneMagicItem != null) onDoneMagicItem();
    }

    public event Action onShuffleItem;
    public void ShuffleItem()
    {
        onShuffleItem?.Invoke();
    }

    public event Action onStartNewLevel;
    public void StartNewLevel()
    {
        onStartNewLevel?.Invoke();
    }

    public event Action<ItemAbilityUIController> onUseItem;
    public void UseItem(ItemAbilityUIController item)
    {
        onUseItem?.Invoke(item);
    }

    public event Action<int> onStarAdded;
    public void OnStarAdded(int starCount) => onStarAdded?.Invoke(starCount);

    public event Action<Transform, int, Vector3, Vector3> onStartStarFly;
    public void StartStarFly(Transform parent, int count, Vector3 start, Vector3 end) => onStartStarFly?.Invoke(parent, count, start, end);

    /* This callback using for define which action will be do after the loading popup done the anim
     */
    public event Action onLoadingAnimDone;
    public void OnLoadingAnimDone() => onLoadingAnimDone?.Invoke();
    public void ClearLoadingAnimAction() => onLoadingAnimDone = null;

    /* This callback using when claim somethings in another popup but need to update in current popup
     */

    public event Action onClaimedSomeStuff;
    public void OnClaimedSomeStuff() => onClaimedSomeStuff?.Invoke();
}
