using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>An <see cref="IReactive"/> that can log messages to the Unity console.</summary>
    public class LogicLog : MonoBehaviour, IReactive
    {
        #region Required IReactive Implementation

        [SerializeField]
        private ReactiveEditor _reactiveEditor;

        [SerializeField]
        [HideInInspector]
        private List<ReactiveOutput> _reactiveOutputs = new List<ReactiveOutput>();
        public List<ReactiveOutput> reactiveOutputs => _reactiveOutputs;

        public ReactiveMetadata reactiveMetadata => _reactiveMeta;

        private void OnEnable()
        {
            this.OnReactiveEnable();
        }

        private void OnDisable()
        {
            this.OnReactiveDisable();
        }

        #endregion Required IReactive Implementation

        private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata(
            new MetaInterface(MetaInterfaceType.Input, "Message", "Logs a message to the Unity Console.", "message", MetaParameterType.String, "The debug message to be logged in the console."),
            new MetaInterface(MetaInterfaceType.Input, "Warning", "Logs a warning to the Unity Console.", "message", MetaParameterType.String, "The warning message to be logged in the console."),
            new MetaInterface(MetaInterfaceType.Input, "Error", "Logs an error to the Unity Console.", "message", MetaParameterType.String, "The error message to be logged in the console.")
        );

        /// <summary>
        /// The message to be logged to the Unity Console. This string is used when no message
        /// parameter was passed.
        /// </summary>
        [Tooltip("The message to be logged to the Unity Console. This string is used when no message parameter was passed.")]
        public string message;

        public void OnReactiveInput(ReactiveInput input)
        {
            switch (input.name)
            {
                case "Message":
                    Debug.Log(input.parameter.GetString(message));
                    break;

                case "Warning":
                    Debug.LogWarning(input.parameter.GetString(message));
                    break;

                case "Error":
                    Debug.LogError(input.parameter.GetString(message));
                    break;
            }
        }
    }
}