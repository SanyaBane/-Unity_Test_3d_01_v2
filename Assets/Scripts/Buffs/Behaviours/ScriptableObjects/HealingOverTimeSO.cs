using Assets.Scripts.Abilities.Behaviours;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Interfaces;
using Assets.Scripts.Abilities.Parameters;
using UnityEngine;

namespace Assets.Scripts.Buffs.Behaviours.ScriptableObjects
{
    [CreateAssetMenu(menuName = "BuffBehaviour/HealingOverTime")]
    public class HealingOverTimeSO : BaseBuffBehaviourSO, IBehaviourWithName, IBuffBehaviourTick
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
            var heal = DamageInfo.CalculateHealFromPotency(iAbilityParameters.DefaultAbilityParameters.Source, Potency);
            
            if (iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetRootObjectTransform() != null)
            {
                var targetHealth = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.Health;
                if (targetHealth != null)
                {
                    var damageInfo = new DamageInfo(ability, iAbilityParameters, this, heal);
                    targetHealth.TryInflictHealing(damageInfo);
                }
            }
        }
    }
}
