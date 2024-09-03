using System.Collections.Generic;
using System.Linq;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] AllSlotController _allSlotController;
    [SerializeField] SlotController[] _tutorialSlot;
    [SerializeField] float _timeAnim = 1f;
    [SerializeField] List<DestroyModeSlotList> _listDestroySlot;

    private List<int> _listItemIndex= new List<int>();
    private List<int> _listItemIndexNextLayer= new List<int>();
    private List<List<int>> _listCreate= new List<List<int>>();
    private SlotController[] _slots;
    private int _countSpace = 5;
    private int _totalIdSpawn = 2;
    private LevelData _levelData;

    private List<int> _listIndexHasOneItem = new List<int>();

    public float TimePlay => _levelData.TimePlay;

    public void InitData(int level)
    {
        if (level == 0)
        {
            //tutorial
            ShowTutorialSlot(true);
            HandleSpawnTutorial();
        }
        else
        {
            ShowTutorialSlot(false);
            HandleSpawnLevel(level);
        }
    }

    private void HandleSpawnLevel(int level)
    {
        _levelData = MyLevel.Instance.GetLevel(level);

        if (_levelData == null)
        {
            Debug.LogError("level not found: " + level);
            return;
        }
        else
        {
            _countSpace = _levelData.CountSpace;
            _totalIdSpawn = _levelData.TotalIdSpawn;
            MyGame.Instance.SetUpFromConfig(_levelData.MoveSpeed, _levelData.MoveSmooth);

            SlotController[] slots = _allSlotController.GetSlots();

            List<SlotController> newList = new List<SlotController>();
            //set up slot follow data
            for (int i = 0; i < _levelData.SlotData.Length; i++)
            {
                for (int j = 0; j < slots.Length; j++)
                {
                    if (_levelData.SlotData[i].id == slots[j].GetIdSlot())
                    {
                        newList.Add(slots[j]);
                        slots[j].SetUp(_levelData.SlotData[i]);
                    }
                }
            }

            _slots = newList.ToArray();

            foreach (var slot in slots)
            {
                if (!_slots.Contains(slot))
                {
                    slot.gameObject.SetActive(false);
                }
            }

            //create list item 
            CreateListItemId(_levelData.IsDestroyMode);

            //fill item to slot
            InitItemToSlot();
        }
    }

    #region INIT ID TO SLOT
    public List<DestroyModeSlotList> GetAllDestroySlot()
    {
        return _listDestroySlot;
    }

    private List<int> GetDataNextFillList()
    {
        int countItemNeedToFillPerLayer = CalculateTotalItem();
        List<int> newList= new List<int>();

        if (_listItemIndex.Count + _countSpace >= countItemNeedToFillPerLayer)
        {
            newList = _listItemIndex.GetRange(0, countItemNeedToFillPerLayer - _countSpace);
            _listItemIndex.RemoveRange(0, newList.Count);
            newList = Utils.Shuffle(newList);
            newList = AddEmptySpaceToList(newList);
        }
        else
        {
            newList = _listItemIndex.ToList();
            _listItemIndex.RemoveRange(0, newList.Count);
            newList = Utils.Shuffle(newList);

            //thêm các phần tử -2 để ko hỏng logic khi add empty Item
            int countCurrent = newList.Count;
            for (int i = 0; i < countItemNeedToFillPerLayer; i++)
            {
                if (i > countCurrent) newList.Add(-2);
            }
            //thêm các phần tử rỗng
            newList = AddEmptySpaceToList(newList);

            //loại bỏ các phần tử thêm vào cho khỏi hỏng logic
            newList.RemoveAll(item => item == -2);
            newList = Utils.Shuffle(newList);
        }

        return newList;
    }

    public bool CanGetNextItemToFill()
    {
        if (_listItemIndexNextLayer.Count == 0 && _listItemIndex.Count == 0) return false;
        else 
        {
            Debug.Log("xx item Left not fill: "+ _listItemIndexNextLayer.Count+"/"+ _listItemIndex.Count);
            return true;
        } 
    }

    public List<int> GetNextFillId()
    {
        if (_listItemIndexNextLayer.Count == 0)
        {
            //generate index
            _listItemIndexNextLayer = GetDataNextFillList();
        }

        List<int> result= new List<int>();

        int count = 3;

        if(_listItemIndexNextLayer.Count< count)
        {
            result = _listItemIndexNextLayer.GetRange(0, _listItemIndexNextLayer.Count);
        }
        else
        {
            result = _listItemIndexNextLayer.GetRange(0, count);
        }

        _listItemIndexNextLayer.RemoveRange(0, result.Count);

        return result;
    }

    private void InitItemToSlot()
    {
        List<List<int>> newList = new List<List<int>>();
        Dictionary<int, List<int>> dicFinalList = new Dictionary<int, List<int>>();

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].GetItemCountToFillLayer() == 1)
            {
                List<int> list = new List<int>();
                newList.Add(list);
                dicFinalList.Add(i, list);
            }
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].GetItemCountToFillLayer() != 1)
            {
                List<int> list = new List<int>();
                newList.Add(list);
                newList.Add(list);
                newList.Add(list);
                dicFinalList.Add(i, list);
            }
        }

        foreach (var item in _listCreate)
        {
            AddItemToList(newList, item);
        }

        //Debug.Log(newList.Count+"/"+dicFinalList.Count);

        //DebugListItemId(dicFinalList);

        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].InitData(dicFinalList[i]);
        }

        if (_levelData.IsDestroyMode)
        {
            _listItemIndexNextLayer = GetDataNextFillList();
            //if (_listItemIndexNextLayer.All(item => item == -1)) 
            //{
            //    _li
            //}
            //Debug.Log("xx init 1st time :"+ _listItemIndexNextLayer.Count+"/"+_listItemIndex.Count);
        }
    }

    private static void DebugListItemId(Dictionary<int, List<int>> dicFinalList)
    {
        foreach (var item in dicFinalList)
        {
            string test = "";
            List<int> list = item.Value;
            foreach (var item2 in list)
            {
                test += item2.ToString() + "/";
            }

            Debug.Log("test: " + test);
        }
    }

    private void AddItemToList(List<List<int>> listA, List<int> listB )
    {
        //Debug.Log(listA.Count+"/"+listB.Count);
        for(int i = 0; i < listB.Count; i++)
        {
            listA[i].Add(listB[i]);
        }
    }

    int CountItem(List<List<int>> list)
    {
        int count = 0;
        foreach(var item in list)
        {
            for (int j = 0;j < item.Count;j++)
            {
                count++;
            }
        }

        return count;
    }

    public List<List<int>> SplitList(List<int> list, SlotController[] slots)
    {
        List<List<int>> splitLists = new List<List<int>>();

        //all list has 3 item
        for (int i = 0; i < list.Count; i += 3)
        {
            List<int> newList = new List<int>();
            for (int j = 0; j < 3 && i + j < list.Count; j++)
            {
                newList.Add(list[i + j]);
            }
            splitLists.Add(newList);
        }

        if (list.Count % 3 != 0)
        {
            List<int> newList = new List<int>();
            for (int i = list.Count - list.Count % 3; i < list.Count; i++)
            {
                newList.Add(list[i]);
            }
            splitLists.Add(newList);
        }

        Dictionary<int,List<int>> dic= new Dictionary<int,List<int>>();

        for(int i=0;i< splitLists.Count; i++)
        {
            int k = i % slots.Length;
            if (!dic.ContainsKey(k))
            {
                dic.Add(k,new List<int>());
            }

            dic[k].AddRange(splitLists[i]);
        }

        Debug.Log("count dic: "+dic.Count);
        List<List<int>> finalList = new List<List<int>>();
        foreach(var item in dic)
        {
            finalList.Add(item.Value);
        }

        return finalList;
    }
    #endregion

    #region CREATE LIST ID ITEM
    private List<int> AddEmptySpaceToList(List<int> list)
    {
        List<int> listSpace= new List<int>();
        for(int i=0;i< _countSpace; i++)
        {
            listSpace.Add(-1);
        }

        List<int> randomPositions = new List<int>();
        for (int i = 0; i < _countSpace; i++)
        {
            randomPositions.Add(UnityEngine.Random.Range(0, list.Count - 1));
        }

        for (int i = 0; i < randomPositions.Count; i++)
        {
            list.Insert(randomPositions[i], listSpace[i]);
        }

        bool adding = true;
        while (adding)
        {
            adding = false;
            for (int i = 0; i < list.Count - 2; i++)
            {
                int countEmpty = 0;
                for (int j = i; j < i + 3; j++)
                {
                    if (list[j] == -1)
                    {
                        countEmpty++;
                    }
                }

                if (countEmpty == 3)
                {
                    int randomIndex = UnityEngine.Random.Range(i + 3, list.Count);
                    int temp = list[i];
                    list[i] = list[randomIndex];
                    list[randomIndex] = temp;
                    adding = true;
                    break;
                }
            }
        }
        

        return list;
    }

    private void CreateListItemId(bool isDestroyMode)
    {
        _listItemIndex.Clear();

        //fill all the item to the list
        for (int i = 0; i < _totalIdSpawn; i++)
        {
            _listItemIndex.Add(i+1);
            _listItemIndex.Add(i+1);
            _listItemIndex.Add(i+1);
        }

        //tinh tong so luong item cho moi 1 layer cua 1 slot
        int countItemNeedToFillPerLayer = CalculateTotalItem();

        //Debug.Log("countItemNeedToFillPerLayer: "+countItemNeedToFillPerLayer);

        if (_listItemIndex.Count + _countSpace >= countItemNeedToFillPerLayer)
        {
            _listCreate.Clear();
            if(!isDestroyMode) AddEmptyAndSplit(countItemNeedToFillPerLayer);
            else
            {
                AddEmptyAndSplitDestroyMode(countItemNeedToFillPerLayer);
            }
        }
    }

    private bool CheckListAvailable(List<int> list)
    {
        for(int i=0;i< list.Count; i++)
        {
            if (list[i] != -1) return true;
        }

        return false;
    }

    public int CalculateTotalItem()
    {
        int count = 0;

        for(int i=0;i<_slots.Length;i++)
        {
            count += _slots[i].GetItemCountToFillLayer();

            if (_slots[i].GetItemCountToFillLayer() == 1)
            {
                _listIndexHasOneItem.Add(i);
            }
        }

        return count;
    }

    public string DebugList(List<int> list)
    {
        string test = "";
        for (int i = 0; i < list.Count; i++)
        {
            test += list[i].ToString()+"/";
        }
        return test;
    }

    private void AddEmptyAndSplit(int count)
    {
        AddEmptyAndSplitDestroyMode(count);

        if (_listItemIndex.Count + _countSpace >= count)
        {
            //đệ quy nếu list vẫn thỏa điều kiện 
            AddEmptyAndSplit(count);
        }
        else
        {
            _listItemIndex = Utils.Shuffle(_listItemIndex);
            //lấy ra 1 list có số phần tử bằng với các layer đặc biệt (có 1 phần tử)
            List<int> newListOneItem = new List<int>();

            for (int i = 0; i < _listIndexHasOneItem.Count; i++)
            {
                if (i < _listItemIndex.Count - 1)
                {
                    newListOneItem.Add(_listItemIndex[0]);
                    _listItemIndex.RemoveAt(0);
                }
                else
                {
                    newListOneItem.Add(-1);
                }
            }

            //thêm các phần tử -2 để ko hỏng logic khi add empty Item
            int countCurrent = _listItemIndex.Count;
            for(int i = 0; i < count; i++)
            {
                if (i > countCurrent) _listItemIndex.Add(-2);
            }
            //thêm các phần tử rỗng
            _listItemIndex = AddEmptySpaceToList(_listItemIndex);

            //for(int i=0;i<_listItemIndex.Count;i++) 
            //{
            //    if (_listItemIndex[i] ==-2) _listItemIndex[i] =-1;
            //}

            //loại bỏ các phần tử thêm vào cho khỏi hỏng logic
            _listItemIndex.RemoveAll(item => item == -2);
            //trả lại các phần tử lên đầu tiên
            _listItemIndex.InsertRange(0, newListOneItem);
            _listCreate.Add(_listItemIndex.ToList());
            _listItemIndex.Clear();
        }
    }

    private void AddEmptyAndSplitDestroyMode(int count)
    {
        //lấy ra 1 list có số phần tử bằng với tổng số phần tử ở layer 0
        List<int> listSurface = _listItemIndex.GetRange(0, count - _countSpace);
        _listItemIndex.RemoveRange(0, listSurface.Count);
        //xáo trộn vị trí
        listSurface = Utils.Shuffle(listSurface);
        //lấy ra 1 list có số phần tử bằng với các layer đặc biệt (có 1 phần tử)
        List<int> listOneItem = new List<int>();
        for (int i = 0; i < _listIndexHasOneItem.Count; i++)
        {
            listOneItem.Add(listSurface[0]);
            listSurface.RemoveAt(0);
        }
        //thêm các phần tử rỗng vào list
        listSurface = AddEmptySpaceToList(listSurface);
        //trả lại phần tử vừa lấy ra cho các layer đặc biệt (xếp ở đầu list)
        listSurface.InsertRange(0, listOneItem);

        _listCreate.Add(listSurface);
        //Debug.Log("Debug list surface: " + DebugList(listSurface));
    }

    #endregion

    #region TUTORIAL
    private void ShowTutorialSlot(bool isShow)
    {
        SlotController[] slots = _allSlotController.GetSlots();

        foreach (var slot in slots)
        {
            slot.gameObject.SetActive(!isShow);
        }

        SlotController[] slotTutorial = _tutorialSlot;
        for (int i = 0; i < slotTutorial.Length; i++)
        {
            slotTutorial[i].gameObject.SetActive(isShow);
        }
    }

    private void HandleSpawnTutorial()
    {
        var data = Resources.Load<TextAsset>("TurorialLevelData");
        _levelData = JsonUtility.FromJson<LevelData>(data.text);
        MyGame.Instance.SetUpFromConfig(_levelData.MoveSpeed, _levelData.MoveSmooth);
        //init slot
        _slots = _tutorialSlot;

        //create list item 
        CreateListItemIdTutorial();

        //fill item to slot
        InitItemToSlotTutorial();
    }

    private void CreateListItemIdTutorial()
    {
        _listItemIndex.Clear();

        _listItemIndex.Add(1);
        _listItemIndex.Add(1);
        _listItemIndex.Add(0);
        _listItemIndex.Add(0);
        _listItemIndex.Add(2);
        _listItemIndex.Add(2);
        _listItemIndex.Add(2);
        _listItemIndex.Add(1);
        _listItemIndex.Add(0);

        _listCreate.Clear();

        _listCreate.Add(_listItemIndex.ToList());
        _listItemIndex.Clear();
    }

    private void InitItemToSlotTutorial()
    {
        for(int i=0;i< _slots.Length; i++)
        {
            _slots[i].SetUp(_levelData.SlotData[i]);
        }

        InitItemToSlot();
    }
    #endregion

    public float GetTimeAnim()
    {
        return _timeAnim;
    }

    public SlotController[] GetAllSlot()
    {
        return _slots;
    }

    public bool GetPlayMode()
    {
        return _levelData.IsDestroyMode;
    }

    public List<SlotController> GetAllLockSlot()
    {
        List<SlotController> newList= new List<SlotController>();
        for(int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].IsLocked()) newList.Add(_slots[i]);
        }
        return newList;
    }
}

public class MySpawn : SingletonMonoBehaviour<SpawnManager> { }


public class LevelData
{
    public bool IsDestroyMode;
    public int CountSpace;
    public int TotalIdSpawn;
    public int Level;
    public float MoveSpeed;
    public float MoveSmooth;
    public float TimePlay;
    public SlotData[] SlotData;

    //public LevelData(int countSpace, int totalIdSpawn, int level, float moveSpeed, float moveSmooth, List<SlotData> slotData)
    //{
    //    CountSpace = countSpace;
    //    TotalIdSpawn = totalIdSpawn;
    //    Level = level;
    //    MoveSpeed = moveSpeed;
    //    MoveSmooth = moveSmooth;
    //    SlotData = slotData.ToArray();
    //}
}

[System.Serializable]
public class SlotData
{
    public int id;
    public bool isSpecial;
    public bool isLocked;
    public int turnCount;
    public MOVE_SLOT_TYPE moveType;

    public SlotData(int id, SlotGameDesign slot)
    {
        this.id = id;
        this.isSpecial = slot.Options[0];
        this.isLocked = slot.Options[1];
        this.turnCount = slot.CountLock;
        this.moveType = (MOVE_SLOT_TYPE)slot.MoveType;
    }
}

[System.Serializable]
public enum MOVE_SLOT_TYPE
{
    STAY,
    MOVE_RIGHT,
    MOVE_LEFT,
    MOVE_UP,
    MOVE_DOWN
}

[System.Serializable]
public class SlotGameDesign
{
    public int id;
    public bool IsActive = false;
    public bool[] Options = new bool[2] { false, false };
    public int CountLock;
    public int MoveType;
    public string[] MoveOptions = new string[] { MOVE_SLOT_TYPE.STAY.ToString(), 
                                                    MOVE_SLOT_TYPE.MOVE_RIGHT.ToString(), 
                                                    MOVE_SLOT_TYPE.MOVE_LEFT.ToString(), 
                                                    MOVE_SLOT_TYPE.MOVE_UP.ToString(), 
                                                    MOVE_SLOT_TYPE.MOVE_DOWN.ToString() };
}

[System.Serializable]
public class DestroyModeSlotList
{
    public List<CanBeDestroySlot> ListSlot;
}
