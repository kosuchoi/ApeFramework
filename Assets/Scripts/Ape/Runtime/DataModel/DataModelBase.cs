using System;
using UnityEngine;

namespace Ape.Runtime.DataModel
{
    public abstract class DataModelBase : ScriptableObject
    {
        public event Action OnChanged;
        public event Action OnRemoved;

        protected void TriggerChanged()
        {
            OnChanged?.Invoke();
        }

        internal void TriggerRemoved()
        {
            OnRemoved?.Invoke();
        }
    }
    
    public abstract class NoKeyDataModelBase : DataModelBase
    {

    }
    
    public abstract class LongKeyDataModelBase : DataModelBase
    {
        [SerializeField] private long key = 0;

        public long Key
        {
            get => key;
            internal set => key = value;
        }
    }

    public abstract class StringKeyDataModelBase : DataModelBase
    {
        [SerializeField] private string key = string.Empty;

        public string Key
        {
            get => key;
            internal set => key = value;
        }
    }
}