using Assets.Scripts.Abilities.Behaviours.General;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.Specific.BLM.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Buffs.ScriptableObjects;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Specific.BLM
{
    public class ApplyFireOrIceStacksBuff : AbilityBehaviour
    {
        public ApplyFireOrIceStacksBuffSO ApplyFireOrIceStacksBuffSO => (ApplyFireOrIceStacksBuffSO) base.AbilityBehaviourSO;
        
        public BaseBuffSO BuffToApplySO;
        public string BuffIdToRemove;

        public bool ApplyBuffIfBuffToRemoveExist = false;

        
        public ApplyFireOrIceStacksBuff(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            BuffToApplySO = ApplyFireOrIceStacksBuffSO.BuffToApplySO;
            BuffIdToRemove = ApplyFireOrIceStacksBuffSO.BuffIdToRemove;
            ApplyBuffIfBuffToRemoveExist = ApplyFireOrIceStacksBuffSO.ApplyBuffIfBuffToRemoveExist;
        }   

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            IBaseCreature creature = iAbilityParameters.DefaultAbilityParameters.Source;

            if (creature.GetRootObjectTransform() != null)
            {
                var buffsController = creature.BuffsController;

                if (buffsController == null)
                {
                    Debug.LogError($"{nameof(buffsController)} == null");
                    return;
                }

                var existingBuff = buffsController.GetBuffById(BuffIdToRemove);
                if (existingBuff != null)
                {
                    buffsController.RemoveRuntimeBuff(existingBuff);

                    if (!ApplyBuffIfBuffToRemoveExist)
                        return;
                }

                buffsController.ApplyBuff(BuffToApplySO, ability, iAbilityParameters, true);
            }
        }
    }
}