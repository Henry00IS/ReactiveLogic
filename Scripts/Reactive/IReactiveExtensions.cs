using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>Provides extension methods for <see cref="IReactive"/>.</summary>
    public static class IReactiveExtensions
    {
        /// <summary>Fires an output and starts a new I/O chain.</summary>
        /// <param name="reactive">The reactive logic component that will trigger this input.</param>
        /// <param name="target">The reactive logic component that will receive this input.</param>
        /// <param name="name">The name of the input that will be triggered.</param>
        /// <param name="parameter">A parameter that will be passed from this output to this input.</param>
        public static void FireReactiveOutput(this IReactive reactive, string target, string name, object parameter)
        {
            ReactiveLogicManager.Instance.FireOutput(reactive, reactive, target, name, parameter);
        }

        /// <summary>Fires an output and starts a new I/O chain.</summary>
        /// <param name="reactive">The reactive logic component that will trigger this input.</param>
        /// <param name="activator">The reactive logic component originally caused this input.</param>
        /// <param name="target">The reactive logic component that will receive this input.</param>
        /// <param name="name">The name of the input that will be triggered.</param>
        /// <param name="parameter">A parameter that will be passed from this output to this input.</param>
        public static void FireReactiveOutput(this IReactive reactive, IReactive activator, string target, string name, object parameter)
        {
            ReactiveLogicManager.Instance.FireOutput(activator, reactive, target, name, parameter);
        }

        /// <summary>Fires an output and finds a matching output handler starting new I/O chains.</summary>
        /// <param name="reactive">The reactive logic component that will trigger this input.</param>
        /// <param name="name">The name of the output that is being triggered.</param>
        public static void OnReactiveOutput(this IReactive reactive, string name) // need an override with parameter.
        {
            var outputs = reactive.reactiveOutputs;
            var outputsCount = outputs.Count;
            for (int i = 0; i < outputsCount; i++)
            {
                var output = outputs[i];

                if (output.name == name)
                    ReactiveLogicManager.Instance.FireOutput(reactive, reactive, output.target, output.targetInput, output.parameter);
            }
        }
    }
}