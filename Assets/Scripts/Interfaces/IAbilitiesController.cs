using System;
using Assets.Scripts.Abilities.Controller;
using Assets.Scripts.Abilities.General;

namespace Assets.Scripts.Interfaces
{
    public interface IAbilitiesController
    {
        IBaseCreature IBaseCreature { get; }
        
        event Action<AbilitiesController, Ability> CastStarted;
        event Action<AbilitiesController, Ability> CastFinished;
        event Action<AbilitiesController, Ability> CastFinishedAndExecuted;

        event Action<AbilitiesController, Ability, float> CastTicked;
        event Action<AbilitiesController, Ability> CastInterrupted;
    }
}
