using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLevelController : MonoBehaviour
{
    public bool IsDestroyMode = false;
    public int CameraModeIndex = 0;
    public int CountSpace = 5;
    public int TotalIdSpawn = 6;
    public int Level=2;
    public float MoveSpeed = 1f;
    public float MoveSmooth = 1.05f;
    public float TimeForLevel = 30f;
    public Transform CameraTranform;

    public void SetUpMainCamera()
    {

    }
}
