using Assets.Scripts.Abilities;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffDuration;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType.ScriptableObjects;
using Assets.Scripts.Buffs.Behaviours;
using Assets.Scripts.Buffs.Behaviours.ScriptableObjects;
using Assets.Scripts.Buffs.ScriptableObjects;
using Assets.Scripts.Interfaces;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Buffs
{
    [DebuggerDisplay("BuffSO Id: {BaseBuffSO.Id};")]
    public class Buff
    {
        public BaseBuffSO BaseBuffSO { get; }

        public BaseBuffDuration BuffDuration { get; }
        public BaseBuffRecastType BuffRecastType { get; }

        public Ability Ability{ get; }
        public IAbilityParameters IAbilityParameters{ get; }

        public IBaseCreature Source { get; }
        
        public DateTime CreationTime { get; }
        
        public Guid Guid { get; }

        private readonly List<BaseBuffBehaviourSO> BuffBehavioursSO;

        public bool IsPurgeableByEsuna => BaseBuffSO.IsPurgeableByEsuna;
        public bool IsFriendly => BaseBuffSO.IsFriendly;

        private float _nextTickTime;

        private int _stacksCount;
        public int StacksCount
        {
            get => _stacksCount;
            set
            {
                _stacksCount = value; 
                StacksCountChanged?.Invoke();
            }
        }
        public event Action StacksCountChanged;

        public bool ReadyToBeRemoved { get; private set; } = false;

        public bool OnlyOneInstance { get; set; }

        public Buff(BaseBuffSO baseBuffSO, Ability ability, IAbilityParameters iAbilityParameters)
        {
            BaseBuffSO = baseBuffSO;

            Ability = ability;
            IAbilityParameters = iAbilityParameters;

            OnlyOneInstance = BaseBuffSO.OnlyOneInstance;

            Source = IAbilityParameters.DefaultAbilityParameters.Source;

            Guid = Guid.NewGuid();

            BuffDuration = BaseBuffSO.DurationSO.CreateBaseBuffDuration();

            if (BaseBuffSO.RecastTypeSO == null)
            {
                BuffRecastType = BuffRecastTypeDefaultUpdateDurationSO.CreateBuffRecastTypeDefaultRestart();
            }
            else
            {
                BuffRecastType = BaseBuffSO.RecastTypeSO.CreateBaseBuffRecastType();
            }

            _nextTickTime = Time.time + Constants.BUFF_TICK_TIME;

            BuffBehavioursSO = new List<BaseBuffBehaviourSO>();
            foreach (var buffBehaviourSO in BaseBuffSO.BuffBehavioursSO)
            {
                BuffBehavioursSO.Add(buffBehaviourSO);
            }

            StacksCount = BaseBuffSO.InitialStacksCount;

            CreationTime = DateTime.Now;
        }

        public void Tick()
        {
            BuffDuration.TickDuration(this);

            if (Time.time >= _nextTickTime)
            {
                foreach (var buffBehaviour in BuffBehavioursSO)
                {
                    if (buffBehaviour is IBuffBehaviourTick iBuffBehaviourTick)
                    {
                        iBuffBehaviourTick.TickBuffBehaviour(Ability, IAbilityParameters);
                    }
                }

                _nextTickTime += Constants.BUFF_TICK_TIME;
            }
        }

        public void RemoveStack()
        {
            StacksCount--;

            if (StacksCount <= 0)
                SetBuffReadyToBeRemoved();
        }

        public void SetBuffReadyToBeRemoved()
        {
            ReadyToBeRemoved = true;
        }
    }
}