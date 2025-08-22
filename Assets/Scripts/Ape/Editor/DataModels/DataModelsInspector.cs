using System;
using System.Collections.Generic;
using Ape.Runtime.DataModel;
using UnityEditor;
using UnityEngine;

namespace Ape.Editor.DataModels
{
    [CustomEditor(typeof(DataModelManager))]
    public class DataModelsInspector : UnityEditor.Editor
    {
        private Vector2 _scrollPosition;
        private IReadOnlyDictionary<Type, NoKeyDataModelBase> _noKeyDictionary;
        private IReadOnlyDictionary<Type, Dictionary<long, LongKeyDataModelBase>> _longKeyDictionary;
        private IReadOnlyDictionary<Type, Dictionary<string, StringKeyDataModelBase>> _stringKeyDictionary;
        private string _searchQuery = "";
        
        private bool _showNoKeyModels = true;
        private bool _showLongKeyModels = true;
        private bool _showStringKeyModels = true;

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Play Mode에서만 DataModels 정보를 볼 수 있습니다.", MessageType.Info);
                return;
            }

            _searchQuery = EditorGUILayout.TextField("Search Models", _searchQuery);

            RefreshData();

            EditorGUILayout.LabelField("Managed Data Models", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawNoKeyModels();
            DrawLongKeyModels();
            DrawStringKeyModels();
            EditorGUILayout.EndScrollView();

            Repaint();
        }

        private void RefreshData()
        {
            if (!DataModelManager.Instance)
            {
                _noKeyDictionary = null;
                _longKeyDictionary = null;
                _stringKeyDictionary = null;
                return;
            }

            _noKeyDictionary = DataModelManager.Instance.NoKeyDictionary;
            _longKeyDictionary = DataModelManager.Instance.LongKeyDictionary;
            _stringKeyDictionary = DataModelManager.Instance.StringKeyDictionary;
        }

        private void DrawNoKeyModels()
        {
            _showNoKeyModels = EditorGUILayout.Foldout(_showNoKeyModels, $"No-Key Models ({_noKeyDictionary?.Count ?? 0})");
            if (_showNoKeyModels)
            {
                EditorGUI.indentLevel++;
                if (_noKeyDictionary == null || _noKeyDictionary.Count == 0)
                {
                    EditorGUILayout.HelpBox("No key-less models are currently managed.", MessageType.Info);
                }
                else
                {
                    foreach (var entry in _noKeyDictionary)
                    {
                        if (string.IsNullOrEmpty(_searchQuery) || 
                            entry.Key.Name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            entry.Value.name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            EditorGUILayout.ObjectField(entry.Value, typeof(DataModelBase), false);
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawLongKeyModels()
        {
            int totalCount = 0;
            if (_longKeyDictionary != null)
            {
                foreach (var typeEntry in _longKeyDictionary)
                {
                    totalCount += typeEntry.Value.Count;
                }
            }
            _showLongKeyModels = EditorGUILayout.Foldout(_showLongKeyModels, $"Long-Key Models ({totalCount})");
           
            if (_showLongKeyModels)
            {
                EditorGUI.indentLevel++;
                if (_longKeyDictionary == null || _longKeyDictionary.Count == 0)
                {
                    EditorGUILayout.HelpBox("No long-key models are currently managed.", MessageType.Info);
                }
                else
                {
                    foreach (var typeEntry in _longKeyDictionary)
                    {
                        // 타입 이름으로 검색
                        bool typeMatches = typeEntry.Key.Name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (string.IsNullOrEmpty(_searchQuery) || typeMatches)
                        {
                            EditorGUILayout.LabelField($"Model Type: {typeEntry.Key.Name}", EditorStyles.boldLabel);
                            EditorGUI.indentLevel++;
                        }
                        
                        foreach (var instanceEntry in typeEntry.Value)
                        {
                            // 인스턴스 이름 또는 키로 검색
                            bool instanceMatches = instanceEntry.Key.ToString().IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                   instanceEntry.Value.name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0;

                            if (string.IsNullOrEmpty(_searchQuery) || typeMatches || instanceMatches)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField($"ID: {instanceEntry.Key}", GUILayout.Width(100));
                                EditorGUILayout.ObjectField(instanceEntry.Value, typeof(LongKeyDataModelBase), false);
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        
                        if (string.IsNullOrEmpty(_searchQuery) || typeMatches)
                        {
                            EditorGUI.indentLevel--;
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawStringKeyModels()
        {
            int totalCount = 0;
            if (_stringKeyDictionary != null)
            {
                foreach (var typeEntry in _stringKeyDictionary)
                {
                    totalCount += typeEntry.Value.Count;
                }
            }
            _showStringKeyModels = EditorGUILayout.Foldout(_showStringKeyModels, $"String-Key Models ({totalCount})");
            
            if (_showStringKeyModels)
            {
                EditorGUI.indentLevel++;
                if (_stringKeyDictionary == null || _stringKeyDictionary.Count == 0)
                {
                    EditorGUILayout.HelpBox("No string-key models are currently managed.", MessageType.Info);
                }
                else
                {
                    foreach (var typeEntry in _stringKeyDictionary)
                    {
                        // 타입 이름으로 검색
                        bool typeMatches = typeEntry.Key.Name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (string.IsNullOrEmpty(_searchQuery) || typeMatches)
                        {
                            EditorGUILayout.LabelField($"Model Type: {typeEntry.Key.Name}", EditorStyles.boldLabel);
                            EditorGUI.indentLevel++;
                        }
                        
                        foreach (var instanceEntry in typeEntry.Value)
                        {
                            // 인스턴스 이름 또는 키로 검색
                            bool instanceMatches = instanceEntry.Key.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                   instanceEntry.Value.name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0;

                            if (string.IsNullOrEmpty(_searchQuery) || typeMatches || instanceMatches)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField($"Key: {instanceEntry.Key}", GUILayout.Width(100));
                                EditorGUILayout.ObjectField(instanceEntry.Value, typeof(StringKeyDataModelBase), false);
                                EditorGUILayout.EndHorizontal();
                            }
                        }

                        if (string.IsNullOrEmpty(_searchQuery) || typeMatches)
                        {
                            EditorGUI.indentLevel--;
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}