namespace Assets.Scripts.Abilities
{
    public interface IBehaviourWithName
    {
        string Name { get; }
        bool ShareNameWithAbility { get; }
    }
}