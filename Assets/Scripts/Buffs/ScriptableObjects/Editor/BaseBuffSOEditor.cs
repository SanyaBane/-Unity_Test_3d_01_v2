using System;
using System.IO;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffDuration.ScriptableObjects;
using Assets.Scripts.Abilities.Behaviours.ScriptableObjects.Buffs.BuffRecastType.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Buffs.ScriptableObjects
{
    [CustomEditor(typeof(BaseBuffSO), true)]
    public class BaseBuffSOEditor : Editor
    {
        private BaseBuffSO _baseBuffSO => (BaseBuffSO) target;

        private SerializedObject _getTarget;

        private void OnEnable()
        {
            _getTarget = new SerializedObject((BaseBuffSO) target);
        }

        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(_getTarget, nameof(BaseBuffSO.DurationSO), nameof(BaseBuffSO.RecastTypeSO));

            DrawBuffDuration();
            DrawBuffRecastTypeSO();

            _getTarget.ApplyModifiedProperties();
            // DrawDefaultInspector();
        }

        private void DrawBuffDuration()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Duration", EditorStyles.boldLabel);

            SerializedProperty serializedProperty = _getTarget.FindProperty(nameof(BaseBuffSO.DurationSO));
            EditorGUILayout.PropertyField(serializedProperty);
            if (_baseBuffSO.DurationSO == null)
            {
                if (GUILayout.Button("Create Default BuffDurationSO", new GUILayoutOption[] { }))
                {
                    DrawEditorForCustomSO(nameof(BaseBuffSO.DurationSO), typeof(BuffDurationDefaultSO), "BuffDuration_Default");
                }
            }
            else
            {
                Editor e = Editor.CreateEditor(_baseBuffSO.DurationSO);
                e.DrawDefaultInspector();
            }
        }

        private void DrawBuffRecastTypeSO()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Recast Type", EditorStyles.boldLabel);

            SerializedProperty serializedProperty = _getTarget.FindProperty(nameof(BaseBuffSO.RecastTypeSO));
            EditorGUILayout.PropertyField(serializedProperty);
            if (_baseBuffSO.RecastTypeSO == null)
            {
                if (GUILayout.Button("Create Default BuffRecastType", new GUILayoutOption[] { }))
                {
                    DrawEditorForCustomSO(nameof(BaseBuffSO.RecastTypeSO), typeof(BuffRecastTypeDefaultUpdateDurationSO), "BuffRecastType_DefaultUpdateDuration");
                }
            }
            else
            {
                Editor e = Editor.CreateEditor(_baseBuffSO.RecastTypeSO);
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