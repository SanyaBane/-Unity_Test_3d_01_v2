using Assets.Scripts.Abilities;

namespace Assets.Scripts.Buffs.Behaviours
{
    public interface IBuffBehaviourTick
    {
        void TickBuffBehaviour(Ability ability, IAbilityParameters iAbilityParameters);
    }
}