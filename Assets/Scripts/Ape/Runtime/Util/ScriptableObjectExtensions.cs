using UnityEngine;

namespace Ape.Runtime.Util
{
    public static class ScriptableObjectExtensions
    {
        public static void SafeDestroy(this ScriptableObject scriptableObject)
        {
            if (!scriptableObject)
            {
                return;
            }
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(scriptableObject);
                return;
            }
#endif
            Object.Destroy(scriptableObject);
        }
    }
}