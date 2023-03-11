using System;
using Assets.Scripts.Buffs.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/Specific/BLM/ApplyFireOrIceStacksBuff")]
    public class ApplyFireOrIceStacksBuffSO : AbilityBehaviourSO
    {
        public BaseBuffSO BuffToApplySO;
        public string BuffIdToRemove;

        public bool ApplyBuffIfBuffToRemoveExist = false;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new ApplyFireOrIceStacksBuff(this);
            return ret;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (BuffToApplySO == null)
                Debug.LogError($"{nameof(BuffToApplySO)} == null");
            
            if (String.IsNullOrWhiteSpace(BuffIdToRemove))
                Debug.LogError($"{nameof(BuffIdToRemove)} IsNullOrWhiteSpace");
        }
    }
}