using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Interfaces;
using Assets.Scripts.ManaSystem.Modifiers.ScriptableObjects;

namespace Assets.Scripts.ManaSystem.Modifiers
{
    public class IncreaseRegenPercentageBasedOnBuff : BaseManaModifier
    {
        public IncreaseRegenPercentageBasedOnBuffSO IncreaseRegenPercentageBasedOnBuffSO => (IncreaseRegenPercentageBasedOnBuffSO) BaseManaModifierSO;

        public string BuffId { get; }

        public List<IncreaseRegenPercentageBasedOnBuffSO.StacksIncreaseRegenPercentage> ListStacksIncreaseRegenPercentage { get; }

        public IncreaseRegenPercentageBasedOnBuff(IncreaseRegenPercentageBasedOnBuffSO increaseRegenPercentageBasedOnBuffSO, IBaseCreature iBaseCreature) :
            base(increaseRegenPercentageBasedOnBuffSO, iBaseCreature)
        {
            BuffId = IncreaseRegenPercentageBasedOnBuffSO.BuffId;
            ListStacksIncreaseRegenPercentage = IncreaseRegenPercentageBasedOnBuffSO.ListStacksIncreaseRegenPercentage;
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
            throw new NotImplementedException();
        }
    }
}