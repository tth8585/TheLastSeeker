using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TitlePopupController))]
public class TitlePopupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TitlePopupController controller = (TitlePopupController)target;
        controller._title=EditorGUILayout.TextField("Title:",controller._title);

        if (GUILayout.Button("Init"))
        {
            controller.SetText();
        }
    }
}
