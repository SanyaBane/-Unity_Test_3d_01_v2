using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UI.Frames;
using UnityEngine;

namespace Assets.Scripts.UI.Abilities
{
    public class ActionBarsContainer : MonoBehaviour
    {
        public List<ActionBar> ActionBars { get; private set; }
        
        private void Awake()
        {
            ActionBars = this.GetComponentsInChildren<ActionBar>().ToList();
        }
    }
}
