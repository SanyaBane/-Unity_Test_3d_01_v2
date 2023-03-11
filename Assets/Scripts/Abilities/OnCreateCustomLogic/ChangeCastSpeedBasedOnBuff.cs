using System;
using System.Linq;
using Assets.Scripts.Abilities.Modifiers;
using Assets.Scripts.Abilities.OnCreateCustomLogic.ScriptableObjects;
using Assets.Scripts.Buffs;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Abilities.OnCreateCustomLogic
{
    public class ChangeCastSpeedBasedOnBuff : BaseOnCreateCustomLogic
    {
        public ChangeCastSpeedBasedOnBuffSO ChangeCastSpeedBasedOnBuffSO => (ChangeCastSpeedBasedOnBuffSO) base.BaseOnCreateCustomLogicSO;

        // public string ModifierId { get; set; }
        public Guid ModifierId { get; }

        public string BuffId { get; set; }
        public ChangeCastSpeedBasedOnBuffSO.StacksToCastSpeedPercentage[] StacksToCastSpeedPercentage { get; set; }

        public ChangeCastSpeedBasedOnBuff(ChangeCastSpeedBasedOnBuffSO ChangeCastSpeedBasedOnBuffSO, IAbilitiesController iAbilitiesController, Ability ability) : base(ChangeCastSpeedBasedOnBuffSO, iAbilitiesController, ability)
        {
            // ModifierId = ChangeCastSpeedBasedOnBuffSO.ModifierId;
            ModifierId = Guid.NewGuid();

            BuffId = ChangeCastSpeedBasedOnBuffSO.BuffId;
            StacksToCastSpeedPercentage = ChangeCastSpeedBasedOnBuffSO.ListStacksToCastSpeedPercentage;

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

            UpdateAbilityCastSpeed(buff.StacksCount);
        }

        private void BuffsControllerOnBuffDurationUpdated(Buff buff)
        {
            // todo maybe we should ensure that another buff with same effect is not exist
            if (buff.BaseBuffSO.Id != BuffId)
                return;

            UpdateAbilityCastSpeed(buff.StacksCount);
        }

        private void BuffsControllerOnBuffRemoved(Buff buff)
        {
            // todo maybe we should ensure that another buff with same effect is not exist
            if (buff.BaseBuffSO.Id != BuffId)
                return;

            ResetAbilityCastSpeed();
        }

        private void UpdateAbilityCastSpeed(int stacksCount)
        {
            var existedCastSpeedModifier = GetModifierFromAbility();

            var stacksToCastSpeedElement = StacksToCastSpeedPercentage.FirstOrDefault(x => x.StacksCount == stacksCount);
            if (stacksToCastSpeedElement != null)
            {
                existedCastSpeedModifier.CanApply = true;
                existedCastSpeedModifier.CastSpeedPercentageBonus = stacksToCastSpeedElement.CastSpeedPercentage;
            }
        }

        private void ResetAbilityCastSpeed()
        {
            var existedCastSpeedModifier = GetModifierFromAbility();
            existedCastSpeedModifier.CanApply = false;
        }

        private CastSpeedModifier GetModifierFromAbility()
        {
            var existedCastSpeedModifier = Ability.CastSpeedModifiers.FirstOrDefault(x => x.ModifierId == ModifierId);
            if (existedCastSpeedModifier == null)
            {
                existedCastSpeedModifier = new CastSpeedModifier(ModifierId);
                Ability.CastSpeedModifiers.Add(existedCastSpeedModifier);
            }

            return existedCastSpeedModifier;
        }
    }
}