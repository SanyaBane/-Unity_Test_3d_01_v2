using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.OnCreateCustomLogic.ScriptableObjects;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Abilities.OnCreateCustomLogic
{
    public abstract class BaseOnCreateCustomLogic
    {
        public BaseOnCreateCustomLogicSO BaseOnCreateCustomLogicSO { get; }
        public IAbilitiesController IAbilitiesController { get; }
        public Ability Ability { get; }

        public BaseOnCreateCustomLogic(BaseOnCreateCustomLogicSO baseOnCreateCustomLogicSO, IAbilitiesController iAbilitiesController, Ability ability)
        {
            BaseOnCreateCustomLogicSO = baseOnCreateCustomLogicSO;
            IAbilitiesController = iAbilitiesController;
            Ability = ability;
        }
    }
}