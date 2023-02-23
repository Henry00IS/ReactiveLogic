using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="name">The target name to find.</param>
        public IEnumerable<IReactive> ForEachReactive(string name)
        {
            if (string.IsNullOrEmpty(name)) yield break;

            // make sure we can execute this before FixedUpdate.
            if (reactives == null) FindReactives();

            // iterate over all reactives in the scene.
            for (int i = 0; i < reactives.Length; i++)
            {
                var reactive = reactives[i];

                // direct name comparison:
                if (reactive.gameObject && reactive.gameObject.name == name)
                {
                    yield return reactive;
                }
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
            foreach (var reactive in ForEachReactive(target))
                chains.Add(new ReactiveChainLink(activator, caller, reactive, input, delay, new ReactiveChainLinkParameter(parameter)));
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