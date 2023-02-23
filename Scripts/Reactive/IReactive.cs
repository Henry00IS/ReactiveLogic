using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>Implement this interface to make your <see cref="MonoBehaviour"/> reactive.</summary>
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
        /// <para>For full Unity Editor support it should always be implemented like this:</para>
        /// <code>
        ///[SerializeField]
        ///private ReactiveEditor _reactiveEditor;
        ///
        ///public ReactiveMetadata reactiveMetadata =&gt; _reactiveMeta;
        ///private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata();</code>
        /// <para>The 'reactiveEditor' adds a section to the Unity Inspector. We set it to private
        /// and expose it with [SerializeField] to keep the code clean. The 'reactiveMetadata' is a
        /// required property that accesses 'reactiveMeta' to prevent instantiating the class every
        /// time the property is accessed. It's also static to keep the memory footprint low and to
        /// prevent instantiating the class whenever the component gets created.</para>
        /// </summary>
        public ReactiveMetadata reactiveMetadata { get; }

        /// <summary>
        /// Gets a list of user-configured output handlers of the <see cref="IReactive"/>. These
        /// outputs are usually configured in Unity Editor by the level designer.
        /// </summary>
        public List<ReactiveOutput> reactiveOutputs { get; }

        /// <summary>
        /// Called when an input gets invoked on this <see cref="IReactive"/>. This input may invoke
        /// an output on this <see cref="IReactive"/> creating a chain of events.
        /// </summary>
        /// <param name="input">The input that was invoked on this <see cref="IReactive"/>.</param>
        void OnReactiveInput(ReactiveInput input);
    }
}