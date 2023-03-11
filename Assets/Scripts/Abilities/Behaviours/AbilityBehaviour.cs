using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using Assets.Scripts.Abilities.Parameters;

namespace Assets.Scripts.Abilities.Behaviours
{
    public abstract class AbilityBehaviour
    {
        public bool WorksOnlyIfComboPerformed;

        public AbilityBehaviourSO AbilityBehaviourSO { get; }
        
        protected AbilityBehaviour(AbilityBehaviourSO abilityBehaviourSO)
        {
            AbilityBehaviourSO = abilityBehaviourSO;
            
            WorksOnlyIfComboPerformed = AbilityBehaviourSO.WorksOnlyIfComboPerformed;
        }
        
        public abstract void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters);

        public bool CanApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (WorksOnlyIfComboPerformed)
            {
                if (ability.AbilitySO.ComboContinuerAbilitySO == null)
                    return false;

                if (ability.PerformCombo == false)
                    return false;
            }
          
            return true;
        }
    }
}