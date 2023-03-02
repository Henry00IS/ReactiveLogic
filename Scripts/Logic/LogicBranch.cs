using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>An <see cref="IReactive"/> that can branch between two outputs.</summary>
    public class LogicBranch : MonoBehaviour, IReactive
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
            new MetaInterface(MetaInterfaceType.Input, "SetValue", "Set the boolean value without performing the test.", "value", MetaParameterType.Boolean, "The boolean value to be stored for future tests."),
            new MetaInterface(MetaInterfaceType.Input, "SetValueTest", "Set the boolean value and perform the test. This invokes either the True or False output accordingly.", "value", MetaParameterType.Boolean, "The boolean value to be stored and tested."),
            new MetaInterface(MetaInterfaceType.Input, "Toggle", "Toggle the boolean value between true and false without performing the test."),
            new MetaInterface(MetaInterfaceType.Input, "ToggleTest", "Toggle the boolean value between true and false and perform the test. This invokes either the True or False output accordingly."),
            new MetaInterface(MetaInterfaceType.Input, "Test", "Performs the test invoking either the True or False output accordingly."),

            new MetaInterface(MetaInterfaceType.Output, "True", "Invoked by the test when the boolean value is true.", "parameter", MetaParameterType.String, "The parameter to be passed to the input."),
            new MetaInterface(MetaInterfaceType.Output, "False", "Invoked by the test when the boolean value is false.", "parameter", MetaParameterType.String, "The parameter to be passed to the input.")
        );

        /// <summary>The initial boolean value stored inside of the <see cref="LogicBranch"/>.</summary>
        [Tooltip("The initial boolean value stored inside of the logic branch.")]
        public bool initialValue = false;

        /// <summary>The current internal boolean value.</summary>
        private bool currentValue;

        private void Awake()
        {
            // copy the initial value when this logic branch gets created.
            currentValue = initialValue;
        }

        public void OnReactiveInput(ReactiveInput input)
        {
            switch (input.name)
            {
                // set the boolean value without performing the test.
                case "SetValue":
                    currentValue = input.parameter.GetBool();
                    break;

                // set the boolean value and perform the test.
                case "SetValueTest":
                    currentValue = input.parameter.GetBool();
                    DoTest(input);
                    break;

                // toggle the boolean value between true and false without performing the test.
                case "Toggle":
                    currentValue = !currentValue;
                    break;

                // toggle the boolean value between true and false and perform the test.
                case "ToggleTest":
                    currentValue = !currentValue;
                    DoTest(input);
                    break;

                // only perform the test.
                case "Test":
                    DoTest(input);
                    break;
            }
        }

        /// <summary>Performs the test invoking either the True or False output accordingly.</summary>
        public void DoTest(ReactiveInput input)
        {
            if (currentValue)
            {
                this.OnReactiveOutput(input.activator, "True", input.parameter);
            }
            else
            {
                this.OnReactiveOutput(input.activator, "False", input.parameter);
            }
        }
    }
}