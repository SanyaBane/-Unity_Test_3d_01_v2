using Assets.Scripts.Abilities.Behaviours.Buffs.Applier.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Applier
{
    public class BuffApplierTarget : BaseBuffApplier
    {
        public BuffApplierTargetSO BuffApplierTargetSO => (BuffApplierTargetSO) base.AbilityBehaviourSO;

        public BuffApplierTarget(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetRootObjectTransform() != null)
            {
                var targetBuffsController = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.BuffsController;
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