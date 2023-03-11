using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>An <see cref="IReactive"/> that can instantiate a game object.</summary>
    [HelpURL("https://github.com/Henry00IS/ReactiveLogic/wiki/Logic-Instantiate")]
    public class LogicInstantiate : MonoBehaviour, IReactive
    {
        // Tell me, game developer, if you can: you have instantiated so much. What is it, exactly,
        // that you have destroyed? Can you name even one thing? I thought not.

        #region Required IReactive Implementation

        [SerializeField]
        private ReactiveData _reactiveData;
        public ReactiveData reactiveData => _reactiveData;
        public ReactiveMetadata reactiveMetadata => _reactiveMeta;

        private void OnEnable() => this.OnReactiveEnable();

        private void OnDisable() => this.OnReactiveDisable();

        #endregion Required IReactive Implementation

        private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata(
            new MetaInterface(MetaInterfaceType.Input, "Instantiate", "Instantiates a new instance of the template game object."),
            new MetaInterface(MetaInterfaceType.Output, "Instantiated", "Invoked when a game object has been instantiated. This is a new chain of events with the instantiated game object as the activator.", "parameter", MetaParameterType.String, "The parameter that was passed to the input.")
        );

        /// <summary>The template <see cref="GameObject"/> or prefab to be instantiated.</summary>
        [Tooltip("The template game object or prefab to be instantiated.")]
        public GameObject template;

        public void OnReactiveInput(ReactiveInput input)
        {
            switch (input.name)
            {
                case "Instantiate":
                    if (template)
                    {
                        var go = Instantiate(template, transform.position, transform.rotation);
                        go.name = template.name; // prevent ...(Clone) names to not break LogicFilter.
                        this.OnReactiveOutput(new ReactiveObject(go), "Instantiated", input.parameter);
                    }
                    break;
            }
        }
    }
}