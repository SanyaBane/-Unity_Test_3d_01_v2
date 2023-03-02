using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Factions
{
    public enum EFactionRelation
    {
        Enemy = -101,
        Unfriendly = -100,

        Neutral = 0, // default

        Friendly = 100,
        Allies = 101,
    }
}
