using Assets.Scripts.Interfaces;
using Assets.Scripts.TargetHandling;
using Assets.Scripts.Creatures;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TargetFrame : CreatureFrame
    {
        private TargetHandler _targetHandler;

        [SerializeField] private TargetCastBar _targetCastBar;

        public override void Setup()
        {
            base.Setup();

            var playerCreatureInfoContainer = GameManager.Instance.PlayerGameObject.GetComponent<CreatureInfoContainer>();

            _targetHandler = playerCreatureInfoContainer.BaseCreature.GetComponent<TargetHandler>();
            if (_targetHandler == null)
                Debug.LogError($"{nameof(_targetHandler)} == null");

            if (_targetCastBar == null)
                Debug.LogError($"{nameof(_targetCastBar)} == null");

            _targetCastBar.Setup();

            _targetHandler.SelectedTargetChanged += TargetHandler_SelectedTargetChanged;
        }

        private void TargetHandler_SelectedTargetChanged(ITargetable newFrameOwner)
        {
            if (newFrameOwner == null)
            {
                HideUIElement();
                
                SetNewOwner(null);
                _targetCastBar.UpdateOwnerInfo(null);
            }
            else
            {
                ShowUIElement();
                
                SetNewOwner(newFrameOwner.IBaseCreature);
                _targetCastBar.UpdateOwnerInfo(newFrameOwner.IBaseCreature);
            }
        }
    }
}
