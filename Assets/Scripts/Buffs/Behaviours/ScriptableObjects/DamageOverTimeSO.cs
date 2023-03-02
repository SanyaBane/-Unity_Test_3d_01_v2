using Assets.Scripts.Abilities;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Buffs.Behaviours.ScriptableObjects
{
    [CreateAssetMenu(menuName = "BuffBehaviour/DamageOverTime")]
    public class DamageOverTimeSO : BaseBuffBehaviourSO, IBehaviourWithName, IBuffBehaviourTick
    {
        [Header("BehaviourWithName")]
        [SerializeField] private string _BuffName;
        public string Name => _BuffName;
        
        [SerializeField] private bool _ShareNameWithAbility = true;
        public bool ShareNameWithAbility => _ShareNameWithAbility;
        
        [Header("General")]
        public int Potency;
        
        public void TickBuffBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            var damage = DamageInfo.CalculateDamageFromPotency(iAbilityParameters.DefaultAbilityParameters.Source, Potency);
            
            if (iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetRootObjectTransform() != null)
            {
                var targetHealth = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.Health;
                if (targetHealth != null)
                {
                    var damageInfo = new DamageInfo(ability, iAbilityParameters, this, damage);
                    targetHealth.TryInflictDamage(damageInfo);
                }
            }
        }
    }
}
