using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// An <see cref="IReactive"/> that stores, manipulates and outputs a numerical value.
    /// </summary>
    public class LogicCounter : MonoBehaviour, IReactive
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
            new MetaInterface(MetaInterfaceType.Input, "Add", "Adds the float parameter to the counter without invoking the output.", "amount", MetaParameterType.Float, "The number to be added to the counter."),
            new MetaInterface(MetaInterfaceType.Input, "AddInvoke", "Adds the float parameter to the counter and invokes the output with the result.", "amount", MetaParameterType.Float, "The number to be added to the counter."),
            new MetaInterface(MetaInterfaceType.Input, "Multiply", "Multiplies the counter by the float parameter without invoking the output.", "amount", MetaParameterType.Float, "The number to multiply the counter with."),
            new MetaInterface(MetaInterfaceType.Input, "MultiplyInvoke", "Multiplies the counter by the float parameter and invokes the output.", "amount", MetaParameterType.Float, "The number to multiply the counter with."),
            new MetaInterface(MetaInterfaceType.Input, "Divide", "Divides the counter by the float parameter without invoking the output. If this causes a division by zero then the counter value will be unchanged.", "amount", MetaParameterType.Float, "The number to divide the counter with."),
            new MetaInterface(MetaInterfaceType.Input, "DivideInvoke", "Divides the counter by the float parameter and invokes the output. If this causes a division by zero then the counter value will be unchanged.", "amount", MetaParameterType.Float, "The number to divide the counter with."),
            new MetaInterface(MetaInterfaceType.Input, "Subtract", "Subtracts the float parameter from the counter without invoking the output.", "amount", MetaParameterType.Float, "The number to be subtracted from the counter."),
            new MetaInterface(MetaInterfaceType.Input, "SubtractInvoke", "Subtracts the float parameter from the counter and invokes the output with the result.", "amount", MetaParameterType.Float, "The number to be subtracted from the counter."),
            new MetaInterface(MetaInterfaceType.Input, "SetValue", "Sets the counter to the float parameter without invoking the output.", "amount", MetaParameterType.Float, "The number the counter will be set to."),
            new MetaInterface(MetaInterfaceType.Input, "SetValueInvoke", "Sets the counter to the float parameter and invokes the output with the result.", "amount", MetaParameterType.Float, "The number the counter will be set to."),
            new MetaInterface(MetaInterfaceType.Input, "Invoke", "Invokes the output with the current value of the counter."),

            new MetaInterface(MetaInterfaceType.Output, "Invoked", "Invoked when an operation on the current value of the counter finished.", "amount", MetaParameterType.Float, "The current value of the counter.")
        );

        /// <summary>The current value of the counter.</summary>
        [Tooltip("The current value of the counter.")]
        public float currentValue = 0.0f;

        public void OnReactiveInput(ReactiveInput input)
        {
            float inputValue = input.parameter.GetFloat();

            switch (input.name)
            {
                case "Add": currentValue += inputValue; break;
                case "AddInvoke": currentValue += inputValue; InvokeOutput(input); break;
                case "Multiply": currentValue *= inputValue; break;
                case "MultiplyInvoke": currentValue *= inputValue; InvokeOutput(input); break;
                case "Divide": if (inputValue != 0f) currentValue /= inputValue; break;
                case "DivideInvoke": if (inputValue != 0f) currentValue /= inputValue; InvokeOutput(input); break;
                case "Subtract": currentValue -= inputValue; break;
                case "SubtractInvoke": currentValue -= inputValue; InvokeOutput(input); break;
                case "SetValue": currentValue = inputValue; break;
                case "SetValueInvoke": currentValue = inputValue; InvokeOutput(input); break;
                case "Invoke": InvokeOutput(input); break;
            }
        }

        private void InvokeOutput(ReactiveInput input)
        {
            this.OnReactiveOutput(input.activator, "Invoked", currentValue);
        }
    }
}