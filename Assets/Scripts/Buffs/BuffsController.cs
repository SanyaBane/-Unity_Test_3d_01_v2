using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Buffs.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Buffs
{
    public class BuffsController : MonoBehaviour
    {
        public event Action<Buff> BuffAdded;
        public event Action<Buff> BuffRemoved;
        public event Action<Buff> BuffDurationUpdated;

        public void RaiseBuffDurationUpdated(Buff buff)
        {
            BuffDurationUpdated?.Invoke(buff);
        }

        // [SerializeField] private List<BaseBuffSO> _permanentBuffsSO;

        private readonly List<Buff> _defaultBuffs = new List<Buff>();
        private readonly List<Buff> _runtimeBuffs = new List<Buff>();

        public IEnumerable<Buff> GetAllBuffs()
        {
            var ret = _runtimeBuffs.Concat(_defaultBuffs);
            return ret;
        }

        public IEnumerable<Buff> GetDefaultBuffs()
        {
            var ret = _defaultBuffs.AsEnumerable();
            return ret;
        }

        public IEnumerable<Buff> GetRuntimeBuffs()
        {
            var ret = _runtimeBuffs.AsEnumerable();
            return ret;
        }

        public Buff GetBuffById(string id)
        {
            var ret = GetAllBuffs().FirstOrDefault(x => x.BaseBuffSO.Id == id);
            return ret;
        }

        // public IEnumerable<Buff> GetCurrentBuffsBehaviours()
        // {
        //     var ret = GetCurrentBuffs();
        //     return ret;
        // }

        public bool CanApplyRuntimeBuff { get; set; } = true;

        public void ApplyBuff(BaseBuffSO baseBuffSO, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
            if (isRuntimeBuff && CanApplyRuntimeBuff == false)
                return;

            //Debug.Log($"Apply buff '{baseBuffSO.Name}' to '{IBaseCreature.CreatureSO.Name}'.");

            var existingSameBuff = _runtimeBuffs.FirstOrDefault(x => x.BaseBuffSO.Id == baseBuffSO.Id && (x.OnlyOneInstance || x.Source == iAbilityParameters.DefaultAbilityParameters.Source));
            if (existingSameBuff != null)
            {
                var buff = baseBuffSO.CreateBuff(ability, iAbilityParameters);
                
                buff.BuffRecastType.ApplyRecastLogic(this, existingSameBuff, ability, iAbilityParameters, isRuntimeBuff);
                buff.BuffRecastType.ApplyPostRecastLogic(this, existingSameBuff, ability, iAbilityParameters, isRuntimeBuff);
                
                // existingSameBuff.BuffRecastType.ApplyRecastLogic(this, existingSameBuff, ability, iAbilityParameters, isRuntimeBuff);
            }
            else
            {
                CreateAndAddBuff(baseBuffSO, ability, iAbilityParameters, isRuntimeBuff);
            }
        }

        public void CreateAndAddBuff(BaseBuffSO baseBuffSO, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
            var buff = baseBuffSO.CreateBuff(ability, iAbilityParameters);

            CreateAndAddBuff(buff, ability, iAbilityParameters, isRuntimeBuff);
        }
        
        public void CreateAndAddBuff(Buff buff, Ability ability, IAbilityParameters iAbilityParameters, bool isRuntimeBuff)
        {
            if (isRuntimeBuff)
                _runtimeBuffs.Add(buff);
            else
                _defaultBuffs.Add(buff);

            BuffAdded?.Invoke(buff);
        }

        private void Update()
        {
            UpdateBuffs(_defaultBuffs);
            UpdateBuffs(_runtimeBuffs);
        }

        private void UpdateBuffs(IList<Buff> buffs)
        {
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                var buff = buffs[i];

                buff.Tick(); // maybe it should be before Duration check?

                if (buff.ReadyToBeRemoved)
                {
                    //Debug.Log($"Remove buff '{buff.BaseBuffSO.Name}' from '{IBaseCreature.CreatureSO.Name}'.");

                    BuffRemoved?.Invoke(buff);
                    buffs.RemoveAt(i);
                }
            }
        }

        private void RemoveBuff(Buff buff, bool runtimeBuff = true)
        {
            BuffRemoved?.Invoke(buff);

            if (runtimeBuff)
                _runtimeBuffs.Remove(buff);
            else
                _defaultBuffs.Remove(buff);
        }

        public void RemoveRuntimeBuff(Buff buff)
        {
            RemoveBuff(buff, true);
        }

        public void ResetRuntimeBuffs()
        {
            for (int i = _runtimeBuffs.Count - 1; i >= 0; i--)
            {
                var buff = _runtimeBuffs[i];

                RemoveRuntimeBuff(buff);
            }
        }
    }
}