using Assets.Scripts.Abilities.Enums;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.General.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Abilities/AutoAttack")]
    public class AbilityAutoAttackSO : AbilityTargetSO
    {
        [Header("AutoAttack Properties")]
        public float PreferableMaxDistance = 1.5f;
        public EJob Job;
        
        private void OnEnable()
        {
            CanCastOn = EAbilityAffects.Enemies;
        }
        
        public override Ability CreateAbility(IAbilitiesController iAbilitiesController)
        {
            var ability = new AbilityAutoAttack(this, iAbilitiesController);
            return ability;
        }
    }
}
