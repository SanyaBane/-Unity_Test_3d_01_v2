using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Abilities.Parameters
{
    public class DefaultAbilityParameters
    {
        public IBaseCreature Source { get; }
        public ITargetable Target { get; }

        public bool IsTargetSameAsSource => Target != null && Target.IBaseCreature == Source;

        public DefaultAbilityParameters(IBaseCreature source, ITargetable target)
        {
            Source = source;
            Target = target;
        }
    }
}