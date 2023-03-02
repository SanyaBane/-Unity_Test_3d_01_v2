using Assets.Scripts.AutoAttack;
using Assets.Scripts.Interfaces;
using Assets.Scripts.HelpersUnity;
using Assets.Scripts.TargetHandling;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class PlayerCreature : BaseCreature
    {
        private PlayerAutoAttackAssigner _playerAutoAttackAssigner;

        public IPlayerController IPlayerController { get; private set; }
        public PlayerTargetHandler PlayerTargetHandler => (PlayerTargetHandler) TargetHandler;

        protected override void Awake()
        {
            base.Awake();

            if (CurrentJob == EJob.None)
            {
                Debug.Log($"{nameof(CurrentJob)} not assigned.");
            }

            IPlayerController = CreatureHelper.GetIPlayerControllerFromPlayerCreature(this.transform);
            _playerAutoAttackAssigner = this.GetComponent<PlayerAutoAttackAssigner>();
        }

        protected override void UpdateJob()
        {
            base.UpdateJob();
            _playerAutoAttackAssigner.SetJob(this.CurrentJob);
        }
    }
}