﻿using Assets.Scripts.Abilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Abilities
{
    public class MenuActionsSingleActionContainer : MonoBehaviour
    {
        public TextMeshProUGUI AbilityTextNameTMP;
        public ActionCell ActionCell;

        public void SetAbility(Ability ability)
        {
            var abilitiesController = GameManager.Instance.PlayerCreature.AbilitiesController;
            ActionCell.AbilityUI.SetAbility(ability, abilitiesController);
            AbilityTextNameTMP.text = ability.AbilitySO.Name;
        }
    }
}