using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Factions
{
    // https://youtu.be/O8BMCQnjUe42
    [CreateAssetMenu(menuName = "Factions/FactionSO")]
    public class Faction : ScriptableObject
    {
        public const int MAX_NEGATIVE_RELATION = -25000;
        public const int MAX_POSITIVE_RELATION = +25000;

        public const int RELATION_ENEMIES = -5000;
        public const int RELATION_UNFRIENDLY = -1000;
        public const int RELATION_NEUTRAL = 0;
        public const int RELATION_FRIENDLY = 1000;
        public const int RELATION_ALLIES = 5000;

        public string Title = "New Faction";

        public List<Faction> enemies;
        public List<Faction> unfriends;
        public List<Faction> neutrals;
        public List<Faction> friends;
        public List<Faction> allies;

        public EFactionRelation DefaultRelationToNotListedFactions = EFactionRelation.Neutral;

        //public Dictionary<Faction, int> cache = new Dictionary<Faction, int>();

        public int? GetReputationWith(Faction otherFaction)
        {
            //// pull from cache
            //if (cache.ContainsKey(otherFaction))
            //    return cache[otherFaction];

            // init, if not inside cache

            var ally = allies.Where(x => x == otherFaction).FirstOrDefault();
            if (ally != null)
                return RELATION_ALLIES;

            var friendly = friends.Where(x => x == otherFaction).FirstOrDefault();
            if (friendly != null)
                return RELATION_FRIENDLY;

            var neutral = neutrals.Where(x => x == otherFaction).FirstOrDefault();
            if (neutral != null)
                return RELATION_NEUTRAL;

            var unfriendly = unfriends.Where(x => x == otherFaction).FirstOrDefault();
            if (unfriendly != null)
                return RELATION_UNFRIENDLY;

            var enemy = enemies.Where(x => x == otherFaction).FirstOrDefault();
            if (enemy != null)
                return RELATION_ENEMIES;

            //cache.Add(otherFaction, ret);

            return null;
        }

        public EFactionRelation GetRelationWith(Faction otherFaction)
        {
            if (otherFaction == this)
                return EFactionRelation.Allies; // если одна и та же фракция, по умолчанию будут считаться союзниками

            int? currentReputation = GetReputationWith(otherFaction);

            if (currentReputation == null)
            {
                return DefaultRelationToNotListedFactions;
            }

            if (currentReputation < RELATION_UNFRIENDLY)
                return EFactionRelation.Enemy;
            else if (currentReputation >= RELATION_UNFRIENDLY && currentReputation < RELATION_NEUTRAL)
                return EFactionRelation.Unfriendly;
            else if (currentReputation >= RELATION_NEUTRAL && currentReputation < RELATION_FRIENDLY)
                return EFactionRelation.Neutral;
            else if (currentReputation >= RELATION_FRIENDLY && currentReputation < RELATION_ALLIES)
                return EFactionRelation.Friendly;
            else if (currentReputation >= RELATION_ALLIES)
                return EFactionRelation.Allies;
            else
                throw new NotSupportedException(nameof(GetRelationWith));
        }
    }
}
