using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>Provides extension methods for <see cref="IReactive"/>.</summary>
    public static class IReactiveExtensions
    {
        /// <summary>
        /// Invokes user-configured output handlers on the <see cref="IReactive"/> matching the
        /// specified output <paramref name="name"/>.
        /// </summary>
        /// <param name="reactive">The <see cref="IReactive"/> whose output handlers will be invoked.</param>
        /// <param name="activator">
        /// The game object that caused the current chain of events. This could for example be a
        /// physics object that fell onto a button. It would be the physics object that
        /// caused/activated it and not the button itself.
        /// </param>
        /// <param name="name">The output name to be invoked on the <see cref="IReactive"/>.</param>
        public static void OnReactiveOutput(this IReactive reactive, GameObject activator, string name)
        {
            var outputs = reactive.reactiveOutputs;
            var outputsCount = outputs.Count;
            for (int i = 0; i < outputsCount; i++)
            {
                var output = outputs[i];

                if (output.name == name)
                    ReactiveLogicManager.Instance.ScheduleInput(activator, reactive, output.targetName, output.targetInput, output.delay, output.targetInputParameter);
            }
        }

        /// <summary>
        /// Invokes user-configured output handlers on the <see cref="IReactive"/> matching the
        /// specified output <paramref name="name"/>.
        /// </summary>
        /// <param name="reactive">The <see cref="IReactive"/> whose output handlers will be invoked.</param>
        /// <param name="activator">
        /// The game object that caused the current chain of events. This could for example be a
        /// physics object that fell onto a button. It would be the physics object that
        /// caused/activated it and not the button itself.
        /// </param>
        /// <param name="name">The output name to be invoked on the <see cref="IReactive"/>.</param>
        /// <param name="parameter">
        /// The parameter to be passed to the next input. This may be overridden by the level
        /// designer when they put a custom parameter into the output handler.
        /// </param>
        public static void OnReactiveOutput(this IReactive reactive, GameObject activator, string name, object parameter)
        {
            var outputs = reactive.reactiveOutputs;
            var outputsCount = outputs.Count;
            for (int i = 0; i < outputsCount; i++)
            {
                var output = outputs[i];

                // only pass the parameter directly when not overridden by a value in the output.
                object outputParameter = parameter;
                if (!string.IsNullOrEmpty(output.targetInputParameter))
                    outputParameter = output.targetInputParameter;

                if (output.name == name)
                    ReactiveLogicManager.Instance.ScheduleInput(activator, reactive, output.targetName, output.targetInput, output.delay, outputParameter);
            }
        }

        /// <summary>
        /// This function must be called when the behaviour becomes enabled and active.
        /// <para>
        /// Every <see cref="IReactive"/> must call <see
        /// cref="IReactiveExtensions.OnReactiveEnable"/> here:
        /// </para>
        /// <code>
        ///void OnEnable()
        ///{
        ///     this.OnReactiveEnable();
        ///}
        /// </code>
        /// </summary>
        public static void OnReactiveEnable(this IReactive reactive)
        {
            ReactiveLogicManager.Instance.OnReactiveEnable(reactive);
        }

        /// <summary>
        /// This function must be called when the behaviour becomes disabled or inactive.
        /// <para>
        /// Every <see cref="IReactive"/> must call <see
        /// cref="IReactiveExtensions.OnReactiveDisable"/> here:
        /// </para>
        /// <code>
        ///void OnDisable()
        ///{
        ///     this.OnReactiveDisable();
        ///}
        /// </code>
        /// </summary>
        public static void OnReactiveDisable(this IReactive reactive)
        {
            if (ReactiveLogicManager.hasInstance)
                ReactiveLogicManager.Instance.OnReactiveDisable(reactive);
        }
    }
}