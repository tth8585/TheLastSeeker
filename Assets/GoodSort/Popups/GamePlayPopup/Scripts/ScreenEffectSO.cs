using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScreenEffectContainer", menuName ="ScriptableObjects/ScreenEffect")]
public class ScreenEffectSO : ScriptableObject
{
    public List<ScreenEffect> ScreenEffects;
}

[Serializable]
public class ScreenEffect
{
    public OverlayEffectType EffectType;
    public Sprite EffectSprite;
}
