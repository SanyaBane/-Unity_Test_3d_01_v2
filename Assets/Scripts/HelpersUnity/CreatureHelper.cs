using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using Pathfinding;
using UnityEngine;

namespace Assets.Scripts.HelpersUnity
{
    public static class CreatureHelper
    {
        public static ITargetable GetITargetableFromSelectableLayerObject(Transform selectableLayerTransform)
        {
            var creatureInfoContainer = selectableLayerTransform.GetComponentInParent<CreatureInfoContainer>();
            var targetable = creatureInfoContainer.BaseCreature.ITargetable;
            return targetable;
        }

        public static IPlayerController GetIPlayerControllerFromPlayerCreature(Transform playerCreatureInfo)
        {
            var ret = playerCreatureInfo.parent.GetComponent<IPlayerController>();
            return ret;
        }

        public static CreatureLayerInfo GetCreatureLayerInfoInfoFromCreatureLayerObject(Transform creatureLayerTransform)
        {
            var creatureInfoContainer = GetCreatureInfoContainerFromCreatureLayerObject(creatureLayerTransform);
            var ret = creatureInfoContainer.GetComponentInChildren<CreatureLayerInfo>();
            return ret;
        }
        
        public static CreatureInfoContainer GetCreatureInfoContainerFromCreatureLayerObject(Transform creatureLayerTransform)
        {
            var ret = creatureLayerTransform.GetComponentInParent<CreatureInfoContainer>();
            return ret;
        }
        
        public static CreatureInfoContainer GetCreatureInfoContainerFromPlayerLayerObject(Transform playerLayerTransform)
        {
            var ret = playerLayerTransform.GetComponentInParent<CreatureInfoContainer>();
            return ret;
        }
        
        public static CreatureInfoContainer GetCreatureInfoContainerFromPlayerTagObject(Transform playerTagTransform)
        {
            var ret = playerTagTransform.GetComponentInParent<CreatureInfoContainer>();
            return ret;
        }
        
        public static CreatureInfoContainer GetCreatureInfoContainerFromBaseCreature(IBaseCreature baseCreature)
        {
            var ret = baseCreature.GetRootObjectTransform().GetComponent<CreatureInfoContainer>();
            return ret;
        }
        
        public static Seeker GetSeekerFromIBaseCreature(IBaseCreature baseCreature)
        {
            var ret = baseCreature.GetRootObjectTransform().GetComponent<Seeker>();
            return ret;
        }
        
        public static AIPath GetAIPathFromIBaseCreature(IBaseCreature baseCreature)
        {
            var ret = baseCreature.GetRootObjectTransform().GetComponent<AIPath>();
            return ret;
        }
        
        public static AIDestinationSetter GetAIDestinationSetterFromIBaseCreature(IBaseCreature baseCreature)
        {
            var ret = baseCreature.GetRootObjectTransform().GetComponent<AIDestinationSetter>();
            return ret;
        }
    }
}