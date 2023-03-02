using System;
using System.Linq;
using Assets.Scripts.Buffs.Behaviours;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Stats
{
    public class StatsController : MonoBehaviour
    {
        private IBaseCreature _iBaseCreature { get; set; }

        private IncreasedDamagePercentage IncreasedDamagePercentage { get; set; }
        private IncreasedOutputThreatPercentage IncreasedOutputThreatPercentage { get; set; }

        [Header("Debug (readonly)")]
        [SerializeField] private float _increaseDamagePercentage = 0;

        [SerializeField] private float _increaseOutputThreatPercentage = 0;

        private void Awake()
        {
            _iBaseCreature = this.GetComponent<IBaseCreature>();
        }

        private void Start()
        {
            IncreasedDamagePercentage = new IncreasedDamagePercentage(_iBaseCreature);
            IncreasedOutputThreatPercentage = new IncreasedOutputThreatPercentage(_iBaseCreature);
        }

        public float GetIncreaseDamagePercentage()
        {
            _increaseDamagePercentage = IncreasedDamagePercentage.GetWrapped().Value;
            return _increaseDamagePercentage;
        }

        public float GetIncreasedOutputThreatPercentage()
        {
            _increaseOutputThreatPercentage = IncreasedOutputThreatPercentage.GetWrapped().Value;
            return _increaseOutputThreatPercentage;
        }

        private void LateUpdate()
        {
            IncreasedDamagePercentage.ResetCache();
            IncreasedOutputThreatPercentage.ResetCache();
        }
    }
}