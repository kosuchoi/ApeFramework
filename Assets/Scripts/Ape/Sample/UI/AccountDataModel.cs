using System;
using Ape.Runtime.DataModel;
using UnityEngine;

namespace Ape.Sample.UI
{
    public class AccountDataModel : NoKeyDataModelBase
    {
        [SerializeField] private string _nickname;
        [SerializeField] private int _level;

        public string Nickname => _nickname;
        public int Level => _level;
        
        public event Action OnDataChanged;
        
        public void ChangeNickname()
        {
            string[] nicknames = new string[]
            {
                "Shadow",
                "Blaze",
                "Titan",
                "Phoenix",
                "Vortex"
            };

            _nickname = nicknames[UnityEngine.Random.Range(0, nicknames.Length)];
            
            OnDataChanged?.Invoke();
        }
        
        public void IncreaseLevel()
        {
            ++_level;
            
            OnDataChanged?.Invoke();
        }
    }
}