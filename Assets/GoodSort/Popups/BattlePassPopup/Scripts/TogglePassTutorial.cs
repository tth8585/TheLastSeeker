using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePassTutorial : MonoBehaviour
{
    [SerializeField] GameObject Panel;
    public void TogglePanel()
    {
        Panel.SetActive(!Panel.activeSelf);
    }
}
