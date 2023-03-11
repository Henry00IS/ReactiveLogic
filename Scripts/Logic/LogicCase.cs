using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// An <see cref="IReactive"/> that matches the input parameter to a list of cases and invokes
    /// the associated output.
    /// </summary>
    [HelpURL("https://github.com/Henry00IS/ReactiveLogic/wiki/Logic-Case")]
    public class LogicCase : MonoBehaviour, IReactive
    {
        #region Required IReactive Implementation

        [SerializeField]
        private ReactiveData _reactiveData;
        public ReactiveData reactiveData => _reactiveData;

        private void OnEnable() => this.OnReactiveEnable();

        private void OnDisable() => this.OnReactiveDisable();

        #endregion Required IReactive Implementation

        // as the outputs change depending on the cases, we create the metadata every time it's requested.
        public ReactiveMetadata reactiveMetadata
        {
            get
            {
                var interfaces = new List<MetaInterface>();
                interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "Invoke", "Invokes a case output matching the string parameter.", "parameter", MetaParameterType.String, "The parameter value that determines the case output."));

                var outputCasesCount = cases.Count;
                for (int i = 0; i < outputCasesCount; i++)
                {
                    var outputCase = cases[i];
                    if (!string.IsNullOrWhiteSpace(outputCase.name))
                        interfaces.Add(new MetaInterface(MetaInterfaceType.Output, "Case" + outputCase.name, "Invoked when the input parameter matched '" + outputCase.match + "'.", "parameter", MetaParameterType.String, "The parameter value that determined the case output."));
                }

                interfaces.Add(new MetaInterface(MetaInterfaceType.Output, "Default", "Invoked when no case output matched the input parameter.", "parameter", MetaParameterType.String, "The parameter value that determined the case output."));
                return new ReactiveMetadata(interfaces.ToArray());
            }
        }

        [System.Serializable]
        public class Case
        {
            /// <summary>The input parameter value must match this value for the case to be output.</summary>
            [Tooltip("The input parameter value must match this value for the case to be output.")]
            public string match;

            /// <summary>The output name for the case. This will be invoked as Case[Name].</summary>
            [Tooltip("The output name for the case. This will be invoked as Case[Name].")]
            public string name;
        }

        /// <summary>
        /// The collection of cases each with value to be matched and the name of the output.
        /// </summary>
        public List<Case> cases = new List<Case>();

        public void OnReactiveInput(ReactiveInput input)
        {
            var parameter = input.parameter.GetString();

            switch (input.name)
            {
                case "Invoke":
                    if (TryGetCase(parameter, out Case outputCase))
                    {
                        this.OnReactiveOutput(input, "Case" + outputCase.name, input.parameter);
                    }
                    else
                    {
                        this.OnReactiveOutput(input, "Default", input.parameter);
                    }
                    break;
            }
        }

        private bool TryGetCase(string parameter, out Case outputCase)
        {
            outputCase = null;
            var outputCasesCount = cases.Count;
            for (int i = 0; i < outputCasesCount; i++)
            {
                outputCase = cases[i];
                if (!string.IsNullOrWhiteSpace(outputCase.name) && outputCase.match == parameter)
                    return true;
            }
            return false;
        }
    }
}