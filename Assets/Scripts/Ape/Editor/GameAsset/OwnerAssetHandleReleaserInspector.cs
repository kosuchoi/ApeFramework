using Ape.Runtime.GameAsset.AssetHandleReleasers;
using UnityEditor;
using UnityEngine;

namespace Ape.Editor.GameAsset
{
    [CustomEditor(typeof(OwnerAssetHandleReleaser))]
    public class OwnerAssetHandleReleaserInspector : UnityEditor.Editor
    {
        private bool _showHandles = false;

        public override void OnInspectorGUI()
        {
            // 기본 인스펙터 UI를 그립니다. (예: 스크립트 참조 필드)
            DrawDefaultInspector();

            // 타겟 컴포넌트를 가져옵니다.
            OwnerAssetHandleReleaser releaser = (OwnerAssetHandleReleaser)target;

            EditorGUILayout.Space();

            // 핸들 리스트를 토글할 수 있는Foldout을 만듭니다.
            _showHandles = EditorGUILayout.Foldout(_showHandles, "Managed Assets");

            if (_showHandles)
            {
                var handles = releaser.Handles;

                if (releaser.Handles == null || handles.Count == 0)
                {
                    EditorGUILayout.HelpBox("No assets are currently being managed.", MessageType.Info);
                }
                else
                {
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < handles.Count; i++)
                    {
                        // 핸들 객체 내부의 에셋 정보를 가져옵니다.
                        // (주의: GameAssetsHandle의 필드를 public 또는 [SerializeField]로 바꿔야 함)
                        Object asset = handles[i].GetAsset();

                        EditorGUILayout.ObjectField($"Handle {i}", asset, typeof(Object), true);
                    }

                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}