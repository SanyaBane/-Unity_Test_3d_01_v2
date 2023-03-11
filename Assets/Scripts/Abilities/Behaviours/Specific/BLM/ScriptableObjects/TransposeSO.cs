using Assets.Scripts.Abilities.Behaviours.General;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using Assets.Scripts.Abilities.Interfaces;
using Assets.Scripts.Buffs.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Specific.BLM.ScriptableObjects
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