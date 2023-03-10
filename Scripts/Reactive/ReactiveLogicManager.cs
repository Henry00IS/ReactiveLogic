using AlpacaIT.ReactiveLogic.Internal;
using System.Collections.Generic;
using UnityEngine;

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
        /// <param name="target">
        /// The target name that yields one or more <see cref="IReactive"/> that will receive this input.
        /// </param>
        /// <param name="input">The target input name that will be invoked.</param>
        /// <param name="delay">
        /// The delay in seconds to wait before invoking the input on the target. For unsafe
        /// immediate execution that can crash the game when an infinite loop occurs, you can set
        /// this value to -1.
        /// </param>
        /// <param name="parameter">The parameter that will be passed to the input of the target.</param>
        public void ScheduleInput(ReactiveObject activator, IReactive caller, string target, string input, float delay, object parameter)
        {
            foreach (var reactive in ForEachReactive(caller, target))
                ScheduleInput(activator, caller, reactive, input, delay, parameter);
        }

        /// <summary>
        /// Schedules invoking an input on an <see cref="IReactive"/>. This takes at least one fixed
        /// update step and may optionally be delayed further.
        /// </summary>
        /// <param name="activator">The game object that caused this chain of events.</param>
        /// <param name="caller">The <see cref="IReactive"/> that invoked this input.</param>
        /// <param name="target">The <see cref="IReactive"/> that will receive this input.</param>
        /// <param name="input">The target input name that will be invoked.</param>
        /// <param name="delay">
        /// The delay in seconds to wait before invoking the input on the target. For unsafe
        /// immediate execution that can crash the game when an infinite loop occurs, you can set
        /// this value to -1.
        /// </param>
        /// <param name="parameter">The parameter that will be passed to the input of the target.</param>
        public void ScheduleInput(ReactiveObject activator, IReactive caller, IReactive target, string input, float delay, object parameter)
        {
            var link = new ReactiveChainLink(activator, caller, target, input, delay, new ReactiveParameter(parameter));

            // allow for unsafe immediate execution when the delay is set the negative one.
            if (delay == -1f)
                ProcessLink(link);
            else
                links.AddLast(link);
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

        private void CancelLinks(IReactive target)
        {
            // iterate over all of the active links.
            var node = links.First;
            while (node != null)
            {
                var next = node.Next;
                var link = node.Value;

                // cancel links where the caller is the target:
                if (link.caller == target)
                    link.canceled = true;

                node = next;
            }
        }

        private void ProcessLink(ReactiveChainLink link)
        {
            // make sure the link has not been canceled.
            if (link.canceled) return;

            // make sure the target component has not been destroyed.
            if (link.target as UnityEngine.Object) // implicit Unity bool "exists" operator.
            {
                // The "Enable"-input always works.
                if (link.targetInput == "Enable")
                {
                    link.target.reactiveData.enabled = true;
                }
                // The "Cancel"-input always works.
                else if (link.targetInput == "Cancel")
                {
                    // cancel all active links of the target.
                    CancelLinks(link.target);
                }
                else if (link.target.reactiveData.enabled)
                {
                    // "Disable"-input.
                    if (link.targetInput == "Disable")
                        link.target.reactiveData.enabled = false;

                    // "User"-inputs invoke the output handlers at the target reactive.
                    else if (link.targetInputIsUserInput)
                        link.target.OnReactiveOutput(link.activator, link.targetInput, link.targetParameter);

                    // Invoke the input handler at the target reactive.
                    else
                        link.target.OnReactiveInput(link.ToReactiveInput());
                }
            }
        }

        /// <summary>All of the logic gets executed once per fixed update.</summary>
        private void FixedUpdate()
        {
            // update all of the active chains.
            var node = links.First;
            while (node != null)
            {
                var next = node.Next;
                var link = node.Value;

                // decrease the delay time remaining by delta time.
                link.delay -= Time.fixedDeltaTime;
                if (link.delay <= 0.0f)
                {
                    // remove the chain link before processing it.
                    links.Remove(node);

                    // process the link.
                    ProcessLink(link);
                }

                node = next;
            }
        }
    }
}