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
        private ReactiveData _reactiveData;
        public ReactiveData reactiveData => _reactiveData;
        public ReactiveMetadata reactiveMetadata => _reactiveMeta;

        private void OnEnable() => this.OnReactiveEnable();

        private void OnDisable() => this.OnReactiveDisable();

        #endregion Required IReactive Implementation

        private static ReactiveMetadata _reactiveMeta = new ReactiveMetadata();

        public void OnReactiveInput(ReactiveInput input)
        {
        }
    }
}