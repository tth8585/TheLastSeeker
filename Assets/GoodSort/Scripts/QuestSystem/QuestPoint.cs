using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPoint : MonoBehaviour
{
    [SerializeField] bool _startPoint = false;
    [SerializeField] bool _endPoint = false;

    public bool StartPoint { get { return _startPoint; } }
    public bool EndPoint { get { return _endPoint; } }
}
