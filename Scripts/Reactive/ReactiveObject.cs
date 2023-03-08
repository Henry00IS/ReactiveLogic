using System.Collections;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Represents a <see cref="GameObject"/> passed around in the <see cref="IReactive"/>
    /// ecosystem. In Unity the object may already be destroyed, but we still need to know some
    /// properties about the object as it was during an output such as the name.
    /// </summary>
    public class ReactiveObject
    {
        /// <summary>Gets the <see cref="GameObject"/> instance that may already be destroyed.</summary>
        public GameObject instance { get; }

        /// <summary>
        /// Gets the name of the <see cref="instance"/>, as it was during construction of the class.
        /// </summary>
        public string name { get; } = "";

        /// <summary>Creates a new <see cref="ReactiveObject"/> for the specified <paramref name="instance"/>.</summary>
        /// <param name="instance">The instance to be stored inside.</param>
        public ReactiveObject(GameObject instance)
        {
            this.instance = instance;
            if (instance)
            {
                name = instance.name;
            }
        }

        /// <summary>
        /// Checks whether the <see cref="instance"/> has been destroyed (same as checking <see
        /// cref="instance"/> directly).
        /// </summary>
        /// <param name="exists">The <see cref="ReactiveObject"/> to check.</param>
        public static implicit operator bool(ReactiveObject exists)
        {
            return exists.instance;
        }
    }
}