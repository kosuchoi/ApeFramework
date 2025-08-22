using Ape.Runtime;
using Ape.Runtime.GameAsset;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ape.Samples
{
    /// <summary>
    /// GameAssets 클래스의 사용법을 보여주는 샘플 스크립트입니다.
    /// Resources와 Addressables를 GameAssets의 통합된 인터페이스를 통해
    /// 동기 및 비동기 방식으로 로드하고 인스턴스화하는 예제를 포함합니다.
    /// </summary>
    public class GameAssetSample : MonoBehaviour
    {
        /// <summary>
        /// Resources 폴더에 있는 Material을 동기적으로 로드합니다.
        /// Owner가 지정되지 않았으므로, 로드된 자산은 GlobalHandles에 등록됩니다.
        /// </summary>
        public void Load_Resources()
        {
            GameAssetManager.Instance.Load<Material>("ResourcesMaterial");
        }

        /// <summary>
        /// Resources 폴더에 있는 Material을 비동기적으로 로드합니다.
        /// Owner로 이 게임 오브젝트를 지정하여, 오브젝트 파괴 시 자산이 자동으로 해제됩니다.
        /// </summary>
        public void LoadAsync_Resources()
        {
            GameAssetManager.Instance.LoadAsync<Material>("ResourcesMaterial", gameObject).Forget();
        }

        /// <summary>
        /// Addressables로 등록된 Material을 동기적으로 로드합니다.
        /// Owner로 이 게임 오브젝트를 지정하여, 오브젝트 파괴 시 자산이 자동으로 해제됩니다.
        /// </summary>
        public void Load_Addressables()
        {
            GameAssetManager.Instance.Load<Material>("Assets/ApeSample/GameAsset/Addressables/AddressableMaterial.mat", gameObject);
        }

        /// <summary>
        /// Addressables로 등록된 Material을 비동기적으로 로드합니다.
        /// Owner가 지정되지 않았으므로, 로드된 자산은 GlobalHandles에 등록됩니다.
        /// </summary>
        public void LoadAsync_Addressables()
        {
            GameAssetManager.Instance.LoadAsync<Material>("Assets/ApeSample/GameAsset/Addressables/AddressableMaterial.mat").Forget();
        }
        
        /// <summary>
        /// Resources 폴더에 있는 프리팹을 동기적으로 인스턴스화합니다.
        /// 이 인스턴스에 SelfAssetHandleReleaser 컴포넌트가 자동으로 추가됩니다.
        /// </summary>
        public void Instantiate_Resources()
        {
            GameAssetManager.Instance.Instantiate("ResourcesPrefab");
        }

        /// <summary>
        /// Resources 폴더에 있는 프리팹을 비동기적으로 인스턴스화합니다.
        /// 이 인스턴스에 SelfAssetHandleReleaser 컴포넌트가 자동으로 추가됩니다.
        /// </summary>
        public void InstantiateAsync_Resources()
        {
            GameAssetManager.Instance.InstantiateAsync("ResourcesPrefab").Forget();
        }

        /// <summary>
        /// Addressables로 등록된 프리팹을 동기적으로 인스턴스화합니다.
        /// 이 인스턴스에 SelfAssetHandleReleaser 컴포넌트가 자동으로 추가됩니다.
        /// </summary>
        public void Instantiate_Addressables()
        {
            GameAssetManager.Instance.Instantiate("Assets/ApeSample/GameAsset/Addressables/AddressablesPrefab.prefab");
        }

        /// <summary>
        /// Addressables로 등록된 프리팹을 비동기적으로 인스턴스화합니다.
        /// 이 인스턴스에 SelfAssetHandleReleaser 컴포넌트가 자동으로 추가됩니다.
        /// </summary>
        public void InstantiateAsync_Addressables()
        {
            GameAssetManager.Instance.Instantiate("Assets/ApeSample/GameAsset/Addressables/AddressablesPrefab.prefab");
        }

        /// <summary>
        /// GameAssets가 관리하는 모든 자산을 동기적으로 언로드합니다.
        /// </summary>
        public void UnloadAll()
        {
            GameAssetManager.Instance.UnloadAll();
        }

        /// <summary>
        /// GameAssets가 관리하는 모든 자산을 비동기적으로 언로드합니다.
        /// </summary>
        public void UnloadAllAsync()
        {   
            GameAssetManager.Instance.UnloadAllAsync().Forget();
        }
    }
}
