using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>An <see cref="IReactive"/> that can enable or disable collision between two colliders.</summary>
    [HelpURL("https://github.com/Henry00IS/ReactiveLogic/wiki/Logic-Collision-Pair")]
    public class LogicCollisionPair : MonoBehaviour, IReactive
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
            new MetaInterface(MetaInterfaceType.Input, "EnableCollision", "Enables collision between the configured colliders."),
            new MetaInterface(MetaInterfaceType.Input, "DisableCollision", "Disables collision between the configured colliders.")
        );

        /// <summary>The first collider.</summary>
        [Tooltip("The first collider.")]
        public Collider collider1;

        /// <summary>The second collider.</summary>
        [Tooltip("The second collider.")]
        public Collider collider2;

        /// <summary>
        /// Whether the two colliders should be prevented from colliding upon starting the scene.
        /// </summary>
        [Tooltip("Whether the two colliders should be prevented from colliding upon starting the scene.")]
        public bool startCollisionDisabled = false;

        /// <summary>Start is called before the first frame update.</summary>
        private void Start()
        {
            if (!collider1 || !collider2)
                return; // nothing to do.

            if (startCollisionDisabled)
            {
                Physics.IgnoreCollision(collider1, collider2, true);
            }
        }

        public void OnReactiveInput(ReactiveInput input)
        {
            if (!collider1 || !collider2)
                return; // nothing to do.

            switch (input.name)
            {
                case "EnableCollision": Physics.IgnoreCollision(collider1, collider2, false); break;
                case "DisableCollision": Physics.IgnoreCollision(collider1, collider2, true); break;
            }
        }
    }
}