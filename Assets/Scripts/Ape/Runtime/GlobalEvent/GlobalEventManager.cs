using System;
using System.Collections.Generic;
using Ape.Runtime.Util;

namespace Ape.Runtime.GlobalEvent
{
    public class GlobalEventManager : Singleton<GlobalEventManager>
    {
        private Dictionary<Enum, HashSet<Action<GlobalEventParamBase>>> _eventDictionary = new();

        internal IReadOnlyDictionary<Enum, HashSet<Action<GlobalEventParamBase>>> Events => _eventDictionary;

        public bool TryAddListener<TEnum>(TEnum eventType, Action<GlobalEventParamBase> listener)
            where TEnum : Enum
        {
            if (!_eventDictionary.TryGetValue(eventType, out var hashSet))
            {
                hashSet = new HashSet<Action<GlobalEventParamBase>>();
                _eventDictionary.Add(eventType, hashSet);
            }

            return hashSet.Add(listener);
        }
        
        public void AddAllListeners<TEnum>(Action<GlobalEventParamBase> listener)
            where TEnum : Enum
        {
            foreach (var enumValue in Enum.GetValues(typeof(TEnum)))
            {
                TryAddListener((TEnum)enumValue, listener);
            }
        }
        
        public bool TryRemoveListener<TEnum>(TEnum eventType, Action<GlobalEventParamBase> listener)
            where TEnum : Enum
        {
            if (!_eventDictionary.TryGetValue(eventType, out var hashSet))
            {
                return false;
            }

            if (!hashSet.Remove(listener))
            {
                return false;
            }

            if (hashSet.Count == 0)
            {
                _eventDictionary.Remove(eventType);
            }
            
            return true;
        }
        
        public void RemoveAllListeners<TEnum>(Action<GlobalEventParamBase> listener)
            where TEnum : Enum
        {
            foreach (var enumValue in Enum.GetValues(typeof(TEnum)))
            {
                TryRemoveListener((TEnum)enumValue, listener);
            }
        }
        
        public void RemoveObjectListeners(object target)
        {
            List<Enum> toRemove = new();
            foreach (var v in _eventDictionary)
            {
                var hashSet = v.Value;
                
                var listenersToRemove = new List<Action<GlobalEventParamBase>>();
                foreach (var listener in hashSet)
                {
                    if (listener.Target == target)
                    {
                        listenersToRemove.Add(listener);
                    }
                }
                
                foreach (var listenerToRemove in listenersToRemove)
                {
                    hashSet.Remove(listenerToRemove);
                }
                
                // HashSet이 비어있으면 딕셔너리에서 제거합니다.
                if (hashSet.Count == 0)
                {
                    toRemove.Add(v.Key);
                }
            }

            foreach (var enumType in toRemove)
            {
                _eventDictionary.Remove(enumType);
            }
        }
        
        public void Trigger(GlobalEventParamBase param)
        {
            if (_eventDictionary.TryGetValue(param.EventType, out var list))
            {
                foreach (var v in list)
                {
                    v.Invoke(param);
                }
            }
        }
    }
}