using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Represents an invoked input on an <see cref="IReactive"/> that will be handled by C#. This
    /// input may invoke an output on the current <see cref="IReactive"/> creating a chain of events.
    /// </summary>
    public class ReactiveInput
    {
        /// <summary>
        /// Creates a new instance with information on an invoked input on an <see
        /// cref="IReactive"/>. This can then be passed into <see cref="IReactive.OnReactiveInput"/>.
        /// </summary>
        /// <param name="activator">The game object that caused the current chain of events.</param>
        /// <param name="caller">The <see cref="IReactive"/> that invoked this input.</param>
        /// <param name="name">The name of this input that got triggered.</param>
        /// <param name="parameter">The parameter to be passed into this input.</param>
        public ReactiveInput(GameObject activator, IReactive caller, string name, ReactiveChainLinkParameter parameter)
        {
            this.activator = activator;
            this.caller = caller;
            this.name = name;
            this.parameter = parameter;
        }

        /// <summary>
        /// The game object that caused the current chain of events. This could for example be a
        /// physics object that fell onto a button. It would be the physics object that
        /// caused/activated it and not the button itself.
        /// </summary>
        public GameObject activator { get; }

        /// <summary>The <see cref="IReactive"/> that invoked this input.</summary>
        public IReactive caller { get; }

        /// <summary>The name of the input that got invoked.</summary>
        public string name { get; }

        /// <summary>The parameter that was passed from the caller output to this input.</summary>
        public ReactiveChainLinkParameter parameter { get; }
    }
}