using System;
using Assets.Scripts.Factions;
using Assets.Scripts.Abilities.Controller;
using Assets.Scripts.Attachments;
using Assets.Scripts.Buffs;
using Assets.Scripts.Creatures;
using Assets.Scripts.AutoAttack;
using Assets.Scripts.Creatures.Combat;
using Assets.Scripts.Enums;
using Assets.Scripts.Health;
using Assets.Scripts.ManaSystem;
using Assets.Scripts.Stats;
using Assets.Scripts.TargetHandling;
using QuickOutline;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IBaseCreature : IHasTransform, IHaveHitloc
    {
        bool IsGrounded { get; set; }
        bool IsMoving { get; set; }
        bool IsTurning { get; set; }

        CreatureInfoContainer CreatureInfoContainer { get; }
        CreatureMeasures CreatureMeasures { get; }
        AttachmentsController AttachmentsController { get; }
        BaseHealth Health { get; }
        BaseManaController ManaController { get; }
        Faction Faction { get; }
        ITargetable ITargetable { get; }
        CreatureSO CreatureSO { get; }
        Animator Animator { get; }
        AbilitiesController AbilitiesController { get; }
        BuffsController BuffsController { get; }
        CombatInfoHandler CombatInfoHandler { get; }
        StatsController StatsController { get; }
        AutoAttackController AutoAttackController { get; }
        ICanSelectTarget ICanSelectTarget { get; }
        CreatureAttitudeEnum CreatureAttitude { get; }
        TargetHandler TargetHandler { get; }
        Outline Outline { get; }
        
        EJob CurrentJob { get; }
        PartyController PartyController { get; }

        event Action BeforeCreatureDestroy;
        void DestroyCreature(float delay);

        //bool CanBeAttacked { get; }
        //event Action<bool> CanBeAttackedChanged;
    }
}
