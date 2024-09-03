using System;
using System.Collections;
using System.Collections.Generic;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;

public class HeartManager : MonoBehaviour
{
    [SerializeField] private int _maxHeart = 5;
    [SerializeField] private float _timeRecoverSeconds = 60;
    private float _timeRecoverRemain = 0;

    private int _currentHeartCount = 5;
    private double _buffTime;

    private const string heartDataSaveKey = "HeartSaveKey";

    public int CurrentHeartCount => _currentHeartCount;
    public bool IsActiveBuff() => _buffTime > 0;
    public float TimeRecoverRemain => _timeRecoverRemain;
    public int MaxHeart => _maxHeart;

    IEnumerator _counterExpireBuff, _heartRecoverCount;

    public void Awake()
    {
        LoadHeartData();
        _timeRecoverRemain = _timeRecoverSeconds;
        MyEvent.Instance.UserDataManagerEvent.onClaimedItem += ActiveBuff;
    }
    public void OnDestroy()
    {
        if (_heartRecoverCount != null)
        {
            StopCoroutine(_heartRecoverCount);
            _heartRecoverCount = null;
        }
        MyEvent.Instance.UserDataManagerEvent.onClaimedItem -= ActiveBuff;
    }
    #region Heart

    public void UpdateCurrentHeart(int count)
    {
        _currentHeartCount += count;
        CheckHeartRecover();
    }

    private void CheckHeartRecover()
    {
        if(_currentHeartCount >= _maxHeart)
        {
            _currentHeartCount = _maxHeart;
            if(_heartRecoverCount != null)
            {
                StopCoroutine(_heartRecoverCount);
                _heartRecoverCount = null;
            }
            SaveHeartData();

            MyEvent.Instance.HeartEventsManager.UpdateHeart(_currentHeartCount);
            return;
        }
        if (_currentHeartCount < 0) _currentHeartCount = 0;

        if (_heartRecoverCount != null)
        {
            StopCoroutine(_heartRecoverCount);
            _heartRecoverCount = null;
        }
        _heartRecoverCount = RecoverHeart();
        StartCoroutine(_heartRecoverCount);
        MyEvent.Instance.HeartEventsManager.UpdateHeart(_currentHeartCount);
    }

    private void CheckHeartRecoverOnStartGame(HeartDataSave dataSave)
    {
        //Get recovered heart when app not start
        DateTime timeQuitGame;

        if (!DateTime.TryParse(dataSave.TimeExpire, null, System.Globalization.DateTimeStyles.RoundtripKind, out timeQuitGame))
        {
            Debug.LogError("Cant parse TimeExpire !");
        }

        double seconds = (DateTime.Now - timeQuitGame).TotalSeconds;

        int totalHeartRecovered = (int)(seconds / _timeRecoverSeconds);

        if(totalHeartRecovered >= 1)
        {
            _currentHeartCount += totalHeartRecovered;
        }
        CheckHeartRecover();
    }

    IEnumerator RecoverHeart()
    {
        while (_currentHeartCount < _maxHeart)
        {
            _timeRecoverRemain = _timeRecoverSeconds;

            while (_timeRecoverRemain > 0)
            {
                yield return new WaitForSeconds(1);
                _timeRecoverRemain -= 1;
            }
            //yield return new WaitForSeconds(_timeRecoverSeconds);

            _currentHeartCount += 1;
            MyEvent.Instance.HeartEventsManager.UpdateHeart(_currentHeartCount);

            _heartRecoverCount = RecoverHeart();
            StartCoroutine(_heartRecoverCount);
        }
    }

    #endregion Heart

    #region Buff_Heart_Unlimited

    IEnumerator CountBuffExpire()
    {
        while(_buffTime > 0)
        {
            yield return new WaitForSeconds(1);
            _buffTime -= 1;
            MyEvent.Instance.HeartEventsManager.GetHeartUnlimited(_buffTime);
            yield return null;
        }

        _buffTime = 0;
        MyEvent.Instance.HeartEventsManager.ExpireHeartUnlimited();
        MyEvent.Instance.HeartEventsManager.UpdateHeart(_currentHeartCount);
    }

    public void ActiveBuff(ITEM_TYPE type,int minutes)
    {
        if (type != ITEM_TYPE.HeartUnlimited) return;

        double seconds = minutes * 60;
        _buffTime += seconds;

        SaveHeartData();
        LoadHeartData();
    }

    private DateTime CalculateTimeExpireBuff()
    {
        DateTime time = DateTime.Now;

        double minutes = _buffTime / 60;

        time = time.AddMinutes(minutes);

        return time;
    }

    #endregion Buff_Heart_Unlimited

    #region SAVE_LOAD
    private void SaveHeartData()
    {
        HeartDataSave heartDataSave = new()
        {
            CurrentHeartCount = _currentHeartCount ,
            TimeExpire = CalculateTimeExpireBuff().ToString(),
            TimeExit = DateTime.Now.ToString()
        };

        string json = JsonUtility.ToJson(heartDataSave);

        PlayerPrefs.SetString(heartDataSaveKey, json);
        PlayerPrefs.Save();

        //Debug.Log("Save HEART :::::: " + json);
    }

    private void LoadHeartData()
    {
        if (PlayerPrefs.HasKey(heartDataSaveKey))
        {
            string json = PlayerPrefs.GetString(heartDataSaveKey);
            //Debug.Log("LOAD Heart :::::: "+ json);
            HeartDataSave data = JsonUtility.FromJson<HeartDataSave>(json);
            _currentHeartCount = data.CurrentHeartCount;
            CheckHeartRecoverOnStartGame(data);
            GetBuffTime(data);
        }
        else
        {
            _buffTime = 0;
            _currentHeartCount = 5;
            MyEvent.Instance.HeartEventsManager.UpdateHeart(_currentHeartCount);
            SaveHeartData();
        }
        if(_counterExpireBuff != null)
        {
            StopCoroutine(_counterExpireBuff);
            _counterExpireBuff = null;
        }
        _counterExpireBuff = CountBuffExpire();
        StartCoroutine(_counterExpireBuff);
    }

    private void GetBuffTime(HeartDataSave dataSave)
    {
        DateTime timeExpire;

        if (!DateTime.TryParse(dataSave.TimeExpire, null, System.Globalization.DateTimeStyles.RoundtripKind, out timeExpire))
        {
            Debug.LogError("Cant parse TimeExpire !");
        }

        double timeRemain = (timeExpire - DateTime.Now).TotalSeconds;

        //Debug.Log("Time Buff Expire remain seconds: " +timeRemain);

        _buffTime = timeRemain;
    }

    private void OnApplicationQuit()
    {
        SaveHeartData();
    }

    #endregion SAVE_LOAD

    //Test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UpdateCurrentHeart(-1);
        }
    }
}

public class MyHeart: SingletonMonoBehaviour<HeartManager> { }

[Serializable]
public class HeartDataSave
{
    public int CurrentHeartCount;
    public string TimeExpire;
    public string TimeExit;
}
