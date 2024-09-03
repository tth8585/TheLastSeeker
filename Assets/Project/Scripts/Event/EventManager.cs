using System;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    #region ADS EVENT

    //private AdsEvents _adsEvents;
    //public AdsEvents AdsEvents
    //{
    //    get
    //    {
    //        if (_adsEvents == null) _adsEvents = new AdsEvents();
    //        return _adsEvents;
    //    }
    //    set { _adsEvents = value; }
    //}

    #endregion

    #region GAME EVENT
    private GameEventManager _gameEventManager;
    public GameEventManager GameEventManager
    {
        get
        {
            if (_gameEventManager == null)
            {
                _gameEventManager = new GameEventManager();
            }
            return _gameEventManager;
        }
        set { _gameEventManager = value; }
    }

    #endregion
}

public class MyEvent : SingletonMonoBehaviour<EventManager> { }
