﻿namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Represents an input containing a parameter and references for the triggered reactive logic.
    /// </summary>
    public class ReactiveInput
    {
        public ReactiveInput(IReactive activator, IReactive caller, string input, ChainParameter parameter)
        {
            this.activator = activator;
            this.caller = caller;
            this.input = input;
            this.parameter = parameter;
        }

        /// <summary>The reactive logic component that caused the entire I/O chain.</summary>
        public IReactive activator { get; }

        /// <summary>The reactive logic component that triggered this input.</summary>
        public IReactive caller { get; }

        /// <summary>The name of the input that got triggered.</summary>
        public string input { get; }

        /// <summary>The parameter that was passed from an output to this input.</summary>
        public ChainParameter parameter { get; }
    }
}