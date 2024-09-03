using System;

public class UserDataManagerEvent
{
    public event Action onLoadedUserData;
    public void OnLoadedUserData() => onLoadedUserData?.Invoke();

    public event Action<ITEM_TYPE, int> onClaimedItem;
    public void ClaimItem(ITEM_TYPE type, int quantity) => onClaimedItem?.Invoke(type, quantity);

    public event Action onItemChanged;
    public void ItemAbilityChanged() 
    {
        onItemChanged?.Invoke();
    }
}
