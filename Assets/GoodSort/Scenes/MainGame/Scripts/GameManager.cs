using DG.Tweening;
using Imba.UI;
using System.Collections;
using System.Collections.Generic;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GAMEPLAY_STATE
{
    NONE,
    INIT,
    START,
    DRAG_AND_DROP,
    ANIMATION,
    CHECK_MATCH,
    USE_ITEM,
    PAUSE,
    RESUME,
    STOP,
    END
}

public class GameManager : MonoBehaviour
{
    #region VARIANT
    [SerializeField] SpawnManager _spawnManager;
    [SerializeField] TimeManager _timeManager;
    [SerializeField] ComboManager _comboManager;
    [SerializeField] SetUpManager _setupManager;
    [SerializeField] TipManager _tipManager;
    [SerializeField] StarManager _starManager;
    [SerializeField] List<ItemSlot> _listItemOnUseMagic = new();
    [SerializeField] private List<int> _winStreakAddingTimes;

    public GAMEPLAY_STATE CurrentState { get { return _currentGameState; } }
    public DragAndDrop CurrentItem { get { return _currentItem; } set { _currentItem = value; } }

    private GAMEPLAY_STATE _currentGameState= GAMEPLAY_STATE.NONE;
    private DragAndDrop _currentItem;
    private SlotController[] _allSlot;
    private int _countSlotProgress = 0;
    private bool _isCheckingMatchFirstTime = true;
    private bool _doneInitData = false;
    private List<SlotController> _listLockSlot = new List<SlotController>();
    private bool _isDestroyMode = false;
    private int _currentCountHitItem = 0;
    private int _currentCountChangeItem = 0;
    private bool _hasDoubleStar = false;

    #endregion

    #region Unity Method
    private void OnEnable()
    {
        MyEvent.Instance.GameEventManager.onSlotDoneProgress += CountProgressSlot;
        MyEvent.Instance.GameEventManager.onMatchItem += MatchItemUpdateLockSlot;
        MyEvent.Instance.GameEventManager.onDoneAnimHitItem += DoneAnimHitItem;
        MyEvent.Instance.GameEventManager.onDoneMagicItem += DoneChangeItem;
        MyEvent.Instance.GameEventManager.onStartStarFly += _starManager.ShowStarEffect;
    }

    private void OnDisable()
    {
        MyEvent.Instance.GameEventManager.onSlotDoneProgress -= CountProgressSlot;
        MyEvent.Instance.GameEventManager.onMatchItem -= MatchItemUpdateLockSlot;
        MyEvent.Instance.GameEventManager.onDoneAnimHitItem -= DoneAnimHitItem;
        MyEvent.Instance.GameEventManager.onDoneMagicItem -= DoneChangeItem;
        MyEvent.Instance.GameEventManager.onStartStarFly -= _starManager.ShowStarEffect;
    }

    private void Start()
    {
        ChangeGameState(GAMEPLAY_STATE.INIT);
    }

    private void Update()
    {
        //Test
        if (Input.GetKeyDown(KeyCode.P))
        {
            HandleTimeItem();
        }

        if (_doneInitData)
        {
            if (_currentGameState == GAMEPLAY_STATE.CHECK_MATCH && _countSlotProgress == _allSlot.Length)
            {
                if (_isCheckingMatchFirstTime)
                {
                    _isCheckingMatchFirstTime = false;
                    _countSlotProgress = 0;
                    ChangeGameState(GAMEPLAY_STATE.START);
                }
                else
                {
                    if (CheckDoneGame())
                    {
                        ChangeGameState(GAMEPLAY_STATE.END);
                    }
                    else
                    {
                        _countSlotProgress = 0;
                        ChangeGameState(GAMEPLAY_STATE.DRAG_AND_DROP);

                        //need to check any move to show tip use item
                        _tipManager.CheckCanMoveAndShowTip(_allSlot);
                    }
                }
            }
        }

        
    }
    #endregion

    #region LOGIC GAME
    public void RestartGame()
    {
        SceneManager.UnloadSceneAsync(1).completed += (AsyncOperation op) =>
        {
            // Then load the scene again
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
        };
    }

    private void MatchItemUpdateLockSlot()
    {
        if (_listLockSlot.Count <= 0) return;

        SlotController slot = _listLockSlot[0];
        slot.CountToUnlock();
        if (slot.GetCountToUnlock() == 0)
        {
            _listLockSlot.RemoveAt(0);
        }
    }

    private void GetListLockSlot()
    {
        _listLockSlot = _spawnManager.GetAllLockSlot();
        _listLockSlot = Utils.Shuffle(_listLockSlot);
    }

    private void CountProgressSlot()
    {
        _countSlotProgress++;
    }

    public void ChangeGameState(GAMEPLAY_STATE state)
    {
        _currentGameState = state;
        Debug.Log("Change Game State: "+ _currentGameState);
        switch(state)
        {
            case GAMEPLAY_STATE.INIT:
                _hasDoubleStar = false;

                _spawnManager.InitData(MyUserData.Instance.GetCurrentUserLevel());
                _allSlot = _spawnManager.GetAllSlot();
                _isDestroyMode = _spawnManager.GetPlayMode();
                _doneInitData = true;
                _isCheckingMatchFirstTime = true;
                GetListLockSlot();
                
                StartCoroutine(COShowUI());
                break;
            case GAMEPLAY_STATE.START:
                _hasDoubleStar = false;
                MyEvent.Instance.QuestEvents.StartALevelQuest();
                //show ui ?
                //time count down ?
                InjectUIControllerToManager();
                OnUseItemPrePlay();

                var streakTime = GetTimeAddingByProgressStreak();
                Debug.Log("=======time " + MySpawn.Instance.TimePlay + streakTime);
                SetUpTimePlay(MySpawn.Instance.TimePlay + streakTime);
                ChangeGameState(GAMEPLAY_STATE.DRAG_AND_DROP);

                break;
            case GAMEPLAY_STATE.DRAG_AND_DROP:
                break;
            case GAMEPLAY_STATE.ANIMATION:
                break;
            case GAMEPLAY_STATE.CHECK_MATCH:
                CheckLayer();
                break;
            case GAMEPLAY_STATE.USE_ITEM:
                break;
            case GAMEPLAY_STATE.PAUSE: 
                PauseOrResumeGame(0);
                break;
            case GAMEPLAY_STATE.RESUME:
                PauseOrResumeGame(1);
                ChangeGameState(GAMEPLAY_STATE.DRAG_AND_DROP);
                break;
            case GAMEPLAY_STATE.STOP:
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.LoadingPopup);
                MyEvent.Instance.GameEventManager.onLoadingAnimDone += ExitToMenu;
                break;
            case GAMEPLAY_STATE.END:
                //show ui complete
                EndLevel();
                MyEvent.Instance.QuestEvents.CompleteLevelQuest();
                Debug.LogError("need handle progress");
                //MyUserData.Instance.UpdateCurrentLevel(MyUserData.Instance.GetCurrentUserLevel() + 1, 0);
                break;
            default:
                break;
                
        }
    }

    private IEnumerator COShowUI()
    {
        yield return new WaitForEndOfFrame();
        ShowGamePlayUI();

        yield return new WaitForEndOfFrame();
        //show ui tutorial if need(tutorial popup is on top)
        HandleTutorialIfNeed();
    }

    private void HandleTutorialIfNeed()
    {
        //hide object if need
        UIPopup popup = UIManager.Instance.PopupManager.GetPopupFromCacheOrCreate(UIPopupName.GamePlayPopup);
        GamePlayPopup newPop = popup as GamePlayPopup;
        newPop.HandleForTutorialUI();

        if (!CheckLevelNeedTutorial())
        {
            MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.CHECK_MATCH);
            return;
        }

        int currentLevel = MyUserData.Instance.GetCurrentUserLevel();
        RectTransform focusRect = newPop.GetRectAbility(currentLevel);
        //only level 0-4 has tutorial
        switch (currentLevel)
        {
            case 0:
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.TutorialPopup);
                break;
            case 1:
                //bonus hammer +5 ?
                MyUserData.Instance.SetItemInfoData(ITEM_TYPE.Hit, 5);
                MyItemAbility.Instance.OnClaimItemForcusCenterScreen(ITEM_TYPE.Hit, newPop.transform, Vector3.zero, focusRect, 5,()=> OnClaimItemDone(currentLevel, focusRect));
                break;
            case 2:
                MyUserData.Instance.SetItemInfoData(ITEM_TYPE.Refesh, 5);
                MyItemAbility.Instance.OnClaimItemForcusCenterScreen(ITEM_TYPE.Refesh, newPop.transform, Vector3.zero, focusRect, 5, () => OnClaimItemDone(currentLevel, focusRect));
                break;
            case 3:
                MyUserData.Instance.SetItemInfoData(ITEM_TYPE.MagicStick, 5);
                MyItemAbility.Instance.OnClaimItemForcusCenterScreen(ITEM_TYPE.MagicStick, newPop.transform, Vector3.zero, focusRect, 5, () => OnClaimItemDone(currentLevel, focusRect));
                break;
            case 4:
                MyUserData.Instance.SetItemInfoData(ITEM_TYPE.Frozen, 5);
                MyItemAbility.Instance.OnClaimItemForcusCenterScreen(ITEM_TYPE.Frozen, newPop.transform, Vector3.zero, focusRect, 5, () => OnClaimItemDone(currentLevel, focusRect));
                break;
        }
    }
    private void OnClaimItemDone(int curLvl, RectTransform rec)
    {
        if (curLvl != 0)
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.TutorialPopup, rec);
        }

        MyGame.Instance.ChangeGameState(GAMEPLAY_STATE.USE_ITEM);
    }

    private void ShowGamePlayUI()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.LoadingPopup);
        UIManager.Instance.ViewManager.HideView(UIViewName.MainView);
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.GamePlayPopup);
        MyEvent.Instance.GameEventManager.StartNewLevel();
    }

    private void InjectUIControllerToManager()
    {
        GamePlayPopup gamePlayPopup = (GamePlayPopup) UIManager.Instance.PopupManager.GetPopup(UIPopupName.GamePlayPopup);

        //_timeManager.SetTimeSlider(gamePlayPopup.TimeSlider);
        _timeManager.SetTimeTxt(gamePlayPopup.TimeTxt);
        _comboManager.SetComboBarController(gamePlayPopup.ComboBarController);
        _starManager.SetStarUI(gamePlayPopup.StarCountTxt);
    }

    private void CheckLayer()
    {
        for(int i = 0; i < _allSlot.Length; i++)
        {
            _allSlot[i].CheckLayer();
        }
    }

    private bool CheckDoneGame()
    {
        for (int i = 0; i < _allSlot.Length; i++)
        {
            if (!_allSlot[i].CheckDone()) return false;
        }

        return true;
    }

    private bool CheckLevelNeedTutorial()
    {
        if (MyUserData.Instance.GetCurrentUserLevel() < 5) return true;
        return false;
    }

    private int GetTimeAddingByProgressStreak()
    {
        var currentProgress = MyUserData.Instance.UserDataSave.ProgressStreak;
        return _winStreakAddingTimes[currentProgress];
    }

    #region Game state handle

    private void PauseOrResumeGame(int value) => Time.timeScale = value;

    private void ExitToMenu()
    {
        MyEvent.Instance.GameEventManager.onLoadingAnimDone -= ExitToMenu;

        SceneManager.UnloadSceneAsync(1);
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.GamePlayPopup);
        UIManager.Instance.ViewManager.ShowView(UIViewName.MainView);
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.LoadingPopup);
    }

    private void EndLevel()
    {
        MyEvent.Instance.GameEventManager.onLoadingAnimDone += ExitToMenu;
        if(!_timeManager.isTimeout)
        {
            var currentLvl = MyUserData.Instance.GetCurrentUserLevel();
            MyUserData.Instance.UpdateCurrentLevel(currentLvl + 1, 1);
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.EndGamePopup, _currentStar);
        }
        else
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.TimeOutPopup);
        }

    }
    #endregion

    #endregion

    #region  Show Screen Effect
    public void ShowScreenEffect(OverlayEffectType type, float time)
    {
        GamePlayPopup gamePlayPopup = (GamePlayPopup)UIManager.Instance.PopupManager.GetPopup(UIPopupName.GamePlayPopup);
        gamePlayPopup.ShowOverlayEffect(type, time);
    }

    public void ForceStopScreenEffect()
    {
        GamePlayPopup gamePlayPopup = (GamePlayPopup)UIManager.Instance.PopupManager.GetPopup(UIPopupName.GamePlayPopup);
        //gamePlayPopup.ForceStopCurrentEffect()
    }

    #endregion

    #region SET UP GAME FROM GD CONFIG

    public void SetUpFromConfig(float speedMove, float smoothMove)
    {
        _setupManager.SetMoveSpeed(speedMove);
        _setupManager.SetMoveSmooth(smoothMove);
    }

    public float GetMoveSpeed()
    {
        return _setupManager.GetMoveSpeed();
    }

    public float GetMoveSmooth() { return _setupManager.GetMoveSmooth(); }

    public void SetUpTimePlay(float timePlay)
    {
        bool isTutorial = false;
        if(MyUserData.Instance.GetCurrentUserLevel()==0) isTutorial= true;
        
        _timeManager.SetUpTimePlay(timePlay, isTutorial, false);
    }
    #endregion

    #region ITEM ABILITY //need to split to item ability manager later
    private int _currentIdForMagicAbility = -1;

    public void ActiveItem(ITEM_TYPE type)
    {
        //if (GetState() == GAMEPLAY_STATE.END) return;

        ChangeGameState(GAMEPLAY_STATE.USE_ITEM);
        switch (type)
        {
            case ITEM_TYPE.Hit:
                HandleHitItem();
                break;
            case ITEM_TYPE.MagicStick:
                HandleMagicItem();
                break;
            case ITEM_TYPE.Frozen:
                HandleFrozenTime();
                break;
            case ITEM_TYPE.Refesh:
                HandleReFreshItem();
                break;
            case ITEM_TYPE.Boom:
                HandleBoomItem();
                break;
            case ITEM_TYPE.Time:
                HandleTimeItem();
                break;
            case ITEM_TYPE.DoubleStar:
                _hasDoubleStar= true;
                HandleDoubleStarItem();
                break;
            default: break;
        }
        MyUserData.Instance.UpdateItemInfo(type, -1);
    }

    private void OnUseItemPrePlay()
    {
        foreach (var item in MyItemAbility.Instance.ItemPreplay)
        {
            ActiveItem(item);
            Debug.Log("Active item :::: "+ item);
        }
        MyItemAbility.Instance.ClearItemPrePlayOnUsed();
    }

    private void HandleBoomItem()
    {
        GamePlayPopup gamePlayPopup = (GamePlayPopup)UIManager.Instance.PopupManager.GetPopup(UIPopupName.GamePlayPopup);
        gamePlayPopup.DoBoomAnimEffect();

        HandleHitItem();
    }

    private void HandleTimeItem()
    {
        GamePlayPopup gamePlayPopup = (GamePlayPopup)UIManager.Instance.PopupManager.GetPopup(UIPopupName.GamePlayPopup);
        gamePlayPopup.DoTimeAnimEffect();
        _timeManager.SetUpTimePlay(MySpawn.Instance.TimePlay, false, true);
    }

    private void HandleDoubleStarItem()
    {
        GamePlayPopup gamePlayPopup = (GamePlayPopup)UIManager.Instance.PopupManager.GetPopup(UIPopupName.GamePlayPopup);
        gamePlayPopup.DoDoubleStarEffect();
    }

    private void HandleRefreshAnim(TweenCallback callback)
    {
        GamePlayPopup gamePlayPopup = (GamePlayPopup)UIManager.Instance.PopupManager.GetPopup(UIPopupName.GamePlayPopup);
        gamePlayPopup.DoRefreshEffect(callback);
    }

    private void HandleHitAnim()
    {
        GamePlayPopup gamePlayPopup = (GamePlayPopup)UIManager.Instance.PopupManager.GetPopup(UIPopupName.GamePlayPopup);
        gamePlayPopup.DoHitEffect();
    }

    private void HandleMagicItem()
    {
        _listItemOnUseMagic.Clear();
        //change 6 item into 2 type id that can match 3
        _currentCountChangeItem = 2;

        _currentIdForMagicAbility = -1;

        DoneChangeItem();
    }

    private void DoneChangeItem()
    {
        if (_currentCountChangeItem == 0)
        {
            Debug.Log("xx count item magic: " + _listItemOnUseMagic.Count) ;
            MyGameFx.Instance.SetItemToMagicFX(_listItemOnUseMagic);

            GamePlayPopup gamePlayPopup = (GamePlayPopup)UIManager.Instance.PopupManager.GetPopup(UIPopupName.GamePlayPopup);
            gamePlayPopup.DoMagicWandAnim(() => { ChangeGameState(GAMEPLAY_STATE.CHECK_MATCH); });
            return;
        }

        _currentCountChangeItem--;

        List<int> listIdSurfaceForShining = new List<int>();// = GetAllIdItemOfSlotAtLayerForShining(0);

        if (_isDestroyMode)
        {
            listIdSurfaceForShining = GetAllIdItemForMagicInDestroyMode();
        }
        else
        {
            listIdSurfaceForShining = GetAllIdItemOfSlotAtLayerForMagicAbility(0, _currentIdForMagicAbility);
        }

        int randomId = GetRandomIdFromList(listIdSurfaceForShining);

        FindAndHandleMatchItemForMagicItem(randomId);
    }

    private void FindAndHandleMatchItemForMagicItem(int randomId)
    {
        if (randomId == 0)
        {
            //that mean not enough item id or out of item
            Debug.LogError("Something happend when use item and get id for effect");
            ChangeGameState(GAMEPLAY_STATE.CHECK_MATCH);
        }
        else
        {
            if(_currentIdForMagicAbility==-1) _currentIdForMagicAbility = randomId;
            //find all slot contain that id
            List<ItemSlot> listSlot = GetSlotForIndex(randomId);

            if (listSlot.Count >= 3)
            {
                //List<ItemSlot> newList= 
                List<ItemSlot> listSurfaceSlot = GetSlotSurface();
                ItemSlot slot = listSlot[0];

                for (int i = 0; i < listSlot.Count; i++)
                {
                    if (listSurfaceSlot.Contains(listSlot[i]))
                    {
                        //lay ra slot co id can tim
                        slot = listSlot[i];
                        break;
                    }
                }

                List<ItemSlot> suffleSlot = new List<ItemSlot>();

                for (int i = 0; i < listSurfaceSlot.Count; i++)
                {
                    if (listSurfaceSlot[i].GetItemIndex() != slot.GetItemIndex())
                    {
                        //lay ra cac slot ko giong voi id cua slot
                        suffleSlot.Add(listSurfaceSlot[i]);
                    }
                    else
                    {
                        //Debug.Log("item on sufface id: " + slot.GetItemIndex());
                    }
                }

                //trao' vi tri
                suffleSlot = Utils.Shuffle(suffleSlot);

                List<ItemSlot> newList= new List<ItemSlot>();
                //luu cache callback vao cac slot can thay doi visual
                for (int i = 0; i < 3; i++)
                {
                    if (!listSurfaceSlot.Contains(listSlot[i]))
                    {
                        listSlot[i].SwapItemCache(suffleSlot[i], _currentIdForMagicAbility);
                    }
                    else
                    {
                        listSlot[i].ChangeIdCache(_currentIdForMagicAbility);
                    }

                    newList.Add(listSlot[i]);
                }

                _listItemOnUseMagic.AddRange(newList);

                MyEvent.Instance.GameEventManager.DoneMagicItem();
            }
            else
            {
                Debug.LogError("cant get enough 3 slot to match, some bug make this happend");
                MyEvent.Instance.GameEventManager.DoneAnimHitItem();
            }
        }
    }

    private void HandleHitItem()
    {
        //match 3 times for 3 random id
        _currentCountHitItem = 3;

        HandleHitAnim();

        DoneAnimHitItem();
    }

    private void DoneAnimHitItem()
    {
        if (_currentCountHitItem == 0)
        {
            ChangeGameState(GAMEPLAY_STATE.CHECK_MATCH);
        }
        else
        {
            _currentCountHitItem--;

            List<int> listIdSurface = new List<int>();

            if (_isDestroyMode)
            {
                listIdSurface = GetAllIdItemForDestroyMode();
            }
            else
            {
                listIdSurface = GetAllIdItemOfSlotAtLayer(0);
            }

            int randomId = GetRandomIdFromList(listIdSurface);
            FindAndHandleMatchItemWhenUsingHitItem(randomId);
        }
    }

    private void FindAndHandleMatchItemWhenUsingHitItem(int randomId)
    {
        if (randomId == 0)
        {
            //that mean not enough item id or out of item
            Debug.LogError("Something happend when use item and get id for effect");
            ChangeGameState(GAMEPLAY_STATE.CHECK_MATCH);
        }
        else
        {
            //find all slot contain that id
            List<ItemSlot> listSlot = GetSlotForIndex(randomId);

            Debug.Log("xx GetSlotForIndex: "+listSlot.Count);

            if (listSlot.Count >=3)
            {
                int count = 3;
                for (int j = 0; j < 3; j++)
                {
                    if (count > 0)
                    {
                        count--;
                        if (j == 2) listSlot[j].HideItemByUsingItem(true);//the finnal item to do effect
                        else
                        {
                            listSlot[j].HideItemByUsingItem(false);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("cant get enough 3 slot to match, some bug make this happend");
                MyEvent.Instance.GameEventManager.DoneAnimHitItem();
            }
        }
    }

    private void HandleReFreshItem()
    {
        List<int> mergedList = new List<int>();
        List<int> listIdSurface = GetAllIdItemOfSlotAtLayer(0);
        List<int> listIdSecond= new List<int>();
        //List<SlotController> 
        mergedList.AddRange(listIdSurface);

        if (!_isDestroyMode)
        {
            listIdSecond = GetAllIdItemOfSlotAtLayer(1);
            mergedList.AddRange(listIdSecond);
        }

        HandleRefreshAnim(()=>
        {
            MyEvent.Instance.GameEventManager.ShuffleItem();
            mergedList = Utils.Shuffle(mergedList);

            List<int> shuffledListIdSurface = mergedList.GetRange(0, listIdSurface.Count);

            if (!_isDestroyMode)
            {
                List<int> shuffledListIdSecond = mergedList.GetRange(listIdSurface.Count, listIdSecond.Count);
                InitIdItemAfterSuffle(1, shuffledListIdSecond);
            }

            InitIdItemAfterSuffle(0, shuffledListIdSurface);

            ChangeGameState(GAMEPLAY_STATE.CHECK_MATCH);
        });
    }

    private void HandleFrozenTime()
    {
        ChangeGameState(GAMEPLAY_STATE.CHECK_MATCH);
        float pauseTime = 10f;
        _timeManager.PauseTime(pauseTime);
        ShowScreenEffect(OverlayEffectType.Frozen, pauseTime);
    }

    private List<int> GetAllIdItemOfSlotAtLayer(int layerIndex)
    {
        List<int> newList= new List<int>();

        for (int i = 0; i < _allSlot.Length; i++)
        {
            List<int> listId = _allSlot[i].GetListIdOfLayer(layerIndex);
            newList.AddRange(listId);
        }

        return newList;
    }

    private List<int> GetAllIdItemOfSlotAtLayerForMagicAbility(int layerIndex, int excludeId)
    {
        List<int> newList = new List<int>();

        for (int i = 0; i < _allSlot.Length; i++)
        {
            List<int> listId = _allSlot[i].GetListIdOfLayer(layerIndex, excludeId);
            newList.AddRange(listId);
        }

        return newList;
    }

    private void InitIdItemAfterSuffle(int layerIndex, List<int> listId)
    {
        for (int i = 0; i < _allSlot.Length; i++)
        {
            List<int> newList = listId.GetRange(0,_allSlot[i].GetItemCountToFillLayer());
            listId.RemoveRange(0, newList.Count);
            _allSlot[i].InitReplaceNewListIndex(layerIndex, newList);
        }
    }

    private int GetRandomIdFromList(List<int> listId)
    {
        List<int> newList = new List<int>();
        for(int i=0; i < listId.Count; i++)
        {
            if (listId[i] != 0)
            {
                newList.Add(listId[i]);
            }
        }

        return GetRandomItem(newList);
    }

    private int GetRandomItem(List<int> list)
    {
        // Check if the list is empty
        if (list.Count == 0)
        {
            return 0;
        }

        // Initialize the random number generator
        System.Random random = new System.Random();

        // Generate a random index
        int randomIndex = random.Next(list.Count);

        // Return the item at the random index
        return list[randomIndex];
    }

    private List<ItemSlot> GetSlotForIndex(int randomId)
    {
        List<ItemSlot> newList= new List<ItemSlot>();
        for(int i = 0; i < _allSlot.Length; i++)
        {
            List<ItemSlot> list = _allSlot[i].GetSlot(randomId);
            if(list != null) newList.AddRange(list);
        }
        return newList;
    }

    private List<ItemSlot> GetSlotSurface()
    {
        List<ItemSlot> newList = new List<ItemSlot>();

        for (int i = 0; i < _allSlot.Length; i++)
        {
            List<ItemSlot> list = _allSlot[i].GetSurfaceSlot();
            if (list != null) newList.AddRange(list);
        }

        return newList;
    }

    private List<int> GetAllIdItemForDestroyMode()
    {
        List<int> newList = new List<int>();

        for (int i = 0; i < _allSlot.Length; i++)
        {
            List<int> listId = _allSlot[i].GetListIdOfLayer(0);
            newList.AddRange(listId);
        }

        return newList;
    }

    private List<int> GetAllIdItemForMagicInDestroyMode()
    {
        List<int> newList = new List<int>();

        for (int i = 0; i < _allSlot.Length; i++)
        {
            if (CheckAvailableSlotForMagicItem(_allSlot[i]))
            {
                List<int> listId = _allSlot[i].GetListIdOfLayer(0);
                newList.AddRange(listId);
            }
        }

        return newList;
    }

    private bool CheckAvailableSlotForMagicItem(SlotController slot)
    {
        //min and max Ypos follow the camera setup, fix later if camera change
        Debug.LogWarning("min and max Ypos follow the camera setup, fix later if camera change");
        int minYpos = -2;
        int maxYpos = 3;

        float currentYpos = slot.transform.position.y;
        if(currentYpos>=minYpos&& currentYpos<=maxYpos)
        {
            Debug.Log(slot.GetIdSlot() + "/" + slot.transform.position.y);
            return true;
        }

        return false;
    }

    #endregion

    #region STAR SYSTEM
    private int _currentStar = 0;
    public int CurrentStar => _currentStar;

    public void AddStar(int value)
    {
        _currentStar += value;
        if (_hasDoubleStar)
        {
            _currentStar += value;
        }
        MyEvent.Instance.GameEventManager.OnStarAdded(_currentStar);
    }

    #endregion
}

public class MyGame : SingletonMonoBehaviour<GameManager> { }
