using Assets.Scripts.Interfaces;
using System;
using UnityEngine;

namespace Assets.Scripts.TargetHandling
{
    public class TargetHandler : MonoBehaviour, ICanSelectTarget
    {
        protected IBaseCreature _baseCreature;

        #region ICanHaveTarget

        public event Action<ITargetable> SelectedTargetChanged;
        private ITargetable _SelectedTarget;
        public ITargetable SelectedTarget
        {
            get => _SelectedTarget;
            set
            {
                bool valueIsDifferentFromBefore = _SelectedTarget != value;

                _SelectedTarget = value;

                if (valueIsDifferentFromBefore)
                    SelectedTargetChanged?.Invoke(value);
            }
        }

        #endregion

        protected virtual void Start()
        {
            _baseCreature = this.GetComponent<IBaseCreature>();

            RemoveTarget();
        }

        protected virtual void Update()
        {
            if (SelectedTarget != null && !SelectedTarget.CanBeTargeted)
            {
                RemoveTarget();
            }
        }

        public bool CanSelect(ITargetable target)
        {
            if (target != null && target.CanBeTargeted) // && target.IBaseCreature.GetTransform().gameObject != this.transform.gameObject 
            {
                return true;
            }

            return false;
        }

        public virtual bool TrySelectTarget(ITargetable target)
        {
            if (CanSelect(target))
            {
                SelectTarget(target);
                return true;
            }

            return false;
        }

        public void SelectTarget(ITargetable target)
        {
            if (SelectedTarget == target)
                return;

            RemoveTarget();

            SelectedTarget = target;
            SelectedTarget.CanBeTargetedChanged += SelectedTarget_CanBeTargetedChanged;

            WhenTargetSelected(SelectedTarget);
        }

        public void RemoveTarget()
        {
            if (SelectedTarget == null)
                return;

            SelectedTarget.CanBeTargetedChanged -= SelectedTarget_CanBeTargetedChanged;

            WhenTargetDeselected(SelectedTarget);
            SelectedTarget = null;
        }

        private void SelectedTarget_CanBeTargetedChanged(bool canBeTargeted)
        {
            if (!canBeTargeted)
            {
                RemoveTarget();
            }
        }

        protected virtual void WhenTargetSelected(ITargetable target)
        {
        }

        protected virtual void WhenTargetDeselected(ITargetable target)
        {
        }

        protected virtual void WhenTargetHovered(ITargetable target)
        {
        }

        protected virtual void WhenTargetUnhovered(ITargetable target)
        {
        }
    }
}