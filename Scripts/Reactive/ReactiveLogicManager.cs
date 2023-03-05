using AlpacaIT.ReactiveLogic.Internal;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlpacaIT.ReactiveLogic
{
    public partial class ReactiveLogicManager : MonoBehaviour
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
        private LinkedList<IReactive> reactives = new LinkedList<IReactive>();

        /// <summary>The list of active chain links that are executing.</summary>
        private LinkedList<ReactiveChainLink> links = new LinkedList<ReactiveChainLink>();

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
                links.AddLast(new ReactiveChainLink(activator, caller, reactive, input, delay, new ReactiveParameter(parameter)));
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
            links.AddLast(new ReactiveChainLink(activator, caller, target, input, delay, new ReactiveParameter(parameter)));
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
        internal void OnReactiveEnable(IReactive reactive)
        {
            RefreshReactiveData(reactive);
            reactives.AddLast(reactive);
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
        internal void OnReactiveDisable(IReactive reactive)
        {
            reactives.Remove(reactive);
        }

        /// <summary>Refreshes the cache data for an <see cref="IReactive"/> in the scene.</summary>
        /// <param name="reactive">The <see cref="IReactive"/> to be refreshed.</param>
        private void RefreshReactiveData(IReactive reactive)
        {
            // find the logic group that the reactive is in. the usual getcomponentinparent call
            // will include the current reactive which makes it impossible for a group to speak to
            // global objects as it will find itself, so we must begin our search in the parent.
            var parent = reactive.transform.parent;
            reactive.reactiveData.group = parent ? parent.GetComponentInParent<LogicGroup>() : null;
        }

        /// <summary>All of the logic gets executed once per fixed update.</summary>
        private void FixedUpdate()
        {
            // update all of the active chains.
            var node = links.First;
            while (node != null)
            {
                var next = node.Next;
                var chain = node.Value;

                // decrease the delay time remaining by delta time.
                chain.delay -= Time.fixedDeltaTime;
                if (chain.delay <= 0.0f)
                {
                    // remove the chain link before processing it.
                    links.Remove(node);

                    // make sure the target component has not been destroyed.
                    if (chain.target as UnityEngine.Object) // implicit Unity bool "exists" operator.
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