using System.Collections.Generic;
using Ape.Runtime;
using Ape.Runtime.GameScene;
using UnityEditor;
using UnityEngine;

namespace Ape.Editor.GameScenes
{
    [CustomEditor(typeof(Runtime.GameScene.GameSceneManager))]
    public class GameScenesInspector : UnityEditor.Editor
    {
        private Vector2 _scrollPosition;
        private Dictionary<string, GameSceneHandle> _sceneHandles;

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Play Mode에서만 씬 핸들 정보를 볼 수 있습니다.", MessageType.Info);
                return;
            }

            // 매번 GUI를 그리기 전에 데이터를 갱신합니다.
            RefreshHandles();
            
            EditorGUILayout.LabelField("Managed Scene Handles", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawHandles();
            EditorGUILayout.EndScrollView();

            // 인스펙터 창이 매 프레임마다 갱신되도록 합니다.
            Repaint();
        }

        private void RefreshHandles()
        {
            // GameScenes.Instance가 null인 경우를 방어합니다.
            if (Runtime.GameScene.GameSceneManager.Instance == null)
            {
                _sceneHandles = new Dictionary<string, GameSceneHandle>();
                return;
            }

            _sceneHandles = new Dictionary<string, GameSceneHandle>(Runtime.GameScene.GameSceneManager.Instance.Handles);
        }

        private void DrawHandles()
        {
            if (_sceneHandles == null || _sceneHandles.Count == 0)
            {
                EditorGUILayout.HelpBox("No scene handles are currently active.", MessageType.Info);
                return;
            }

            foreach (var kvp in _sceneHandles)
            {
                string key = kvp.Key;
                GameSceneHandle handle = kvp.Value;

                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                
                string sourceType = handle.AddressableHandle.HasValue ? "Addressables" : "Builtin";
                
                EditorGUILayout.LabelField(sourceType, GUILayout.Width(100));
                EditorGUILayout.LabelField($"Key: {key}", GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}