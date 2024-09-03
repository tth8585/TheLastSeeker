using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateUI : MonoBehaviour
{
    public RectTransform uiElement;
    public float rotationSpeed = 30f; // Adjust the speed as needed

    private void Reset()
    {
        uiElement = GetComponent<RectTransform>();
    }
    void Update()
    {
        // Rotate the UI element continuously around the Z-axis
        uiElement.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
