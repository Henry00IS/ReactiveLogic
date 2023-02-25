using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// An <see cref="IReactive"/> that passes through an invoked input. The relay makes it easy to
    /// invoke multiple targets with a single input. This is very helpful for organization.
    /// </summary>
    public class LogicRelay : MonoBehaviour, IReactive
    {
        #region Required IReactive Implementation

        [SerializeField]
        private ReactiveEditor _reactiveEditor;

        [SerializeField]
        [HideInInspector]
        private List<ReactiveOutput> _reactiveOutputs = new List<ReactiveOutput>();
        public List<ReactiveOutput> reactiveOutputs => _reactiveOutputs;

        public ReactiveMetadata reactiveMetadata => _reactiveMeta;

        #endregion Required IReactive Implementation

        private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata(
            new MetaInterface(MetaInterfaceType.Input, "Invoke", "Invokes the relay passing through the input.", "parameter", MetaParameterType.String, "The parameter to be passed to the output."),
            new MetaInterface(MetaInterfaceType.Output, "Invoked", "Invoked when the relay is invoked.", "parameter", MetaParameterType.String, "The parameter to be passed to the input.")
        );

        public void OnReactiveInput(ReactiveInput input)
        {
            if (input.name == "Invoke")
            {
                this.OnReactiveOutput(input.activator, "Invoked", input.parameter);
            }
        }
    }
}