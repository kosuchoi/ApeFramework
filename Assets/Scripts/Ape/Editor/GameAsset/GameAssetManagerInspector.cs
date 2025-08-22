using System.Collections.Generic;
using Ape.Runtime;
using Ape.Runtime.GameAsset;
using Ape.Runtime.GameAsset.AssetHandleReleasers;
using UnityEditor;
using UnityEngine;

namespace Ape.Editor.GameAssets
{
    [CustomEditor(typeof(Runtime.GameAsset.GameAssetManager))]
    public class GameAssetManagerInspector : UnityEditor.Editor
    {
        private Vector2 _scrollPosition;
        private List<GameAssetHandle> _globalHandles;
        private List<OwnerAssetHandleReleaser> _ownerReleasers;
        private List<SelfAssetHandleReleaser> _selfReleasers;

        private bool _showGlobalHandles = true;
        private bool _showOwnerHandles = true;
        private bool _showSelfHandles = true;
        
        // OnInspectorGUI() 함수가 호출될 때마다 핸들 정보를 갱신합니다.
        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Play Mode에서만 에셋 정보를 볼 수 있습니다.", MessageType.Info);
                return;
            }

            // 매번 GUI를 그리기 전에 데이터를 갱신합니다.
            RefreshHandles();

            EditorGUILayout.LabelField("Managed Assets Overview", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            try
            {
                DrawGlobalHandles();
                DrawOwnerHandles();
                DrawSelfHandles();
            }
            catch
            {
                // 예외 발생 시 리스트 초기화
                _globalHandles?.Clear();
                _ownerReleasers?.Clear();
                _selfReleasers?.Clear();
            }

            EditorGUILayout.EndScrollView();
            
            // 인스펙터 창이 매 프레임마다 갱신되도록 강제합니다.
            // Repaint()를 사용하면 OnInspectorGUI()를 다시 호출합니다.
            Repaint();
        }

        // 기존의 RefreshHandles() 함수 로직은 그대로 사용
        private void RefreshHandles()
        {
            // GameAssets.Instance가 null인 경우를 방어합니다.
            if (GameAssetManager.Instance == null)
            {
                _globalHandles = new List<GameAssetHandle>();
                _ownerReleasers = new List<OwnerAssetHandleReleaser>();
                _selfReleasers = new List<SelfAssetHandleReleaser>();
                return;
            }

            _globalHandles = new List<GameAssetHandle>(GameAssetManager.Instance.GlobalHandles);

            _ownerReleasers = new List<OwnerAssetHandleReleaser>();
            _selfReleasers = new List<SelfAssetHandleReleaser>();

            foreach (var instance in GameAssetManager.Instance.Releasers)
            {
                if (instance is OwnerAssetHandleReleaser ownerReleaser)
                {
                    _ownerReleasers.Add(ownerReleaser);
                }
                else if (instance is SelfAssetHandleReleaser selfReleaser)
                {
                    _selfReleasers.Add(selfReleaser);
                }
            }
        }
        
        // 기존의 Draw 함수들은 동일하게 유지
        private void DrawGlobalHandles()
        {
            _showGlobalHandles = EditorGUILayout.Foldout(_showGlobalHandles, $"Global Handles ({_globalHandles?.Count ?? 0})");
            if (_showGlobalHandles)
            {
                if (_globalHandles == null || _globalHandles.Count == 0)
                {
                    EditorGUILayout.HelpBox("No global handles are currently active.", MessageType.Info);
                }
                else
                {
                    foreach (var handle in _globalHandles)
                    {
                        DrawHandleDetails(handle);
                    }
                }
            }
        }
        
        private void DrawOwnerHandles()
        {
            int totalOwnerHandles = 0;
            if (_ownerReleasers != null)
            {
                foreach (var releaser in _ownerReleasers)
                {
                    totalOwnerHandles += releaser.Handles.Count;
                }
            }
            _showOwnerHandles = EditorGUILayout.Foldout(_showOwnerHandles, $"Owner Handles ({totalOwnerHandles})");

            if (_showOwnerHandles)
            {
                if (_ownerReleasers == null || _ownerReleasers.Count == 0)
                {
                    EditorGUILayout.HelpBox("No OwnerAssetHandleReleaser components found in the scene.", MessageType.Info);
                }
                else
                {
                    foreach (var releaser in _ownerReleasers)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Owner:", GUILayout.Width(50));
                        EditorGUILayout.ObjectField(releaser.gameObject, typeof(GameObject), true, GUILayout.ExpandWidth(true));
                        EditorGUILayout.LabelField($"({releaser.Handles.Count})", GUILayout.Width(40));
                        EditorGUILayout.EndHorizontal();

                        foreach (var handle in releaser.Handles)
                        {
                            DrawHandleDetails(handle);
                        }
                    }
                }
            }
        }
        
        private void DrawSelfHandles()
        {
            _showSelfHandles = EditorGUILayout.Foldout(_showSelfHandles, $"Self Handles ({_selfReleasers?.Count ?? 0})");
            if (_showSelfHandles)
            {
                if (_selfReleasers == null || _selfReleasers.Count == 0)
                {
                    EditorGUILayout.HelpBox("No SelfAssetHandleReleaser components found in the scene.", MessageType.Info);
                }
                else
                {
                    foreach (var releaser in _selfReleasers)
                    {
                        if (releaser.Handle.HasValue)
                        {
                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                            string assetType = releaser.Handle.Value.IsAddressable ? "Addressables" : "Resources";
                            EditorGUILayout.LabelField(assetType, GUILayout.Width(100));
                            EditorGUILayout.ObjectField(releaser.gameObject, typeof(GameObject), true, GUILayout.ExpandWidth(true));
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }
        }
        
        private void DrawHandleDetails(GameAssetHandle handle, string label = "")
        {
            Object managedAsset = handle.GetAsset();
            string assetType = handle.IsAddressable ? "Addressables" : "Resources";

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            
            if (managedAsset != null)
            {
                EditorGUILayout.LabelField(assetType, GUILayout.Width(100));
                EditorGUILayout.ObjectField(managedAsset, typeof(Object), false, GUILayout.ExpandWidth(true));
            }
            else
            {
                EditorGUILayout.LabelField("Invalid or Null asset.", EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}