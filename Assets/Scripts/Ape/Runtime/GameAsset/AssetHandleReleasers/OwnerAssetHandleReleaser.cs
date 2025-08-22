using System.Collections.Generic;

namespace Ape.Runtime.GameAsset.AssetHandleReleasers
{
    /// <summary>
    /// 게임 오브젝트에 종속된 자산 핸들을 관리하고 해제하는 컴포넌트입니다.
    /// 이 컴포넌트가 부착된 게임 오브젝트가 파괴되면, 
    /// 자동으로 관리 중인 모든 자산 핸들이 해제됩니다.
    /// </summary>
    internal class OwnerAssetHandleReleaser : AssetHandleReleaserBase
    {
        internal List<GameAssetHandle> Handles { get; } = new();

        internal override int HandleCount => Handles.Count;

        internal void AddHandle(GameAssetHandle handle)
        {
            Handles.Add(handle);
        }

        internal override void ReleaseHandles()
        {
            foreach (var handle in Handles)
            {
                handle.Release();
            }
            Handles.Clear();
        }
    }
}
