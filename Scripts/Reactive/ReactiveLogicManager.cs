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
        private List<Chain> chains = new List<Chain>();

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

        /// <summary>Fires an output triggering an input on a reactive logic component.</summary>
        /// <param name="activator">The reactive logic component that caused the entire I/O chain.</param>
        /// <param name="caller">The reactive logic component that is triggering this input.</param>
        /// <param name="target">The name of the target object that receives this input.</param>
        /// <param name="name">The name of the input.</param>
        /// <param name="parameter">The parameter to be passed to the input.</param>
        public void FireOutput(IReactive activator, IReactive caller, string target, string name, object parameter)
        {
            foreach (var reactive in ForEachReactive(target))
                chains.Add(new Chain(activator, caller, reactive, name, new ChainParameter(parameter)));
        }

        /// <summary>All of the logic gets executed once per fixed update.</summary>
        private void FixedUpdate()
        {
            // find all of the reactive components in the scene.
            FindReactives();

            for (int i = chains.Count; i-- > 0;)
            {
                var chain = chains[i];
                chain.target?.OnReactiveInput(chain.ToReactiveInput());
                chains.RemoveAt(i);
            }
        }
    }
}