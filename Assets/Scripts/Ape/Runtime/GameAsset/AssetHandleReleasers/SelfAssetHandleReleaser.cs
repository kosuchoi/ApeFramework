using Ape.Runtime.Util;

namespace Ape.Runtime.GameAsset.AssetHandleReleasers
{
    /// <summary>
    /// **단일 자산 핸들을 관리하고 해제하는 컴포넌트입니다.**
    /// 이 컴포넌트가 부착된 게임 오브젝트가 파괴될 때,
    /// 자신에게 할당된 하나의 자산 핸들을 자동으로 해제하고 게임 오브젝트도 함께 파괴합니다.
    /// 주로 `InstantiateAsync`로 생성된 프리팹 인스턴스에 사용됩니다.
    /// </summary>
    internal class SelfAssetHandleReleaser : AssetHandleReleaserBase
    {
        internal GameAssetHandle? Handle;

        internal override int HandleCount => Handle.HasValue ? 1 : 0;

        internal void SetHandle(GameAssetHandle handle)
        {
            if (Handle.HasValue)
            {
                Handle.Value.Release();
                Handle = null;
            }
            
            Handle = handle;
        }

        internal override void ReleaseHandles()
        {
            if (Handle.HasValue)
            {
                Handle.Value.Release();
                Handle = null;
            }
            
            // 어드레서블과 유사하게 동작하기 위해서 InstantiateFromResources로 로드된 인스턴스들도 파괴함
            var go = gameObject;
            if (go)
            {
                go.SafeDestroy();
            }
        }
    }
}
