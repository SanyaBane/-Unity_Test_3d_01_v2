using System.Linq;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using Assets.Scripts.Abilities.Parameters;

namespace Assets.Scripts.Abilities.Behaviours
{
    public class DirectRemoveSingleDebuff : AbilityBehaviour, IBehaviourWithName
    {
        public DirectRemoveSingleDebuffSO DirectRemoveSingleDebuffSO => (DirectRemoveSingleDebuffSO) base.AbilityBehaviourSO;

        public string Name { get; }
        public bool ShareNameWithAbility { get; }

        public DirectRemoveSingleDebuff(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            Name = DirectRemoveSingleDebuffSO.Name;
            ShareNameWithAbility = DirectRemoveSingleDebuffSO.ShareNameWithAbility;
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetRootObjectTransform() != null)
            {
                var buffsController = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.BuffsController;
                var purgeableRuntimeBuffs = buffsController.GetRuntimeBuffs().Where(x => x.IsFriendly == false && x.IsPurgeableByEsuna).ToList();

                if (purgeableRuntimeBuffs.Count == 0)
                    return;

                var buffToPurge = purgeableRuntimeBuffs[purgeableRuntimeBuffs.Count - 1]; // last buff
                buffsController.RemoveRuntimeBuff(buffToPurge);
            }
        }
    }
}