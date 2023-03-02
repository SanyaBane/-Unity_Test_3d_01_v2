using Assets.Scripts.Abilities.Behaviours.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs;
using Assets.Scripts.Buffs.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Abilities.Behaviours
{
    public class DirectRemoveBuffStack : AbilityBehaviour, IBehaviourWithName
    {
        public DirectRemoveBuffStackSO DirectRemoveBuffStackSO => (DirectRemoveBuffStackSO) base.AbilityBehaviourSO;

        public string Name { get; }
        public bool ShareNameWithAbility { get; }

        public BaseBuffSO BaseBuffSO { get; }

        public DirectRemoveBuffStack(AbilityBehaviourSO abilityBehaviourSO) : base(abilityBehaviourSO)
        {
            Name = DirectRemoveBuffStackSO.Name;
            ShareNameWithAbility = DirectRemoveBuffStackSO.ShareNameWithAbility;
            BaseBuffSO = DirectRemoveBuffStackSO.BaseBuffSO;
        }

        public override void ApplyBehaviour(Ability ability, IAbilityParameters iAbilityParameters)
        {
            if (iAbilityParameters.DefaultAbilityParameters.Source.GetRootObjectTransform() != null)
            {
                var buffsController = iAbilityParameters.DefaultAbilityParameters.Source.BuffsController;
                var buff = buffsController.GetBuffById(BaseBuffSO.Id);

                if (buff == null)
                {
                    Debug.LogError($"Buff with ID: '{BaseBuffSO.Id}' not founded on gameobject with name: '{iAbilityParameters.DefaultAbilityParameters.Source.GetRootObjectTransform().gameObject.name}'.");
                    return;
                }

                buff.RemoveStack();
            }
        }
    }
}