using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Specific;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Buffs.ScriptableObjects;

namespace Assets.Scripts.Abilities.Behaviours
{
    public class Transpose : AbilityBehaviour
    {
        public TransposeSO TransposeSO => (TransposeSO) base.AbilityBehaviourSO;
        
        public string Name { get; }
        public bool ShareNameWithAbility { get; }
        
        public BaseBuffSO FireStacks1StackBuffSO;
        public BaseBuffSO IceStacks1StackBuffSO;
        
        public Transpose(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            Name = TransposeSO.Name;
            ShareNameWithAbility = TransposeSO.ShareNameWithAbility;
            FireStacks1StackBuffSO = TransposeSO.FireStacks1StackBuffSO;
            IceStacks1StackBuffSO = TransposeSO.IceStacks1StackBuffSO;
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (iAbilityParameters.DefaultAbilityParameters.Source.GetRootObjectTransform() != null)
            {
                var buffsController = iAbilityParameters.DefaultAbilityParameters.Source.BuffsController;

                var existingFireBuff = buffsController.GetBuffById(ConstantsAbilities_BLM.BUFF_BLM_FIRE_STACKS);
                var existingIceBuff = buffsController.GetBuffById(ConstantsAbilities_BLM.BUFF_BLM_ICE_STACKS);

                if (existingFireBuff != null)
                {
                    buffsController.RemoveRuntimeBuff(existingFireBuff);
                    buffsController.ApplyBuff(IceStacks1StackBuffSO, ability, iAbilityParameters, true);
                }
                else if (existingIceBuff != null)
                {
                    buffsController.RemoveRuntimeBuff(existingIceBuff);
                    buffsController.ApplyBuff(FireStacks1StackBuffSO, ability, iAbilityParameters, true);
                }
                else
                {
                    // do nothing
                    // todo display "Transpose has no effect" floating message (moving down) 
                }
            }
        }
    }
}