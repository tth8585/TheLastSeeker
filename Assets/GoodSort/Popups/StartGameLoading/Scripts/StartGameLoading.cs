using DG.Tweening;
using Imba.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartGameLoading : MonoBehaviour
{
    [SerializeField] private TMP_Text _loadingTxt;
    [SerializeField] private float _textAnimSpeed = 0.5f; 

    [SerializeField] private Image _gameIcon;
    [SerializeField] private Image _loadingFillImg;

    public float totalTime = 1f;
    public float stopDelay = 1f;
    private float _currentTime = 0f;

    private int dotCount = 0;
    private bool _isOnLoading = true;

    void Start()
    {
        _gameIcon.transform.localScale = Vector3.one * 0.1f;
        _gameIcon.transform.DOScale(Vector3.one * 1f, 1f).OnComplete(() => 
        {
            StartCoroutine(OnLoading());
            StartCoroutine(UpdateLoadingText());
        });
    }

    private IEnumerator OnLoading()
    {
        while (_currentTime < totalTime * 0.65f)
        {
            _loadingFillImg.fillAmount = _currentTime / totalTime;
            yield return null;
            _currentTime += Time.deltaTime;
        }

        yield return new WaitForSeconds(stopDelay);

        while (_currentTime / totalTime < 1)
        {
            _loadingFillImg.fillAmount = _currentTime / totalTime;
            yield return null;
            _currentTime += Time.deltaTime;
        }

        _loadingFillImg.fillAmount = 1f;
        _isOnLoading = false;

        //UIManager.Instance.ViewManager.ShowView(UIViewName.MainView);
        this.gameObject.SetActive(false);
    }

    IEnumerator UpdateLoadingText()
    {
        while (_isOnLoading)
        {
            _loadingTxt.text = "Loading" + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4;

            yield return new WaitForSeconds(_textAnimSpeed);
        }
    }
}
