using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Exposing this field on an <see cref="IReactive"/><see cref="MonoBehaviour"/> will display
    /// the tools in the Unity Inspector. It contains a list of user-configured output handlers of
    /// the <see cref="IReactive"/> as well as runtime data.
    /// </summary>
    [System.Serializable]
    public class ReactiveData
    {
        /// <summary>
        /// The list of user-configured output handlers of the <see cref="IReactive"/>. These
        /// outputs are usually configured in Unity Editor by the level designer.
        /// </summary>
        public List<ReactiveOutput> outputs = new List<ReactiveOutput>();

        /// <summary>
        /// Whether the <see cref="IReactive"/> is currently enabled. When disabled the <see
        /// cref="IReactive"/> will not receive any inputs and can't invoke any outputs (even if the
        /// <see cref="MonoBehaviour"/> script tries to do so). This is not the same as <see cref="Behaviour.enabled"/>.
        /// </summary>
        public bool enabled = true;
    }
}