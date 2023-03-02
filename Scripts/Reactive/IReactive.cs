using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>Implement this interface to make your <see cref="MonoBehaviour"/> reactive.
    /// <para>Every <see cref="IReactive"/> should be implemented like this:</para>
    /// <code>
    ///[SerializeField]
    ///private ReactiveData _reactiveData;
    ///public ReactiveData reactiveData =&gt; _reactiveData;
    ///public ReactiveMetadata reactiveMetadata =&gt; _reactiveMeta;
    ///
    ///private void OnEnable() =&gt; this.OnReactiveEnable();
    ///
    ///private void OnDisable() =&gt; this.OnReactiveDisable();
    ///
    ///private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata(...);
    /// </code>
    /// </summary>
    public interface IReactive
    {
        /// <summary>
        /// The <see cref="Transform"/> attached to the <see cref="GameObject"/> of the <see cref="IReactive"/>.
        /// </summary>
        public Transform transform { get; }

        /// <summary>The <see cref="GameObject"/> of the <see cref="IReactive"/>.</summary>
        public GameObject gameObject { get; }

        /// <summary>
        /// Gets metadata for the <see cref="IReactive"/>. This contains a list of inputs, outputs,
        /// parameters and descriptions among other things. This is primarily used to support the
        /// game developer within Unity Editor and is not a requirement for function.
        /// <para>
        /// It's recommended to make the returned value static to keep the memory footprint low and
        /// to prevent instantiating the <see cref="ReactiveMetadata"/> whenever the component gets created.
        /// </para>
        /// </summary>
        public ReactiveMetadata reactiveMetadata { get; }

        /// <summary>
        /// Gets the runtime data of the <see cref="IReactive"/> including a list of user-configured
        /// output handlers. This data is usually configured in Unity Editor by the level designer
        /// or used internally by the <see cref="ReactiveLogicManager"/> and preferably should not
        /// be modified from user-scripts.
        /// </summary>
        public ReactiveData reactiveData { get; }

        /// <summary>
        /// Called when an input gets invoked on this <see cref="IReactive"/>. This input may invoke
        /// an output on this <see cref="IReactive"/> creating a chain of events.
        /// </summary>
        /// <param name="input">The input that was invoked on this <see cref="IReactive"/>.</param>
        void OnReactiveInput(ReactiveInput input);
    }
}