using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Ape.Runtime.GameScene
{
    internal struct GameSceneHandle
    {
        internal AsyncOperationHandle<SceneInstance>? AddressableHandle { get; }
        internal AsyncOperation BuiltinHandle { get; }

        internal GameSceneHandle(AsyncOperationHandle<SceneInstance> handle)
        {
            AddressableHandle = handle;
            BuiltinHandle = null;
        }

        internal GameSceneHandle(AsyncOperation handle)
        {
            AddressableHandle = null;
            BuiltinHandle = handle;
        }
    }
}