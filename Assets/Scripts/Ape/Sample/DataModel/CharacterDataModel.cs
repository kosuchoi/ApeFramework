using Ape.Runtime.DataModel;
using UnityEngine;

namespace Ape.Sample.DataModel
{
    public class CharacterDataModel : LongKeyDataModelBase
    {
        [SerializeField] private Vector3 _position;
        [SerializeField] private Color _color;

        public Vector3 Position => _position;
        public Color Color => _color;

        public void MoveLeft()
        {
            _position += new Vector3(-1f, 0f, 0f);
            TriggerChanged();
        }

        public void MoveRight()
        {
            _position += new Vector3(1f, 0f, 0f);
            TriggerChanged();
        }

        public void ChangeColor()
        {
            _color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            TriggerChanged();
        }
    }
}