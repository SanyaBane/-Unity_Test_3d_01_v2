using System.Linq;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs;
using Assets.Scripts.Buffs.ScriptableObjects;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Specific
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/Specific/BLM/Transpose")]
    public class TransposeSO : AbilityBehaviourSO, IBehaviourWithName
    {
        [Header("BehaviourWithName")]
        [SerializeField] private string _BehaviourName;

        public string Name => _BehaviourName;

        [SerializeField] private bool _ShareNameWithAbility = true;
        public bool ShareNameWithAbility => _ShareNameWithAbility;

        public BaseBuffSO FireStacks1StackBuffSO;
        public BaseBuffSO IceStacks1StackBuffSO;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new Transpose(this);
            return ret;
        }
    }
}