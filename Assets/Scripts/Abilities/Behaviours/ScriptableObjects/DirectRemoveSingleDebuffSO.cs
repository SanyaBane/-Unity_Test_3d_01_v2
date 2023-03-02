using System.Linq;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/DirectRemoveSingleDebuff")]
    public class DirectRemoveSingleDebuffSO : AbilityBehaviourSO, IBehaviourWithName
    {
        [Header("BehaviourWithName")]
        [SerializeField] private string _BehaviourName;

        public string Name => _BehaviourName;

        [SerializeField] private bool _ShareNameWithAbility = true;
        public bool ShareNameWithAbility => _ShareNameWithAbility;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new DirectRemoveSingleDebuff(this);
            return ret;
        }
    }
}