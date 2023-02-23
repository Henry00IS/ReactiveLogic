using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    public class LogicRelay : MonoBehaviour, IReactive
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
            new MetaInterface(MetaInterfaceType.Input, "Trigger", "Triggers the relay. This will call the Trigger output."),
            new MetaInterface(MetaInterfaceType.Output, "Trigger", "Called when the relay is triggered.")
        );

        public void OnReactiveInput(ReactiveInput input)
        {
            if (input.name == "Trigger")
            {
                this.OnReactiveOutput(input.activator, "Trigger");
            }
        }
    }
}