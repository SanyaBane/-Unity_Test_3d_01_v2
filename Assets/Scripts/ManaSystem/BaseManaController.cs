using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.ManaSystem.Modifiers;
using Assets.Scripts.ManaSystem.Modifiers.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.ManaSystem
{
    public class BaseManaController : MonoBehaviour
    {
        public const int MAX_MANA = 10000;

        public const int MANA_REGEN_IN_COMBAT = 200;
        public const int MANA_REGEN_OUTSIDE_COMBAT = 600;

        protected IBaseCreature IBaseCreature { get; private set; }

        public List<BaseManaModifierSO> ManaModifiersSO;
        private List<BaseManaModifier> _manaModifiers = new List<BaseManaModifier>();

        [Header("Debug")]
        [SerializeField] int _CurrentMP;

        public int CurrentMP
        {
            get => _CurrentMP;
            private set
            {
                _CurrentMP = value;

                if (_CurrentMP > MAX_MANA)
                    _CurrentMP = MAX_MANA;

                if (_CurrentMP < 0)
                    _CurrentMP = 0;

                CurrentMPChanged?.Invoke(this);
            }
        }

        public event Action<BaseManaController> CurrentMPChanged;

        public void RestoreFullMP()
        {
            CurrentMP = MAX_MANA;
        }

        private void Awake()
        {
            RestoreFullMP();
        }

        private void Start()
        {
            var creatureInfoContainer = GetComponentInParent<CreatureInfoContainer>();

            IBaseCreature = creatureInfoContainer.BaseCreature;
            if (IBaseCreature == null)
                Debug.LogError($"{nameof(IBaseCreature)} == null");

            foreach (var manaModifierSO in ManaModifiersSO)
            {
                if (manaModifierSO == null)
                {
                    Debug.LogError($"{nameof(manaModifierSO)} == null");
                    continue;
                }

                var manaModifier = manaModifierSO.CreateBaseManaModifier(IBaseCreature);
                _manaModifiers.Add(manaModifier);
            }

            GameManager.Instance.ManaTicked += GameManagerOnManaTicked;
        }

        private void GameManagerOnManaTicked()
        {
            int? usedManaModifierMinPriority = null;
            foreach (var manaModifier in _manaModifiers.Where(x => x.ManaModifierType == BaseManaModifierSO.EManaModifierType.OnGameTick))
            {
                if (manaModifier.CanModify() == false)
                    continue;

                if (usedManaModifierMinPriority == null || usedManaModifierMinPriority.Value < manaModifier.Priority)
                    usedManaModifierMinPriority = manaModifier.Priority;
            }

            if (usedManaModifierMinPriority == null)
            {
                if (IBaseCreature.CombatInfoHandler.IsInCombat)
                {
                    CurrentMP += MANA_REGEN_IN_COMBAT;
                }
                else
                {
                    CurrentMP += MANA_REGEN_OUTSIDE_COMBAT;
                }
            }
            else
            {
                foreach (var manaModifier in _manaModifiers.Where(x => x.ManaModifierType == BaseManaModifierSO.EManaModifierType.OnGameTick && x.Priority == usedManaModifierMinPriority.Value))
                {
                    if (manaModifier.CanModify() == false)
                        continue;

                    CurrentMP = manaModifier.Modify(CurrentMP);
                }
            }
        }

        public bool CanSpentAmountOfMana(int amount)
        {
            return CurrentMP >= amount;
        }

        public void SpentAmountOfMana(int amount)
        {
            if (!CanSpentAmountOfMana(amount))
                throw new NotEnoughManaException();

            CurrentMP -= amount;
        }

        public class NotEnoughManaException : Exception
        {
        }
    }
}