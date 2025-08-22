using System.Collections.Generic;
using System.Threading;
using Ape.Runtime.GameAsset.AssetHandleReleasers;
using Ape.Runtime.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ape.Runtime.GameAsset
{
    /// <summary>
    /// **게임 내 모든 자산(Addressables, Resources)을 통합 관리하는 싱글톤 클래스.**
    /// 로드/언로드, 생성/삭제 등 자산 관리의 복잡성을 숨기고
    /// `UnityEngine.Resources`처럼 편리한 인터페이스를 제공합니다.
    /// </summary>
    public partial class GameAssetManager : Singleton<GameAsset.GameAssetManager>
    {
        internal List<GameAssetHandle> GlobalHandles { get; } = new();
        internal HashSet<AssetHandleReleaserBase> Releasers { get; } = new();

        /// <summary>
        /// 현재 관리 중인 모든 자산 핸들의 총 개수를 반환합니다.
        /// (전역 핸들 + 각 Releaser 컴포넌트의 핸들)
        /// </summary>
        public int HandleCount
        {
            get
            {
                int count = GlobalHandles.Count;
                
                foreach (var releaser in Releasers)
                {
                    count += releaser.HandleCount;
                }
                
                return count;
            }
        }

        /// <summary>
        /// 관리 중인 모든 자산(Global Handles, Releaser Handles)을 동기적으로 언로드합니다.
        /// </summary>
        public void UnloadAll()
        {
            foreach (var handle in GlobalHandles)
            {
                handle.Release();
            }
            GlobalHandles.Clear();

            foreach (var releaser in new HashSet<AssetHandleReleaserBase>(Releasers))
            {
                releaser.ReleaseHandles();
            }
        }

        /// <summary>
        /// 관리 중인 모든 자산(Global Handles, Releaser Handles)을 비동기적으로 언로드합니다.
        /// </summary>
        public async UniTask UnloadAllAsync(CancellationToken cancellationToken = default)
        {
            UnloadAll();
            
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// 지정된 경로의 자산을 동기적으로 로드합니다.
        /// </summary>
        public T Load<T>(string path, GameObject owner = null) where T : Object
        {
            T resource = LoadResources<T>(path, owner);
            
            if (resource)
                return resource;
            
            return LoadAddressables<T>(path, owner);
        }
        
        /// <summary>
        /// 지정된 경로의 자산을 비동기적으로 로드합니다.
        /// </summary>
        public async UniTask<T> LoadAsync<T>(string path, GameObject owner = null, CancellationToken cancellationToken = default) where T : Object
        {
            T resource = await LoadResourcesAsync<T>(path, owner, cancellationToken);

            if (resource)
                return resource;
            
            return await LoadAssetFromAddressablesAsync<T>(path, owner, cancellationToken);
        }
        
        /// <summary>
        /// 지정된 경로의 프리팹을 동기적으로 인스턴스화합니다.
        /// </summary>
        public GameObject Instantiate(string path)
        {
            GameObject instance = InstantiateResources(path);
            if (instance)
                return instance;

            return InstantiateFromAddressables(path);
        }
        
        /// <summary>
        /// 지정된 경로의 프리팹을 비동기적으로 인스턴스화합니다.
        /// </summary>
        public async UniTask<GameObject> InstantiateAsync(string path, CancellationToken cancellationToken = default)
        {
            GameObject go = await InstantiateResourcesAsync(path, cancellationToken);

            if (go)
                return go;
            
            return await InstantiateAsyncFromAddressables(path, cancellationToken);
        }

        /// <summary>
        /// 지정된 경로의 자산이 존재하는지 동기적으로 확인합니다.
        /// </summary>
        public bool Exists<T>(string path) where T : Object
        {
            if (ExistsResources<T>(path))
            {
                return true;
            }

            return ExistsFromAddressables<T>(path);
        }

        /// <summary>
        /// 지정된 경로의 자산이 존재하는지 비동기적으로 확인합니다.
        /// </summary>
        public async UniTask<bool> ExistsAsync<T>(string path, CancellationToken cancellationToken = default) where T : Object
        {
            if (await ExistsResourcesAsync<T>(path, cancellationToken))
            {
                return true;
            }

            return await ExistsFromAddressablesAsync<T>(path, cancellationToken);
        }
    }
}
