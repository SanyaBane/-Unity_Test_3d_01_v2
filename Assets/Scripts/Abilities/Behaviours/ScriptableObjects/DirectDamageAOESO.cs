using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/DirectDamageAOE")]
    public class DirectDamageAOESO : AbilityBehaviourSO, IBehaviourWithName
    {
        [Header("BehaviourWithName")]
        [SerializeField] private string _BehaviourName;

        public string Name => _BehaviourName;

        [SerializeField] private bool _ShareNameWithAbility = true;
        public bool ShareNameWithAbility => _ShareNameWithAbility;

        [Header("General")]
        public int Potency;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new DirectDamageAOE(this);
            return ret;
        }
    }
}