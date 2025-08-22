using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ape.Runtime.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Ape.Runtime.GameScene
{
    /// <summary>
    /// 게임 씬(Builtin, Addressables)을 통합적으로 관리하는 싱글톤 클래스입니다.
    /// 여러 씬 작업 요청을 순차적으로 처리하여 안정적인 씬 로드 및 언로드를 보장합니다.
    /// </summary>
    public class GameSceneManager : Singleton<GameSceneManager>
    {
        internal Dictionary<string, GameSceneHandle> Handles { get; } = new();

        private readonly Queue<UniTaskCompletionSource<bool>> _operationQueue = new();
        private bool _isOperationRunning;

        /// <summary>
        /// 다음 씬 작업을 시작하기 전에 자신의 차례를 비동기적으로 기다립니다.
        /// 이미 다른 작업이 진행 중인 경우, 큐에 대기 작업을 추가하고 완료될 때까지 기다립니다.
        /// </summary>
        private async UniTask WaitForTurnAsync(CancellationToken cancellationToken)
        {
            // 현재 다른 작업이 진행 중인지 확인
            if (_isOperationRunning)
            {
                var tcs = new UniTaskCompletionSource<bool>();
                _operationQueue.Enqueue(tcs);
                // 큐에서 대기하며, 외부에서 취소 요청이 오면 대기를 중단할 수 있습니다.
                await tcs.Task.AttachExternalCancellation(cancellationToken);
            }
            
            // 자신의 차례가 되면, 진행 중 플래그를 활성화합니다.
            _isOperationRunning = true;
        }

        /// <summary>
        /// 현재 씬 작업이 완료된 후, 대기 중인 다음 작업이 있다면 실행 신호를 보냅니다.
        /// 대기 중인 작업이 없으면, 진행 중 상태를 해제합니다.
        /// </summary>
        private void ReleaseTurn()
        {
            if (_operationQueue.Count > 0)
            {
                // 큐에서 다음 대기 작업을 꺼내 실행 신호를 보냅니다.
                var nextTcs = _operationQueue.Dequeue();
                nextTcs.TrySetResult(true);
            }
            else
            {
                // 대기열이 비어있으면 진행 중 상태를 완전히 해제합니다.
                _isOperationRunning = false;
            }
        }

        /// <summary>
        /// 관리 중인 모든 씬을 비동기적으로 언로드합니다.
        /// </summary>
        public async UniTask UnloadAllAsync(CancellationToken cancellationToken = default)
        {
            await WaitForTurnAsync(cancellationToken);
            try
            {
                var pathsToUnload = Handles.Keys.ToList();
                foreach (var path in pathsToUnload)
                {
                    await UnloadInternalAsync(path, cancellationToken);
                }
            }
            finally
            {
                ReleaseTurn();
            }
        }
        
        /// <summary>
        /// 지정된 경로의 씬을 비동기적으로 추가(additive) 로드합니다.
        /// </summary>
        public async UniTask<bool> LoadAdditiveAsync(string path, Action<float> onProgress = null, CancellationToken cancellationToken = default)
        {
            await WaitForTurnAsync(cancellationToken);
            try
            {
                if (Handles.ContainsKey(path))
                {
                    return false;
                }

                if (ExistsBuiltinScenes(path))
                {
                    return await LoadAdditiveBuiltinScenesAsync(path, onProgress, cancellationToken);
                }
            
                return await LoadAdditiveAddressablesAsync(path, onProgress, cancellationToken);
            }
            finally
            {
                ReleaseTurn();
            }
        }
        
        /// <summary>
        /// 지정된 경로의 씬을 비동기적으로 언로드합니다.
        /// </summary>
        public async UniTask<bool> UnloadAsync(string path, CancellationToken cancellationToken = default)
        {
            await WaitForTurnAsync(cancellationToken);
            try
            {
                return await UnloadInternalAsync(path, cancellationToken);
            }
            finally
            {
                ReleaseTurn();
            }
        }
        
        private async UniTask<bool> LoadAdditiveBuiltinScenesAsync(string path, Action<float> onProgress, CancellationToken cancellationToken)
        {
            var loadRequest = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
            if (loadRequest == null) return false;
            
            try
            {
                Handles.Add(path, new GameSceneHandle(loadRequest));

                if (onProgress == null)
                {
                    await loadRequest.ToUniTask(cancellationToken: cancellationToken);
                }
                else
                {
                    while (!loadRequest.isDone)
                    {
                        onProgress.Invoke(loadRequest.progress);

                        await UniTask.Yield(cancellationToken);
                    }
                    
                    onProgress.Invoke(1f);
                }

                return loadRequest.isDone;
            }
            catch
            {
                if (Handles.ContainsKey(path))
                    Handles.Remove(path);

                throw;
            }
        }
        
        private async UniTask<bool> LoadAdditiveAddressablesAsync(string path, Action<float> onProgress, CancellationToken cancellationToken)
        {
            if (!await ExistsAddressablesAsync(path, cancellationToken))
            {
                Debug.LogError($"[GameScenes] Addressable scene not found at path: {path}");
                return false;
            }

            var handle = Addressables.LoadSceneAsync(path, LoadSceneMode.Additive);
            if (!handle.IsValid()) return false;
            
            try
            {
                Handles.Add(path, new GameSceneHandle(handle));

                while (handle.IsDone == false)
                {
                    onProgress?.Invoke(handle.PercentComplete);

                    if (handle.IsValid() == false)
                        return false;

                    if (handle.Status == AsyncOperationStatus.Failed)
                        return false;

                    await UniTask.Yield(cancellationToken);
                }

                onProgress?.Invoke(1f);

                return handle.Status == AsyncOperationStatus.Succeeded;
            }
            catch
            {
                if (handle.IsValid())
                    Addressables.Release(handle);

                if (Handles.ContainsKey(path))
                    Handles.Remove(path);

                throw;
            }
        }
        
        private async UniTask<bool> UnloadInternalAsync(string path, CancellationToken cancellationToken)
        {
            if (!Handles.TryGetValue(path, out var handle)) return false;

            if (handle.BuiltinHandle != null)
            {
                await SceneManager.UnloadSceneAsync(path).ToUniTask(cancellationToken: cancellationToken);
            }
            else if (handle.AddressableHandle.HasValue && handle.AddressableHandle.Value.IsValid())
            {
                AsyncOperationHandle<SceneInstance> unloadHandle = default; 
                
                try
                {
                    unloadHandle = Addressables.UnloadSceneAsync(handle.AddressableHandle.Value);
                    while (unloadHandle.IsDone == false)
                    {
                        if (unloadHandle.IsValid() == false)
                            return false;

                        if (unloadHandle.Status == AsyncOperationStatus.Failed)
                            return false;

                        await UniTask.Yield(cancellationToken);
                    }
                }
                finally
                {
                    if (unloadHandle.IsValid())
                        Addressables.Release(unloadHandle);
                }
            }
            
            Handles.Remove(path);
            return true;
        }
        
        private bool ExistsBuiltinScenes(string path)
        {
            return Application.CanStreamedLevelBeLoaded(path);
        }
        
        private async UniTask<bool> ExistsAddressablesAsync(string path, CancellationToken cancellationToken)
        {
            var handle = Addressables.LoadResourceLocationsAsync(path, typeof(SceneInstance));
            if (!handle.IsValid()) return false;

            try
            {
                await handle.ToUniTask(cancellationToken: cancellationToken);
                return handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null && handle.Result.Count > 0;
            }
            finally
            {
                if (handle.IsValid()) Addressables.Release(handle);
            }
        }
    }
}