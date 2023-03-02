using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public abstract class BaseUIFrame : MonoBehaviour, IUIFrame
    {
        public abstract void Setup();

        public void ShowUIElement()
        {
            this.gameObject.SetActive(true);
        }
        public void HideUIElement()
        {
            this.gameObject.SetActive(false);
        }
    }
}
