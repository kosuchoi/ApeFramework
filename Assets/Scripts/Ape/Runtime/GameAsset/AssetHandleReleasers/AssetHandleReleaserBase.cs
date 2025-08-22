using UnityEngine;

namespace Ape.Runtime.GameAsset.AssetHandleReleasers
{
    /// <summary>
    /// GameAssets에서 관리하는 자산(Asset)의 핸들을 해제하는 기본 추상 클래스입니다.
    /// 이 클래스를 상속받아, 게임 오브젝트의 생명주기에 맞춰 자동으로 자산을 해제하는 로직을 구현합니다.
    /// </summary>
    public abstract class AssetHandleReleaserBase : MonoBehaviour
    {
        internal abstract int HandleCount { get; }

        private void Awake()
        {
            GameAssetManager.Instance.Releasers.Add(this);
            
            hideFlags = HideFlags.HideAndDontSave;
        }

        private void OnDestroy()
        {
            if (GameAssetManager.Instance)
            {
                GameAssetManager.Instance.Releasers.Remove(this);
            }

            ReleaseHandles();
        }

        internal abstract void ReleaseHandles();
    }
}
