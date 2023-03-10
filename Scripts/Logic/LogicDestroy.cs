using UnityEngine;
using UnityEngine.Events;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>An <see cref="IReactive"/> that destroys game objects in the scene.</summary>
    public class LogicDestroy : MonoBehaviour, IReactive
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
            new MetaInterface(MetaInterfaceType.Input, "DestroyActivator", "Destroys the game object that caused the current chain of events."),
            new MetaInterface(MetaInterfaceType.Input, "DestroyTarget", "Searches for targets by the string parameter and destroys the associated game objects.", "target", MetaParameterType.String, "The reactive target to search for.")
        );

        public void OnReactiveInput(ReactiveInput input)
        {
            switch (input.name)
            {
                case "DestroyActivator":
                    if (input.activator.instance)
                        Destroy(input.activator.instance);
                    break;

                case "DestroyTarget":
                    foreach (var target in ReactiveLogicManager.Instance.ForEachReactive(this, input.parameter.GetString()))
                    {
                        var unityObject = target as Object;
                        if (unityObject && target.gameObject)
                            Destroy(target.gameObject);
                    }
                    break;
            }
        }
    }
}