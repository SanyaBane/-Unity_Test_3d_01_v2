using Assets.Scripts.Interfaces;
using System.Diagnostics;
using Assets.Scripts.Abilities.General;

namespace Assets.Scripts.Health
{
    [DebuggerDisplay("Amount: {Amount}; DisplayName: {DisplayName};")]
    public class DamageVisual
    {
        public IBaseCreature Source { get; set; }
        public int Amount { get; set; }
        public Ability Ability { get; private set; }
        public string DisplayName { get; private set; }

        public DamageVisual(IBaseCreature source, int amount, Ability ability, string displayName)
        {
            Source = source;
            Amount = amount;
            Ability = ability;
            DisplayName = displayName;
        }
    }
}
