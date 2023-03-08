using System.Collections.Generic;
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
            new MetaInterface(MetaInterfaceType.Output, "Exit", "Invoked when a collider stops touching the trigger."),
            new MetaInterface(MetaInterfaceType.Output, "Occupied", "Invoked when the trigger becomes occupied by colliders touching the trigger."),
            new MetaInterface(MetaInterfaceType.Output, "Empty", "Invoked when all colliders stop touching the trigger.")
        );

        /// <summary>The collection of colliders that are occupying the trigger.</summary>
        private List<(GameObject gameObject, Collider collider)> occupants = new List<(GameObject gameObject, Collider collider)>();

        public void OnReactiveInput(ReactiveInput input)
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            // keep track of the occupants inside of the trigger.
            occupants.Add((other.gameObject, other));

            this.OnReactiveOutput(new ReactiveObject(other.gameObject), "Enter");

            if (occupants.Count == 1)
                this.OnReactiveOutput(new ReactiveObject(other.gameObject), "Occupied");
        }

        private void OnTriggerExit(Collider other)
        {
            // keep track of the occupants inside of the trigger.
            occupants.Remove((other.gameObject, other));

            this.OnReactiveOutput(new ReactiveObject(other.gameObject), "Exit");

            if (occupants.Count == 0)
                this.OnReactiveOutput(new ReactiveObject(other.gameObject), "Empty");
        }

        private void FixedUpdate()
        {
            // when an occupant gets deleted then OnTriggerExit is never called.
            var occupantsCount = occupants.Count;
            for (int i = occupantsCount; i-- > 0;)
            {
                var occupant = occupants[i];
                if (!occupant.collider)
                {
                    // remove the occupant from the collection.
                    occupants.RemoveAt(i);

                    // yes this game object is probably destroyed but other logic can check for that.
                    this.OnReactiveOutput(new ReactiveObject(occupant.gameObject), "Exit");

                    if (occupants.Count == 0)
                        this.OnReactiveOutput(new ReactiveObject(occupant.gameObject), "Empty");
                }
            }
        }
    }
}