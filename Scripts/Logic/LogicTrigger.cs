using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// An <see cref="IReactive"/> that invokes an output whenever colliders enter or exit a trigger.
    /// </summary>
    public class LogicTrigger : MonoBehaviour, IReactive
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
            new MetaInterface(MetaInterfaceType.Output, "Enter", "Invoked when a collider starts touching the trigger."),
            new MetaInterface(MetaInterfaceType.Output, "Exit", "Invoked when a collider stops touching the trigger.")
        );

        public void OnReactiveInput(ReactiveInput input)
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            this.OnReactiveOutput(other.gameObject, "Enter");
        }

        private void OnTriggerExit(Collider other)
        {
            this.OnReactiveOutput(other.gameObject, "Exit");
        }
    }
}