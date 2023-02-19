using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Represents a reactive logic component that provides the ability to log messages to the Unity console.
    /// </summary>
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

        #endregion Required IReactive Implementation

        private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata(
            new MetaInterface(MetaInterfaceType.Input, "Message", "Logs a message to the Unity Console.", "message", MetaParameterType.String, "The debug message to be logged in the console."),
            new MetaInterface(MetaInterfaceType.Input, "Warning", "Logs a warning to the Unity Console.", "message", MetaParameterType.String, "The warning message to be logged in the console."),
            new MetaInterface(MetaInterfaceType.Input, "Error", "Logs an error to the Unity Console.", "message", MetaParameterType.String, "The error message to be logged in the console.")
        );

        public void OnReactiveInput(ReactiveInput input)
        {
            switch (input.name)
            {
                case "Message":
                    Debug.Log(input.parameter.GetString());
                    break;

                case "Warning":
                    Debug.LogWarning(input.parameter.GetString());
                    break;

                case "Error":
                    Debug.LogError(input.parameter.GetString());
                    break;
            }
        }
    }
}