using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Interfaces;
using Assets.Scripts.ManaSystem.Modifiers.ScriptableObjects;

namespace Assets.Scripts.ManaSystem.Modifiers
{
    public class RegenStaticAmountBasedOnBuff : BaseManaModifier
    {
        public RegenStaticAmountBasedOnBuffSO RegenStaticAmountBasedOnBuffSO => (RegenStaticAmountBasedOnBuffSO) BaseManaModifierSO;

        public string BuffId { get; }

        public List<RegenStaticAmountBasedOnBuffSO.StacksRegenStaticAmount> ListStacksIncreaseRegenPercentage { get; }

        public RegenStaticAmountBasedOnBuff(RegenStaticAmountBasedOnBuffSO regenStaticAmountBasedOnBuffSO, IBaseCreature iBaseCreature) :
            base(regenStaticAmountBasedOnBuffSO, iBaseCreature)
        {
            BuffId = regenStaticAmountBasedOnBuffSO.BuffId;
            ListStacksIncreaseRegenPercentage = regenStaticAmountBasedOnBuffSO.ListStacksRegenStatic;
        }

        public override bool CanModify()
        {
            var existingBuff = IBaseCreature.BuffsController.GetBuffById(BuffId);
            if (existingBuff == null)
                return false;

            var stacksIncreaseRegenPercentage = ListStacksIncreaseRegenPercentage.FirstOrDefault(x => x.StacksCount == existingBuff.StacksCount);
            if (stacksIncreaseRegenPercentage == null)
                return false;

            return true;
        }


        public override int Modify(int manaAmount)
        {
            var existingBuff = IBaseCreature.BuffsController.GetBuffById(BuffId);
            if (existingBuff == null)
                throw new NullReferenceException();

            var stacksIncreaseRegenPercentage = ListStacksIncreaseRegenPercentage.FirstOrDefault(x => x.StacksCount == existingBuff.StacksCount);
            if (stacksIncreaseRegenPercentage == null)
                throw new NullReferenceException();

            if (IBaseCreature.CombatInfoHandler.IsInCombat)
            {
                manaAmount += stacksIncreaseRegenPercentage.Amount;
            }
            else
            {
                manaAmount += stacksIncreaseRegenPercentage.Amount;
            }

            return manaAmount;
        }
    }
}