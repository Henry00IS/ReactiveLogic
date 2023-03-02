#if UNITY_EDITOR

using UnityEditor;

namespace AlpacaIT.ReactiveLogic.Editor
{
    [CustomEditor(typeof(LogicUnity))]
    public class LogicUnityEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This makes it easy to communicate with a component that is not reactive, but it encourages visual programming, which is a bad idea. You essentially have code in the scene that tinkers around in C# and can cause serious bugs that are hard to trace and fix months from now. Use this component sparingly!", MessageType.Warning);

            var sParameterMode = serializedObject.FindProperty(nameof(LogicUnity.parameterMode));
            var sUnityEvent = serializedObject.FindProperty(nameof(LogicUnity.unityEvent));
            var sUnityEventBoolean = serializedObject.FindProperty(nameof(LogicUnity.unityEventBoolean));
            var sUnityEventInteger = serializedObject.FindProperty(nameof(LogicUnity.unityEventInteger));
            var sUnityEventFloat = serializedObject.FindProperty(nameof(LogicUnity.unityEventFloat));
            var sUnityEventString = serializedObject.FindProperty(nameof(LogicUnity.unityEventString));

            EditorGUILayout.PropertyField(sParameterMode);

            switch ((LogicUnity.ParameterMode)sParameterMode.enumValueIndex)
            {
                case LogicUnity.ParameterMode.None:
                    EditorGUILayout.PropertyField(sUnityEvent);
                    break;

                case LogicUnity.ParameterMode.Boolean:
                    EditorGUILayout.PropertyField(sUnityEventBoolean);
                    break;

                case LogicUnity.ParameterMode.Integer:
                    EditorGUILayout.PropertyField(sUnityEventInteger);
                    break;

                case LogicUnity.ParameterMode.Float:
                    EditorGUILayout.PropertyField(sUnityEventFloat);
                    break;

                case LogicUnity.ParameterMode.String:
                    EditorGUILayout.PropertyField(sUnityEventString);
                    break;
            }

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif