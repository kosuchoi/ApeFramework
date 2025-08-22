using System.Threading;
using Ape.Runtime;
using Ape.Runtime.GlobalCanceller;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ape.Samples
{
    /// <summary>
    /// GlobalCanceller 클래스의 사용법을 보여주는 샘플 스크립트입니다.
    /// 전역 취소와 로컬(오브젝트) 취소를 결합하여 비동기 작업을 제어하는 방법을 보여줍니다.
    /// </summary>
    public class GlobalCancellerSample : MonoBehaviour
    {
        /// <summary>
        /// GlobalCanceller의 전역 취소 토큰을 사용하여 로그 루프를 시작합니다.
        /// 이 루프는 `CancelAll()` 함수가 호출될 때 종료됩니다.
        /// </summary>
        public void StartGlobalLogLoop()
        {
            LogLoopAsync(GlobalCanceller.Instance.GetCancellationToken()).Forget();
        }

        /// <summary>
        /// GlobalCanceller의 전역 취소와 이 MonoBehaviour가 파괴될 때 발생하는 취소 이벤트를
        /// 결합한 토큰을 사용하여 로그 루프를 시작합니다.
        /// 이 루프는 `CancelAll()`이 호출되거나, 이 오브젝트가 파괴될 때 종료됩니다.
        /// </summary>
        public void StartLocalLogLoop()
        {
            LogLoopAsync(GlobalCanceller.Instance.GetLinkedToken(this.GetCancellationTokenOnDestroy())).Forget();
        }

        private async UniTask LogLoopAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                Debug.Log("Log");
                await UniTask.WaitForSeconds(1f, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// GlobalCanceller에 의해 관리되는 모든 전역 비동기 작업을 취소합니다.
        /// </summary>
        public void CancelAll()
        {   
            GlobalCanceller.Instance.CancelAll();
        }
    }
}