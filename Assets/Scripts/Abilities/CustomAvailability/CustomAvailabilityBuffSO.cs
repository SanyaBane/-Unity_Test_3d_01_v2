using System.Collections.Generic;
using Assets.Scripts.Abilities;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs;
using Assets.Scripts.Buffs.ScriptableObjects;
using UnityEngine;

namespace Abilities.CustomAvailability
{
    [CreateAssetMenu(menuName = "Abilities/CustomAvailability/CustomAvailabilityBuff")]
    public class CustomAvailabilityBuffSO : BaseCustomAvailabilitySO
    {
        public List<BaseBuffSO> BuffsAllowCast;

        public override bool IsCanStartCast(IAbilityParameters iAbilityParameters)
        {
            foreach (var buffAllowCast in BuffsAllowCast)
            {
                var foundedBuff = iAbilityParameters.DefaultAbilityParameters.Source.BuffsController.GetBuffById(buffAllowCast.Id);
                if (foundedBuff == null)
                    return false;
            }

            return true;
        }
    }
}