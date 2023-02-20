namespace AlpacaIT.ReactiveLogic
{
    /// <summary>Represents an input output chain for reactive logic components.</summary>
    public class Chain
    {
        public Chain(IReactive activator, IReactive caller, IReactive target, string input, ChainParameter parameter)
        {
            this.activator = activator;
            this.caller = caller;
            this.target = target;
            this.input = input;
            this.parameter = parameter;
        }

        /// <summary>The reactive logic component that caused the entire I/O chain.</summary>
        public IReactive activator { get; }

        /// <summary>The reactive logic component that triggered this input.</summary>
        public IReactive caller { get; }

        /// <summary>The reactive logic component that will receive this input.</summary>
        public IReactive target { get; }

        /// <summary>The name of the input that got triggered.</summary>
        public string input { get; }

        /// <summary>The parameter that was passed from an output to this input.</summary>
        public ChainParameter parameter { get; }

        /// <summary>Gets a <see cref="ReactiveInput"/> that represents this chain.</summary>
        /// <returns>The <see cref="ReactiveInput"/> instance.</returns>
        public ReactiveInput ToReactiveInput()
        {
            return new ReactiveInput(activator, caller, input, parameter);
        }
    }
}