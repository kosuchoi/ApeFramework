using System;
using System.Collections.Generic;
using Ape.Runtime.GlobalEvent;
using UnityEditor;
using UnityEngine;

namespace Ape.Editor.GlobalEvent
{
    [CustomEditor(typeof(GlobalEventManager))]
    public class GlobalEventManagerInspector : UnityEditor.Editor
    {
        private Vector2 _scrollPosition;
        private IReadOnlyDictionary<Enum, HashSet<Action<GlobalEventParamBase>>> _eventDictionary;
        private string _searchQuery = "";

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Play Mode에서만 GlobalEvents 정보를 볼 수 있습니다.", MessageType.Info);
                return;
            }

            // 검색 필드를 추가합니다.
            _searchQuery = EditorGUILayout.TextField("Search Events", _searchQuery);

            RefreshEvents();

            EditorGUILayout.LabelField("Active Global Events", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawEvents();
            EditorGUILayout.EndScrollView();

            Repaint();
        }

        private void RefreshEvents()
        {
            if (!GlobalEventManager.Instance)
            {
                _eventDictionary = null;
                return;
            }

            _eventDictionary = GlobalEventManager.Instance.Events;
        }

        private void DrawEvents()
        {
            if (_eventDictionary == null || _eventDictionary.Count == 0)
            {
                EditorGUILayout.HelpBox("현재 등록된 이벤트가 없습니다.", MessageType.Info);
                return;
            }

            // 검색어가 비어있지 않은 경우, 필터링된 이벤트 목록을 만듭니다.
            var filteredEvents = new Dictionary<Enum, HashSet<Action<GlobalEventParamBase>>>();
            if (!string.IsNullOrEmpty(_searchQuery))
            {
                foreach (var kvp in _eventDictionary)
                {
                    string eventTypeName = kvp.Key.GetType().Name + "." + kvp.Key.ToString();
                    HashSet<Action<GlobalEventParamBase>> listeners = kvp.Value;
                    
                    // 이벤트 타입 이름이 검색어를 포함하는 경우
                    if (eventTypeName.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        filteredEvents.Add(kvp.Key, kvp.Value);
                        continue;
                    }

                    // 리스너의 타겟 또는 메서드 이름이 검색어를 포함하는 경우
                    var matchingListeners = new HashSet<Action<GlobalEventParamBase>>();
                    foreach (var listener in listeners)
                    {
                        string targetName = listener.Target != null ? listener.Target.ToString() : "Static/Anonymous";
                        string methodName = listener.Method.Name;

                        if (targetName.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            methodName.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            matchingListeners.Add(listener);
                        }
                    }

                    if (matchingListeners.Count > 0)
                    {
                        filteredEvents.Add(kvp.Key, matchingListeners);
                    }
                }
            }
            else
            {
                // 검색어가 비어있으면 전체 이벤트를 표시합니다.
                filteredEvents = new Dictionary<Enum, HashSet<Action<GlobalEventParamBase>>>(_eventDictionary);
            }

            // 필터링된 이벤트 목록을 그립니다.
            foreach (var kvp in filteredEvents)
            {
                string eventType = kvp.Key.GetType().Name + "." + kvp.Key.ToString();
                HashSet<Action<GlobalEventParamBase>> listeners = kvp.Value;

                EditorGUILayout.BeginVertical(GUI.skin.box);
                
                EditorGUILayout.LabelField($"Event: {eventType}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Listener Count: {listeners.Count}");
                
                EditorGUILayout.Space();
                
                if (listeners.Count > 0)
                {
                    EditorGUILayout.LabelField("Listeners:", EditorStyles.miniBoldLabel);

                    foreach (var listener in listeners)
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        if (listener.Target is UnityEngine.Object unityObject)
                        {
                            EditorGUILayout.ObjectField(unityObject, typeof(UnityEngine.Object), true);
                            EditorGUILayout.LabelField($"Method: {listener.Method.Name}");
                        }
                        else
                        {
                            string targetName = listener.Target != null ? listener.Target.ToString() : "Static/Anonymous";
                            EditorGUILayout.LabelField($"Target: {targetName}");
                            EditorGUILayout.LabelField($"Method: {listener.Method.Name}");
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
                
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space();
            }
        }
    }
}