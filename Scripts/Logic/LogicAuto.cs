using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>An <see cref="IReactive"/> that can invoke an input on scene start.</summary>
    public class LogicAuto : MonoBehaviour, IReactive
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
            new MetaInterface(MetaInterfaceType.Output, "Start", "Automatically called before the first frame update.")
        );

        /// <summary>Start is called before the first frame update.</summary>
        private void Start()
        {
            // logic auto begins a new chain of events and is the activator.
            this.OnReactiveOutput(gameObject, "Start");
        }

        public void OnReactiveInput(ReactiveInput input)
        {
        }
    }
}