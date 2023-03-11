using System.Collections.Generic;
using Assets.Scripts.Abilities.Behaviours.General;
using Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.Buffs.Remover.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/BuffRemover")]
    public class BuffRemoverSO : AbilityBehaviourSO
    {
        public enum EBuffRemoverTarget
        {
            Caster,
            Target
        }

        public List<string> BuffsId;

        public EBuffRemoverTarget BuffRemoverTarget = EBuffRemoverTarget.Caster;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new BuffRemover(this);
            return ret;
        }
    }
}