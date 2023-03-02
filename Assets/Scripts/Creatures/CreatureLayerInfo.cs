using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class CreatureLayerInfo : MonoBehaviour
    {
        public IBaseCreature IBaseCreature { get; private set; }

        private void Start()
        {
            IBaseCreature = this.GetComponentInParent<IBaseCreature>();
        }
    }
}
