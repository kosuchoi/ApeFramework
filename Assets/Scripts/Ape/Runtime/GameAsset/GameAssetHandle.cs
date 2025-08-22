using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Ape.Runtime.GameAsset
{
    /// <summary>
    /// Resources와 Addressables 자산을 통합 관리하기 위한 핸들 구조체입니다.
    /// 이 핸들 하나로 두 시스템의 자산에 모두 접근하고 해제할 수 있습니다.
    /// </summary>
    internal struct GameAssetHandle
    {
        private AsyncOperationHandle? _addressableHandle;
        private Object _resourceAsset;
        
        internal bool IsAddressable => _addressableHandle.HasValue;

        internal GameAssetHandle(AsyncOperationHandle handle)
        {
            _addressableHandle = handle;
            _resourceAsset = null;
        }

        internal GameAssetHandle(Object resourceAsset)
        {
            _resourceAsset = resourceAsset;
            _addressableHandle = null;
        }

        internal void Release()
        {
            if (_addressableHandle.HasValue)
            {
                if(_addressableHandle.Value.IsValid())
                    Addressables.Release(_addressableHandle.Value);
            }
            else if (_resourceAsset)
            {
                if (_resourceAsset is GameObject)
                {
                    // GameObject는 유니티 내부적으로 레퍼런스 카운트가 0이 되면 제거된다
                    _resourceAsset = null;
                }
                else
                {
                    Resources.UnloadAsset(_resourceAsset);
                }
            }
        }

        internal Object GetAsset()
        {
            if (_resourceAsset != null)
            {
                return _resourceAsset;
            }

            if (_addressableHandle.HasValue && _addressableHandle.Value.IsValid())
            {
                return _addressableHandle.Value.Result as Object;
            }

            return null;
        }
    }
}
