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

        /// <summary>Collection of all reactive logic components in the scene.</summary>
        private List<IReactive> reactives = new List<IReactive>();

        /// <summary>The list of active chains that are executing.</summary>
        private LinkedList<ReactiveChainLink> chains = new LinkedList<ReactiveChainLink>();

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
        /// <para>This should only be called by editor code.</para>
        /// </summary>
        internal void EditorUpdateReactives()
        {
            reactives = FindObjectsOfTypeImplementing<IReactive>().ToList();
        }

        /// <summary>
        /// Iterates over all <see cref="IReactive"/> components in the scene matching the name.
        /// </summary>
        /// <param name="caller">The <see cref="IReactive"/> that wants to look up the target name.</param>
        /// <param name="name">The target name to find.</param>
        public IEnumerable<IReactive> ForEachReactive(IReactive caller, string name)
        {
            if (string.IsNullOrEmpty(name)) yield break;

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
            var reactivesCount = reactives.Count;
            for (int i = 0; i < reactivesCount; i++)
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
            var reactivesCount = reactives.Count;
            for (int i = 0; i < reactivesCount; i++)
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
        public static Func<string, bool> CreateTargetNameMatcher(string name)
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
                chains.AddLast(new ReactiveChainLink(activator, caller, reactive, input, delay, new ReactiveChainLinkParameter(parameter)));
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
            chains.AddLast(new ReactiveChainLink(activator, caller, target, input, delay, new ReactiveChainLinkParameter(parameter)));
        }

        /// <summary>
        /// Called when an <see cref="IReactive"/> gets enabled in the scene.
        /// <para>
        /// Do not call this method directly, please rely on <see
        /// cref="IReactiveExtensions.OnReactiveEnable"/>. You can access it inside of your <see
        /// cref="MonoBehaviour"/> using:
        /// </para>
        /// <code>
        ///void OnEnable()
        ///{
        ///     this.OnReactiveEnable();
        ///}
        /// </code>
        /// </summary>
        /// <param name="reactive">The <see cref="IReactive"/> that got enabled.</param>
        public void OnReactiveEnable(IReactive reactive)
        {
            reactives.Add(reactive);
        }

        /// <summary>
        /// Called when an <see cref="IReactive"/> gets disabled in the scene.
        /// <para>
        /// Do not call this method directly, please rely on <see
        /// cref="IReactiveExtensions.OnReactiveDisable"/>. You can access it inside of your <see
        /// cref="MonoBehaviour"/> using:
        /// </para>
        /// <code>
        ///void OnDisable()
        ///{
        ///     this.OnReactiveDisable();
        ///}
        /// </code>
        /// </summary>
        /// <param name="reactive">The <see cref="IReactive"/> that got disabled.</param>
        public void OnReactiveDisable(IReactive reactive)
        {
            reactives.Remove(reactive);
        }

        /// <summary>All of the logic gets executed once per fixed update.</summary>
        private void FixedUpdate()
        {
            // update all of the active chains.
            var node = chains.First;
            while (node != null)
            {
                var next = node.Next;
                var chain = node.Value;

                // decrease the delay time remaining by delta time.
                chain.delay -= Time.fixedDeltaTime;
                if (chain.delay <= 0.0f)
                {
                    // remove the chain link before processing it.
                    chains.Remove(node);

                    // make sure the target component has not been destroyed.
                    if (chain.target != null) // confirm: should we check chain.target.gameObject here instead?
                    {
                        // The "Enable"-input always works.
                        if (chain.targetInput == "Enable")
                        {
                            chain.target.reactiveData.enabled = true;
                        }
                        else if (chain.target.reactiveData.enabled)
                        {
                            // "Disable"-input.
                            if (chain.targetInput == "Disable")
                                chain.target.reactiveData.enabled = false;

                            // "User"-inputs invoke the output handlers at the target reactive.
                            else if (chain.targetInputIsUserDefined)
                                chain.target.OnReactiveOutput(chain.activator, chain.targetInput, chain.targetParameter);

                            // Invoke the input handler at the target reactive.
                            else
                                chain.target.OnReactiveInput(chain.ToReactiveInput());
                        }
                    }
                }

                node = next;
            }
        }
    }
}