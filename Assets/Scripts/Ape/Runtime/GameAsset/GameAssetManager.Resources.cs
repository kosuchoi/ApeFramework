using System.Threading;
using Ape.Runtime.GameAsset.AssetHandleReleasers;
using Ape.Runtime.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ape.Runtime.GameAsset
{
    public partial class GameAssetManager
    {
        private T LoadResources<T>(string path, GameObject owner = null) where T : Object
        {
            T resource = Resources.Load<T>(path);
           
            if (!resource)
                return null;
            
            if (owner)
            {
                owner.GetOrAddComponent<OwnerAssetHandleReleaser>().
                    AddHandle(new GameAssetHandle(resource));
            }
            else
            {
                GlobalHandles.Add(new GameAssetHandle(resource));
            }

            return resource;
        }
        
        private async UniTask<T> LoadResourcesAsync<T>(string path, GameObject owner = null, CancellationToken cancellationToken = default) where T : Object
        {
            var loadRequest = Resources.LoadAsync<T>(path);
            if (loadRequest == null)
            {
                return null;
            }
            
            try
            {
                await loadRequest.ToUniTask(cancellationToken: cancellationToken);
                
                T resource = loadRequest.asset as T;

                if (!resource)
                    return null;
                
                if (owner)
                {
                    owner.GetOrAddComponent<OwnerAssetHandleReleaser>().
                        AddHandle(new GameAssetHandle(resource));
                }
                else
                {
                    GlobalHandles.Add(new GameAssetHandle(resource));
                }
                
                return resource;
            }
            catch
            {
                if (loadRequest.asset)
                    SafeUnloadAsset(loadRequest.asset);

                throw;
            }
        }
        
        private GameObject InstantiateResources(string path)
        {
            GameObject resource = Resources.Load<GameObject>(path);
            
            if (!resource)
                return null;
            
            GameObject instance = Object.Instantiate(resource);
            instance.AddComponent<SelfAssetHandleReleaser>().SetHandle(new GameAssetHandle(resource));

            return instance;
        }
        
        private async Awaitable<GameObject> InstantiateResourcesAsync(string path, CancellationToken cancellationToken = default)
        {
            var loadRequest = Resources.LoadAsync<GameObject>(path);
            if (loadRequest == null)
            {
                return null;
            }
            
            try
            {
                await loadRequest.ToUniTask(cancellationToken: cancellationToken);

                GameObject resource = loadRequest.asset as GameObject;

                if (!resource)
                    return null;
            
                var instantiateRequest = Object.InstantiateAsync(resource);
                
                await instantiateRequest;
                
                GameObject go = null;
                if (instantiateRequest.Result != null && instantiateRequest.Result.Length > 0 )
                    go = instantiateRequest.Result[0];
            
                if (go)
                    go.AddComponent<SelfAssetHandleReleaser>().SetHandle(new GameAssetHandle(resource));
              
                return go;
            }
            catch
            {
                if (loadRequest.asset)
                    SafeUnloadAsset(loadRequest.asset);

                throw;
            }
        }
        
        private bool ExistsResources<T>(string path) where T : Object
        {
            var resource = Resources.Load<T>(path);
            if (!resource)
                return false;

            SafeUnloadAsset(resource);

            return true;
        }
        
        private async Awaitable<bool> ExistsResourcesAsync<T>(string path, CancellationToken cancellationToken = default) where T : Object
        {
            var loadRequest = Resources.LoadAsync<T>(path);
            if (loadRequest == null)
            {
                return false;
            }
            
            try
            {
                await loadRequest.ToUniTask(cancellationToken: cancellationToken);

                T resource = loadRequest.asset as T;

                if (!resource)
                    return false;
                
                return true;
            }
            catch
            {
                if (loadRequest.asset)
                    SafeUnloadAsset(loadRequest.asset);

                throw;
            }
        }

        private void SafeUnloadAsset(Object assetToUnload)
        {
            if (assetToUnload && assetToUnload is GameObject == false)
            {
                Resources.UnloadAsset(assetToUnload);
            }
        }
    }
}
