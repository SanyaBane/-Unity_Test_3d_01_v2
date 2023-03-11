using System.Collections;
using Assets.Scripts.Abilities.Enums;
using Assets.Scripts.Abilities.General;
using Assets.Scripts.Abilities.Parameters;
using Assets.Scripts.Creatures;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Utilities;
using Assets.Scripts.VFX;
using UnityEngine;

namespace Assets.Scripts.Abilities.Controller
{
    public class CastAbilityCoroutineWrapper : CoroutineWrapper
    {
        private bool _canContinueCastingFirstCheck = true;
        
        public AbilitiesController AbilitiesController { get; }
        public Ability Ability { get; }
        public IAbilityParameters IAbilityParameters { get; }

        private EllipseProjection _ellipseProjection;
        
        public float CurrentlyCastedTime { get; private set; } = 0;

        public CastAbilityCoroutineWrapper() : base()
        {
        }

        public CastAbilityCoroutineWrapper(MonoBehaviour owner, AbilitiesController abilitiesController, Ability ability, IAbilityParameters iAbilityParameters) : base(owner)
        {
            AbilitiesController = abilitiesController;
            Ability = ability;
            IAbilityParameters = iAbilityParameters;

            _mainIEnumerator = StartCastingAbility();
        }

        private IEnumerator StartCastingAbility()
        {
            AbilitiesController.RaiseCastStart(Ability, IAbilityParameters);

            AbilitiesController.Animator.SetBool(ConstantsAnimator.ABILITIES_BOOL_CASTING_1_SUCCESS, false);

            // Debug.Log("DisplayAbilityAoeZone");
            DisplayAbilityAoeZone();

            _canContinueCastingFirstCheck = true;
            while (CurrentlyCastedTime < Ability.CastTime)
            {
                if (StopCoroutineFlag)
                    yield break;
                
                yield return CastingAbility();
            }
            
            if (StopCoroutineFlag)
                yield break;

            if (AbilitiesController.IsCanFinishCast(Ability, IAbilityParameters) == false)
            {
                AbilitiesController.InterruptCast();
                
                if (StopCoroutineFlag)
                    yield break;
            }

            AbilitiesController.StartCoroutine(AbilitiesController.FinishCastSuccessfully(Ability, IAbilityParameters, this));
        }

        private IEnumerator CastingAbility()
        {
            float castSpeedPercentage = 0;
            bool appliedModifierForCastSpeed = false;
                
            foreach (var castSpeedModifier in Ability.CastSpeedModifiers)
            {
                if (castSpeedModifier.CanApply)
                {
                    appliedModifierForCastSpeed = true;
                    castSpeedPercentage += castSpeedModifier.CastSpeedPercentageBonus;
                }
            }

            if (!appliedModifierForCastSpeed)
            {
                castSpeedPercentage = 100;
            }
                
            CurrentlyCastedTime += Time.deltaTime / 100 * castSpeedPercentage;

            AbilitiesController.RaiseCastTick(Ability, CurrentlyCastedTime, IAbilityParameters);
            
            if (AbilitiesController.ShouldRotateToTargetOfCast(Ability, IAbilityParameters) == ERotateOnCast.RotateUsingRotationSpeed)
            {
                if (AbilitiesController.IBaseCreature is NpcBaseCreature npcBaseCreature)
                {
                    RotateToTarget(IAbilityParameters, npcBaseCreature);
                }
            }

            // Interrupt cast if attempting to move and ability is not instant
            if (AbilitiesController.IsCanContinueCasting(Ability, IAbilityParameters, _canContinueCastingFirstCheck) == false)
            {
                AbilitiesController.InterruptCast();
                
                if (StopCoroutineFlag)
                    yield break;
            }

            _canContinueCastingFirstCheck = false;

            yield return null;
        }
        
        private void RotateToTarget(IAbilityParameters iAbilityParameters, INpcBaseCreature npcBaseCreature)
        {
            var directionToTarget = iAbilityParameters.DefaultAbilityParameters.Target.IBaseCreature.GetGroundedPosition() - npcBaseCreature.GetRootObjectTransform().position;
            var directionToTargetWithoutY = new Vector3(directionToTarget.x, 0, directionToTarget.z);
            if (directionToTargetWithoutY != Vector3.zero)
            {
                var lookRotation = Quaternion.LookRotation(directionToTargetWithoutY);
                var rotateTowards = Quaternion.RotateTowards(npcBaseCreature.GetRootObjectTransform().rotation, lookRotation, npcBaseCreature.NpcAI.AIPath.rotationSpeed * Time.deltaTime);
        
                npcBaseCreature.GetRootObjectTransform().rotation = rotateTowards;
            }
        }

        public void DestroyEllipseProjection()
        {
            if (_ellipseProjection != null)
            {
                GameManager.Instance.StartCoroutine(DestroyEllipseProjectionCoroutine(0.3f));
            }
        }

        private IEnumerator DestroyEllipseProjectionCoroutine(float delay)
        {
            _ellipseProjection.FadeIn(delay);

            yield return new WaitForSeconds(delay);

            // Maybe after delay it was already destroyed. Need to check.
            if (_ellipseProjection != null)
            {
                MonoBehaviour.Destroy(_ellipseProjection.gameObject);
                _ellipseProjection = null;
            }
        }

        private void DisplayAbilityAoeZone()
        {
            if (!(Ability is AbilityAOEFromSelf abilityAOEFromSelf))
                return;

            if (!abilityAOEFromSelf.DisplayAoeZone)
                return;

            var ellipseProjectionPrefab = Resources.Load(ConstantsResources.ELLIPSE_PROJECTION);
            if (ellipseProjectionPrefab == null)
                Debug.LogError(Constants.MESSAGE_RESOURCE_NOT_FOUNDED);

            var ellipseProjectionGO = (GameObject) MonoBehaviour.Instantiate(ellipseProjectionPrefab);

            _ellipseProjection = ellipseProjectionGO.GetComponent<EllipseProjection>();

            _ellipseProjection.Height = abilityAOEFromSelf.Height;
            _ellipseProjection.Radius = abilityAOEFromSelf.Radius;
            _ellipseProjection.Angle = abilityAOEFromSelf.Angle;
            _ellipseProjection.ClockwiseRotation = abilityAOEFromSelf.ClockwiseRotation;
            
            _ellipseProjection.Color = Constants.COLOR_AOE_GROUND_INDICATOR;

            _ellipseProjection.Parent = this.IAbilityParameters.DefaultAbilityParameters.Source.GetRootObjectTransform();

            _ellipseProjection.UpdateValues();
        }
    }
}