namespace Assets.Scripts.Interfaces
{
    public interface IDamageInflicter
    {
        string Name { get; }
        string DisplayName { get; }
        bool IsShowNameNearAmount { get; }
    }
}
