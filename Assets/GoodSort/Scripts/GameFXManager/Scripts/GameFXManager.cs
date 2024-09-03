using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;
using UnityEngine.UI;

public class GameFXManager : MonoBehaviour
{
    [SerializeField] ParticleSystem _magicFX;
    [SerializeField] ParticleSystem _magicExploFX;
    [SerializeField] ParticleSystem _magicItemFX;
    [SerializeField] ParticleSystem _unlockSlotFX;
    [SerializeField] ParticleSystem _boomFX;
    [SerializeField] ParticleSystem _boomExploFX;
    [SerializeField] Transform _timeFX;
    [SerializeField] List<ItemSlot> _listItemMagic = new();
    [SerializeField] float _itemMagicMoveDuration = .3f;

    [SerializeField] Ease _itemMagicMoveEase = Ease.OutQuad;

    [Header("Time FX")]
    [SerializeField] float _itemTimeMoveDuration = .3f;
    [SerializeField] float _timeIconUpDistance = 100;
    [SerializeField] float _timeIconScaleUp = .6f;
    [SerializeField] float _timeIconScaleUpDown = .1f;

    private ObjectPoolsManager _magicItemPaticalPools;
    private ObjectPoolsManager _magicItemExploPaticalPools;
    private void OnEnable()
    {
        _magicItemPaticalPools = new();
        _magicItemPaticalPools.InitPoolObjects(6, _magicItemFX.transform, this.transform);
        _magicItemExploPaticalPools = new();
        _magicItemExploPaticalPools.InitPoolObjects(6, _magicExploFX.transform, this.transform);
    }

    public void PlayTimeFX()
    {
        _timeFX.gameObject.SetActive(true);
        _timeFX.localScale = Vector3.one * _timeIconScaleUp;
        _timeFX.localPosition = new Vector3(0, _timeIconUpDistance-3, -1.2f);
        _timeFX.DOMove(_timeFX.position + Vector3.up * _timeIconUpDistance, _itemTimeMoveDuration).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            _timeFX.gameObject.SetActive(false);
        });
        _timeFX.DOScale(Vector3.one * _timeIconScaleUpDown, _itemTimeMoveDuration);
    }

    public void PlayMagicFX(TweenCallback callback)
    {
        _magicFX.Play();
        PlayItemFXOnMagic(callback);
    }

    public void PlayBoomFX()
    {
        _boomFX.Play();
        StartCoroutine(OnBoomExplo());
    }
    IEnumerator OnBoomExplo()
    {
        //wait to boom fx done and explo
        yield return new WaitForSeconds(.8f);
        _boomExploFX.Play();
    }

    public void SetItemToMagicFX(List<ItemSlot> listItem)
    {
        _listItemMagic = listItem;
    }

    public void PlayItemFXOnMagic(TweenCallback callback)
    {
        for(int i=0; i< _listItemMagic.Count; i++)
        {
            if(i== _listItemMagic.Count-1)
            {
                DoMagicFXOnItem(_magicItemPaticalPools.GetObject().transform, _listItemMagic[i].transform, callback);
            }
            else
            {
                DoMagicFXOnItem(_magicItemPaticalPools.GetObject().transform, _listItemMagic[i].transform);
            }
            
        }
    }
    private void DoMagicFXOnItem(Transform item, Transform end, TweenCallback callback= null)
    {
        Vector3 endPos = new Vector3(end.position.x, end.position.y, -1);
        item.position = _magicFX.transform.position;
        ParticleSystem exploFX = _magicItemExploPaticalPools.GetObject().GetComponent<ParticleSystem>();
        item.DOMove(endPos, _itemMagicMoveDuration).SetEase(_itemMagicMoveEase).OnComplete(()=>
        {
            _magicItemPaticalPools.ReturnObjToPools(item);
            exploFX.transform.position = item.position;
            exploFX.Play();
            StartCoroutine(_magicItemExploPaticalPools.DelayReturnObject(1f, exploFX.transform));
            //change visual of item when orb fly to
            end.GetComponent<ItemSlot>().ChangeVisualCache();
            callback?.Invoke();
        });
    }
    public void PlayUnLockSlotFxAtPos(Vector3 pos)
    {
        _unlockSlotFX.transform.position = new Vector3(pos.x,pos.y, -0.5f);
        _unlockSlotFX.Play();
    }
}
public class MyGameFx : SingletonMonoBehaviour<GameFXManager> { }


public class ObjectPoolsManager: MonoBehaviour
{
    private List<Transform> _itemPaticalPools = new();
    private int _initPoolsCount = 6;
    private Transform _poolObj;
    private Transform _poolParent;

    public void InitPoolObjects(int poolsCount, Transform objPool, Transform parent)
    {
        _initPoolsCount = poolsCount;
        _poolObj = objPool;
        _poolParent = parent;

        for (int i=0; i< _initPoolsCount; i++)
        {
            Transform obj = Instantiate(_poolObj,_poolParent);
            obj.gameObject.SetActive(false);
            _itemPaticalPools.Add(obj);
        }
    }

    public Transform GetObject()
    {
        Transform obj = _itemPaticalPools.Find(x => x.gameObject.activeSelf == false);
        if (obj == null)
        {
            obj = InitNewObj();
        }
        obj.gameObject.SetActive(true);
        return obj;
    }
    private Transform InitNewObj()
    {
        Transform obj = Instantiate(_poolObj, _poolParent);
        obj.gameObject.SetActive(false);
        _itemPaticalPools.Add(obj);
        return obj;
    }
    public IEnumerator DelayReturnObject(float delayTime, Transform obj)
    {
        yield return new WaitForSeconds(delayTime);
        ReturnObjToPools(obj);
    }
    public void ReturnObjToPools(Transform obj)
    {
        obj.transform.parent = _poolParent;
        obj.gameObject.SetActive(false);
    }
}
