using System.Threading;
using Ape.Runtime.Util;

namespace Ape.Runtime.GlobalCanceller
{
    /// <summary>
    /// **전역 비동기 작업 취소를 관리하는 싱글톤 클래스입니다.**
    /// 게임 내 모든 비동기 작업에 대한 일괄 취소 기능을 제공합니다.
    /// </summary>
    public class GlobalCanceller : Singleton<GlobalCanceller>
    {
        private CancellationTokenSource _globalCancellationTokenSource = new();

        /// <summary>
        /// 모든 비동기 작업에 연결할 수 있는 전역 취소 토큰을 가져옵니다.
        /// 이 토큰은 `CancelAll()` 함수에 의해 취소됩니다.
        /// </summary>
        public CancellationToken GetCancellationToken()
        {
            return _globalCancellationTokenSource.Token;
        }

        /// <summary>
        /// 전역 취소 토큰과 외부에서 전달받은 취소 토큰을 연결한 새로운 토큰을 생성합니다.
        /// 이 토큰은 전역 취소 또는 외부 토큰 중 어느 하나라도 취소되면 함께 취소됩니다.
        /// </summary>
        public CancellationToken GetLinkedToken(CancellationToken externalToken)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(Instance._globalCancellationTokenSource.Token, externalToken).Token;
        }
        
        /// <summary>
        /// **관리 중인 모든 비동기 작업을 취소합니다.**
        /// 기존의 취소 토큰 소스를 취소하고, 새로운 소스로 교체하여 다음 작업들을 받을 준비를 합니다.
        /// </summary>
        public void CancelAll()
        {
            _globalCancellationTokenSource.Cancel();
            _globalCancellationTokenSource = new CancellationTokenSource();
        }
    }
}