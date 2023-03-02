using Assets.Scripts.Abilities;
using Assets.Scripts.HelpersUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

namespace Assets.Scripts.AutoAttack
{
    public class PlayerAutoAttackController : AutoAttackController
    {
        protected override void Update()
        {
            base.Update();

            if (_health.IsAlive && GameManager.Instance.InputController_WoW.IsWeaponSheathPressed)
                WeaponSheathed = !WeaponSheathed;
        }
    }
}
