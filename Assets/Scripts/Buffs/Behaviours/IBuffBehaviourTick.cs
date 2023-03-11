using Assets.Scripts.Abilities;
using Assets.Scripts.Abilities.Parameters;

namespace Assets.Scripts.Buffs.Behaviours
{
    public interface IBuffBehaviourTick
    {
        void TickBuffBehaviour(Ability ability, IAbilityParameters iAbilityParameters);
    }
}