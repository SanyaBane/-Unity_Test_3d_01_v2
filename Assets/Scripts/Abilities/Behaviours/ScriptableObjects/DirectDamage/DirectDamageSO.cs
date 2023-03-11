using Assets.Scripts.Abilities.Behaviours.DirectDamageBehaviour;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.DamageModifiers;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/DirectDamage")]
    public class DirectDamageSO : AbilityBehaviourSO, IBehaviourWithName
    {
        public enum ECalculationMode
        {
            OnHitEnemy,
            OnCastFinish
        }
        
        [Header("BehaviourWithName")]
        [SerializeField] private string _BehaviourName;

        public string Name => _BehaviourName;

        [SerializeField] private bool _ShareNameWithAbility = true;
        public bool ShareNameWithAbility => _ShareNameWithAbility;

        [Header("General")]
        public int Potency;

        public BaseDirectDamagePercentageModifierSO[] DirectDamagePercentageModifiers;

        public ECalculationMode CalculationMode = ECalculationMode.OnCastFinish;
        
        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new DirectDamage(this);
            return ret;
        }
    }
}