#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic.Editor
{
    [CustomEditor(typeof(LogicGroup))]
    public class LogicGroupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var sReactiveData = serializedObject.FindProperty("_reactiveData");
            var sGroupDescription = serializedObject.FindProperty(nameof(LogicGroup.groupDescription));

            EditorGUILayout.PropertyField(sReactiveData);

            EditorGUILayout.LabelField("Group Description:", EditorStyles.boldLabel);

            var wordWrapTextField = new GUIStyle(EditorStyles.textField);
            wordWrapTextField.wordWrap = true;
            sGroupDescription.stringValue = EditorGUILayout.TextArea(sGroupDescription.stringValue, wordWrapTextField);

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif