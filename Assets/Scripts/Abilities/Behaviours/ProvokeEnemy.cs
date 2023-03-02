using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours
{
    public class ProvokeEnemy : AbilityBehaviour, IBehaviourWithName
    {
        public ProvokeEnemySO ProvokeEnemySO => (ProvokeEnemySO) base.AbilityBehaviourSO;

        public string Name { get; }
        public bool ShareNameWithAbility { get; }

        public int BonusThreatValue { get; }

        public ProvokeEnemy(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            Name = ProvokeEnemySO.Name;
            ShareNameWithAbility = ProvokeEnemySO.ShareNameWithAbility;
            BonusThreatValue = ProvokeEnemySO.BonusThreatValue;
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetRootObjectTransform() != null)
            {
                var sourceCreature = iAbilityParameters.DefaultAbilityParameters.Source;

                var maxThreatCombatInfo = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.CombatInfoHandler.ThreatResolver.GetCombatInfoWithMaxInputThreat();
                if (maxThreatCombatInfo == null)
                {
                    sourceCreature.CombatInfoHandler.EngageCombat(iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature);
                    return;
                }

                var sourceCreatureCombatInfo = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.CombatInfoHandler.GetCombatInfoBySecondCreature(sourceCreature);
                if (sourceCreatureCombatInfo == null)
                {
                    sourceCreatureCombatInfo = sourceCreature.CombatInfoHandler.EngageCombat(iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature);
                }

                if (maxThreatCombatInfo == sourceCreatureCombatInfo)
                    return;

                int maxThreatToTarget = maxThreatCombatInfo.GetThreatToCreature(iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature);
                // Debug.Log($"maxThreatToTarget: {maxThreatToTarget}; BonusThreatValue: {BonusThreatValue}");
                sourceCreatureCombatInfo.ChangeThreatFromCreature(sourceCreature, maxThreatToTarget + BonusThreatValue);

                // sourceCreatureCombatInfo.ChangeThreatFromCreature(sourceCreature, BonusThreatValue);
            }
        }
    }
}