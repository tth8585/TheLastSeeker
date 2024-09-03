using Imba.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCount : MonoBehaviour
{
    public TMP_Text FPSTxt;
    public float DeltaTime = 0f;

    void Update()
    {
        DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;
    }
    private void OnGUI()
    {
        float fps = 1.0f / DeltaTime;
        int fpsInt = Mathf.CeilToInt(fps);
        if (FPSTxt != null)
        {
            FPSTxt.text = fpsInt.ToString();
        }
        else
        {
            int width = Screen.width, height = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(0, 0, width, height * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = height * 2 / 100;
            style.normal.textColor = Color.white;
            string text = string.Format("{0:0.} FPS", fpsInt);
            GUI.Label(rect, text, style);
        }
    }
}
