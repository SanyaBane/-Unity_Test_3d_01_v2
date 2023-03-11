using Assets.Scripts.Abilities.Behaviours.Buffs.Applier.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Applier
{
    public class BuffApplierSource : BaseBuffApplier
    {
        public BuffApplierSourceSO BuffApplierSourceSO => (BuffApplierSourceSO) base.AbilityBehaviourSO;
        
        public BuffApplierSource(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
        }
        
        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (iAbilityParameters.DefaultAbilityParameters.Source.GetRootObjectTransform() != null)
            {
                var targetBuffsController = iAbilityParameters.DefaultAbilityParameters.Source.BuffsController;
                if (targetBuffsController != null)
                {
                    foreach (var buffSO in BuffsSO)
                    {
                        targetBuffsController.ApplyBuff(buffSO, ability, iAbilityParameters, true);
                    }
                }
            }
        }
    }
}