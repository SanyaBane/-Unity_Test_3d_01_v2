using System;
using System.IO;
using Abilities.Cooldown.ScriptableObjects;
using Assets.Scripts.Abilities.AnimationRules.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Abilities.ScriptableObjects
{
    [CustomEditor(typeof(AbilitySO), true)]
    public class AbilitySOEditor : Editor
    {
        private AbilitySO _abilitySO => (AbilitySO) target;

        private SerializedObject _getTarget;

        private void OnEnable()
        {
            _getTarget = new SerializedObject((AbilitySO) target);
        }

        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(_getTarget, nameof(AbilitySO.AbilityAnimationRuleSO), nameof(AbilitySO.CooldownSO));

            DrawAnimationRuleArea();

            DrawCooldownArea();

            _getTarget.ApplyModifiedProperties();
            // DrawDefaultInspector();
        }

        private void DrawAnimationRuleArea()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("AnimationRule", EditorStyles.boldLabel);

            SerializedProperty abilityAnimationRuleSP = _getTarget.FindProperty(nameof(AbilitySO.AbilityAnimationRuleSO));
            EditorGUILayout.PropertyField(abilityAnimationRuleSP);
            if (_abilitySO.AbilityAnimationRuleSO == null)
            {
                if (GUILayout.Button("Create Default AbilityAnimationRuleSO", new GUILayoutOption[] { }))
                {
                    DrawEditorForCustomSO(nameof(AbilitySO.AbilityAnimationRuleSO), typeof(AnimationRuleDefaultOnCastSO), "AnimationRule_DefaultOnCast");
                }
            }
            else
            {
                Editor e = Editor.CreateEditor(_abilitySO.AbilityAnimationRuleSO);
                e.DrawDefaultInspector();
            }
        }

        private void DrawCooldownArea()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Cooldown", EditorStyles.boldLabel);

            SerializedProperty cooldownSP = _getTarget.FindProperty(nameof(AbilitySO.CooldownSO));
            EditorGUILayout.PropertyField(cooldownSP);
            if (_abilitySO.CooldownSO == null)
            {
                if (GUILayout.Button("Create Default CooldownSO", new GUILayoutOption[] { }))
                {
                    DrawEditorForCustomSO(nameof(AbilitySO.CooldownSO), typeof(CooldownDefaultSO), "Cooldown_Default");
                }
            }
            else
            {
                Editor e = Editor.CreateEditor(_abilitySO.CooldownSO);
                e.DrawDefaultInspector();
            }
        }

        private void DrawEditorForCustomSO(string scriptableObjectPropertyName, Type defaultScriptableObjectType, string defaultNameSuffix)
        {
            var fullPathToCurrentAsset = AssetDatabase.GetAssetPath(target);
            var file = new FileInfo(fullPathToCurrentAsset);

            var currentAssetFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullPathToCurrentAsset);

            string stub = currentAssetFileNameWithoutExtension;

            if (!string.IsNullOrEmpty(defaultNameSuffix))
            {
                stub = stub + "_" + defaultNameSuffix;
            }

            string newAssetName = IndexedFilename(file.Directory.FullName, stub, "asset");
            string projectRelativePathForNewAsset = fullPathToCurrentAsset.Substring(0, fullPathToCurrentAsset.LastIndexOf("/") + 1) + newAssetName;

            var asset = ScriptableObject.CreateInstance(defaultScriptableObjectType);

            AssetDatabase.CreateAsset(asset, projectRelativePathForNewAsset);

            AssetDatabase.SaveAssets();

            _getTarget.Update();

            SerializedProperty serializedProperty = _getTarget.FindProperty(scriptableObjectPropertyName);
            serializedProperty.objectReferenceValue = asset;

            _getTarget.ApplyModifiedProperties();
        }

        private string IndexedFilename(string pathToDirectory, string stub, string extension)
        {
            int ix = 0;
            string filename;
            do
            {
                ix++;
                filename = string.Format("{0}{1}.{2}", stub, ix, extension);

                if (ix >= 10)
                {
                    Debug.LogError("It looks like we did something wrong.");
                    throw new Exception("It looks like we did something wrong.");
                }
            } while (File.Exists(Path.Combine(pathToDirectory, filename)));

            return filename;
        }
    }
}