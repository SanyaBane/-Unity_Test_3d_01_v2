using Assets.Scripts.Abilities.General.ScriptableObjects;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Abilities.General
{
    public class AbilityAutoAttack : AbilityTarget
    {
        private AbilityAutoAttackSO _abilityAutoAttackSO;

        public float PreferableMaxDistance = 1.5f;
        public EJob Job;
        
        public AbilityAutoAttack(AbilityAutoAttackSO abilityAutoAttackSO, IAbilitiesController iAbilitiesController) : base(abilityAutoAttackSO, iAbilitiesController)
        {
            _abilityAutoAttackSO = (AbilityAutoAttackSO) abilityAutoAttackSO;

            PreferableMaxDistance = _abilityAutoAttackSO.PreferableMaxDistance;
            Job = _abilityAutoAttackSO.Job;
        }
    }
}