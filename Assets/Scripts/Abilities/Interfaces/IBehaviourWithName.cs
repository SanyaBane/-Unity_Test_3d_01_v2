namespace Assets.Scripts.Abilities.Interfaces
{
    public interface IBehaviourWithName
    {
        string Name { get; }
        bool ShareNameWithAbility { get; }
    }
}