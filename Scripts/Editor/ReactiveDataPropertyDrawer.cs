#if UNITY_EDITOR

using AlpacaIT.ReactiveLogic.Editor.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic.Editor
{
    /// <summary>Adds a custom inspector to all reactive logic components.</summary>
    [CustomPropertyDrawer(typeof(ReactiveData))]
    public class ReactiveDataPropertyDrawer : PropertyDrawer
    {
        #region Workaround to get a halfway decent OnGUI method

        // we use static to keep the settings open or closed between different selections.
        private static bool isFoldoutHeaderGroupActive = false;

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

        /// <summary>This flag gets reset whenever the editor selection changes.</summary>
        private bool isNewInstance = true;

        private GUIContent[] cacheReactiveOutputs;
        private GUIContent[] cacheReactiveTargets;
        private GUIContent[] cacheTargetInputs;
        private string cacheTargetInputsTargetName;

        /// <summary>Handles setup after the editor selection changes.</summary>
        private void OnSetup(IReactive reactive)
        {
            if (!isNewInstance) return;
            isNewInstance = false;

            // make sure we have an up to date collection of all reactives in the scene.
            ReactiveLogicManager.Instance.EditorUpdateReactives();

            // we want to prevent doing a lot of work every update so we prepare everything we need here.
            OnSetupOutputs(reactive);
            OnSetupTargets(reactive);
        }

        private void OnSetupOutputs(IReactive reactive)
        {
            // build the auto-complete list for reactive outputs.
            var interfaces = reactive.reactiveMetadata.interfaces;
            var reactiveOutputs = new List<GUIContent>(interfaces.Length + 8) { new GUIContent("...", "Select the name of an output to insert it.") };
            HashSet<string> outputNames = new HashSet<string>();
            interfaces.GetOutputsAsGUIContents(reactiveOutputs);

            // special handler for groups where we find outputs meant for the group.
            if (reactive is LogicGroup group)
            {
                foreach (var groupOutput in ReactiveLogicManager.Instance.EditorForEachGroupOutput(group))
                    if (outputNames.Add(groupOutput))
                        reactiveOutputs.Add(new GUIContent(groupOutput, "An output invoked by logic inside of the group."));
            }
            // special handler when the reactive is part of a group to add group inputs.
            else if (reactive.reactiveData.group)
            {
                foreach (var groupInput in ReactiveLogicManager.Instance.EditorForEachGroupInput(reactive.reactiveData.group))
                {
                    var name = "Group" + groupInput;
                    if (outputNames.Add(name))
                        reactiveOutputs.Add(new GUIContent(name, "An input invoked on the group."));
                }
            }

            // check for user inputs on the reactive.
            foreach (var targetInput in reactive.reactiveData.outputs)
            {
                var name = targetInput.name;
                if (name.StartsWith("User") && outputNames.Add(name))
                    reactiveOutputs.Add(new GUIContent(name, "Custom user output that is invoked by invoking the " + name + " input."));
            }

            cacheReactiveOutputs = reactiveOutputs.ToArray();
        }

        private void OnSetupTargets(IReactive reactive)
        {
            var targets = new List<GUIContent>() { new GUIContent("...", "Select the name of a target to insert it.") };
            HashSet<string> targetNames = new HashSet<string>();

            // build the auto-complete list for all possible context sensitive targets.
            foreach (var target in ReactiveLogicManager.Instance.ForEachReactive(reactive, "*")) // we use the wildcard! brilliant!
            {
                var name = target.gameObject.name;
                if (targetNames.Add(name))
                    targets.Add(new GUIContent(name, $"Targets {name} ({target.GetType().Name})."));
            }

            // we order the list alphabetically.
            cacheReactiveTargets = targets.OrderBy(t => t.text).ToArray();
        }

        private void OnRefreshInputs(IReactive reactive, string target)
        {
            if (cacheTargetInputsTargetName == target) return;
            cacheTargetInputsTargetName = target;

            var options = new List<GUIContent>() { new GUIContent("...", "Select the name of an input to insert it.") };
            HashSet<string> inputNames = new HashSet<string>();

            // build the auto-complete list for all possible inputs in all the selected targets.
            foreach (var targetReactive in ReactiveLogicManager.Instance.ForEachReactive(reactive, target))
            {
                // check the meta data of the reactive.
                foreach (var targetInput in targetReactive.reactiveMetadata.interfaces.ForEachInput())
                {
                    var name = targetInput.name;
                    if (inputNames.Add(name))
                        options.Add(new GUIContent(name, targetInput.description));
                }

                // special handler for groups where we find group inputs.
                if (targetReactive is LogicGroup group)
                {
                    foreach (var groupInput in ReactiveLogicManager.Instance.EditorForEachGroupInput(group))
                        if (inputNames.Add(groupInput))
                            options.Add(new GUIContent(groupInput, "An input invoking logic inside of the group."));
                }

                // special handler when the reactive is part of a group to add group outputs but
                // only when it targets the group.
                if (target == ReactiveLogicManager.keywordGroup && reactive.reactiveData.group)
                {
                    foreach (var groupOutput in ReactiveLogicManager.Instance.EditorForEachGroupOutput(reactive.reactiveData.group))
                    {
                        var name = "Group" + groupOutput;
                        if (inputNames.Add(name))
                            options.Add(new GUIContent(name, "An output invoked on the group."));
                    }
                }

                // check for user inputs on the reactive.
                foreach (var targetInput in targetReactive.reactiveData.outputs)
                {
                    var name = targetInput.name;
                    if (name.StartsWith("User") && inputNames.Add(name))
                        options.Add(new GUIContent(name, "Custom user input that will invoke the " + name + " output on the target reactive."));
                }
            }

            cacheTargetInputs = options.ToArray();
        }

        /// <summary>
        /// Called whenever the property gets drawn or the height of the property inspector is to be calculated.
        /// </summary>
        /// <param name="reactive">The reactive logic component that is being edited.</param>
        /// <param name="serializedObject">The serialized object for editing fields in a generic way.</param>
        /// <param name="gui">Whether we should call GUI functions or only calculate the height.</param>
        /// <param name="position">The current position (the height is 20 by default).</param>
        private void OnActualGUI(IReactive reactive, SerializedObject serializedObject, bool gui, ref Rect position)
        {
            // handle first-time setup after editor selection changes.
            OnSetup(reactive);

            // prepare the editor gui methods for this step.
            HenryEditorGUI.rectInitial = position;
            HenryEditorGUI.rect = position;
            HenryEditorGUI.active = gui;

            try
            {
                HenryEditorGUI.indentLevel++;

                // state for the delete action.
                bool actionDelete = false;
                int actionDeleteIndex = 0;

                // state for the move actions.
                bool actionMove = false;
                int actionMoveFrom = 0;
                int actionMoveTo = 0;

                // state for the ping targets action.
                bool actionSelectTargets = false;
                string actionSelectTarget = "";

                // state for the paste action.
                bool actionPaste = false;
                int actionPasteIndex = 0;

                var sOutputs = serializedObject.FindProperty("_reactiveData.outputs");
                if (sOutputs != null)
                {
                    HenryEditorGUI.LabelField("Outputs: " + sOutputs.arraySize, EditorStyles.boldLabel);

                    HenryEditorGUI.Space(5);

                    for (int i = 0; i < sOutputs.arraySize; i++)
                    {
                        var sOutput = sOutputs.GetArrayElementAtIndex(i);

                        var sOutputName = sOutput.FindPropertyRelative(nameof(ReactiveOutput.name));
                        var sOutputDelay = sOutput.FindPropertyRelative(nameof(ReactiveOutput.delay));
                        var sOutputTarget = sOutput.FindPropertyRelative(nameof(ReactiveOutput.targetName));
                        var sOutputTargetInput = sOutput.FindPropertyRelative(nameof(ReactiveOutput.targetInput));
                        var sOutputParameter = sOutput.FindPropertyRelative(nameof(ReactiveOutput.targetInputParameter));

                        GetHorizontalWidths(HenryEditorGUI.rect, out var width1, out var width2);

                        // display the output name auto-complete, output name, output delay and remove button.

                        string text = sOutputName.stringValue;
                        HenryEditorGUI.Horizontal(width1, width2 - 115, 50, 65, () =>
                        {
                            int defaultSelection = FindGuiContentsTextIndex(cacheReactiveOutputs, text);
                            int selectedIndex = HenryEditorGUI.Popup(defaultSelection, cacheReactiveOutputs);
                            if (selectedIndex != 0 && selectedIndex != defaultSelection)
                                text = cacheReactiveOutputs[selectedIndex].text;
                        }, () =>
                        {
                            sOutputName.stringValue = HenryEditorGUI.TextField(text);
                        }, () =>
                        {
                            sOutputDelay.floatValue = HenryEditorGUI.FloatField(sOutputDelay.floatValue);
                        }, () =>
                        {
                            if (HenryEditorGUI.Button(EditorGUIUtility.IconContent("d_TreeEditor.Trash", "Remove Output")))
                            {
                                actionDelete = true;
                                actionDeleteIndex = i;
                            }
                        });

                        // display the target name auto-complete, target name and move up button.

                        text = sOutputTarget.stringValue;
                        HenryEditorGUI.Horizontal(width1, width2 - 65, 65, () =>
                        {
                            int defaultSelection = FindGuiContentsTextIndex(cacheReactiveTargets, text);
                            int selectedIndex = HenryEditorGUI.Popup(defaultSelection, cacheReactiveTargets);
                            if (selectedIndex != 0 && selectedIndex != defaultSelection)
                                text = cacheReactiveTargets[selectedIndex].text;
                        }, () =>
                        {
                            sOutputTarget.stringValue = HenryEditorGUI.TextField(text);
                        }, () =>
                        {
                            if (HenryEditorGUI.Button(EditorGUIUtility.IconContent("d_BuildSettings.SelectedIcon", "Select Targets")))
                            {
                                actionSelectTargets = true;
                                actionSelectTarget = sOutputTarget.stringValue;
                            }
                        });

                        // this is fast to call due to a cache.
                        OnRefreshInputs(reactive, text);

                        text = sOutputTargetInput.stringValue;
                        HenryEditorGUI.Horizontal(width1, width2 - 65, 40, 40, () =>
                        {
                            int defaultSelection = FindGuiContentsTextIndex(cacheTargetInputs, text);
                            int selectedIndex = HenryEditorGUI.Popup(defaultSelection, cacheTargetInputs);
                            if (selectedIndex != 0 && selectedIndex != defaultSelection)
                                text = cacheTargetInputs[selectedIndex].text;
                        }, () =>
                        {
                            sOutputTargetInput.stringValue = HenryEditorGUI.TextField(text);
                        }, () =>
                        {
                            if (HenryEditorGUI.Button(EditorGUIUtility.IconContent("CollabPull", "Move Output Down")))
                            {
                                actionMove = true;
                                actionMoveFrom = i;
                                actionMoveTo = i + 1;
                            }
                        }, () =>
                        {
                            if (HenryEditorGUI.Button(EditorGUIUtility.IconContent("CollabPush", "Move Output Up"), false))
                            {
                                actionMove = true;
                                actionMoveFrom = i;
                                actionMoveTo = i - 1;
                            }
                        });

                        text = sOutputParameter.stringValue;
                        HenryEditorGUI.Horizontal(width1, width2 - 65, 40, 40, () =>
                        {
                            HenryEditorGUI.LabelField("Parameter:");
                        }, () =>
                        {
                            sOutputParameter.stringValue = HenryEditorGUI.TextField(text);
                        }, () =>
                        {
                            if (HenryEditorGUI.Button(EditorGUIUtility.IconContent("TreeEditor.Duplicate", "Copy")))
                            {
                                // copy this entry to the clipboard.
                                EditorGUIUtility.systemCopyBuffer = JsonUtility.ToJson(new ReactiveOutput()
                                {
                                    name = sOutputName.stringValue,
                                    targetName = sOutputTarget.stringValue,
                                    targetInput = sOutputTargetInput.stringValue,
                                    targetInputParameter = sOutputParameter.stringValue,
                                    delay = sOutputDelay.floatValue
                                });
                            }
                        }, () =>
                        {
                            if (HenryEditorGUI.Button(EditorGUIUtility.IconContent("Clipboard", "Paste"), false))
                            {
                                actionPaste = true;
                                actionPasteIndex = i;
                            }
                        });

                        HenryEditorGUI.Space(10);
                    }

                    HenryEditorGUI.Horizontal(HenryEditorGUI.rect.width - 65, 65, () =>
                    {
                        if (HenryEditorGUI.Button(EditorGUIUtility.IconContent("CreateAddNew", "Add Output")))
                        {
                            sOutputs.InsertArrayElementAtIndex(sOutputs.arraySize);
                        }
                    }, () =>
                    {
                        if (HenryEditorGUI.Button(EditorGUIUtility.IconContent("Clipboard", "Paste")))
                        {
                            actionPaste = true;
                            actionPasteIndex = sOutputs.arraySize;
                        }
                    });

                    HenryEditorGUI.Space(5);
                }

                if (actionDelete)
                {
                    sOutputs.DeleteArrayElementAtIndex(actionDeleteIndex);
                }

                if (actionMove)
                {
                    sOutputs.MoveArrayElement(actionMoveFrom, actionMoveTo);
                }

                if (actionSelectTargets)
                {
                    var objects = ReactiveLogicManager.Instance.ForEachReactive(reactive, actionSelectTarget).Select(r => r.gameObject).ToArray();
                    if (objects.Length > 0)
                    {
                        EditorGUIUtility.PingObject(reactive.gameObject);
                        Selection.objects = objects;
                    }
                }

                if (actionPaste)
                {
                    // create entry from the clipboard.
                    try
                    {
                        // this can throw an exception when it fails to parse the data.
                        var clipboardOutput = JsonUtility.FromJson<ReactiveOutput>(EditorGUIUtility.systemCopyBuffer);
                        sOutputs.InsertArrayElementAtIndex(actionPasteIndex);
                        var sOutput = sOutputs.GetArrayElementAtIndex(actionPasteIndex);
                        var sOutputName = sOutput.FindPropertyRelative(nameof(ReactiveOutput.name));
                        var sOutputDelay = sOutput.FindPropertyRelative(nameof(ReactiveOutput.delay));
                        var sOutputTarget = sOutput.FindPropertyRelative(nameof(ReactiveOutput.targetName));
                        var sOutputTargetInput = sOutput.FindPropertyRelative(nameof(ReactiveOutput.targetInput));
                        var sOutputParameter = sOutput.FindPropertyRelative(nameof(ReactiveOutput.targetInputParameter));
                        sOutputName.stringValue = clipboardOutput.name;
                        sOutputDelay.floatValue = clipboardOutput.delay;
                        sOutputTarget.stringValue = clipboardOutput.targetName;
                        sOutputTargetInput.stringValue = clipboardOutput.targetInput;
                        sOutputParameter.stringValue = clipboardOutput.targetInputParameter;
                    }
                    catch
                    {
                    }
                }

                position = HenryEditorGUI.rect;
            }
            catch
            {
                throw;
            }
            finally
            {
                HenryEditorGUI.indentLevel--;
            }
        }

        private int FindGuiContentsTextIndex(GUIContent[] options, string text)
        {
            // if the current text matches an input name we select that by default.
            int result = 0;
            var optionsCount = options.Length;
            for (int i = 0; i < optionsCount; i++)
                if (options[i].text == text)
                    result = i;
            return result;
        }

        private void GetHorizontalWidths(Rect position, out float width1, out float width2)
        {
            width1 = position.width;
            width1 /= 3f;

            width2 = position.width;
            width2 -= width1;
        }
    }
}

#endif