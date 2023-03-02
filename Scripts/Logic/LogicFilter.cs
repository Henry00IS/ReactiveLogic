using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Represents an <see cref="IReactive"/> that passes through an invoked input if the activator
    /// passes a filter.
    /// </summary>
    public class LogicFilter : MonoBehaviour, IReactive
    {
        #region Required IReactive Implementation

        [SerializeField]
        private ReactiveData _reactiveData;
        public ReactiveData reactiveData => _reactiveData;
        public ReactiveMetadata reactiveMetadata => _reactiveMeta;

        private void OnEnable() => this.OnReactiveEnable();

        private void OnDisable() => this.OnReactiveDisable();

        #endregion Required IReactive Implementation

        private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata(
            new MetaInterface(MetaInterfaceType.Input, "Filter", "Passes through the input if the activator passes the filter.", "parameter", MetaParameterType.String, "The parameter to be passed to the output."),
            new MetaInterface(MetaInterfaceType.Output, "Passed", "Invoked when the activator passed the filter.", "parameter", MetaParameterType.String, "The parameter to be passed to the input."),
            new MetaInterface(MetaInterfaceType.Output, "Rejected", "Invoked when the activator got rejected by the filter.", "parameter", MetaParameterType.String, "The parameter to be passed to the input.")
        );

        /// <summary>The activator name must match this target name in order to pass the filter.</summary>
        [Tooltip("The activator name must match this target name in order to pass the filter.")]
        public string matchActivatorName;

        public void OnReactiveInput(ReactiveInput input)
        {
            if (input.name == "Filter")
            {
                var targetNameMatcher = ReactiveLogicManager.CreateTargetNameMatcher(matchActivatorName);
                if (targetNameMatcher(input.activator.name))
                {
                    OnFilterPassed(input);
                }
                else
                {
                    OnFilterRejected(input);
                }
            }
        }

        private void OnFilterRejected(ReactiveInput input)
        {
            this.OnReactiveOutput(input.activator, "Rejected", input.parameter);
        }

        private void OnFilterPassed(ReactiveInput input)
        {
            this.OnReactiveOutput(input.activator, "Passed", input.parameter);
        }
    }
}