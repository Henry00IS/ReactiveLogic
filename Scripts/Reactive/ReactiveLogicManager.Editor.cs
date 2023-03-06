using System.Collections.Generic;
using AlpacaIT.ReactiveLogic.Internal;

namespace AlpacaIT.ReactiveLogic
{
    // this partial class handles everything related to unity editor integration.
    public partial class ReactiveLogicManager
    {
        /// <summary>
        /// Finds all of the reactive components in the scene and stores them in <see cref="reactives"/>.
        /// <para>This should only be called by editor code.</para>
        /// </summary>
        internal void EditorUpdateReactives()
        {
            reactives = new LinkedList<IReactive>(Utilities.FindObjectsOfTypeImplementing<IReactive>());

            // also refresh the data for all of them as they may have moved in the hierarchy.
            foreach (var reactive in reactives)
                RefreshReactiveData(reactive);
        }

        /// <summary>
        /// Iterates over all child <see cref="IReactive"/> logic inside of a group and finds output
        /// names starting with the "Group"-prefix. The returned names do not have the prefix.
        /// </summary>
        /// <param name="group">The group to find outputs for.</param>
        internal IEnumerable<string> EditorForEachGroupOutput(LogicGroup group)
        {
            foreach (var reactive in ForEachReactiveInGroup(group))
            {
                // skip groups inside of groups.
                if (reactive is LogicGroup) continue;

                var outputs = reactive.reactiveData.outputs;
                var outputsCount = outputs.Count;
                for (int i = 0; i < outputsCount; i++)
                {
                    var output = outputs[i];
                    if (output.targetName == keywordGroup && output.targetInput.StartsWith("Group"))
                        yield return output.targetInput.Substring(5);
                }
            }
        }

        /// <summary>
        /// Iterates over all child <see cref="IReactive"/> logic inside of a group and finds input
        /// names starting with the "Group"-prefix. The returned names do not have the prefix.
        /// </summary>
        /// <param name="group">The group to find inputs for.</param>
        internal IEnumerable<string> EditorForEachGroupInput(LogicGroup group)
        {
            foreach (var reactive in ForEachReactiveInGroup(group))
            {
                // skip groups inside of groups.
                if (reactive is LogicGroup) continue;

                var outputs = reactive.reactiveData.outputs;
                var outputsCount = outputs.Count;
                for (int i = 0; i < outputsCount; i++)
                {
                    var output = outputs[i];
                    if (output.name.StartsWith("Group"))
                        yield return output.name.Substring(5);
                }
            }
        }
    }
}