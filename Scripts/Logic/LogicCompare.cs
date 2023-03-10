using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// An <see cref="IReactive"/> that compares two numbers and invokes an output depending on
    /// their relationship.
    /// </summary>
    public class LogicCompare : MonoBehaviour, IReactive
    {
        #region Required IReactive Implementation

        [SerializeField]
        private ReactiveData _reactiveData;
        public ReactiveData reactiveData => _reactiveData;
        public ReactiveMetadata reactiveMetadata => _reactiveMeta;

        private void OnEnable() => this.OnReactiveEnable();

        private void OnDisable() => this.OnReactiveDisable();

        #endregion Required IReactive Implementation

        /// <summary>The compare value to test the input against.</summary>
        [Tooltip("The compare value to test the input against.")]
        public float compareValue = 0.0f;

        /// <summary>The input value stored for tests.</summary>
        private float internalValue = 0.0f;

        private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata(
            new MetaInterface(MetaInterfaceType.Input, "SetValue", "Sets the input value to the float parameter without performing the test.", "value", MetaParameterType.Float, "The input value to be tested."),
            new MetaInterface(MetaInterfaceType.Input, "SetValueTest", "Sets the input value to the float parameter and performs the test.", "value", MetaParameterType.Float, "The input value to be tested."),
            new MetaInterface(MetaInterfaceType.Input, "SetCompareValue", "Sets the compare value to the float parameter without performing the test.", "value", MetaParameterType.Float, "The compare value to test the input against."),
            new MetaInterface(MetaInterfaceType.Input, "SetCompareValueTest", "Sets the compare value to the float parameter and performs the test.", "value", MetaParameterType.Float, "The compare value to test the input against."),
            new MetaInterface(MetaInterfaceType.Input, "Test", "Performs the test comparing the numbers and invoking the outputs accordingly."),

            new MetaInterface(MetaInterfaceType.Output, "Less", "Invoked when the input value is less than the compare value.", "parameter", MetaParameterType.Float, "The unmodified input value."),
            new MetaInterface(MetaInterfaceType.Output, "Equal", "Invoked when the input value is equal to the compare value.", "parameter", MetaParameterType.Float, "The unmodified input value."),
            new MetaInterface(MetaInterfaceType.Output, "NotEqual", "Invoked when the input value is not equal to the compare value.", "parameter", MetaParameterType.Float, "The unmodified input value."),
            new MetaInterface(MetaInterfaceType.Output, "Greater", "Invoked when the input value is greater than the compare value.", "parameter", MetaParameterType.Float, "The unmodified input value.")
        );

        public void OnReactiveInput(ReactiveInput input)
        {
            float inputValue = input.parameter.GetFloat();

            switch (input.name)
            {
                case "SetValue": internalValue = inputValue; break;
                case "SetValueTest": internalValue = inputValue; DoTest(input); break;
                case "SetCompareValue": compareValue = inputValue; break;
                case "SetCompareValueTest": compareValue = inputValue; DoTest(input); break;
                case "Test": DoTest(input); break;
            }
        }

        /// <summary>Performs the test comparing the numbers and invoking the outputs accordingly.</summary>
        private void DoTest(ReactiveInput input)
        {
            if (internalValue < compareValue)
                this.OnReactiveOutput(input, "Less", input.parameter);

            if (Mathf.Approximately(internalValue, compareValue))
                this.OnReactiveOutput(input, "Equal", input.parameter);
            else
                this.OnReactiveOutput(input, "NotEqual", input.parameter);

            if (internalValue > compareValue)
                this.OnReactiveOutput(input, "Greater", input.parameter);
        }
    }
}