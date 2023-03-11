using System.Collections.Generic;
using Assets.Scripts.Abilities.Behaviours.Buffs.Remover;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.Remover
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