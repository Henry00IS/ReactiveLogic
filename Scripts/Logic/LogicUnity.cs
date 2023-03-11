using UnityEngine;
using UnityEngine.Events;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// An <see cref="IReactive"/> that executes a <see cref="UnityEvent"/> when invoked. This makes
    /// it easy to communicate with a component that is not reactive, but it encourages visual
    /// programming, which is a bad idea. You essentially have code in the scene that tinkers around
    /// in C# and can cause serious bugs that are hard to trace and fix months from now. Use this
    /// component sparingly!
    /// </summary>
    [HelpURL("https://github.com/Henry00IS/ReactiveLogic/wiki/Logic-Unity")]
    public class LogicUnity : MonoBehaviour, IReactive
    {
        public enum ParameterMode
        {
            None,
            Boolean,
            Integer,
            Float,
            String,
            Activator,
        }

        #region Required IReactive Implementation

        [SerializeField]
        private ReactiveData _reactiveData;
        public ReactiveData reactiveData => _reactiveData;
        public ReactiveMetadata reactiveMetadata => _reactiveMeta;

        private void OnEnable() => this.OnReactiveEnable();

        private void OnDisable() => this.OnReactiveDisable();

        #endregion Required IReactive Implementation

        private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata(
            new MetaInterface(MetaInterfaceType.Input, "Invoke", "Invokes the Unity event.")
        );

        /// <summary>The parameter type expected to pass to the Unity event.</summary>
        [Header("This component is dangerous, use sparingly!")]
        [Space(15f, order = 1)]
        [Tooltip("The parameter type expected to pass to the Unity event.")]
        public ParameterMode parameterMode = ParameterMode.None;

        [Space]
        public UnityEvent unityEvent;

        [Space]
        public UnityEvent<bool> unityEventBoolean;

        [Space]
        public UnityEvent<int> unityEventInteger;

        [Space]
        public UnityEvent<float> unityEventFloat;

        [Space]
        public UnityEvent<string> unityEventString;

        [Space]
        public UnityEvent<GameObject> unityEventActivator;

        public void OnReactiveInput(ReactiveInput input)
        {
            if (input.name == "Invoke")
            {
                switch (parameterMode)
                {
                    case ParameterMode.None:
                        unityEvent?.Invoke();
                        break;

                    case ParameterMode.Boolean:
                        unityEventBoolean?.Invoke(input.parameter.GetBool());
                        break;

                    case ParameterMode.Integer:
                        unityEventInteger?.Invoke(input.parameter.GetInt());
                        break;

                    case ParameterMode.Float:
                        unityEventFloat?.Invoke(input.parameter.GetFloat());
                        break;

                    case ParameterMode.String:
                        unityEventString?.Invoke(input.parameter.GetString());
                        break;

                    case ParameterMode.Activator:
                        unityEventActivator?.Invoke(input.activator.instance);
                        break;
                }
            }
        }
    }
}