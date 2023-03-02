using System;

namespace Assets.Scripts.Abilities.Modifiers
{
    public class CastSpeedModifier
    {
        // public string ModifierId { get; }
        public Guid ModifierId { get; }

        public bool CanApply = false;

        public float CastSpeedPercentageBonus = 0;

        // public CastSpeedModifier(string modifierId)
        public CastSpeedModifier(Guid modifierId)
        {
            ModifierId = modifierId;
        }
    }
}