using System;
using System.Diagnostics;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Creatures.Combat
{
    [DebuggerDisplay("GO name: {BaseCreature.GetRootObjectTransform().gameObject.name}; Threat: {Threat}")]
    public class SingleCreatureCombatData
    {
        public SingleCreatureCombatData(CombatInfo combatInfo, IBaseCreature baseBaseCreature, IBaseCreature secondCreature)
        {
            CombatInfo = combatInfo;
            BaseCreature = baseBaseCreature;
            SecondCreature = secondCreature;
        }
        
        public CombatInfo CombatInfo { get; }
        public IBaseCreature BaseCreature { get; }
        public IBaseCreature SecondCreature { get; }

        private int _threat;
        public int Threat
        {
            get => _threat;
            set
            {
                _threat = value;

                if (_threat < 0)
                    _threat = 0;

                ThreatChanged?.Invoke(this);
            }
        }
        
        public event Action<SingleCreatureCombatData> ThreatChanged;
    }
}