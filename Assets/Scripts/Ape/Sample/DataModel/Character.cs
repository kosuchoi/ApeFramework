using Ape.Runtime.DataModel;
using Ape.Runtime.Util;
using UnityEngine;

namespace Ape.Sample.DataModel
{
    public class Character : MonoBehaviour
    {
        private Renderer _renderer;
        private long _characterKey = 1L;

        public void SetCharacterKey(long characterKey)
        {
            _characterKey = characterKey;
        }
        
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _renderer.sharedMaterial = new Material(_renderer.sharedMaterial);
        }
        
        private void OnEnable()
        {
            if (DataModelManager.Instance.TryGet<CharacterDataModel>(_characterKey, out var characterDataModel))
            {
                characterDataModel.OnChanged += CharacterDataModelOnOnChanged;
                characterDataModel.OnRemoved += CharacterDataModelOnOnRemoved;
            }

            CharacterDataModelOnOnChanged();
        }

        private void OnDisable()
        {
            if (DataModelManager.Instance &&
                DataModelManager.Instance.TryGet<CharacterDataModel>(_characterKey, out var characterDataModel))
            {
                characterDataModel.OnChanged -= CharacterDataModelOnOnChanged;
                characterDataModel.OnRemoved -= CharacterDataModelOnOnRemoved;
            }
        }

        private void CharacterDataModelOnOnChanged()
        {
            if (DataModelManager.Instance.TryGet<CharacterDataModel>(_characterKey, out var characterDataModel))
            {
                transform.position = characterDataModel.Position;
                _renderer.sharedMaterial.color = characterDataModel.Color;
            }
        }
        
        private void CharacterDataModelOnOnRemoved()
        {
            gameObject.SafeDestroy();
        }
    }
}