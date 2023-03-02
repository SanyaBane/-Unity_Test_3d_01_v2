using Assets.Scripts.Factions;
using Assets.Scripts.AutoAttack;
using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using Assets.Scripts.VFX;
using System;
using System.Linq;
using Assets.Scripts.HelpersUnity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.TargetHandling
{
    public class PlayerTargetHandler : TargetHandler, ICanHoverTarget
    {
        private AutoAttackController _autoAttackScript;

        private readonly float _checkTargetRayMaxDistance = 300.0f;

        private GameObject _targetSelectionProjectionEffect;
        private TargetAutoAttackIndicator _targetAutoAttackIndicatorScript;

        private PlayerCreature _creaturePlayer;
        private Camera _camera;

        #region ICanHoverTarget

        public event Action<ITargetable> HoveredTargetChanged;
        private ITargetable _HoveredTarget;

        public ITargetable HoveredTarget
        {
            get => _HoveredTarget;
            set
            {
                bool valueIsDifferentFromBefore = _HoveredTarget != value;

                _HoveredTarget = value;

                if (valueIsDifferentFromBefore)
                    HoveredTargetChanged?.Invoke(value);
            }
        }

        #endregion

        protected override void Start()
        {
            base.Start();

            _creaturePlayer = this.GetComponent<PlayerCreature>();
            _autoAttackScript = this.GetComponent<AutoAttackController>();
            _camera = Camera.main;
        }

        protected override void Update()
        {
            base.Update();

            if (GameManager.Instance.InputController_WoW.SelectPlayerCharacterButtonDown)
            {
                TrySelectTarget(_creaturePlayer.ITargetable);
            }

            if (SelectedTarget != null && GameManager.Instance.InputController_WoW.CommandUnselectTarget)
            {
                RemoveTarget();
            }

            HandleAutoAttackIndicator();

            if (EventSystem.current.IsPointerOverGameObject()) // if mouse over GUI element, "return"
            {
                GameManager.Instance.CursorManager.SetActiveCursorType(CursorManager.CursorType.Normal);
                return;
            }

            HandleTargetSelectionWithRay();
        }

        public override bool TrySelectTarget(ITargetable target)
        {
            this.RemoveTarget(); // игрок, при неудачной попытке выбора цели (щелчок мышкой по земле) теряет цель

            return base.TrySelectTarget(target);
        }


        private void HandleTargetSelectionWithRay()
        {
            var targetableEntityMouseOver = CheckForTargetWithRay();

            // if we already have something hovered and its not same as "targetableEntityMouseOver" or "SelectedTarget", then we need to remove this "hover effect"
            if (HoveredTarget != null &&
                HoveredTarget != targetableEntityMouseOver &&
                HoveredTarget != SelectedTarget)
            {
                WhenTargetUnhovered(HoveredTarget);
                HoveredTarget = null;
            }

            // if hovered on new possible target and its is not SelectedTarget
            if (targetableEntityMouseOver != null &&
                targetableEntityMouseOver.IBaseCreature.GetRootObjectTransform().gameObject != this.transform.gameObject &&
                HoveredTarget != targetableEntityMouseOver &&
                SelectedTarget != targetableEntityMouseOver &&
                targetableEntityMouseOver.CanBeTargeted)
            {
                HoveredTarget = targetableEntityMouseOver;
                WhenTargetHovered(HoveredTarget);
            }


            var cursorType = CursorManager.CursorType.Normal;

            if (targetableEntityMouseOver != null &&
                targetableEntityMouseOver.CanBeTargeted &&
                targetableEntityMouseOver.CanBeAttacked &&
                targetableEntityMouseOver.IBaseCreature.GetRootObjectTransform().gameObject != this.transform.gameObject)
            {
                var relationWithMouseOverTarget = targetableEntityMouseOver.IBaseCreature.Faction.GetRelationWith(_creaturePlayer.Faction);

                if ((int) relationWithMouseOverTarget < (int) Factions.EFactionRelation.Friendly)
                {
                    cursorType = CursorManager.CursorType.Sword;
                }
            }

            GameManager.Instance.CursorManager.SetActiveCursorType(cursorType);


            bool selectTargetWithLMB = GameManager.Instance.InputController_WoW.IsMouseLeftButtonUp &&
                                       GameManager.Instance.InputController_WoW.HowLongMouseLeftButtonHolded <= GameManager.Instance.InputController_WoW.TargetSelectionClickAllowedDelay;

            bool selectTargetWithRMB = GameManager.Instance.InputController_WoW.IsMouseRightButtonUp &&
                                       GameManager.Instance.InputController_WoW.HowLongMouseRightButtonHolded <= GameManager.Instance.InputController_WoW.TargetSelectionClickAllowedDelay;

            if ((selectTargetWithLMB || selectTargetWithRMB)
                && !(selectTargetWithLMB && selectTargetWithRMB))
            {
                bool clickingOnAlreadySelectedTarget = false;

                // если происходит попытка выбрать цель которая уже является выбранной, то не пытаемся выбрать её снова
                if (targetableEntityMouseOver != null && SelectedTarget == targetableEntityMouseOver)
                {
                    clickingOnAlreadySelectedTarget = true;
                }
                else
                {
                    TrySelectTarget(targetableEntityMouseOver);
                }


                if (SelectedTarget != null)
                {
                    var relationWithSelectedTarget = SelectedTarget.IBaseCreature.Faction.GetRelationWith(_creaturePlayer.Faction);
                    bool targetCanBeEnemy = relationWithSelectedTarget <= EFactionRelation.Neutral;

                    // если происходит нажатие через ПКМ, включаем режим автоатаки 
                    if (SelectedTarget.CanBeTargeted &&
                        SelectedTarget.CanBeAttacked &&
                        targetCanBeEnemy)
                    {
                        if (selectTargetWithRMB)
                            _autoAttackScript.EnableAutoAttackMode();
                        else if (selectTargetWithLMB && !clickingOnAlreadySelectedTarget)
                            _autoAttackScript.DisableAutoAttackMode();
                    }
                }
            }
        }

        protected ITargetable CheckForTargetWithRay()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            float rayMaxDistance = _checkTargetRayMaxDistance;

            var hits = Physics.RaycastAll(ray, rayMaxDistance, GameManager.Instance.SelectableLayerMask)
                .OrderBy(x => x.distance)
                .ToList();
            foreach (RaycastHit raycastHit in hits)
            {
                if (raycastHit.collider != null)
                {
                    var targetable = CreatureHelper.GetITargetableFromSelectableLayerObject(raycastHit.transform);
                    if (targetable == null)
                        continue;

                    if (targetable.IBaseCreature.GetRootObjectTransform().gameObject == this.gameObject)
                        continue;

                    return targetable;
                }
            }

            return null;

            //bool raycast = Physics.Raycast(ray, out RaycastHit raycastHit, rayMaxDistance, GameManager.Instance.SelectableLayerMask);
            //if (raycast && raycastHit.collider != null)
            //{
            //    var targetable = raycastHit.transform.GetComponentInParent<ITargetable>();
            //    if (targetable == null)
            //        return null;

            //    return targetable;
            //}

            //return null;
        }

        protected override void WhenTargetSelected(ITargetable target)
        {
            AddSelectedEffect(target);
        }

        protected override void WhenTargetDeselected(ITargetable target)
        {
            RemoveSelectedEffect(target);
        }

        protected override void WhenTargetHovered(ITargetable target)
        {
            AddHoveredEffect(target);
        }

        protected override void WhenTargetUnhovered(ITargetable target)
        {
            RemoveHoveredEffect(target);
        }

        #region AutoAttack Indicator

        private void HandleAutoAttackIndicator()
        {
            if (_targetAutoAttackIndicatorScript != null &&
                (SelectedTarget != _targetAutoAttackIndicatorScript.Target || !_autoAttackScript.AutoAttackModeOn))
            {
                // если индикатор автоатаки уже существует и либо цель автоатаки не та что была перед этим, либо автоатака выключена, то выключаем удаляем индикатор
                Destroy(_targetAutoAttackIndicatorScript.gameObject);
                _targetAutoAttackIndicatorScript = null;
            }

            if (_targetAutoAttackIndicatorScript == null && SelectedTarget != null && _autoAttackScript.AutoAttackModeOn)
            {
                // если индикатор автоатаки не существует, но есть выбранная цель и включен режим автоатаки, то спавним индикатор
                var targetAutoAttackIndicatorPrefab = Resources.Load(ConstantsResources.TARGET_AUTO_ATTACK_INDICATOR_PREFAB);
                if (targetAutoAttackIndicatorPrefab == null)
                    Debug.LogError(Constants.MESSAGE_RESOURCE_NOT_FOUNDED);

                var targetAutoAttackIndicator = (GameObject) Instantiate(targetAutoAttackIndicatorPrefab, GameManager.Instance.PlayerCanvas.transform);
                _targetAutoAttackIndicatorScript = targetAutoAttackIndicator.GetComponent<TargetAutoAttackIndicator>();
                _targetAutoAttackIndicatorScript.SetupAndStart(SelectedTarget);
            }
        }

        #endregion

        #region Selected and Hovered VFX

        #region Selected Effect

        private void AddSelectedEffect(ITargetable target)
        {
            if (target != null && target.IBaseCreature.GetRootObjectTransform() != null)
            {
                Color color;
                var factionRelation = target.IBaseCreature.Faction.GetRelationWith(this._creaturePlayer.Faction);

                if (factionRelation >= EFactionRelation.Friendly)
                    color = Constants.FRIENDLY_SELECTED_TARGET_OUTLINE_COLOR;
                else
                    color = Constants.NEUTRAL_SELECTED_TARGET_OUTLINE_COLOR;

                var targetOutline = target.IBaseCreature.Outline;
                if (targetOutline != null)
                {
                    targetOutline.OutlineColor = color;
                    targetOutline.OutlineWidth = Constants.TARGET_OUTLINE_WIDTH;
                }

                if (target.HasFrontAndBack)
                {
                    // spawn ground-projection effect on target GameObject
                    var targetSelectionProjectionEffectPrefab = Resources.Load(ConstantsResources.TARGET_SELECTION_VFX_PREFAB);
                    if (targetSelectionProjectionEffectPrefab == null)
                        Debug.LogError(Constants.MESSAGE_RESOURCE_NOT_FOUNDED);

                    _targetSelectionProjectionEffect = (GameObject) Instantiate(targetSelectionProjectionEffectPrefab, target.IBaseCreature.GetRootObjectTransform());
                    var _targetSelectionProjectionEffectScript = _targetSelectionProjectionEffect.GetComponent<TargetSelectedVFX>();
                    _targetSelectionProjectionEffectScript.SetupAndStart(target.IBaseCreature, color);
                }
                else
                {
                    Debug.LogError("Not implemented 'target.HasFrontAndBack == false'");
                }
            }
        }

        private void RemoveSelectedEffect(ITargetable target)
        {
            if (target != null && target.IBaseCreature.GetRootObjectTransform() != null)
            {
                var targetOutline = target.IBaseCreature.Outline;
                if (targetOutline != null)
                {
                    targetOutline.OutlineWidth = 0;
                }

                if (_targetSelectionProjectionEffect != null)
                    Destroy(_targetSelectionProjectionEffect);
            }
        }

        #endregion

        #region Hovered Effect

        private void AddHoveredEffect(ITargetable target)
        {
            //Debug.Log("AddHoveredEffect()");

            if (target != null && target.IBaseCreature.GetRootObjectTransform() != null)
            {
                var targetOutline = target.IBaseCreature.Outline;
                if (targetOutline != null)
                {
                    var factionRelation = target.IBaseCreature.Faction.GetRelationWith(this._creaturePlayer.Faction);

                    Color color;
                    if (factionRelation >= EFactionRelation.Friendly)
                    {
                        color = Constants.FRIENDLY_HOVERED_TARGET_OUTLINE_COLOR;
                    }
                    else
                    {
                        color = Constants.NEUTRAL_HOVERED_TARGET_OUTLINE_COLOR;
                    }

                    targetOutline.OutlineColor = color;
                    targetOutline.OutlineWidth = Constants.TARGET_OUTLINE_WIDTH;
                }
            }
        }

        private void RemoveHoveredEffect(ITargetable target)
        {
            //Debug.Log("RemoveHoveredEffect()");

            if (target != null && target.IBaseCreature.GetRootObjectTransform() != null)
            {
                var targetOutline = target.IBaseCreature.Outline;
                if (targetOutline != null)
                {
                    targetOutline.OutlineWidth = 0;
                }
            }
        }

        #endregion

        #endregion
    }
}