using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffectPool : MonoBehaviour
{
    [SerializeField] private GameObject _starObj;
    private List<GameObject> _starsPool;
    private List<GameObject> _inUsingStars;

    private void Start()
    {
        _starsPool= new List<GameObject>();
        _inUsingStars= new List<GameObject>();
    }

    public GameObject GetObject(Transform parent = null)
    {
        lock(_starsPool)
        {
            if(_starsPool.Count > 0)
            {
                var star = _starsPool[0];
                _starsPool.RemoveAt(0);
                _inUsingStars.Add(star);

                if (parent != null)
                    star.transform.parent = parent;
                else
                    star.transform.parent = this.gameObject.transform;

                star.gameObject.SetActive(true);
                return star;
            }
            else
            {
                var star = Instantiate(_starObj);
                if (parent != null)
                    star.transform.parent = parent;
                else
                    star.transform.parent = this.gameObject.transform;

                _inUsingStars.Add(star);
                return star;
            }
        }
    }

    public void ReturnStarToPool(GameObject star)
    {
        lock(_starsPool)
        {
            if (_inUsingStars.Contains(star))
            {
                _inUsingStars.Remove(star);
                _starsPool.Add(star);

                star.transform.parent = this.gameObject.transform;
                star.SetActive(false);
            }
        }
    }

    public void ReleaseAllInUse()
    {
        lock(_starsPool)
        {
            _starsPool.AddRange(_inUsingStars);
            _inUsingStars.Clear();
        }
    }

    public void ReleasePool()
    {
        lock(_starsPool)
        {
            foreach(var obj in _starsPool)
                Destroy(obj);
            _starsPool.Clear();
        }
    }
}
