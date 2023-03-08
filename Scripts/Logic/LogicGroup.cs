using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// An <see cref="IReactive"/> that groups together multiple <see cref="IReactive"/>. The
    /// grouped <see cref="IReactive"/> target only each other and not global names outside of the group.
    /// </summary>
    public class LogicGroup : MonoBehaviour, IReactive
    {
        #region Required IReactive Implementation

        [SerializeField]
        private ReactiveData _reactiveData;
        public ReactiveData reactiveData => _reactiveData;
        public ReactiveMetadata reactiveMetadata => _reactiveMeta;

        private void OnEnable() => this.OnReactiveEnable();

        private void OnDisable() => this.OnReactiveDisable();

        #endregion Required IReactive Implementation

        private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata();

        /// <summary>Usage instructions for the group written by the level designer.</summary>
        [Multiline(20)]
        [Tooltip("Usage instructions for the group written by the level designer.")]
        public string groupDescription = "";

        public void OnReactiveInput(ReactiveInput input)
        {
            // "Group"-inputs are treated as outputs without the "Group" prefix.
            if (input.name.StartsWith("Group"))
            {
                this.OnReactiveOutput(input, input.name.Substring(5), input.parameter);
            }
            // every input is treated as a broadcast with an added "Group" prefix.
            else
            {
                var manager = ReactiveLogicManager.Instance;
                foreach (var reactive in manager.ForEachReactiveInGroup(this))
                {
                    if (reactive is LogicGroup) continue; // ignore groups in groups.
                    reactive.OnReactiveOutput(input, "Group" + input.name, input.parameter);
                }
            }
        }
    }
}