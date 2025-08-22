using System.Threading;
using Ape.Runtime.GameAsset.AssetHandleReleasers;
using Ape.Runtime.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Ape.Runtime.GameAsset
{
    public partial class GameAssetManager
    {
        private T LoadAddressables<T>(string path, GameObject owner = null) where T : Object
        {
            if (!ExistsFromAddressables<T>(path))
            {
                return null;
            }
            
            var handle = Addressables.LoadAssetAsync<T>(path);
            if (!handle.IsValid())
                return null;

            T resource = handle.WaitForCompletion();

            if (owner)
            {
                owner.GetOrAddComponent<OwnerAssetHandleReleaser>().
                    AddHandle(new GameAssetHandle(handle));
            }
            else
            {
                GlobalHandles.Add(new GameAssetHandle(handle));
            }

            return resource;
        }
        
        private async UniTask<T> LoadAssetFromAddressablesAsync<T>(string path, GameObject owner = null, CancellationToken cancellationToken = default) where T : Object
        {
            if (!await ExistsFromAddressablesAsync<T>(path))
            {
                return null;
            }
            
            var handle = Addressables.LoadAssetAsync<T>(path);
            if (!handle.IsValid())
                return null;

            try
            {
                await handle.ToUniTask(cancellationToken: cancellationToken);
                
                if (owner)
                {
                    owner.GetOrAddComponent<OwnerAssetHandleReleaser>().
                        AddHandle(new GameAssetHandle(handle));
                }
                else
                {
                    GlobalHandles.Add(new GameAssetHandle(handle));
                }
                
                return handle.Result;
            }
            catch
            {
                if (handle.IsValid())
                    handle.Release();

                throw;
            }
        }
        
        private GameObject InstantiateFromAddressables(string path)
        {
            if (!ExistsFromAddressables<GameObject>(path))
            {
                return null;
            }
            
            var handle = Addressables.InstantiateAsync(path);
            if (!handle.IsValid())
                return null;

            try
            {
                GameObject resource = handle.WaitForCompletion();

                resource.AddComponent<SelfAssetHandleReleaser>().SetHandle(new GameAssetHandle(handle));

                return resource;
            }
            catch
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
                
                throw;
            }
        }
        
        private async UniTask<GameObject> InstantiateAsyncFromAddressables(string path, CancellationToken cancellationToken = default)
        {
            if (!await ExistsFromAddressablesAsync<GameObject>(path))
            {
                return null;
            }
            
            var handle = Addressables.InstantiateAsync(path, trackHandle: false);
            if (!handle.IsValid())
                return null;

            try
            {
                await handle.ToUniTask(cancellationToken: cancellationToken);
                GameObject go = handle.Result;

                if (!go)
                {
                    go.AddComponent<SelfAssetHandleReleaser>().SetHandle(new GameAssetHandle(handle));
                }

                return go;
            }
            catch
            {
                if (handle.IsValid())
                    handle.Release();

                throw;
            }
        }

        private bool ExistsFromAddressables<T>(string path) where T : Object
        {
            var handle = Addressables.LoadResourceLocationsAsync(path, typeof(T));
            if (!handle.IsValid())
                return false;

            try
            {
                handle.WaitForCompletion();

                return handle.Status == AsyncOperationStatus.Succeeded &&
                         handle.Result != null &&
                         handle.Result.Count > 0;
            }
            finally
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
        }
        
        private async UniTask<bool> ExistsFromAddressablesAsync<T>(string path, CancellationToken cancellationToken = default) where T : Object
        {
            var handle = Addressables.LoadResourceLocationsAsync(path, typeof(T));
            if (!handle.IsValid())
                return false;

            try
            {
                await handle.ToUniTask(cancellationToken: cancellationToken);

                return handle.Status == AsyncOperationStatus.Succeeded &&
                       handle.Result != null &&
                       handle.Result.Count > 0;
            }
            finally
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
        }
    }
}
