using UnityEngine;

namespace Ape.Runtime.Util
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;
        private static bool _isQuitting = false;

        public static T Instance
        {
            get
            {
                if (_isQuitting)
                {
                    return null;
                }
                
                if (!_instance)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        return null;
                    }
                
                    _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
                
                    if (_instance)
                    {
                        return _instance;
                    }
#endif
                
                    GameObject managerObject = new GameObject($"{typeof(T).Name}({nameof(Singleton<T>)})");
                    _instance = managerObject.AddComponent<T>();
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            var go = gameObject;
            DontDestroyOnLoad(go);
        }
        
        protected virtual void OnDestroy()
        {
            // 에디터에서 플레이 모드를 종료하거나, 어플리케이션을 종료할 때
            // DontDestroyOnLoad 객체가 정리되지 않아 발생하는 경고를 방지합니다.
            if (!Application.isEditor || !gameObject.scene.isLoaded)
            {
                _isQuitting = true;
            }
        }
    }
}