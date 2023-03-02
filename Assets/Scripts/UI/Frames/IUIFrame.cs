﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.UI
{
    public interface IUIFrame
    {
        void ShowUIElement();
        void HideUIElement();
        void Setup();
    }
}