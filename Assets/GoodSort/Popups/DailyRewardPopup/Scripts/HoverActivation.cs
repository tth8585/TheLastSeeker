using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverActivation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject _hoverObj;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hoverObj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverObj.SetActive(false);
    }
}
