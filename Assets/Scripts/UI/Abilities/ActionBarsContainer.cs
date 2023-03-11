using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UI;
using UnityEngine;

public class ActionBarsContainer : MonoBehaviour
{
    public List<ActionBar> ActionBars { get; private set; }
    
    private void Awake()
    {
        ActionBars = this.GetComponentsInChildren<ActionBar>().ToList();
    }
}
