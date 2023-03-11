using Assets.Scripts.Buffs.Behaviours.ScriptableObjects;
using System.Collections.Generic;
using Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.RecastType.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.Buffs.Modifiers.Duration.ScriptableObjects;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;
using UnityEngine;

namespace Assets.Scripts.Buffs.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Buffs/BaseBuff")]
    public class BaseBuffSO : ScriptableObject
    {
        public Sprite Icon;

        public string Id = "";
        // public string Name = "BuffName"; // probably not needed right now. It's enough to have Ability or Behaviour name atm.

        public bool IsPurgeableByEsuna = false;
        public bool IsFriendly = false;

        public bool OnlyOneInstance = true;
        
        public List<BaseBuffBehaviourSO> BuffBehavioursSO;

        public BaseBuffDurationSO DurationSO;
        public BaseBuffRecastTypeSO RecastTypeSO;

        [Header("Stacks")]
        public bool DisplayStacksCountInIcon = false;

        public int InitialStacksCount = 1;

        public Buff CreateBuff(Ability ability, IAbilityParameters iAbilityParameters)
        {
            var buff = new Buff(this, ability, iAbilityParameters);
            return buff;
        }

        private void OnEnable()
        {
            if (DurationSO == null)
            {
                Debug.LogError($"{nameof(DurationSO)} == null for BaseBuffSO.Id = '{Id}'");
            }
        }
    }
}