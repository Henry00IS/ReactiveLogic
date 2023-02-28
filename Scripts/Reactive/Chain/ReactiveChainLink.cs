using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>Represents a link in a chain of events between two <see cref="IReactive"/>.</summary>
    public class ReactiveChainLink
    {
        /// <summary>
        /// Creates a new chain link instance containing a pending input to be invoked on an <see
        /// cref="IReactive"/> by another <see cref="IReactive"/>.
        /// </summary>
        /// <param name="activator">The game object that caused the current chain of events.</param>
        /// <param name="caller">The <see cref="IReactive"/> that invoked this input.</param>
        /// <param name="target">The <see cref="IReactive"/> that this input will be invoked on.</param>
        /// <param name="targetInput">The target input name that will be invoked.</param>
        /// <param name="delay">The delay in seconds to wait before invoking the input on the target.</param>
        /// <param name="targetParameter">The parameter that will be passed to the input of the target.</param>
        public ReactiveChainLink(GameObject activator, IReactive caller, IReactive target, string targetInput, float delay, ReactiveChainLinkParameter targetParameter)
        {
            this.activator = activator;
            this.caller = caller;
            this.target = target;
            this.targetInput = targetInput;
            this.delay = delay;
            this.targetParameter = targetParameter;
        }

        /// <summary>
        /// The game object that caused the current chain of events. This could for example be a
        /// physics object that fell onto a button. It would be the physics object that
        /// caused/activated it and not the button itself.
        /// </summary>
        public GameObject activator { get; }

        /// <summary>The <see cref="IReactive"/> that invoked this input.</summary>
        public IReactive caller { get; }

        /// <summary>The <see cref="IReactive"/> that this input will be invoked on.</summary>
        public IReactive target { get; }

        /// <summary>The target input name that will be invoked.</summary>
        public string targetInput { get; }

        /// <summary>The parameter that will be passed to the input of the target.</summary>
        public ReactiveChainLinkParameter targetParameter { get; }

        /// <summary>
        /// Gets or sets remaining delay in seconds to wait before invoking the input on the target.
        /// </summary>
        public float delay { get; set; }

        /// <summary>
        /// Gets whether the <see cref="targetInput"/> starts with "User" and should invoke an
        /// output at the target <see cref="IReactive"/> with the same name, instead of calling the
        /// <see cref="IReactive.OnReactiveInput"/> method.
        /// </summary>
        public bool targetInputIsUserDefined => targetInput.StartsWith("User");

        /// <summary>
        /// Returns a new <see cref="ReactiveInput"/> that can be passed into the <see
        /// cref="IReactive.OnReactiveInput"/> of the target.
        /// </summary>
        public ReactiveInput ToReactiveInput()
        {
            return new ReactiveInput(activator, caller, targetInput, targetParameter);
        }
    }
}