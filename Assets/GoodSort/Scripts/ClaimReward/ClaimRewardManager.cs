using DG.Tweening;
using System.Collections.Generic;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;
using UnityEngine.UI;

public enum REWARD_TYPE
{
    Token,
    Cash,
    Diamond,
    GSPassToken,
    Other,
}

public class ClaimRewardManager : MonoBehaviour
{
    [SerializeField] RectTransform _endPosCurency,_endPosSkin, _GSPassBtn;
    [SerializeField] GameObject _imagePrefab;
    [SerializeField] int poolSize = 3;
    [SerializeField] Color _colorOrigin;

    private List<Image> _imagePool = new List<Image>();
    private float _animDuration=1.5f;

    private void Start()
    {
        InitializePool();
    }

    public void Claim(Image icon, TweenCallback callback)
    {
        Image newImage = GetImageFromPool();
        newImage.sprite = icon.sprite;
        newImage.GetComponent<RectTransform>().sizeDelta = icon.GetComponent<RectTransform>().sizeDelta;
        newImage.transform.position = icon.transform.position;

        newImage.DOFade(0, _animDuration);
        newImage.transform.DOMoveY(newImage.transform.position.y+500f, _animDuration).OnComplete(() =>
        {
            ReturnImageToPool(newImage);
            callback?.Invoke();
        });
    }

    public void Claim(REWARD_TYPE type ,Image icon,TweenCallback callback)
    {
        Image newImage = GetImageFromPool();

        newImage.sprite = icon.sprite;
        newImage.GetComponent<RectTransform>().sizeDelta= icon.GetComponent<RectTransform>().sizeDelta;
        newImage.transform.position = icon.transform.position;

        Vector2 endPos = Vector2.zero;

        if (type == REWARD_TYPE.Token || type == REWARD_TYPE.Cash || type == REWARD_TYPE.Diamond)
        {
            endPos = _endPosCurency.position;
        }
        else
        {
            endPos = _endPosSkin.position;
        }

        newImage.transform.DOMove(endPos, _animDuration).OnComplete(() => 
        {
            ReturnImageToPool(newImage);
             callback?.Invoke();
        });
    }

    public void Claim(REWARD_TYPE type, Sprite icon, Vector3 startPos, Vector2 customSize,TweenCallback callback)
    {
        Image newImage = GetImageFromPool();

        newImage.sprite = icon;
        newImage.GetComponent<RectTransform>().sizeDelta = customSize;
        newImage.transform.position = startPos;

        Vector2 endPos = Vector2.zero;

        switch (type)
        {
            case REWARD_TYPE.Token:
            case REWARD_TYPE.Cash:
            case REWARD_TYPE.Diamond:
                endPos = _endPosCurency.position;
                break;
            case REWARD_TYPE.GSPassToken:
                endPos = _GSPassBtn.position;
                break;
            default:
                endPos = _endPosSkin.position;
                break;
        }

        newImage.transform.DOMove(endPos, _animDuration).OnComplete(() =>
        {
            ReturnImageToPool(newImage);
            callback?.Invoke();
        });
    }

    #region POOLING OBJECT
    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject imageObject = Instantiate(_imagePrefab, transform);
            Image image = imageObject.GetComponent<Image>();
            if (image != null)
            {
                image.gameObject.SetActive(false);
                _imagePool.Add(image);
            }
        }
    }
    public Image GetImageFromPool()
    {
        foreach (Image image in _imagePool)
        {
            if (!image.gameObject.activeInHierarchy)
            {
                image.gameObject.SetActive(true);
                return image;
            }
        }

        // If no inactive image found in the pool, create a new one
        GameObject newImageObject = Instantiate(_imagePrefab, transform);
        Image newImage = newImageObject.GetComponent<Image>();
        if (newImage != null)
        {
            _imagePool.Add(newImage);
            return newImage;
        }

        return null;
    }

    public void ReturnImageToPool(Image image)
    {
        image.color = _colorOrigin;
        image.gameObject.SetActive(false);
    }
    #endregion
}

public class MyClaimReward : SingletonMonoBehaviour<ClaimRewardManager> { }
