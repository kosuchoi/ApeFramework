using System;
using Ape.Runtime.DataModel;
using UnityEngine;
using UnityEngine.UI;

namespace Ape.Sample.DataModel
{
    public class DataModelSample : MonoBehaviour
    {
        private long _selectedCharacterId = 1L;
        private GameObject _characterPrefab;

        private void Awake()
        {
            transform.Find("Canvas/Upper/VerticalLayoutGroup/AddChracterButton").GetComponent<Button>().onClick
                .AddListener(OnAddChracterButtonClick);
            transform.Find("Canvas/Upper/VerticalLayoutGroup/RemoveChracterButton").GetComponent<Button>().onClick
                .AddListener(OnRemoveChracterButtonClick);
            transform.Find("Canvas/Upper/VerticalLayoutGroup/ChangeColorButton").GetComponent<Button>().onClick
                .AddListener(OnChangeColorButtonClick);
            transform.Find("Canvas/Upper/VerticalLayoutGroup/MoveLeftButton").GetComponent<Button>().onClick
                .AddListener(OnLeftButtonClick);
            transform.Find("Canvas/Upper/VerticalLayoutGroup/MoveRightButton").GetComponent<Button>().onClick
                .AddListener(OnRightButtonClick);

            _characterPrefab = transform.Find("CharacterPrefab").gameObject;
            _characterPrefab.SetActive(false);
        }

        private void OnAddChracterButtonClick()
        {
            ++_selectedCharacterId;
            
            DataModelManager.Instance.TryCreate<CharacterDataModel>(_selectedCharacterId, out _);
            var go = Instantiate(_characterPrefab);

            var character = go.GetComponent<Character>();
            character.SetCharacterKey(_selectedCharacterId);
            
            go.SetActive(true);
        }

        private void OnRemoveChracterButtonClick()
        {
            DataModelManager.Instance.Remove<CharacterDataModel>(_selectedCharacterId);
            --_selectedCharacterId;
        }
        
        private void OnLeftButtonClick()
        {
            if (_selectedCharacterId == 0)
            {
                return;
            }

            if (DataModelManager.Instance.TryGet<CharacterDataModel>(_selectedCharacterId, out var characterDataModel))
            {
                characterDataModel.MoveLeft();
            }
        }
        
        private void OnRightButtonClick()
        {
            if (_selectedCharacterId == 0)
            {
                return;
            }

            if (DataModelManager.Instance.TryGet<CharacterDataModel>(_selectedCharacterId, out var characterDataModel))
            {
                characterDataModel.MoveRight();
            }
        }

        private void OnChangeColorButtonClick()
        {
            if (_selectedCharacterId == 0)
            {
                return;
            }

            if (DataModelManager.Instance.TryGet<CharacterDataModel>(_selectedCharacterId, out var characterDataModel))
            {
                characterDataModel.ChangeColor();
            }
        }
    }
}