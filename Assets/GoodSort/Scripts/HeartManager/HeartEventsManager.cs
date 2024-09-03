using System;

public class HeartEventsManager 
{
    public event Action<int> onUpdateHeart;
    public void UpdateHeart(int count) => onUpdateHeart?.Invoke(count);

    public event Action<double> onGetHeartUnlimited;
    public void GetHeartUnlimited(double count) => onGetHeartUnlimited?.Invoke(count);

    public event Action onExpireHeartUnlimited;
    public void ExpireHeartUnlimited() => onExpireHeartUnlimited?.Invoke();
}
