using Assets.Scripts.Interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.Abilities.Parameters
{
    public class AbilityParametersAOE : AbilityParameters//, IAbilityParametersAOE
    {
        public List<IBaseCreature> Targets { get; set; }
    }
}
