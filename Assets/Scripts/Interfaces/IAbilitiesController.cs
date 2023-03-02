using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Abilities;
using Assets.Scripts.Creatures;
using UnityEngine;

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
