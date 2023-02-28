using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlpacaIT.ReactiveLogic
{
    public class ReactiveLogicManager : MonoBehaviour
    {
        private static ReactiveLogicManager s_Instance;

        /// <summary>Gets the singleton reactive logic manager instance or creates it.</summary>
        public static ReactiveLogicManager Instance
        {
            get
            {
                // if known, immediately return the instance.
                if (s_Instance) return s_Instance;

                // C# hot reloading support: try finding an existing instance in the scene.
                s_Instance = FindObjectOfType<ReactiveLogicManager>();

                // otherwise create a new instance in scene.
                if (!s_Instance)
                    s_Instance = new GameObject("[Reactive Logic Manager]").AddComponent<ReactiveLogicManager>();

                return s_Instance;
            }
        }

        /// <summary>Whether an instance of the reactive logic manager has been created.</summary>
        public static bool hasInstance => s_Instance;

        /// <summary>
        /// Collection of all reactive logic components in the scene. This is updated once per <see cref="FixedUpdate"/>.
        /// </summary>
        private IReactive[] reactives;

        /// <summary>The list of active chains that are executing.</summary>
        private List<ReactiveChainLink> chains = new List<ReactiveChainLink>();

        /// <summary>Iterates over all components in the scene that implement <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The interface type that components should have implemented.</typeparam>
        private static IEnumerable<T> FindObjectsOfTypeImplementing<T>()
        {
            var gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                var interfaces = gameObjects[i].GetComponentsInChildren<T>();
                for (int j = 0; j < interfaces.Length; j++)
                    yield return interfaces[j];
            }
        }

        /// <summary>
        /// Finds all of the reactive components in the scene and stores them in <see cref="reactives"/>.
        /// </summary>
        private void FindReactives()
        {
            reactives = FindObjectsOfTypeImplementing<IReactive>().ToArray();
        }

        /// <summary>
        /// Iterates over all <see cref="IReactive"/> components in the scene matching the name.
        /// </summary>
        /// <param name="caller">The <see cref="IReactive"/> that wants to look up the target name.</param>
        /// <param name="name">The target name to find.</param>
        public IEnumerable<IReactive> ForEachReactive(IReactive caller, string name)
        {
            if (string.IsNullOrEmpty(name)) yield break;

            // make sure we can execute this before FixedUpdate.
            if (reactives == null) FindReactives();

            // check if the caller is part of a group.
            var groupCaller = caller.gameObject.GetComponentInParent<LogicGroup>();

            // if the caller is part of a group then search for the target inside:
            if (groupCaller)
            {
                foreach (var reactive in ForEachReactiveInsideGroup(groupCaller, name))
                    yield return reactive;
            }
            // if the caller is not part of a group then search for a global name:
            else
            {
                foreach (var reactive in ForEachReactiveOutsideGroups(name))
                    yield return reactive;
            }
        }

        private IEnumerable<IReactive> ForEachReactiveOutsideGroups(string name)
        {
            var targetNameMatcher = CreateTargetNameMatcher(name);

            // iterate over all reactives in the scene.
            for (int i = 0; i < reactives.Length; i++)
            {
                var reactive = reactives[i];

                // match the given target name:
                if (reactive != null && reactive.gameObject && targetNameMatcher(reactive.gameObject.name))
                {
                    yield return reactive;
                }
            }
        }

        private IEnumerable<IReactive> ForEachReactiveInsideGroup(LogicGroup parentGroup, string name)
        {
            var targetNameMatcher = CreateTargetNameMatcher(name);

            // iterate over all reactives in the scene.
            for (int i = 0; i < reactives.Length; i++)
            {
                var reactive = reactives[i];

                // match the given target name:
                if (reactive != null && reactive.gameObject && targetNameMatcher(reactive.gameObject.name))
                {
                    // and the reactive must be in the same group.
                    var group = reactive.gameObject.GetComponentInParent<LogicGroup>();
                    if (group == parentGroup)
                        yield return reactive;
                }
            }
        }

        /// <summary>
        /// Creates a function that matches the given target name to any string.
        /// <para>The character ? is a wildcard for one character.</para>
        /// <para>The character * is a wildcard for zero or more characters.</para>
        /// </summary>
        /// <param name="name">The target name to be matched.</param>
        /// <returns>The function that matches the given string to the target name.</returns>
        public Func<string, bool> CreateTargetNameMatcher(string name)
        {
            if (name.Contains("?") || name.Contains("*"))
            {
                // wildcard regex name comparison:
                var pattern = "^" + Regex.Escape(name).Replace("\\?", ".").Replace("\\*", ".*") + "$";
                var regex = new Regex(pattern);
                return input => regex.IsMatch(input);
            }
            else
            {
                // direct name comparison.
                return input => input == name;
            }
        }

        /// <summary>
        /// Schedules invoking an input on one or more targets that match the specified target name.
        /// This takes at least one fixed update step and may optionally be delayed further.
        /// </summary>
        /// <param name="activator">The game object that caused this chain of events.</param>
        /// <param name="caller">The <see cref="IReactive"/> that invoked this input.</param>
        /// <param name="target">The target name that yields one or more <see cref="IReactive"/> that will receive this input.</param>
        /// <param name="input">The target input name that will be invoked.</param>
        /// <param name="delay">The delay in seconds to wait before invoking the input on the target.</param>
        /// <param name="parameter">The parameter that will be passed to the input of the target.</param>
        public void ScheduleInput(GameObject activator, IReactive caller, string target, string input, float delay, object parameter)
        {
            foreach (var reactive in ForEachReactive(caller, target))
                chains.Add(new ReactiveChainLink(activator, caller, reactive, input, delay, new ReactiveChainLinkParameter(parameter)));
        }

        /// <summary>
        /// Schedules invoking an input on an <see cref="IReactive"/>. This takes at least one fixed
        /// update step and may optionally be delayed further.
        /// </summary>
        /// <param name="activator">The game object that caused this chain of events.</param>
        /// <param name="caller">The <see cref="IReactive"/> that invoked this input.</param>
        /// <param name="target">The <see cref="IReactive"/> that will receive this input.</param>
        /// <param name="input">The target input name that will be invoked.</param>
        /// <param name="delay">The delay in seconds to wait before invoking the input on the target.</param>
        /// <param name="parameter">The parameter that will be passed to the input of the target.</param>
        public void ScheduleInput(GameObject activator, IReactive caller, IReactive target, string input, float delay, object parameter)
        {
            chains.Add(new ReactiveChainLink(activator, caller, target, input, delay, new ReactiveChainLinkParameter(parameter)));
        }

        /// <summary>All of the logic gets executed once per fixed update.</summary>
        private void FixedUpdate()
        {
            // find all of the reactive components in the scene.
            FindReactives();

            // update all of the active chains.
            for (int i = chains.Count; i-- > 0;)
            {
                var chain = chains[i];

                // decrease the delay time remaining by delta time.
                chain.delay -= Time.fixedDeltaTime;
                if (chain.delay <= 0.0f)
                {
                    chain.target?.OnReactiveInput(chain.ToReactiveInput());
                    chains.RemoveAt(i);
                }
            }
        }
    }
}