using System.Collections.Generic;
using Ape.Runtime.Util;
using UnityEngine;

namespace Ape.Runtime.DataModel
{
    public class DataModelManager : Singleton<DataModelManager>
    {
        private Dictionary<System.Type, NoKeyDataModelBase> _noKeydictionary = new();
        private Dictionary<System.Type, Dictionary<long, LongKeyDataModelBase>> _longKeyDictionary = new();
        private Dictionary<System.Type, Dictionary<string, StringKeyDataModelBase>> _stringKeyDictionary = new();

        internal IReadOnlyDictionary<System.Type, NoKeyDataModelBase> NoKeyDictionary => _noKeydictionary;
        internal IReadOnlyDictionary<System.Type, Dictionary<long, LongKeyDataModelBase>> LongKeyDictionary => _longKeyDictionary;
        internal IReadOnlyDictionary<System.Type, Dictionary<string, StringKeyDataModelBase>> StringKeyDictionary => _stringKeyDictionary;
        
        public bool TryGet<TDataModel>(out TDataModel dataModel) where TDataModel : NoKeyDataModelBase
        {
            if (!_noKeydictionary.TryGetValue(typeof(TDataModel), out var result ))
            {
                dataModel = null;
                return false;
            }
           
            dataModel = result as TDataModel;
            return true;
        }
        
        public bool TryGet<TDataModel>(long key, out TDataModel dataModel) where TDataModel : LongKeyDataModelBase
        {
            if (!_longKeyDictionary.TryGetValue(typeof(TDataModel), out var dictionary))
            {
                dataModel = null;
                return false;
            }

            if (!dictionary.TryGetValue(key, out var result))
            {
                dataModel = null;
                return false;
            }
            
            dataModel = result as TDataModel;
            return true;
        }
        
        public bool TryGet<TDataModel>(string key, out TDataModel dataModel) where TDataModel : StringKeyDataModelBase
        {
            if (!_stringKeyDictionary.TryGetValue(typeof(TDataModel), out var dictionary))
            {
                dataModel = null;
                return false;
            }

            if (!dictionary.TryGetValue(key, out var result))
            {
                dataModel = null;
                return false;
            }
            
            dataModel = result as TDataModel;
            return true;
        }

        public bool TryCreate<TDataModel>() where TDataModel : NoKeyDataModelBase => TryCreate<TDataModel>(out _);

        public bool TryCreate<TDataModel>(out TDataModel dataModel) where TDataModel : NoKeyDataModelBase
        {
            if (_noKeydictionary.TryGetValue(typeof(TDataModel), out var result ))
            {
                dataModel = null;
                return false;
            }
            
            dataModel = ScriptableObject.CreateInstance<TDataModel>();
            dataModel.name = typeof(TDataModel).Name;
            _noKeydictionary.Add(typeof(TDataModel), dataModel);
            
            return true;
        }

        public bool TryCreate<TDataModel>(long key) where TDataModel : LongKeyDataModelBase => TryCreate<TDataModel>(key, out _);
        
        public bool TryCreate<TDataModel>(long key, out TDataModel dataModel) where TDataModel : LongKeyDataModelBase
        {
            if (!_longKeyDictionary.TryGetValue(typeof(TDataModel), out var dictionary))
            {
                dictionary = new();
                _longKeyDictionary.Add(typeof(TDataModel), dictionary);
            }

            if (dictionary.TryGetValue(key, out var result))
            {
                dataModel = null;
                return false;
            }
            
            dataModel = ScriptableObject.CreateInstance<TDataModel>();
            dataModel.Key = key;
            dataModel.name = $"{typeof(TDataModel).Name}#{key}";
            dictionary.Add(key, dataModel);
            
            return true;
        }

        public bool TryCreate<TDataModel>(string key) where TDataModel : StringKeyDataModelBase => TryCreate<TDataModel>(key, out TDataModel _);

        public bool TryCreate<TDataModel>(string key, out TDataModel dataModel) where TDataModel : StringKeyDataModelBase
        {
            if (!_stringKeyDictionary.TryGetValue(typeof(TDataModel), out var dictionary))
            {
                dictionary = new();
                _stringKeyDictionary.Add(typeof(TDataModel), dictionary);
            }

            if (dictionary.TryGetValue(key, out var result))
            {
                dataModel = null;
                return false;
            }
            
            dataModel = ScriptableObject.CreateInstance<TDataModel>();
            dataModel.Key = key;
            dataModel.name = $"{typeof(TDataModel).Name}#{key}";
            dictionary.Add(key, dataModel);
            
            return true;
        }
        
        public bool Remove<TDataModel>() where TDataModel : DataModelBase
        {
            if (_noKeydictionary.TryGetValue(typeof(TDataModel), out var dataModel))
            {
                dataModel.TriggerRemoved();
                dataModel.SafeDestroy();

                _noKeydictionary.Remove(typeof(TDataModel));
                DestroyImmediate(dataModel);
                return true;
            }
            return false;
        }

        public bool Remove<TDataModel>(long key) where TDataModel : LongKeyDataModelBase
        {
            if (_longKeyDictionary.TryGetValue(typeof(TDataModel), out var dictionary))
            {
                if (dictionary.Remove(key, out var dataModel))
                {
                    dataModel.TriggerRemoved();
                    dataModel.SafeDestroy();
                    
                    if (dictionary.Count == 0)
                    {
                        _longKeyDictionary.Remove(typeof(TDataModel));
                    }
                    return true;
                }
            }
            return false;
        }

        public bool Remove<TDataModel>(string key) where TDataModel : StringKeyDataModelBase
        {
            if (_stringKeyDictionary.TryGetValue(typeof(TDataModel), out var dictionary))
            {
                if (dictionary.Remove(key, out var dataModel))
                {
                    dataModel.TriggerRemoved();
                    dataModel.SafeDestroy();
                    
                    if (dictionary.Count == 0)
                    {
                        _stringKeyDictionary.Remove(typeof(TDataModel));
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
