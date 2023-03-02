using System;
using Assets.Scripts.Abilities;
using Assets.Scripts.Levels;
using Assets.Scripts.NPC.PartyMember;
using Assets.Scripts.NPC.Tactics;

namespace Assets.Scripts.NPC.PartyMember.PeaceTactics
{
    public class MainTankPeaceTactics : BaseAIPeaceTactics
    {
        public string GetTankStanceAbilityId
        {
            get
            {
                // switch (NpcAI.INpcBaseCreature.CurrentJob)
                switch (NpcAI.Job)
                {
                    case EJob.WAR:
                        return ConstantsAbilities_WAR.ABILITY_WAR_DEFIANCE;
                    case EJob.PAL:
                        return ConstantsAbilities_PAL.ABILITY_PAL_SHIELD_OATH;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string GetTankStanceBuffId
        {
            get
            {
                // switch (NpcAI.INpcBaseCreature.CurrentJob)
                switch (NpcAI.Job)
                {
                    case EJob.WAR:
                        return ConstantsAbilities_WAR.BUFF_WAR_DEFIANCE;
                    case EJob.PAL:
                        return ConstantsAbilities_PAL.BUFF_PAL_SHIELD_OATH;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private AbilityTarget _tankStanceAbility;

        public MainTankPeaceTactics(NpcAI npcAI) : base(npcAI)
        {
            _tankStanceAbility = (AbilityTarget) NpcAI.INpcBaseCreature.AbilitiesController.GetAbilityById(GetTankStanceAbilityId);
        }

        public override void ProcessPeaceTactics()
        {
            base.ProcessPeaceTactics();

            var tankStanceBuff = NpcAI.INpcBaseCreature.BuffsController.GetBuffById(GetTankStanceBuffId);
            if (tankStanceBuff == null)
            {
                if (NpcAI.CanTryCast(_tankStanceAbility))
                {
                    NpcAI.TryCastAbility(_tankStanceAbility);
                }
            }
        }
    }
}