using Assets.Scripts.Interfaces;
using System;
using Assets.Scripts.Abilities.Behaviours;
using Assets.Scripts.Creatures;
using Assets.Scripts.Factions;
using Assets.Scripts.Managers;
using Assets.Scripts.VFX;
using UnityEngine;

namespace Assets.Scripts.Health
{
    public abstract class BaseHealth : MonoBehaviour
    {
        public enum ImmortalityTypeEnum
        {
            NoDamage = 0,
            ResetHealth = 1
        }

        [Header("Invul")]
        public bool IsInvulnerable = false;

        public ImmortalityTypeEnum InvulnerabilityType = ImmortalityTypeEnum.NoDamage;

        [Header("General")]
        [SerializeField] private int _MaxHP = 100;

        public int MaxHP => _MaxHP;

        // public IBaseCreature IBaseCreature { get; private set; }
        protected IBaseCreature IBaseCreature { get; private set; }

        public event Action OnDeath;

        public void RaiseOnDeathEvent()
        {
            OnDeath?.Invoke();
        }

        [Header("Debug")]
        [SerializeField] int _CurrentHP;

        public int CurrentHP
        {
            get => _CurrentHP;
            protected set
            {
                _CurrentHP = value;

                if (_CurrentHP > MaxHP)
                    _CurrentHP = MaxHP;

                if (_CurrentHP < 0)
                    _CurrentHP = 0;

                CurrentHPChanged?.Invoke(this);
            }
        }

        public event Action<BaseHealth> CurrentHPChanged;

        public void RestoreFullHP()
        {
            CurrentHP = MaxHP;
        }

        /// <summary>
        /// Checks if <see cref="CurrentHP"/> is above zero.
        /// </summary>
        public bool IsAlive => this.CurrentHP > 0;

        private void Awake()
        {
            RestoreFullHP();
        }

        private void Start()
        {
            var creatureInfoContainer = GetComponentInParent<CreatureInfoContainer>();

            IBaseCreature = creatureInfoContainer.BaseCreature;
            if (IBaseCreature == null)
                Debug.LogError($"{nameof(IBaseCreature)} == null");
        }

        // private void 

        #region Damaging

        public void TryInflictDamage(DamageInfo damageInfo)
        {
            if (CanInflictDamage)
                TakeDamage(damageInfo);
        }

        private bool CanInflictDamage
        {
            get { return IBaseCreature.ITargetable.CanBeAttacked; }
        }

        protected virtual void TakeDamage(DamageInfo damageInfo)
        {
            // if creature already dying, do nothing
            if (CurrentHP <= 0)
            {
                return;
            }

            int damageValue = damageInfo.Damage;

            if (IsInvulnerable && InvulnerabilityType == ImmortalityTypeEnum.NoDamage)
            {
                damageValue = 0;
            }

            var damageSource = damageInfo.IAbilityParameters.DefaultAbilityParameters.Source;
            var relationWithDamageSource = IBaseCreature.Faction.GetRelationWith(damageSource.Faction);

            // If creature inflict damage to enemy creature, then engage to combat with it
            if (damageSource != IBaseCreature)
            {
                if (relationWithDamageSource <= EFactionRelation.Neutral)
                {
                    var combatInfo = IBaseCreature.CombatInfoHandler.GetCombatInfoBySecondCreature(damageSource);
                    if (combatInfo == null)
                    {
                        IBaseCreature.CombatInfoHandler.EngageCombat(damageSource);
                        
                        combatInfo = IBaseCreature.CombatInfoHandler.GetCombatInfoBySecondCreature(damageSource);
                    }

                    int outputThreat = damageInfo.CalculateThreat(damageValue);

                    // TODO probably can be optimized to receive "CombatInfo" as a one of results from "EngageCombat()"
                    combatInfo.AddThreatFromCreature(damageSource, outputThreat);
                }
                else
                {
                    Debug.LogError($"GameObject with name '{IBaseCreature.GetRootObjectTransform().gameObject.name}' has inflicted damage to non-enemy creature with ability '{damageInfo.Ability.AbilitySO.Name}'.");
                }
            }

            string displayName = "";
            if (damageInfo.IBehaviourWithName != null)
            {
                displayName = damageInfo.IBehaviourWithName.ShareNameWithAbility ? damageInfo.Ability.AbilitySO.Name : damageInfo.IBehaviourWithName.Name;
            }

            var damageVisual = new DamageVisual(damageInfo.IAbilityParameters.DefaultAbilityParameters.Source, damageValue, damageInfo.Ability, displayName);

            TrySpawnDamageFloatingText(damageVisual, false);

            CurrentHP -= damageValue;

            if (CurrentHP == 0)
            {
                if (IsInvulnerable && InvulnerabilityType == ImmortalityTypeEnum.ResetHealth)
                {
                    CurrentHP = MaxHP;
                }
                else
                {
                    Die();
                }
            }
        }

        #endregion

        #region Healing

        public void TryInflictHealing(DamageInfo damageInfo)
        {
            if (CanInflictHealing)
                TakeHeal(damageInfo);
        }

        private bool CanInflictHealing
        {
            get { return IBaseCreature.ITargetable.CanBeAttacked; }
        }

        protected virtual void TakeHeal(DamageInfo damageInfo)
        {
            int healValue = damageInfo.Damage;

            string displayName = "";
            if (damageInfo.IBehaviourWithName != null)
            {
                displayName = damageInfo.IBehaviourWithName.ShareNameWithAbility ? damageInfo.Ability.AbilitySO.Name : damageInfo.IBehaviourWithName.Name;
            }

            var damageVisual = new DamageVisual(damageInfo.IAbilityParameters.DefaultAbilityParameters.Source, healValue, damageInfo.Ability, displayName);

            TrySpawnDamageFloatingText(damageVisual, true);

            CurrentHP += damageVisual.Amount;
        }

        #endregion

        private void TrySpawnDamageFloatingText(DamageVisual damageVisual, bool isHealing)
        {
            bool canDisplayAbilityName = false;

            if (damageVisual.Source.CreatureAttitude == CreatureAttitudeEnum.Player)
            {
                if (GameManager.Instance.SettingsManager.DisplayPlayerAbilityNames)
                    canDisplayAbilityName = true;

                if (!GameManager.Instance.SettingsManager.DisplayPlayerDealingDamage && !isHealing)
                {
                    return;
                }

                if (!GameManager.Instance.SettingsManager.DisplayPlayerDealingHealing && isHealing)
                {
                    return;
                }
            }
            else if (damageVisual.Source.CreatureAttitude == CreatureAttitudeEnum.PartyMemberMob)
            {
                if (GameManager.Instance.SettingsManager.DisplayPartyMembersAbilityNames)
                    canDisplayAbilityName = true;

                if (!GameManager.Instance.SettingsManager.DisplayPartyMembersDealingDamage && !isHealing)
                {
                    return;
                }

                if (!GameManager.Instance.SettingsManager.DisplayPartyMembersDealingHealing && isHealing)
                {
                    return;
                }
            }
            else if (damageVisual.Source.CreatureAttitude == CreatureAttitudeEnum.Mob)
            {
                if (GameManager.Instance.SettingsManager.DisplayMobsAbilityNames)
                    canDisplayAbilityName = true;

                if (!GameManager.Instance.SettingsManager.DisplayMobsDealingDamage && !isHealing)
                {
                    return;
                }

                if (!GameManager.Instance.SettingsManager.DisplayMobsDealingHealing && isHealing)
                {
                    return;
                }
            }

            GameObject damageTextContainerGameObject = GameManager.Instance.FloatingTextDamagePool.GetPooledObject();
            if (damageTextContainerGameObject == null)
            {
                Debug.LogError($"{nameof(damageTextContainerGameObject)} == null");
                return;
            }

            var floatingTextDamageUI = damageTextContainerGameObject.GetComponent<FloatingTextDamageUI>();

            string text;
            if (string.IsNullOrWhiteSpace(damageVisual.DisplayName) || canDisplayAbilityName == false)
                text = damageVisual.Amount.ToString();
            else
                text = $"{damageVisual.DisplayName} {damageVisual.Amount}";

            var animMode = isHealing ? FloatingTextDamageUI.EAnimationMode.MoveDown : FloatingTextDamageUI.EAnimationMode.MoveUp;
            var color = isHealing ? Constants.COLOR_HEAL : Constants.COLOR_DAMAGE;
            floatingTextDamageUI.Setup(IBaseCreature.AttachmentsController.Attach_Hitloc, text, color, animMode);
        }

        public virtual void Die()
        {
            _CurrentHP = 0;

            IBaseCreature.ITargetable.CanBeAttacked = false;

            IBaseCreature.CombatInfoHandler.DisengageCombatWithEveryone();
        }

        //public virtual void Ressurect()
        //{
        //    _baseCreature.Targetable.CanBeAttacked = true;
        //}
    }
}