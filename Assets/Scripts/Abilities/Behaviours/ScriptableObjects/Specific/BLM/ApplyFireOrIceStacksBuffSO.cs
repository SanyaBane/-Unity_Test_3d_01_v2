using System;
using Assets.Scripts.Buffs.Behaviours.ScriptableObjects;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Assets.Scripts.Buffs.ScriptableObjects;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

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