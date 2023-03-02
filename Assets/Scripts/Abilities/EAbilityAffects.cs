namespace Assets.Scripts.Abilities
{
    [System.FlagsAttribute]
    public enum EAbilityAffects
    {
        Self = 1,
        Allies = 2,
        Enemies = 4
    }
}