using Assets.Scripts.Interfaces;
using System;
using System.Diagnostics;

namespace Assets.Scripts.Creatures.Combat
{
    [DebuggerDisplay("Creature1: {SingleCreatureCombatData1.BaseCreature.GetRootObjectTransform().gameObject.name}; Creature2: {SingleCreatureCombatData2.BaseCreature.GetRootObjectTransform().gameObject.name};")]
    public class CombatInfo
    {
        public SingleCreatureCombatData SingleCreatureCombatData1 { get; }
        public SingleCreatureCombatData SingleCreatureCombatData2 { get; }

        public CombatInfo(IBaseCreature creature1, IBaseCreature creature2)
        {
            this.SingleCreatureCombatData1 = new SingleCreatureCombatData(this, creature1, creature2);
            this.SingleCreatureCombatData2 = new SingleCreatureCombatData(this, creature2, creature1);
        }

        public SingleCreatureCombatData GetSingleCreatureCombatData(IBaseCreature baseCreature)
        {
            if (SingleCreatureCombatData1.BaseCreature == baseCreature)
                return SingleCreatureCombatData1;

            if (SingleCreatureCombatData2.BaseCreature == baseCreature)
                return SingleCreatureCombatData2;

            throw new ArgumentException($"{nameof(GetSingleCreatureCombatData)}");
        }

        public IBaseCreature GetSecondCreature(IBaseCreature firstCreature)
        {
            if (SingleCreatureCombatData1.BaseCreature == firstCreature)
                return SingleCreatureCombatData2.BaseCreature;

            if (SingleCreatureCombatData2.BaseCreature == firstCreature)
                return SingleCreatureCombatData1.BaseCreature;

            throw new ArgumentException($"{nameof(GetSecondCreature)}");
        }

        public SingleCreatureCombatData GetSecondSingleCreatureCombatData(IBaseCreature firstCreature)
        {
            if (SingleCreatureCombatData1.BaseCreature == firstCreature)
                return SingleCreatureCombatData2;

            if (SingleCreatureCombatData2.BaseCreature == firstCreature)
                return SingleCreatureCombatData1;

            throw new ArgumentException($"{nameof(GetSecondSingleCreatureCombatData)}");
        }

        public int GetThreatFromCreature(IBaseCreature creature)
        {
            SingleCreatureCombatData creatureCombatInfoData = GetSingleCreatureCombatData(creature);
            return creatureCombatInfoData.Threat;
        }
        
        public int GetThreatToCreature(IBaseCreature creature)
        {
            var secondCreature = GetSecondCreature(creature);
            SingleCreatureCombatData creatureCombatInfoData = GetSingleCreatureCombatData(secondCreature);
            return creatureCombatInfoData.Threat;
        }

        public void AddThreatFromCreature(IBaseCreature fromCreature, int threat)
        {
            SingleCreatureCombatData creatureCombatInfoData = GetSingleCreatureCombatData(fromCreature);
            creatureCombatInfoData.Threat += threat;
        }

        public void ChangeThreatFromCreature(IBaseCreature fromCreature, int threat)
        {
            SingleCreatureCombatData creatureCombatInfoData = GetSingleCreatureCombatData(fromCreature);
            creatureCombatInfoData.Threat = threat;
        }
        
        /// <remarks>
        /// Should be called only from <see cref="CombatInfoHandler"/>.
        /// </remarks>
        public void Engage()
        {
            SingleCreatureCombatData1.BaseCreature.CombatInfoHandler.AddCombatInfo(this);
            SingleCreatureCombatData2.BaseCreature.CombatInfoHandler.AddCombatInfo(this);
        }
        
        /// <remarks>
        /// Should be called only from <see cref="CombatInfoHandler"/>.
        /// </remarks>
        public void Disengage()
        {
            SingleCreatureCombatData1.BaseCreature.CombatInfoHandler.RemoveCombatInfo(this);
            SingleCreatureCombatData2.BaseCreature.CombatInfoHandler.RemoveCombatInfo(this);
        }
    }
}