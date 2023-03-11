using System;
using System.Collections.Generic;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.Remover;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Remover
{
    public class BuffRemover : AbilityBehaviour
    {
        public BuffRemoverSO BuffRemoverSO => (BuffRemoverSO) base.AbilityBehaviourSO;
        
        public List<string> BuffsId;

        public BuffRemoverSO.EBuffRemoverTarget BuffRemoverTarget;
        
        public BuffRemover(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            BuffsId = BuffRemoverSO.BuffsId;
            BuffRemoverTarget = BuffRemoverSO.BuffRemoverTarget;
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            IBaseCreature creature;
            switch(BuffRemoverTarget)
            {
                case BuffRemoverSO.EBuffRemoverTarget.Caster:
                    creature = iAbilityParameters.DefaultAbilityParameters.Source;
                    break;
                case BuffRemoverSO.EBuffRemoverTarget.Target:
                    creature = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (creature.GetRootObjectTransform() != null)
            {
                var buffsController = creature.BuffsController;
                if (buffsController != null)
                {
                    foreach (var buffId in BuffsId)
                    {
                        var buff = buffsController.GetBuffById(buffId);
                        if (buff != null)
                        {
                            buffsController.RemoveRuntimeBuff(buff);
                        }
                    }
                }
            }
        }
    }
}