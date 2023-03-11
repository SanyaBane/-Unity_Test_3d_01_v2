using Assets.Scripts.Abilities.Interfaces;
using Assets.Scripts.Buffs.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/DirectRemoveBuffStack")]
    public class DirectRemoveBuffStackSO : AbilityBehaviourSO, IBehaviourWithName
    {
        [Header("BehaviourWithName")]
        [SerializeField] private string _BehaviourName;

        public string Name => _BehaviourName;

        [SerializeField] private bool _ShareNameWithAbility = true;
        public bool ShareNameWithAbility => _ShareNameWithAbility;

        public BaseBuffSO BaseBuffSO;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new DirectRemoveBuffStack(this);
            return ret;
        }
    }
}