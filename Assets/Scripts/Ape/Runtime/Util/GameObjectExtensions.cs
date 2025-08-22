using UnityEngine;

namespace Ape.Runtime.Util
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            // GameObject에서 T 타입의 컴포넌트를 찾습니다.
            T component = gameObject.GetComponent<T>();

            // 컴포넌트가 없으면 새로 추가합니다.
            if (!component)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
        
        public static void SafeDestroy(this GameObject gameObject)
        {
            if (!gameObject)
            {
                return;
            }
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(gameObject);
                return;
            }
#endif
            Object.Destroy(gameObject);
        }
    }
}
