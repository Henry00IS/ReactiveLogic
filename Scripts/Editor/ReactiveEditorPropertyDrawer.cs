#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic.Editor
{
    /// <summary>Adds a custom inspector to all reactive logic components.</summary>
    [CustomPropertyDrawer(typeof(ReactiveEditor))]
    public class ReactiveEditorPropertyDrawer : PropertyDrawer
    {
        #region Workaround to get a halfway decent OnGUI method

        // we use static to keep the settings open or closed between different selections.
        private static bool isFoldoutHeaderGroupActive = true; // SET THIS TO FALSE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnWhyDidUnityNotDoThisProperlyGUI(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return OnWhyDidUnityNotDoThisProperlyGUI(new Rect(), property, label, false);
        }

        private int OnWhyDidUnityNotDoThisProperlyGUI(Rect position, SerializedProperty property, GUIContent label, bool gui)
        {
            var reactive = property.serializedObject.targetObject as IReactive;
            if (reactive == null) { if (gui) base.OnGUI(position, property, label); return 20; }
            var initialPosition = position;
            position.height = 20f;
            if (gui) EditorGUI.BeginProperty(position, label, property);

            if (gui) isFoldoutHeaderGroupActive = EditorGUI.BeginFoldoutHeaderGroup(position, isFoldoutHeaderGroupActive, "Reactive Logic Settings"); position.y += 20f;
            if (isFoldoutHeaderGroupActive)
                OnActualGUI(reactive, property.serializedObject, gui, ref position);
            EditorGUI.EndFoldoutHeaderGroup();

            if (gui) EditorGUI.EndProperty();
            return Mathf.FloorToInt(position.y - initialPosition.y);
        }

        #endregion Workaround to get a halfway decent OnGUI method

        /// <summary>
        /// Called whenever the property gets drawn or the height of the property inspector is to be calculated.
        /// </summary>
        /// <param name="reactive">The reactive logic component that is being edited.</param>
        /// <param name="serializedObject">The serialized object for editing fields in a generic way.</param>
        /// <param name="gui">Whether we should call GUI functions or only calculate the height.</param>
        /// <param name="position">The current position (the height is 20 by default).</param>
        private void OnActualGUI(IReactive reactive, SerializedObject serializedObject, bool gui, ref Rect position)
        {
            try
            {
                if (gui) EditorGUI.indentLevel++;

                bool actionDelete = false;
                int actionDeleteIndex = 0;

                bool actionMove = false;
                int actionMoveFrom = 0;
                int actionMoveTo = 0;

                var sOutputs = serializedObject.FindProperty("_reactiveOutputs");
                if (sOutputs != null)
                {
                    if (gui) EditorGUI.LabelField(position, "Outputs: " + sOutputs.arraySize, EditorStyles.boldLabel); position.y += 20f;

                    position.y += 5f;

                    for (int i = 0; i < sOutputs.arraySize; i++)
                    {
                        var sOutput = sOutputs.GetArrayElementAtIndex(i);

                        var sOutputName = sOutput.FindPropertyRelative(nameof(ReactiveOutput.name));
                        var sOutputDelay = sOutput.FindPropertyRelative(nameof(ReactiveOutput.delay));
                        var sOutputTarget = sOutput.FindPropertyRelative(nameof(ReactiveOutput.target));
                        var sOutputTargetInput = sOutput.FindPropertyRelative(nameof(ReactiveOutput.input));
                        var sOutputParameter = sOutput.FindPropertyRelative(nameof(ReactiveOutput.parameter));

                        var pos1 = position;
                        pos1.x += 20f;
                        pos1.width -= 55f;

                        var pos2 = position;
                        pos2.x += 20f;
                        pos2.width = 30f;
                        pos2.x += pos1.width + 5f;

                        if (gui)
                        {
                            (sOutputName.stringValue, sOutputDelay.floatValue) = TextFieldReactableOutputsPopup(pos1, reactive.reactiveMetadata.interfaces, sOutputName.stringValue, sOutputDelay.floatValue);

                            if (GUI.Button(pos2, EditorGUIUtility.IconContent("d_TreeEditor.Trash", "Remove Output")))
                            {
                                actionDelete = true;
                                actionDeleteIndex = i;
                            }
                        }
                        position.y += 20f; pos1.y += 20f; pos2.y += 20f;

                        if (gui)
                        {
                            sOutputTarget.stringValue = PrefixTextField(pos1, new GUIContent("Target", "The name of the target that will receive this input."), sOutputTarget.stringValue);

                            if (GUI.Button(pos2, EditorGUIUtility.IconContent("CollabPush", "Move Output Up")))
                            {
                                actionMove = true;
                                actionMoveFrom = i;
                                actionMoveTo = i - 1;
                            }
                        }
                        position.y += 20f; pos1.y += 20f; pos2.y += 20f;

                        if (gui)
                        {
                            sOutputTargetInput.stringValue = TextFieldReactableInputsPopup(pos1, sOutputTarget.stringValue, sOutputTargetInput.stringValue);

                            if (GUI.Button(pos2, EditorGUIUtility.IconContent("CollabPull", "Move Output Down")))
                            {
                                actionMove = true;
                                actionMoveFrom = i;
                                actionMoveTo = i + 1;
                            }
                        }
                        position.y += 20f; pos1.y += 20f; pos2.y += 20f;

                        if (gui)
                        {
                            sOutputParameter.stringValue = PrefixTextField(pos1, new GUIContent("Parameter", "The parameter that will be passed from this output to the input."), sOutputParameter.stringValue);
                        }
                        position.y += 20f; pos1.y += 20f; pos2.y += 20f;

                        position.y += 10f;
                    }

                    position.y += 5f;

                    if (gui)
                    {
                        var pos1 = position;
                        pos1.x += 35f;
                        pos1.width -= 70f;
                        if (GUI.Button(pos1, EditorGUIUtility.IconContent("CreateAddNew", "Add Output")))
                        {
                            sOutputs.InsertArrayElementAtIndex(sOutputs.arraySize);
                        }
                    }
                    position.y += 30f;
                }

                if (actionDelete)
                {
                    sOutputs.DeleteArrayElementAtIndex(actionDeleteIndex);
                }

                if (actionMove)
                {
                    sOutputs.MoveArrayElement(actionMoveFrom, actionMoveTo);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (gui) EditorGUI.indentLevel--;
            }
        }

        private (string, float) TextFieldReactableOutputsPopup(Rect position, MetaInterface[] interfaces, string text, float delay)
        {
            GetHorizontalRects(position, out var pos1, out var pos2);

            var options = new List<GUIContent>(interfaces.Length + 1) { new GUIContent("...", "Select the name of an output to insert it.") };
            interfaces.GetOutputsAsGUIContents(options);

            // if the current text matches an input name we select that by default.
            int defaultSelection = 0;
            var optionsCount = options.Count;
            for (int i = 0; i < optionsCount; i++)
                if (options[i].text == text)
                    defaultSelection = i;

            // allow the user to select the value.
            var selected = EditorGUI.Popup(pos1, defaultSelection, options.ToArray()); position.y += 20f;
            if (selected != 0 && selected != defaultSelection)
                text = options[selected].text;

            GetHorizontalInverseRects(pos2, out var pos3, out var pos4);

            text = EditorGUI.TextField(pos3, text);
            delay = EditorGUI.FloatField(pos4, delay);
            return (text, delay);
        }

        /// <summary>
        /// Caches the list of possible inputs for a target name. This gets reset whenever the
        /// selection changes in Unity Editor.
        /// </summary>
        private Dictionary<string, List<GUIContent>> targetInputsCache = new Dictionary<string, List<GUIContent>>();

        private string TextFieldReactableInputsPopup(Rect position, string target, string text)
        {
            GetHorizontalRects(position, out var pos1, out var pos2);

            // look in the target reactives for input names.
            if (!targetInputsCache.TryGetValue(target, out var options))
            {
                options = new List<GUIContent>() { new GUIContent("...", "Select the name of an input to insert it.") };
                foreach (var reactive in ReactiveLogicManager.Instance.ForEachReactive(target))
                    reactive.reactiveMetadata.interfaces.GetInputsAsGUIContents(options);

                targetInputsCache.Add(target, options);
            }

            // if the current text matches an input name we select that by default.
            int defaultSelection = 0;
            var optionsCount = options.Count;
            for (int i = 0; i < optionsCount; i++)
                if (options[i].text == text)
                    defaultSelection = i;

            // allow the user to select the value.
            var selected = EditorGUI.Popup(pos1, defaultSelection, options.ToArray()); position.y += 20f;
            if (selected != 0 && selected != defaultSelection)
                text = options[selected].text;

            return EditorGUI.TextField(pos2, text);
        }

        private string PrefixTextField(Rect position, GUIContent label, string text)
        {
            GetHorizontalRects(position, out var pos1, out var pos2);

            EditorGUI.LabelField(pos1, label.text);
            return EditorGUI.TextField(pos2, text);
        }

        private void GetHorizontalRects(Rect position, out Rect pos1, out Rect pos2)
        {
            pos1 = position;
            pos1.width /= 3f;

            pos2 = position;
            pos2.width -= pos1.width;
            pos2.x += pos1.width;
        }

        private void GetHorizontalInverseRects(Rect position, out Rect pos1, out Rect pos2)
        {
            pos1 = position;
            pos1.width = pos1.width - (pos1.width / 3f);

            pos2 = position;
            pos2.width -= pos1.width;
            pos2.x += pos1.width;
        }
    }
}

#endif