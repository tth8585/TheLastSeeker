using TTHUnityBase.Base.DesignPattern;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasManager : MonoBehaviour
{
    [SerializeField] SpriteAtlas _commonAtlas;

    public Sprite GetCommonSprite(string nameSprite)
    {
        Sprite newSprite = _commonAtlas.GetSprite(nameSprite);

        if (newSprite)
        {
            return newSprite;
        }
        else
        {
            Debug.LogError("xx "+ _commonAtlas.name +" not contains sprite: "+nameSprite);
            return null;
        }
        
    }
}
public class MyAtlas : SingletonMonoBehaviour<AtlasManager> { }
