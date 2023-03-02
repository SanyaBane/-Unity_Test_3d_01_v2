using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Creatures
{
    public class PartyController : MonoBehaviour
    {
        [SerializeField] private bool _canBeInParty = false;
        public bool CanBeInParty => _canBeInParty;

        private PartyEntity _currentParty;
        public PartyEntity CurrentParty
        {
            get => _currentParty;
            set
            {
                if (_currentParty == value)
                    return;
                
                _currentParty = value;
                
                CurrentPartyChanged?.Invoke(_currentParty);
            }
        }

        public event Action<PartyEntity> CurrentPartyChanged;
    }
}