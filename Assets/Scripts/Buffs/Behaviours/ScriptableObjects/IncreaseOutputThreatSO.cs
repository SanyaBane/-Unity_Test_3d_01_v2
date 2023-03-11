using Assets.Scripts.Abilities;
using UnityEngine;

namespace Assets.Scripts.Buffs.Behaviours.ScriptableObjects
{
    [CreateAssetMenu(menuName = "BuffBehaviour/IncreaseOutputThreat")]
    public class IncreaseOutputThreatSO : BaseBuffBehaviourSO, IBehaviourWithName, IBuffBehaviourIncreaseOutputThreat
    {
        [Header("BehaviourWithName")]
        [SerializeField] private string _BuffName;
        public string Name => _BuffName;
        
        [SerializeField] private bool _ShareNameWithAbility = true;
        public bool ShareNameWithAbility => _ShareNameWithAbility;
        
        [Header("General")]
        public float IncreasePercentage;

        public float GetIncreaseOutputThreatPercentage() => IncreasePercentage;
    }
}
