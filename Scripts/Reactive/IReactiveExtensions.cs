namespace AlpacaIT.ReactiveLogic
{
    /// <summary>Provides extension methods for <see cref="IReactive"/>.</summary>
    public static class IReactiveExtensions
    {
        /// <summary>Fires an output on this reactive logic component.</summary>
        /// <param name="reactive">The reactive logic component that will trigger this input.</param>
        /// <param name="name">The name of the output that is being triggered.</param>
        public static void OnReactiveOutput(this IReactive reactive, string name)
        {
            var outputs = reactive.reactiveOutputs;
            var outputsCount = outputs.Count;
            for (int i = 0; i < outputsCount; i++)
            {
                var output = outputs[i];

                if (output.name == name)
                    ReactiveLogicManager.Instance.FireOutput(reactive, reactive, output.target, output.input, output.delay, output.parameter);
            }
        }

        /// <summary>Fires an output on this reactive logic component.</summary>
        /// <param name="reactive">The reactive logic component that will trigger this input.</param>
        /// <param name="name">The name of the output that is being triggered.</param>
        /// <param name="parameter">The parameter passed to the input when not overridden.</param>
        public static void OnReactiveOutput(this IReactive reactive, string name, object parameter)
        {
            var outputs = reactive.reactiveOutputs;
            var outputsCount = outputs.Count;
            for (int i = 0; i < outputsCount; i++)
            {
                var output = outputs[i];

                // only pass the parameter directly when not overriden by a value in the output.
                object outputParameter = parameter;
                if (!string.IsNullOrEmpty(output.parameter))
                    outputParameter = output.parameter;

                if (output.name == name)
                    ReactiveLogicManager.Instance.FireOutput(reactive, reactive, output.target, output.input, output.delay, outputParameter);
            }
        }
    }
}