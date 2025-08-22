using Ape.Runtime;
using Ape.Runtime.GameScene;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Ape.Samples
{
    /// <summary>
    /// GameScenes 클래스의 씬 로드 및 언로드 기능을 보여주는 샘플 스크립트입니다.
    /// 어드레서블 씬과 빌트인 씬을 통합된 인터페이스로 다루는 방법을 보여줍니다.
    /// </summary>
    public class GameSceneSample : MonoBehaviour
    {
        /// <summary>
        /// Addressables로 등록된 씬을 비동기적으로 추가(Additive) 로드합니다.
        /// </summary>
        public void LoadAdditiveAsync_Addressables()
        {
            GameSceneManager.Instance.LoadAdditiveAsync("Assets/ApeSample/GameScene/Addressables/AddressableScene.unity", (progress)=>
            {
                Debug.Log($"Progress: {progress}");
            }).Forget();
        }
        
        /// <summary>
        /// 빌드 설정(Build Settings)에 포함된 씬을 비동기적으로 추가(Additive) 로드합니다.
        /// </summary>
        public void LoadAdditiveAsync_BuiltinScene()
        {
            GameSceneManager.Instance.LoadAdditiveAsync("ApeSample/GameScene/BuiltinScenes/BuiltinScene", (progress)=>
            {
                Debug.Log($"Progress: {progress}");
            }).Forget();
        }

        /// <summary>
        /// GameScenes가 관리하는 모든 로드된 씬을 비동기적으로 언로드합니다.
        /// </summary>
        public void UnloadAllAsync()
        {   
            GameSceneManager.Instance.UnloadAllAsync().Forget();
        }
    }
}