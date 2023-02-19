using System;
using System.Collections.Generic;
using UnityEngine;

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

        public IReactive FindObjectByName(string name)
        {
            var go = GameObject.Find(name);
            if (!go) return null;
            return go.GetComponent<IReactive>();
        }

        /// <summary>Fires an output triggering an input on a reactive logic component.</summary>
        /// <param name="activator">The reactive logic component that caused the entire I/O chain.</param>
        /// <param name="caller">The reactive logic component is triggering this input.</param>
        /// <param name="target">The name of the target object that receives this input.</param>
        /// <param name="name">The name of the input.</param>
        /// <param name="parameter">The parameter to be passed to the input.</param>
        public void FireOutput(IReactive activator, IReactive caller, string target, string name, object parameter)
        {
            var targetReactive = FindObjectByName(target);
            if (targetReactive == null) return;

            targetReactive.OnReactiveInput(new ReactiveInput(activator, caller, targetReactive, name, new ChainParameter(parameter)));
        }
    }
}