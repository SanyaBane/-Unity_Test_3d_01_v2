using System;
using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Creatures;
using Assets.Scripts.Creatures.Combat;
using Assets.Scripts.Enums;
using Assets.Scripts.Factions;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Frames
{
    public class PlayerFrame : CreatureFrame
    {
        [Serializable]
        public struct JobIconsMapper
        {
            public EJob Job;
            public Sprite Sprite;
        }

        [SerializeField] private Image JobIcon;
        [SerializeField] private Transform AggroContainer;
        [SerializeField] private Image AggroFiller;
        [SerializeField] private List<JobIconsMapper> JobIconsMapperList = new List<JobIconsMapper>();

        private PlayerFrameTargetInfo _playerFrameTargetInfo;

        public PartyFrame PartyFrame { get; set; }

        public override void SetNewOwner(IBaseCreature newFrameOwner)
        {
            base.SetNewOwner(newFrameOwner);

            if (ReferenceEquals(CurrentFrameOwner, GameManager.Instance.PlayerCreature))
            {
                PartyFrame = GameManager.Instance.GUIManager.PartyFrame;
            }

            var jobIcon = JobIconsMapperList.First(x => x.Job == newFrameOwner.CurrentJob);
            JobIcon.sprite = jobIcon.Sprite;
        }

        protected override void NewOwnerSubscribe(IBaseCreature frameOwner)
        {
            base.NewOwnerSubscribe(frameOwner);

            GameManager.Instance.PlayerCreature.PlayerTargetHandler.SelectedTargetChanged += PlayerTargetHandler_OnSelectedTargetChanged;
        }

        protected override void OldOwnerUnsubscribe(IBaseCreature frameOwner)
        {
            base.OldOwnerUnsubscribe(frameOwner);

            GameManager.Instance.PlayerCreature.PlayerTargetHandler.SelectedTargetChanged -= PlayerTargetHandler_OnSelectedTargetChanged;
        }

        protected override void Update()
        {
            // if (_playerFrameTargetInfo.PlayerTarget == null)
            //     return;

            // var combatInfoWithPlayer = _playerTarget.IBaseCreature.CombatInfoHandler.GetCombatInfo(CurrentFrameOwner);
            // if (combatInfoWithPlayer != null)
            // {
            //     var threatFromCreatureToTarget = combatInfoWithPlayer.GetThreatFromCreature(CurrentFrameOwner);
            //     Debug.Log($"{CurrentFrameOwner.GetRootObjectTransform().gameObject.name} - {nameof(threatFromCreatureToTarget)}: {threatFromCreatureToTarget}.");
            // }
        }

        private void PlayerTargetHandler_OnSelectedTargetChanged(ITargetable target)
        {
            if (_playerFrameTargetInfo != null && _playerFrameTargetInfo.IsSubscribedToCombatInfo)
            {
                _playerFrameTargetInfo.PlayerTarget.IBaseCreature.CombatInfoHandler.ThreatResolver.IncomeThreatChanged -= CombatInfoHandlerOnIncomeThreatChanged;
                _playerFrameTargetInfo.PlayerTarget.IBaseCreature.CombatInfoHandler.ThreatResolver.OutcomeThreatChanged -= CombatInfoHandlerOnOutcomeThreatChanged;
            }

            _playerFrameTargetInfo = new PlayerFrameTargetInfo(target);

            if (_playerFrameTargetInfo.PlayerTarget == null)
            {
                AggroContainer.gameObject.SetActive(false);
            }
            else
            {
                var relationFromPlayerToTarget = target.IBaseCreature.Faction.GetRelationWith(GameManager.Instance.PlayerCreature.Faction);
                if (relationFromPlayerToTarget <= EFactionRelation.Neutral)
                {
                    // AggroContainer.gameObject.SetActive(true);

                    var combatInfo = _playerFrameTargetInfo.PlayerTarget.IBaseCreature.CombatInfoHandler.GetCombatInfoBySecondCreature(CurrentFrameOwner);
                    if (combatInfo != null)
                    {
                        AggroContainer.gameObject.SetActive(true);
                        CombatInfoHandlerOnIncomeThreatChanged(combatInfo.GetSingleCreatureCombatData(CurrentFrameOwner));
                    }

                    _playerFrameTargetInfo.PlayerTarget.IBaseCreature.CombatInfoHandler.ThreatResolver.IncomeThreatChanged += CombatInfoHandlerOnIncomeThreatChanged;
                    _playerFrameTargetInfo.PlayerTarget.IBaseCreature.CombatInfoHandler.ThreatResolver.OutcomeThreatChanged += CombatInfoHandlerOnOutcomeThreatChanged;
                    _playerFrameTargetInfo.IsSubscribedToCombatInfo = true;
                }
                else
                {
                    AggroContainer.gameObject.SetActive(false);
                }
            }
        }

        private void CombatInfoHandlerOnIncomeThreatChanged(SingleCreatureCombatData singleCreatureCombatData)
        {
            if (_playerFrameTargetInfo.CombatInfo == null)
                _playerFrameTargetInfo.CombatInfo = _playerFrameTargetInfo.PlayerTarget.IBaseCreature.CombatInfoHandler.GetCombatInfoBySecondCreature(CurrentFrameOwner);

            if (_playerFrameTargetInfo.CombatInfo != null)
            {
                _playerFrameTargetInfo.PlayerTarget.IBaseCreature.CombatInfoHandler.CombatInfosRemoved += CombatInfoHandlerOnCombatInfosRemoved;

                // if (singleCreatureCombatData.BaseCreature == CurrentFrameOwner)
                // {
                //     Debug.Log($"{CurrentFrameOwner.GetRootObjectTransform().gameObject.name}: OnThreatChanged. " +
                //               $"Threat from \"{singleCreatureCombatData.BaseCreature.GetRootObjectTransform().gameObject.name}\" to\" {singleCreatureCombatData.SecondCreature.GetRootObjectTransform().gameObject.name}\": {singleCreatureCombatData.Threat}");
                // }

                var maxThreatInsideParty = PartyFrame.GetMaxThreatInsidePartyForCreature(singleCreatureCombatData.SecondCreature);
                // Debug.Log(maxThreatInsideParty);

                int threat = _playerFrameTargetInfo.CombatInfo.GetThreatFromCreature(CurrentFrameOwner);
                // int threat =  singleCreatureCombatData.Threat;
                float relationCurrentThreatToMax = 100 * threat / (float) maxThreatInsideParty;

                AggroContainer.gameObject.SetActive(true);
                AggroFiller.fillAmount = relationCurrentThreatToMax / 100;
            }
        }

        private void CombatInfoHandlerOnCombatInfosRemoved(CombatInfo obj)
        {
            _playerFrameTargetInfo.PlayerTarget.IBaseCreature.CombatInfoHandler.CombatInfosRemoved -= CombatInfoHandlerOnCombatInfosRemoved;
            _playerFrameTargetInfo.CombatInfo = null;
        }

        private void CombatInfoHandlerOnOutcomeThreatChanged(SingleCreatureCombatData singleCreatureCombatData)
        {
            Debug.Log($"OnThreatChanged. {singleCreatureCombatData.BaseCreature.GetRootObjectTransform().gameObject.name}: {singleCreatureCombatData.Threat}");
        }
    }
}