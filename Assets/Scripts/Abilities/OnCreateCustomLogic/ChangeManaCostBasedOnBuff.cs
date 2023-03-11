using System.Linq;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.OnCreateCustomLogic.ScriptableObjects;
using Assets.Scripts.Buffs;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.OnCreateCustomLogic
{
    public class ChangeManaCostBasedOnBuff : BaseOnCreateCustomLogic
    {
        public ChangeManaCostBasedOnBuffSO ChangeManaCostBasedOnBuffSO => (ChangeManaCostBasedOnBuffSO) base.BaseOnCreateCustomLogicSO;

        public string BuffId { get; set; }
        public ChangeManaCostBasedOnBuffSO.StacksToManaCost[] StacksToManaCost { get; set; }

        public ChangeManaCostBasedOnBuff(ChangeManaCostBasedOnBuffSO changeManaCostBasedOnBuffSO, IAbilitiesController iAbilitiesController, Ability ability) : base(changeManaCostBasedOnBuffSO, iAbilitiesController, ability)
        {
            BuffId = ChangeManaCostBasedOnBuffSO.BuffId;
            StacksToManaCost = ChangeManaCostBasedOnBuffSO.ListStacksToManaCost;

            // Debug.Log($"Called constructor for {nameof(ChangeManaCostBasedOnBuff)}.");

            IAbilitiesController.IBaseCreature.BuffsController.BuffAdded += BuffsControllerOnBuffAdded;
            IAbilitiesController.IBaseCreature.BuffsController.BuffRemoved += BuffsControllerOnBuffRemoved;
            IAbilitiesController.IBaseCreature.BuffsController.BuffDurationUpdated += BuffsControllerOnBuffDurationUpdated;
        }

        // todo we should probably unsubscribe somewhere?
        // ~ChangeManaCostBasedOnBuff()
        // {
        //     // Debug.Log($"Called destructor for ~{nameof(ChangeManaCostBasedOnBuff)}.");
        //
        //     IAbilitiesController.IBaseCreature.BuffsController.BuffAdded -= BuffsControllerOnBuffAdded;
        //     IAbilitiesController.IBaseCreature.BuffsController.BuffRemoved -= BuffsControllerOnBuffRemoved;
        //     IAbilitiesController.IBaseCreature.BuffsController.BuffDurationUpdated -= BuffsControllerOnBuffDurationUpdated;
        // }

        private void BuffsControllerOnBuffAdded(Buff buff)
        {
            // todo maybe we should ensure that another buff with same effect is not exist
            if (buff.BaseBuffSO.Id != BuffId)
                return;

            UpdateAbilityManaCost(buff.StacksCount);
        }

        private void BuffsControllerOnBuffDurationUpdated(Buff buff)
        {
            // todo maybe we should ensure that another buff with same effect is not exist
            if (buff.BaseBuffSO.Id != BuffId)
                return;

            UpdateAbilityManaCost(buff.StacksCount);
        }

        private void BuffsControllerOnBuffRemoved(Buff buff)
        {
            // todo maybe we should ensure that another buff with same effect is not exist
            if (buff.BaseBuffSO.Id != BuffId)
                return;

            ResetAbilityManaCost();
        }

        private void UpdateAbilityManaCost(int stacksCount)
        {
            var stacksToManaCostElement = StacksToManaCost.FirstOrDefault(x => x.StacksCount == stacksCount);
            if (stacksToManaCostElement != null)
            {
                Ability.ManaCost = Mathf.FloorToInt(Ability.AbilitySO.InitialManaCost / 100f * stacksToManaCostElement.ManaCostPercentage);
            }
        }

        private void ResetAbilityManaCost()
        {
            Ability.ManaCost = Ability.AbilitySO.InitialManaCost;
        }
    }
}