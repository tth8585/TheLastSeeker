using UnityEngine;
using System.Collections;
namespace TTHUnityBase.Base.DesignPattern
{
    public abstract class SingletonMonoBehaviour<T> where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindFirstObjectByType<T>();
                }
                if (instance == null)
                {
                    Debug.LogError(typeof(T).Name + " == null");
                }
                return instance;
            }
        }

        void OnDestroy()
        {
            instance = null;
        }
    }
}
