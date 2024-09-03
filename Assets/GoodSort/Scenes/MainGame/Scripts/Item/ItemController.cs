using DG.Tweening;
using Imba.UI;
using System;
using System.Collections;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] GameObject[] _visuals;
    [SerializeField] Transform _model;
    [SerializeField] ItemAnimation _animation;

    private int _index = -1;
    private float _timeAnim = 0f;
    private bool _isDoingAnim = false;
    private float _shakeAnimTime = 2f;
    private float _currentTimeAnim = 0f;

    private void OnEnable()
    {
        MyEvent.Instance.GameEventManager.onShuffleItem += DoAnimPrepareShuffle;
    }

    private void OnDisable()
    {
        if(MyEvent.Instance != null)
            MyEvent.Instance.GameEventManager.onShuffleItem -= DoAnimPrepareShuffle;
    }

    public void InitData(int index)
    {
        if(index==-1) index = 0;
        
        SetIndex(index);

        SetVisual();
    }

    private void ShiningAnim()
    {
        _model.transform.DOShakeScale(0.5f,0.3f);
    }

    public void InitRespawnData(int index)
    {
        if (index == -1) index = 0;

        SetIndex(index);

        StartCoroutine(COSetVisual());
    }

    IEnumerator COSetVisual()
    {
        while (_isDoingAnim)
        {
            yield return null; // Wait for the next frame
        }
        _model.transform.localScale = Vector3.one;
        _model.transform.localPosition = Vector3.zero;
        GetComponent<BoxCollider>().enabled = true;
        SetVisual();
    }

    private void SetVisual()
    {
        for (int i = 0; i < _visuals.Length; i++)
        {
            if (i == _index)
            {
                _visuals[i].SetActive(true);
            }
            else
            {
                _visuals[i].SetActive(false);
            }
        }

        if (_index >= _visuals.Length)
        {
            Debug.LogError("index is larger than visual object. set default visual");
            _visuals[1].SetActive(true);
        }
    }

    public void SetIndex(int index)
    {
        _index= index;
    }

    public int GetIndex()
    {
        return _index;
    }

    public bool IsEmptyObject()
    {
        if (_index == 0) return true;
        return false;
    }

    public void EnableDragAndDrop()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    public void DoAnimAndDestroyWhenUseItem(bool isFinalObj)
    {
        if (_timeAnim == 0f) _timeAnim = MySpawn.Instance.GetTimeAnim();
        SetIndex(0);
        GetComponent<BoxCollider>().enabled = false;
        _isDoingAnim = true;

        Vector3 finalPos = MyCam.Instance.GetMiddleCamObjectPos();
        //Debug.DrawLine(_model.transform.position, finalPos, Color.yellow, 10f);
        // anim move over before move to final position
      
        //Debug.DrawLine(_model.transform.position, _model.transform.position + animDir.normalized * 0.2f, Color.red, 10f);

        _model.DOMoveZ(-.5f, _timeAnim / 3f).OnComplete(() =>
        {
            Vector3 animDir = new Vector3(_model.position.x - finalPos.x, _model.position.y - finalPos.y, _model.position.z);
            Vector3 animPos = _model.transform.position + animDir.normalized * 0.2f;

            _model.DOMove(animPos, _timeAnim).OnComplete(() =>
            {
                _model.DOMove(finalPos, _timeAnim).OnComplete(() =>
                {
                    _model.DOScale(Vector3.zero, _timeAnim * 0.5f).OnComplete(() =>
                    {
                        for (int i = 0; i < _visuals.Length; i++)
                        {
                            _visuals[i].SetActive(false);
                        }
                        _isDoingAnim = false;
                        if (isFinalObj)
                        {
                            MyEvent.Instance.GameEventManager.DoneAnimHitItem();
                            MyEvent.Instance.GameEventManager.MatchItem(finalPos); //get position in this
                        }
                    });
                });
            });

        });
    }

    public void DoAnimAndDestroy()
    {
        SetIndex(0);
        GetComponent<BoxCollider>().enabled = false;
        _isDoingAnim = true;

        _animation.DoAnimAndDestroy(() => 
        {
            for (int i = 0; i < _visuals.Length; i++)
            {
                _visuals[i].SetActive(false);
            }
            _isDoingAnim = false;
        });

        //_model.transform.DOShakeScale(0.5f, .5f).OnComplete(() =>
        //{
        //    _model.DOScale(Vector3.zero, _timeAnim * 0.5f).OnComplete(() =>
        //    {
        //        for (int i = 0; i < _visuals.Length; i++)
        //        {
        //            _visuals[i].SetActive(false);
        //        }
        //        _isDoingAnim = false;
        //    });
        //});
    }

    public void DoAnimPrepareShuffle()
    {
        if (_timeAnim == 0f) _timeAnim = MySpawn.Instance.GetTimeAnim();
        //SetIndex(0);
        GetComponent<BoxCollider>().enabled = false;
        _isDoingAnim = true;

        Vector3 preparePos = MyCam.Instance.GetMiddleCamObjectPos();

        _model.DOMoveZ(-.5f, _timeAnim / 3f).OnComplete(() =>
        {
            Vector3 animDir = new Vector3(_model.position.x - preparePos.x, _model.position.y - preparePos.y, _model.position.z);
            Vector3 animPos = _model.transform.position + animDir.normalized * 0.2f;

            _model.DOMove(animPos, _timeAnim).OnComplete(() =>
            {
                _model.DOMove(preparePos, _timeAnim).OnComplete(() =>
                {
                    for (int i = 0; i < _visuals.Length; i++)
                    {
                        _visuals[i].SetActive(false);
                    }
                    _isDoingAnim = false;
                    ShiningAnim();
                });
            });

        });
    }
}
