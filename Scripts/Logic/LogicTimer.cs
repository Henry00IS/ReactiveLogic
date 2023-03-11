using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>An <see cref="IReactive"/> that invokes an output at regular intervals.</summary>
    [HelpURL("https://github.com/Henry00IS/ReactiveLogic/wiki/Logic-Timer")]
    public class LogicTimer : MonoBehaviour, IReactive
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
            new MetaInterface(MetaInterfaceType.Input, "Reset", "Resets the timer so that it must wait the full interval before the timer elapses."),
            new MetaInterface(MetaInterfaceType.Input, "Elapse", "Resets the timer and forces the timer to elapse immediately invoking the output."),
            new MetaInterface(MetaInterfaceType.Input, "SetInterval", "Sets the interval of the timer to the float parameter.", "seconds", MetaParameterType.Float, "The timer interval in seconds."),
            new MetaInterface(MetaInterfaceType.Input, "SetTime", "Sets the elapsed time of the timer to the float parameter.", "seconds", MetaParameterType.Float, "The elapsed time in seconds."),
            new MetaInterface(MetaInterfaceType.Input, "AdjustTime", "Adjusts the elapsed time of the timer by the float parameter (positive to add and elapse the timer sooner, negative to subtract and elapse the timer later; but it can't go negative below zero seconds elapsed).", "seconds", MetaParameterType.Float, "The elapsed time in seconds to be added or subtracted."),

            new MetaInterface(MetaInterfaceType.Output, "Elapsed", "Invoked when the interval of the timer elapses.")
        );

        /// <summary>The timer interval in seconds.</summary>
        [Tooltip("The timer interval in seconds.")]
        public float interval = 1f;

        /// <summary>The total time elapsed since the last expiration.</summary>
        private float timeAccumulator = 0f;

        private void FixedUpdate()
        {
            timeAccumulator += Time.fixedDeltaTime;

            // check whether the time has elapsed.
            if (timeAccumulator >= interval)
            {
                timeAccumulator = 0f;
                this.OnReactiveOutput(new ReactiveObject(gameObject), "Elapsed");
            }
        }

        public void OnReactiveInput(ReactiveInput input)
        {
            float inputValue = input.parameter.GetFloat();

            switch (input.name)
            {
                case "Elapse": timeAccumulator = 0f; this.OnReactiveOutput(input, "Elapsed"); break;
                case "Reset": timeAccumulator = 0f; break;
                case "SetInterval": interval = inputValue; break;
                case "SetTime": timeAccumulator = inputValue; if (timeAccumulator < 0f) timeAccumulator = 0f; break;
                case "AdjustTime": timeAccumulator += inputValue; if (timeAccumulator < 0f) timeAccumulator = 0f; break;
            }
        }
    }
}