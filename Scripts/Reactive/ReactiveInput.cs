namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Represents an input containing a parameter and references for the triggered reactive logic.
    /// </summary>
    public class ReactiveInput
    {
        public ReactiveInput(IReactive activator, IReactive caller, IReactive receiver, string name, ChainParameter parameter)
        {
            this.activator = activator;
            this.caller = caller;
            self = receiver;
            this.name = name;
            this.parameter = parameter;
        }

        /// <summary>The reactive logic component that caused the entire I/O chain.</summary>
        public IReactive activator { get; }

        /// <summary>The reactive logic component that triggered this input.</summary>
        public IReactive caller { get; }

        /// <summary>The reactive logic component that received this input.</summary>
        public IReactive self { get; }

        /// <summary>The name of the input that got triggered.</summary>
        public string name { get; }

        /// <summary>The parameter that was passed from an output to this input.</summary>
        public ChainParameter parameter { get; }
    }
}