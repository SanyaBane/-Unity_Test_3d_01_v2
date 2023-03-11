using System;
using System.Collections;
using Assets.Scripts.Abilities;
using Assets.Scripts.Abilities.ScriptableObjects;
using Assets.Scripts.Creatures;
using Assets.Scripts.Levels;
using Assets.Scripts.NPC.ShivaBoss;
using Assets.Scripts.VFX;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Shiva_MirrorMirror3 : MonoBehaviour
{
    public enum MirrorMirror3TypeEnum
    {
        Red,
        Yellow
    }
    
    public enum enDisplayAbility
    {
        BitingFrost,
        DrivingFrost
    }

    private static Color _redMirrorColor = new Color(0.65f, 0.25f, 0.25f, 0);
    private static Color _yellowMirrorColor = new Color(0.7f, 0.7f, 0, 0);

    private BaseCreature _shivaCreature;

    private CreatureInfoContainer _creatureInfoContainer;
    private AbilitiesController _abilitiesController;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    private MirrorMirror3TypeEnum _mirrorMirror3Type;

    public float SpawnTime = 3.0f;

    private AbilityAOEFromSelf ReflectedBitingFrost;
    private AbilityAOEFromSelf ReflectedDrivingFrost;

    public AbilityAOEFromSelfSO ReflectedBitingFrostSO;
    public AbilityAOEFromSelfSO ReflectedDrivingFrostSO;
    
    private bool _isInitialized = false;

    public ShivaLevelManager ShivaLevelManager { get; private set; }
    
    [Header("Debug")]
    public EllipsePie AbilityRangeEllipsePie;

    public bool DisplayAbilityRange = false;

    public enDisplayAbility EDisplayAbility;
    
    public Color DisplayAbilityRangeColor = Color.white;
    
    private AbilityAOEFromSelfSO previousDebugDisplayAbility;
    
    private void Start()
    {
        Init();
        
        if (!Application.isPlaying) 
            return;
    }

    private void Update()
    {
        DebugDisplayAbilityRange();   
        
        if (!Application.isPlaying) 
            return;
    }

    private void OnValidate()
    {
        DebugDisplayAbilityRange();
    }

    private void DebugDisplayAbilityRange()
    {
        var correspondingObjectFromSource = PrefabUtility.GetCorrespondingObjectFromSource(this.gameObject);
        if (correspondingObjectFromSource == null)
            return;
        
        if (!DisplayAbilityRange)
        {
            if (AbilityRangeEllipsePie.gameObject.activeInHierarchy)
                AbilityRangeEllipsePie.gameObject.SetActive(false);
            
            return;
        }
        
        AbilityAOEFromSelfSO debugDisplayAbility;
        switch (EDisplayAbility)
        {
            case enDisplayAbility.BitingFrost:
                debugDisplayAbility = ReflectedBitingFrostSO;
                break;
            case enDisplayAbility.DrivingFrost:
                debugDisplayAbility = ReflectedDrivingFrostSO;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (previousDebugDisplayAbility != debugDisplayAbility)
        {
            AbilityRangeEllipsePie.Radius = debugDisplayAbility.Radius;
            AbilityRangeEllipsePie.Height = debugDisplayAbility.Height;
            AbilityRangeEllipsePie.Angle = debugDisplayAbility.Angle;
            AbilityRangeEllipsePie.ClockwiseRotation = debugDisplayAbility.ClockwiseRotation;
            AbilityRangeEllipsePie.Color = DisplayAbilityRangeColor; 
            
            AbilityRangeEllipsePie.UpdateValues();
            
            previousDebugDisplayAbility = debugDisplayAbility;
        }
        
        if (!AbilityRangeEllipsePie.gameObject.activeInHierarchy)
            AbilityRangeEllipsePie.gameObject.SetActive(true);
    }
    

    private void Init()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;
        
        if (_renderer == null)
            _renderer = this.GetComponentInChildren<Renderer>();

        if (_propBlock == null)
            _propBlock = new MaterialPropertyBlock();

        if (_creatureInfoContainer == null)
            _creatureInfoContainer = this.GetComponent<CreatureInfoContainer>();
        
        _abilitiesController = _creatureInfoContainer.BaseCreature.AbilitiesController;

        ShivaLevelManager = FindObjectOfType<ShivaLevelManager>();
        ShivaLevelManager_OnDisplayMirrorAbilityRangeChange(ShivaLevelManager.DisplayMirrorAbilityRange);
        ShivaLevelManager.DisplayMirrorAbilityRangeChange += ShivaLevelManager_OnDisplayMirrorAbilityRangeChange;

        ReflectedBitingFrost = (AbilityAOEFromSelf)ReflectedBitingFrostSO.CreateAbility(_abilitiesController);
        ReflectedDrivingFrost = (AbilityAOEFromSelf)ReflectedDrivingFrostSO.CreateAbility(_abilitiesController);
    }

    private void ShivaLevelManager_OnDisplayMirrorAbilityRangeChange(bool displayMirrorAbilityRangeChange)
    {
        DisplayAbilityRange = displayMirrorAbilityRangeChange;
    }

    public void SetupMirror(MirrorMirror3TypeEnum mirrorMirror3Type, BaseCreature shivaCreature)
    {
        Init();

        _shivaCreature = shivaCreature;
        _mirrorMirror3Type = mirrorMirror3Type;

        StartCoroutine(SpawnEffect());

        _renderer.GetPropertyBlock(_propBlock);

        switch (_mirrorMirror3Type)
        {
            case MirrorMirror3TypeEnum.Red:
                _propBlock.SetColor("_BaseColor", _redMirrorColor);
                this.transform.name += "_Red";
                break;
            case MirrorMirror3TypeEnum.Yellow:
                _propBlock.SetColor("_BaseColor", _yellowMirrorColor);
                this.transform.name += "_Yellow";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_mirrorMirror3Type), _mirrorMirror3Type, null);
        }

        _renderer.SetPropertyBlock(_propBlock);

        // engage to combat with same enemies as Shiva
        foreach (var shivaEngagedEnemy in _shivaCreature.CombatInfoHandler.GetEngagedCreatures())
        {
            _creatureInfoContainer.BaseCreature.CombatInfoHandler.EngageCombat(shivaEngagedEnemy);
        }
    }

    private IEnumerator SpawnEffect()
    {
        float passedTime = 0;

        var initialScale = _creatureInfoContainer.CharacterMeshRoot.transform.localScale;
        _creatureInfoContainer.CharacterMeshRoot.transform.localScale = Vector3.zero;

        while (passedTime < SpawnTime)
        {
            // Debug.Log($"passedTime: {passedTime}; Time.deltaTime: {Time.deltaTime}");

            passedTime += Time.deltaTime;

            var delta1 = passedTime * 100 / SpawnTime;
            var delta2 = delta1 / 100;

            _creatureInfoContainer.CharacterMeshRoot.transform.localScale = Vector3.Lerp(
                Vector3.zero, initialScale, delta2);

            // Debug.Log($"delta2: {delta2}.");

            yield return null;
        }

        _creatureInfoContainer.CharacterMeshRoot.transform.localScale = initialScale;
    }

    public void StartCastingBitingOrDriving(AbilitySO castedBitingOrDrivingAbilitySO)
    {
        switch (_mirrorMirror3Type)
        {
            case MirrorMirror3TypeEnum.Red:
                _propBlock.SetColor("_BaseColor", _redMirrorColor);
                StartCoroutine(CastAbilityWithDelay(castedBitingOrDrivingAbilitySO, true));
                break;
            case MirrorMirror3TypeEnum.Yellow:
                _propBlock.SetColor("_BaseColor", _yellowMirrorColor);
                StartCoroutine(CastAbilityWithDelay(castedBitingOrDrivingAbilitySO, false));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_mirrorMirror3Type), _mirrorMirror3Type, null);
        }
    }

    private IEnumerator CastAbilityWithDelay(AbilitySO abilitySO, bool isDoubleCastTime)
    {
        // _abilitiesController = _creatureInfoContainer.BaseCreature.AbilitiesController;

        Ability abilityToCast;

        switch (abilitySO.Id)
        {
            case ShivaAI.ABILITY_ID_SHIVA_BITING_FROST:
                // abilityToCast = _abilitiesController.GetAbilityById("ShivaMirror_BitingFrost");
                abilityToCast = ReflectedBitingFrost;
                break;

            case ShivaAI.ABILITY_ID_SHIVA_DRIVING_FROST:
                // abilityToCast = _abilitiesController.GetAbilityById("ShivaMirror_DrivingFrost");
                abilityToCast = ReflectedDrivingFrost;
                break;

            default:
                throw new ArgumentException(nameof(abilitySO));
        }

        // _abilitiesController.AddAbility(abilityToCast);

        if (isDoubleCastTime)
            abilityToCast.CastTime *= 2;

        _abilitiesController.TryStartCast(abilityToCast);
        _abilitiesController.CastFinishedAndExecuted += MirrorAbilitiesControllerOnCastFinishedAndExecuted;

        yield break;
    }

    private void MirrorAbilitiesControllerOnCastFinishedAndExecuted(AbilitiesController abilitiesController, Ability ability)
    {
        // var creatureInfoContainer = CreatureHelper.GetCreatureInfoContainerFromBaseCreature(abilitiesController.IBaseCreature);
        StartCoroutine(DieAfterDelay(abilitiesController, 1.0f));

        // Destroy();
        // throw new System.NotImplementedException();
    }

    private IEnumerator DieAfterDelay(AbilitiesController abilitiesController, float delay)
    {
        yield return new WaitForSeconds(delay);

        abilitiesController.IBaseCreature.Health.Die();
    }
}