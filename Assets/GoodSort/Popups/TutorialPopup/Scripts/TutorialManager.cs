using Imba.UI;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] Camera _playerCamera;
    [SerializeField] Canvas _canvasUI;
    [SerializeField] GameObject testObjFocus;

    public void ShowTutorialPopup(GameObject obj)
    {
        if (obj.GetComponent<RectTransform>())
        {
            ShowTutorialPopup(obj.GetComponent<RectTransform>());
        }
        else
        {
            ShowTutorialPopup(obj.GetComponent<Transform>());
        }
    }

    private void ShowTutorialPopup(RectTransform rect)
    {
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.TutorialPopup, rect);
    }

    private void ShowTutorialPopup(Transform obj)
    {
        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.TutorialPopup, CalculatePosScreen(obj));
    }

    public void HideTutorialPopup()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.TutorialPopup);
    }

    private Vector2 CalculatePosScreen(Transform obj)
    {
        Vector3 pos = _playerCamera.WorldToScreenPoint(obj.position);

        float h = Screen.height;
        float w = Screen.width;
        float x = pos.x - (w / 2);
        float y = pos.y - (h / 2);
        float s = _canvasUI.scaleFactor;

        return new Vector2(x, y) / s;
    }
}

public class MyTutorial : SingletonMonoBehaviour<TutorialManager> { }

