using Assets.Scripts.Abilities;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Buffs.Behaviours.ScriptableObjects
{
    [CreateAssetMenu(menuName = "BuffBehaviour/Provoke")]
    public class ProvokeSO : BaseBuffBehaviourSO, IBehaviourWithName
    {
        [Header("BehaviourWithName")]
        [SerializeField] private string _BuffName;
        public string Name => _BuffName;
        
        [SerializeField] private bool _ShareNameWithAbility = true;
        public bool ShareNameWithAbility => _ShareNameWithAbility;
    }
}
