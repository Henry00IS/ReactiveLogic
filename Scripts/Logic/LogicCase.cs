using AlpacaIT.ReactiveLogic.Internal;
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
                interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "Invoke", "Invokes a case output matching the string parameter.", "parameter", MetaParameterType.String, "The parameter value to match against the case values."));
                interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "Random", "Invokes a random case output (even with no configured cases this never invokes Default).", "parameter", MetaParameterType.String, "The parameter to be passed to the output."));
                interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "RandomExclusive", "Invokes a random case output excluding the last random case output (even with no configured cases this never invokes Default). This prevents the same case from getting invoked twice in a row (unless there is only one configured case).", "parameter", MetaParameterType.String, "The parameter to be passed to the output."));
                interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "RandomShuffle", "Invokes a random case output as a shuffled deck of cards, so that each case must occur in random order before it resets and shuffles another deck of cards (even with no configured cases this never invokes Default).", "parameter", MetaParameterType.String, "The parameter to be passed to the output."));

                var outputCasesCount = cases.Count;
                for (int i = 0; i < outputCasesCount; i++)
                {
                    var outputCase = cases[i];
                    if (!string.IsNullOrWhiteSpace(outputCase.name))
                        interfaces.Add(new MetaInterface(MetaInterfaceType.Output, "Case" + outputCase.name, "Invoked when the input parameter matched '" + outputCase.match + "'.", "parameter", MetaParameterType.String, "The parameter that was passed to the input."));
                }

                interfaces.Add(new MetaInterface(MetaInterfaceType.Output, "Default", "Invoked when no case output matched the input parameter.", "parameter", MetaParameterType.String, "The parameter that was passed to the input."));
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

        /// <summary>The last random case picked by <see cref="TryGetRandomCaseExclusive"/>.</summary>
        private Case lastRandomExclusiveCase;

        /// <summary>The deck of cards used by <see cref="TryGetRandomCaseShuffle"/>.</summary>
        private IEnumerator<Case> randomShuffleDeck;

        public void OnReactiveInput(ReactiveInput input)
        {
            var parameter = input.parameter.GetString();

            switch (input.name)
            {
                case "Invoke":
                    {
                        if (TryGetCase(parameter, out Case outputCase))
                        {
                            this.OnReactiveOutput(input, "Case" + outputCase.name, input.parameter);
                        }
                        else
                        {
                            this.OnReactiveOutput(input, "Default", input.parameter);
                        }
                    }
                    break;

                case "Random":
                    {
                        if (TryGetRandomCase(out Case outputCase))
                        {
                            this.OnReactiveOutput(input, "Case" + outputCase.name, input.parameter);
                        }
                    }
                    break;

                case "RandomExclusive":
                    {
                        if (TryGetRandomCaseExclusive(out Case outputCase))
                        {
                            this.OnReactiveOutput(input, "Case" + outputCase.name, input.parameter);
                        }
                    }
                    break;

                case "RandomShuffle":
                    {
                        if (TryGetRandomCaseShuffle(out Case outputCase))
                        {
                            this.OnReactiveOutput(input, "Case" + outputCase.name, input.parameter);
                        }
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

        private bool TryGetRandomCase(out Case outputCase)
        {
            outputCase = Utilities.RandomItem(cases);
            return outputCase != null;
        }

        private bool TryGetRandomCaseExclusive(out Case outputCase)
        {
            outputCase = lastRandomExclusiveCase = Utilities.RandomExcept(cases, lastRandomExclusiveCase);
            return outputCase != null;
        }

        private bool TryGetRandomCaseShuffle(out Case outputCase)
        {
            // must have cases configured.
            if (cases.Count == 0) { outputCase = null; return false; };

            // shuffle a deck of cards.
            if (randomShuffleDeck == null)
                randomShuffleDeck = cases.Shuffle().GetEnumerator();

            // pick a card and return it.
            if (randomShuffleDeck.MoveNext())
            {
                outputCase = randomShuffleDeck.Current;
                return true;
            }
            else
            {
                // shuffle another deck of cards and have this function return a card.
                randomShuffleDeck = null;
                return TryGetRandomCaseShuffle(out outputCase);
            }
        }
    }
}