using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.HelpersUnity;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Attachments
{
    [Serializable]
    [CustomEditor(typeof(AttachmentsController), true)]
    public class AttachmentsControllerEditor : Editor
    {
        public const string PLAYER_PREFS_SAVE_LOAD_KEY = "AttachmentsControllerEditor_SafeLoad";

        private AttachmentsController attachmentsController => (AttachmentsController) target;

        private SerializedProperty _thisList;
        private int _listSize;

        private void OnEnable()
        {
            _thisList = serializedObject.FindProperty("GetGOSelectorHelpersNames"); // Find the List in our script and create a reference of it
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(15);

            // try
            // {
                _listSize = _thisList.arraySize;
                _listSize = EditorGUILayout.IntField(nameof(AttachmentsController.GOSelectorHelpersCount), _listSize);
                
                if (_listSize != _thisList.arraySize)
                {
                    while (_listSize > _thisList.arraySize)
                    {
                        _thisList.InsertArrayElementAtIndex(_thisList.arraySize);
                    }
                
                    while (_listSize < _thisList.arraySize)
                    {
                        _thisList.DeleteArrayElementAtIndex(_thisList.arraySize - 1);
                    }
                }
                
                GUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Save", new GUILayoutOption[] { }))
                {
                    SaveList();
                }
                
                if (GUILayout.Button("Load", new GUILayoutOption[] { }))
                {
                    LoadList();
                }
                
                GUILayout.EndHorizontal();
                
                GUILayout.Space(15);
                
                for (int i = 0; i < _thisList.arraySize; i++)
                {
                    SerializedProperty myListRef = _thisList.GetArrayElementAtIndex(i);
                
                    SerializedProperty myAttachmentName = myListRef.FindPropertyRelative(nameof(AttachmentsController.AttachmentsAndGameObjectsNames.AttachmentName));
                    SerializedProperty myGameObjectName = myListRef.FindPropertyRelative(nameof(AttachmentsController.AttachmentsAndGameObjectsNames.GameObjectName));
                
                    GUILayout.BeginHorizontal();
                
                    #region Vertical
                
                    GUILayout.BeginVertical();
                
                    GUILayout.BeginHorizontal();
                    //
                    EditorGUILayout.PropertyField(myAttachmentName);
                    string attachmentName = myAttachmentName.stringValue;
                
                    GUILayout.EndHorizontal();
                
                    GUILayout.BeginHorizontal();
                
                    EditorGUILayout.PropertyField(myGameObjectName);
                    string gameObjectName = myGameObjectName.stringValue;
                
                    GUILayout.EndHorizontal();
                
                    GUILayout.EndVertical();
                
                    #endregion
                
                    if (GUILayout.Button("Assign", new GUILayoutOption[] {GUILayout.Width(90), GUILayout.Height(40)}))
                    {
                        SetGameObjectAsAttachment(attachmentName, gameObjectName);
                    }
                
                    GUILayout.EndHorizontal();
                }
                
                GUILayout.Space(25);

                // DrawDefaultInspector();
                DrawPropertiesExcluding(serializedObject, "GOSelectorHelpersCount");
                
                serializedObject.ApplyModifiedProperties();
            // }
            // catch (Exception ex)
            // {
            //     Debug.LogError(ex);
            //
            //     attachmentsController.GetGOSelectorHelpersNames.Clear();
            //     attachmentsController.GOSelectorHelpersCount = 0;
            // }
        }

        private void SaveList()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(ms, attachmentsController.GetGOSelectorHelpersNames);
                string encodedObject = Convert.ToBase64String(ms.ToArray());
                Debug.Log($"Saved list:{Environment.NewLine}" +
                          $"{encodedObject}");
                PlayerPrefs.SetString(PLAYER_PREFS_SAVE_LOAD_KEY, encodedObject);
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveList: {ex.Message}");
            }
            finally
            {
                ms.Close();
            }
        }
        
        private void LoadList()
        {
            string encodedObject = PlayerPrefs.GetString(PLAYER_PREFS_SAVE_LOAD_KEY);
            if (String.IsNullOrEmpty(encodedObject))
            {
                Debug.LogError("Can't LoadList() since it's empty.");
                return;
            }
        
            BinaryFormatter bf = new BinaryFormatter();
            byte[] decodedObject = Convert.FromBase64String(encodedObject);
            MemoryStream memoryStream = new MemoryStream(decodedObject);
        
            var retList = (List<AttachmentsController.AttachmentsAndGameObjectsNames>) bf.Deserialize(memoryStream);
            // PlayerPrefs.SetString(PLAYER_PREFS_SAVE_LOAD_KEY, ""); // reset loaded value
            
            serializedObject.Update();
            
            // var tmp = _getTarget.FindProperty(nameof(AttachmentsController.GetGOSelectorHelpersNames));
            attachmentsController.GetGOSelectorHelpersNames = retList;
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void SetGameObjectAsAttachment(string attachmentName, string gameObjectName)
        {
            // Debug.Log($"attachmentName: {attachmentName}; gameObjectName: {gameObjectName};");
        
            serializedObject.Update();
        
            var foundedGO = EditorHelper.FindRecursive(attachmentsController.transform, gameObjectName);
            if (foundedGO == null)
            {
                Debug.LogError($"GameObject with name '{gameObjectName}' not found as child of gameobject '{attachmentsController.gameObject.name}'.");
                return;
            }
        
            var attachment = serializedObject.FindProperty(attachmentName);
            if (attachment == null)
            {
                Debug.LogError($"Attachment with name '{attachmentName}' not found on gameobject '{attachmentsController.gameObject.name}'.");
                return;
            }
        
            attachment.objectReferenceValue = foundedGO;
        
            serializedObject.ApplyModifiedProperties();
        }

        // private void SelectChildGameObject(string gameObjectName)
        // {
        //     var foundedGO = EditorHelper.FindRecursive(attachmentsController.transform, gameObjectName);
        //
        //     if (foundedGO == null)
        //         Debug.LogError($"GameObject with name '{gameObjectName}' not found as child of '{attachmentsController.gameObject.name}'.");
        //     else
        //         Selection.activeObject = foundedGO;
        // }
    }
}