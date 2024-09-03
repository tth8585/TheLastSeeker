using System;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    #region QUEST SYSTEM EVENT
    private QuestEvents _questEvents;
    public QuestEvents QuestEvents
    {
        get
        {
            if (_questEvents == null) _questEvents = new QuestEvents();
            return _questEvents;
        }
        set { _questEvents = value; }
    }
    #endregion

    #region BATTLE PASS EVENT
    private BattlePassEvents _battlePassEvents;
    public BattlePassEvents BattlePassEvents
    {
        get
        {
            if (_battlePassEvents == null) _battlePassEvents = new BattlePassEvents();
            return _battlePassEvents;
        }
        set { _battlePassEvents = value; }
    }
    #endregion

    #region QUEST SYSTEM EVENT
    private HeartEventsManager _heartEventsManager;
    public HeartEventsManager HeartEventsManager
    {
        get
        {
            if (_heartEventsManager == null) _heartEventsManager = new HeartEventsManager();
            return _heartEventsManager;
        }
        set { _heartEventsManager = value; }
    }
    #endregion

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

    #region MAIN MENU EVENT
    private MainMenuEventManager _mainMenuEventManager;
    public MainMenuEventManager MainMenuEventManager
    {
        get
        {
            if (_mainMenuEventManager == null)
            {
                _mainMenuEventManager = new MainMenuEventManager();
            }
            return _mainMenuEventManager;
        }
        set { _mainMenuEventManager = value; }
    }

    #endregion

    #region LOAD USER DATA EVENT
    private UserDataManagerEvent _userDataManagerEvent;
    public UserDataManagerEvent UserDataManagerEvent
    {
        get
        {
            if (_userDataManagerEvent == null) _userDataManagerEvent = new UserDataManagerEvent();
            return _userDataManagerEvent;
        }
        set { _userDataManagerEvent = value; }
    }
    #endregion LOAD USER DATA EVENT

    public event Action<int> OnPlayerLevelChanged;
    public void PlayerLevelChanged(int level)
    {
        OnPlayerLevelChanged?.Invoke(level);
    }
}

public class MyEvent : SingletonMonoBehaviour<EventManager> { }
