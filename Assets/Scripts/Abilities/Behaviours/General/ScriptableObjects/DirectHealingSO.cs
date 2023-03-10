using Assets.Scripts.Abilities.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours.General.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AbilityBehaviours/DirectHealing")]
    public class DirectHealingSO : AbilityBehaviourSO, IBehaviourWithName
    {
        [Header("BehaviourWithName")]
        [SerializeField] private string _BehaviourName;
        public string Name => _BehaviourName;
        
        [SerializeField] private bool _ShareNameWithAbility = true;
        public bool ShareNameWithAbility => _ShareNameWithAbility;
        
        [Header("General")]
        public int Potency;

        public override AbilityBehaviour CreateAbilityBehaviour()
        {
            var ret = new DirectHealing(this);
            return ret;
        }
    }
}
