using Assets.Scripts.Attachments;
using UnityEngine;

namespace Assets.Scripts.Creatures
{
    public class CreatureInfoContainer : MonoBehaviour
    {
        private CharacterMeshRoot _characterMeshRoot;
        public CharacterMeshRoot CharacterMeshRoot
        {
            get
            {
                if (_characterMeshRoot == null)
                    _characterMeshRoot = this.GetComponentInChildren<CharacterMeshRoot>();
        
                return _characterMeshRoot;
            }
        }
        
        private BaseCreature _baseCreature;
        public BaseCreature BaseCreature
        {
            get
            {
                if (_baseCreature == null)
                    _baseCreature = this.GetComponentInChildren<BaseCreature>();

                return _baseCreature;
            }
        }

        public AttachmentsController GetAttachmentsController()
        {
            var ret = BaseCreature.AttachmentsController;
            return ret;
        }

        public Animator GetAnimator()
        {
            var ret = BaseCreature.Animator;
            return ret;
        }
    }
}