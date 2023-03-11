using Assets.Scripts.Abilities.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/ProvokeEnemy")]
    public class ProvokeEnemySO : AbilityBehaviourSO, IBehaviourWithName
    {
        [Header("BehaviourWithName")]
        [SerializeField] private string _BehaviourName;

        public string Name => _BehaviourName;

        [SerializeField] private bool _ShareNameWithAbility = true;
        public bool ShareNameWithAbility => _ShareNameWithAbility;

        [Header("General")]
        public int BonusThreatValue = 500;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new ProvokeEnemy(this);
            return ret;
        }
    }
}