#if ALPACAIT_DYNAMICLIGHTING

using AlpacaIT.DynamicLighting;
using AlpacaIT.ReactiveLogic.Internal;
using System.Collections;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic.External.DynamicLighting
{
    // yes it would be possible to make dynamic lights themselves reactive, but it was difficult to
    // know what the light source was doing. changing the intensity while fading because of one of
    // these functions- but also having c# set the intensity caused conflicts (and which intensity
    // is the one we will return to upon enabling the light? do we have to hack our fading into the
    // light source intensity somehow?). the dynamic light source has a clear c# api of simple
    // fields and this additional state made things too messy. the unity serializer is also eager to
    // delete data on fields that go missing (removing the reactive logic package for just a short
    // moment). having a secondary component that controls the light felt more natural and makes it
    // clear why your own c# code may not work as expected "ah, this component is also changing values".

    /// <summary>An <see cref="IReactive"/> that provides the ability to control a <see cref="DynamicLight"/>.</summary>
    [RequireComponent(typeof(DynamicLight))]
    public class ReactiveDynamicLight : MonoBehaviour, IReactive
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
            new MetaInterface(MetaInterfaceType.Input, "EnableLight", "Enables the dynamic light source."),
            new MetaInterface(MetaInterfaceType.Input, "DisableLight", "Disables the dynamic light source."),
            new MetaInterface(MetaInterfaceType.Input, "ToggleLight", "Toggles the dynamic light source.")
        );

        /// <summary>
        /// Whether the light is enabled (full intensity) or disabled (zero intensity) upon scene start.
        /// </summary>
        public bool lightStartEnabled = true;

        /// <summary>The amount of time in takes in seconds for the light to fade in fully.</summary>
        [Min(0f)]
        public float lightFadeInTime = 0.1f;

        /// <summary>The amount of time in takes in seconds for the light to fade out fully.</summary>
        [Min(0f)]
        public float lightFadeOutTime = 0.1f;

        /// <summary>The <see cref="DynamicLight"/> source that this component is controlling.</summary>
        private DynamicLight dynamicLight;

        /// <summary>The light intensity as it was upon scene start.</summary>
        private float initialLightIntensity;

        private Utilities.LinearMotion fadeLinear;

        private bool reactiveCoroutineRunning = false;

        private void Awake()
        {
            fadeLinear = new Utilities.LinearMotion();
            dynamicLight = GetComponent<DynamicLight>();
            initialLightIntensity = dynamicLight.lightIntensity;

            // disable the light on scene start if so desired.
            if (!lightStartEnabled)
            {
                dynamicLight.lightIntensity = 0.0f;
            }
            else
            {
                fadeLinear.OpenImmediately();
            }
        }

        public void OnReactiveInput(ReactiveInput input)
        {
            switch (input.name)
            {
                case "EnableLight":
                    fadeLinear.Open(lightFadeInTime);
                    TryStartReactiveCoroutine();
                    break;

                case "DisableLight":
                    fadeLinear.Close(lightFadeOutTime);
                    TryStartReactiveCoroutine();
                    break;

                case "ToggleLight":
                    fadeLinear.Reverse(lightFadeInTime, lightFadeOutTime);
                    TryStartReactiveCoroutine();
                    break;
            }
        }

        // we use a coroutine instead of update to prevent unity calling update with nothing to do.

        private void TryStartReactiveCoroutine()
        {
            if (!reactiveCoroutineRunning)
                StartCoroutine(ReactiveCoroutine());
        }

        private IEnumerator ReactiveCoroutine()
        {
            reactiveCoroutineRunning = true;

            while (fadeLinear.moving)
            {
                dynamicLight.lightIntensity = fadeLinear.Update() * initialLightIntensity;
                yield return null;
            }

            reactiveCoroutineRunning = false;
        }
    }
}

#endif