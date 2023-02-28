using System.Collections.Generic;
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
        private ReactiveEditor _reactiveEditor;

        [SerializeField]
        [HideInInspector]
        private List<ReactiveOutput> _reactiveOutputs = new List<ReactiveOutput>();
        public List<ReactiveOutput> reactiveOutputs => _reactiveOutputs;

        public ReactiveMetadata reactiveMetadata => _reactiveMeta;

        #endregion Required IReactive Implementation

        private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata();

        public void OnReactiveInput(ReactiveInput input)
        {
        }
    }
}