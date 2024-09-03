using DG.Tweening;
using TTHUnityBase.Base.DesignPattern;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    [SerializeField] CameraData[] _camData;
    [SerializeField] Transform _middleCamObject;

    public void SetUpCamera(int type)
    {
        CameraData data = _camData[(int)type];
        transform.position = data.Position;
        transform.DORotate(data.Rotation, 0.1f);
        Camera.main.fieldOfView = data.Fov;
    }

    public Vector3 GetMiddleCamObjectPos()
    {
        return _middleCamObject.position;
    }
}

public class MyCam : SingletonMonoBehaviour<MainCameraController> { }

[System.Serializable]
public class CameraData
{
    public Vector3 Position;
    public Vector3 Rotation;
    public float Fov;
}
