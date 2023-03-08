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
            new MetaInterface(MetaInterfaceType.Input, "MultiplyInvoke", "Multiplies the counter by the float parameter and invokes the output with the result.", "amount", MetaParameterType.Float, "The number to multiply the counter with."),
            new MetaInterface(MetaInterfaceType.Input, "Divide", "Divides the counter by the float parameter without invoking the output. If this causes a division by zero then the counter value will be unchanged.", "amount", MetaParameterType.Float, "The number to divide the counter with."),
            new MetaInterface(MetaInterfaceType.Input, "DivideInvoke", "Divides the counter by the float parameter and invokes the output with the result. If this causes a division by zero then the counter value will be unchanged.", "amount", MetaParameterType.Float, "The number to divide the counter with."),
            new MetaInterface(MetaInterfaceType.Input, "Subtract", "Subtracts the float parameter from the counter without invoking the output.", "amount", MetaParameterType.Float, "The number to be subtracted from the counter."),
            new MetaInterface(MetaInterfaceType.Input, "SubtractInvoke", "Subtracts the float parameter from the counter and invokes the output with the result.", "amount", MetaParameterType.Float, "The number to be subtracted from the counter."),
            new MetaInterface(MetaInterfaceType.Input, "SetValue", "Sets the counter to the float parameter without invoking the output.", "amount", MetaParameterType.Float, "The number the counter will be set to."),
            new MetaInterface(MetaInterfaceType.Input, "SetValueInvoke", "Sets the counter to the float parameter and invokes the output with the result.", "amount", MetaParameterType.Float, "The number the counter will be set to."),

            new MetaInterface(MetaInterfaceType.Input, "SetMin", "Sets the minimum value of the counter to the float parameter without invoking the output.", "amount", MetaParameterType.Float, "The minimum number the counter can be set to. If both min and max are set to zero, no clamping will occur."),
            new MetaInterface(MetaInterfaceType.Input, "SetMinInvoke", "Sets the minimum value of the counter to the float parameter and invokes the output with the result.", "amount", MetaParameterType.Float, "The minimum number the counter can be set to. If both min and max are set to zero, no clamping will occur."),
            new MetaInterface(MetaInterfaceType.Input, "SetMax", "Sets the maximum value of the counter to the float parameter without invoking the output.", "amount", MetaParameterType.Float, "The maximum number the counter can be set to. If both min and max are set to zero, no clamping will occur."),
            new MetaInterface(MetaInterfaceType.Input, "SetMaxInvoke", "Sets the maximum value of the counter to the float parameter and invokes the output with the result.", "amount", MetaParameterType.Float, "The maximum number the counter can be set to. If both min and max are set to zero, no clamping will occur."),

            new MetaInterface(MetaInterfaceType.Input, "Freeze", "Freezes the counter value so that it cannot be changed by inputs. This makes the counter read-only, which is very useful for the SetMin and SetMax operations, which would otherwise always forcibly clamp the value before you could set the other field."),
            new MetaInterface(MetaInterfaceType.Input, "Unfreeze", "Unfreezes the counter value so that it can be changed by inputs."),
            new MetaInterface(MetaInterfaceType.Input, "Invoke", "Invokes the output with the current value of the counter."),

            new MetaInterface(MetaInterfaceType.Output, "Invoked", "Invoked when an operation on the current value of the counter finished.", "amount", MetaParameterType.Float, "The current value of the counter."),
            new MetaInterface(MetaInterfaceType.Output, "ReachedMin", "Invoked when clamping is active and the counter reached the minimum value.", "amount", MetaParameterType.Float, "The current value of the counter (always the minimum value)."),
            new MetaInterface(MetaInterfaceType.Output, "ReachedMax", "Invoked when clamping is active and the counter reached the maximum value.", "amount", MetaParameterType.Float, "The current value of the counter (always the maximum value)."),
            new MetaInterface(MetaInterfaceType.Output, "ChangedFromMin", "Invoked when clamping is active and the counter changed from the minimum value.", "amount", MetaParameterType.Float, "The current value of the counter."),
            new MetaInterface(MetaInterfaceType.Output, "ChangedFromMax", "Invoked when clamping is active and the counter changed from the maximum value.", "amount", MetaParameterType.Float, "The current value of the counter.")
        );

        /// <summary>
        /// The initial and current value of the counter. Use <see cref="SetCurrentValue"/> to set
        /// this field and respect the counter being frozen or clamped by the user.
        /// </summary>
        [SerializeField]
        [Tooltip("The initial and current value of the counter.")]
        private float currentValue = 0f;

        /// <summary>
        /// This is the minimum value of the counter. It cannot go below this value. If both <see
        /// cref="clampMin"/> and <see cref="clampMax"/> are set to zero, no clamping will occur.
        /// </summary>
        [Tooltip("This is the minimum value of the counter. It cannot go below this value. If both Clamp Min and Clamp Max are set to zero, no clamping will occur.")]
        public float clampMin = 0f;

        /// <summary>
        /// This is the maximum value of the counter. It cannot go above this value. If both <see
        /// cref="clampMin"/> and <see cref="clampMax"/> are set to zero, no clamping will occur.
        /// </summary>
        [Tooltip("This is the maximum value of the counter. It cannot go above this value. If both Clamp Min and Clamp Max are set to zero, no clamping will occur.")]
        public float clampMax = 0f;

        /// <summary>
        /// Whether the counter value is frozen so that it cannot be changed by inputs.
        /// </summary>
        [Tooltip("Whether the counter value is frozen so that it cannot be changed by inputs.")]
        public bool frozen = false;

        /// <summary>
        /// Gets whether the counter is currently clamping the value because <see cref="clampMin"/>
        /// and <see cref="clampMax"/> or not both set to zero.
        /// </summary>
        public bool isClamping => (clampMin != 0f || clampMax != 0f);

        /// <summary>Used to invoke the ReachedMin output.</summary>
        private bool onReachedMin = false;
        /// <summary>Used to invoke the ReachedMax output.</summary>
        private bool onReachedMax = false;
        /// <summary>Used to invoke the ChangedFromMin output.</summary>
        private bool onChangedFromMin = false;
        /// <summary>Used to invoke the ChangedFromMax output.</summary>
        private bool onChangedFromMax = false;

        /// <summary>
        /// Sets and processes the <see cref="currentValue"/> to ensure it meets all criteria of the
        /// counter such as clamping or freezing the value.
        /// </summary>
        /// <param name="number">The number the counter will maybe be set to.</param>
        public void SetCurrentValue(float number)
        {
            // if the counter value has been frozen we do nothing.
            if (frozen) return;

            // remember the previous value.
            var previousValue = currentValue;

            // set the current value to the number.
            currentValue = number;

            // we now perform clamping on the current value.
            if (isClamping)
            {
                currentValue = Mathf.Clamp(currentValue, clampMin, clampMax);

                // check whether we reached the minimum value.
                onReachedMin = (previousValue != clampMin && currentValue == clampMin);

                // check whether we reached the maximum value.
                onReachedMax = (previousValue != clampMax && currentValue == clampMax);

                // check whether we changed from the minimum value.
                onChangedFromMin = (previousValue == clampMin && currentValue != clampMin);

                // check whether we changed from the maximum value.
                onChangedFromMax = (previousValue == clampMax && currentValue != clampMax);
            }
        }

        public void OnReactiveInput(ReactiveInput input)
        {
            float inputValue = input.parameter.GetFloat();

            switch (input.name)
            {
                case "Add": SetCurrentValue(currentValue + inputValue); break;
                case "AddInvoke": SetCurrentValue(currentValue + inputValue); InvokeOutput(input); break;
                case "Multiply": SetCurrentValue(currentValue * inputValue); break;
                case "MultiplyInvoke": SetCurrentValue(currentValue * inputValue); InvokeOutput(input); break;
                case "Divide": if (inputValue != 0f) SetCurrentValue(currentValue / inputValue); break;
                case "DivideInvoke": if (inputValue != 0f) SetCurrentValue(currentValue / inputValue); InvokeOutput(input); break;
                case "Subtract": SetCurrentValue(currentValue - inputValue); break;
                case "SubtractInvoke": SetCurrentValue(currentValue - inputValue); InvokeOutput(input); break;
                case "SetValue": SetCurrentValue(inputValue); break;
                case "SetValueInvoke": SetCurrentValue(inputValue); InvokeOutput(input); break;

                case "SetMin": clampMin = inputValue; SetCurrentValue(currentValue); break;
                case "SetMinInvoke": clampMin = inputValue; SetCurrentValue(currentValue); InvokeOutput(input); break;
                case "SetMax": clampMax = inputValue; SetCurrentValue(currentValue); break;
                case "SetMaxInvoke": clampMax = inputValue; SetCurrentValue(currentValue); InvokeOutput(input); break;

                case "Freeze": frozen = true; break;
                case "Unfreeze": frozen = false; SetCurrentValue(currentValue); break;
                case "Invoke": SetCurrentValue(currentValue); InvokeOutput(input); break; // force a set for current value unity editor inspector changes.
            }

            // invoke the scheduled outputs detected while setting the current value.
            if (onReachedMin) { onReachedMin = false; this.OnReactiveOutput(input.activator, "ReachedMin", currentValue); }
            if (onReachedMax) { onReachedMax = false; this.OnReactiveOutput(input.activator, "ReachedMax", currentValue); }
            if (onChangedFromMin) { onChangedFromMin = false; this.OnReactiveOutput(input.activator, "ChangedFromMin", currentValue); }
            if (onChangedFromMax) { onChangedFromMax = false; this.OnReactiveOutput(input.activator, "ChangedFromMax", currentValue); }
        }

        private void InvokeOutput(ReactiveInput input)
        {
            this.OnReactiveOutput(input.activator, "Invoked", currentValue);
        }
    }
}