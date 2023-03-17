using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>An <see cref="IReactive"/> that can control an <see cref="Animator"/> component.</summary>
    [HelpURL("https://github.com/Henry00IS/ReactiveLogic/wiki/Logic-Animator")]
    public class LogicAnimator : MonoBehaviour, IReactive
    {
        #region Required IReactive Implementation

        [SerializeField]
        private ReactiveData _reactiveData;
        public ReactiveData reactiveData => _reactiveData;

        private void OnEnable() => this.OnReactiveEnable();

        private void OnDisable() => this.OnReactiveDisable();

        #endregion Required IReactive Implementation

        /// <summary>
        /// Wrapper around <see cref="AnimatorControllerParameter"/> to store additional data.
        /// </summary>
        private class AnimatorParameter
        {
            private AnimatorEngine engine;

            private AnimatorControllerParameter animatorParameter { get; set; }

            private float _damping;

            /// <summary>Gets the name of the parameter.</summary>
            public string name => animatorParameter.name;

            /// <summary>Gets the index of the parameter.</summary>
            public int index => animatorParameter.nameHash;

            /// <summary>Gets the data type of the parameter.</summary>
            public AnimatorControllerParameterType type => animatorParameter.type;

            /// <summary>Gets or sets the damping applied to this parameter.</summary>
            public float damping
            {
                get => _damping;
                set => _damping = Mathf.Abs(value);
            }

            /// <summary>Used with damping to update this parameter over time.</summary>
            private float floatValue;

            public AnimatorParameter(AnimatorEngine engine, AnimatorControllerParameter animatorParameter)
            {
                this.engine = engine;
                this.animatorParameter = animatorParameter;
                floatValue = animatorParameter.defaultFloat;
            }

            /// <summary>Is called every frame for automation.</summary>
            public void Update()
            {
                // only floats have the damping feature.
                if (type == AnimatorControllerParameterType.Float)
                {
                    if (damping > 0f)
                    {
                        engine.animator.SetFloat(index, floatValue, damping, Time.deltaTime);
                    }

                    // without damping immediately set the value. this will fix the user disabling
                    // damping before it finishes reaching the current value causing an
                    // unpredictable speed.
                    else
                    {
                        engine.animator.SetFloat(index, floatValue);
                    }
                }
            }

            /// <summary>Sets the animator parameter to the reactive logic input parameter.</summary>
            /// <param name="parameter">The input parameter that was invoked on this <see cref="IReactive"/>.</param>
            public void Set(ReactiveParameter parameter)
            {
                switch (animatorParameter.type)
                {
                    case AnimatorControllerParameterType.Float:
                        floatValue = parameter.GetFloat();
                        if (damping == 0f) // without damping immediately set the value.
                            engine.animator.SetFloat(index, floatValue);
                        else
                            engine.animator.SetFloat(index, floatValue, damping, Time.deltaTime);
                        break;

                    case AnimatorControllerParameterType.Int:
                        engine.animator.SetInteger(index, parameter.GetInt());
                        break;

                    case AnimatorControllerParameterType.Bool:
                        engine.animator.SetBool(index, parameter.GetBool());
                        break;

                    case AnimatorControllerParameterType.Trigger:
                        engine.animator.SetTrigger(index);
                        break;
                }
            }
        }

        /// <summary>Represents a layer in an <see cref="Animator"/>.</summary>
        private class AnimatorLayer
        {
            private AnimatorEngine engine;

            /// <summary>Gets the name of the layer.</summary>
            public string name { get; }

            /// <summary>Gets the index of the layer.</summary>
            public int index { get; }

            /// <summary>Gets or sets the weight of the layer.</summary>
            public float weight
            {
                get
                {
                    return engine.animator.GetLayerWeight(index);
                }
                set
                {
                    engine.animator.SetLayerWeight(index, Mathf.Clamp01(value));
                }
            }

            /// <summary>
            /// Sets the offset of the current animation on this layer in normalized time.
            /// </summary>
            /// <param name="offsetNormalized">The normalized time between 0.0 and 1.0.</param>
            public void SetOffset(float offsetNormalized)
            {
                engine.animator.Play(0, index, offsetNormalized);
            }

            /// <summary>Sets the offset of the current animation on this layer in seconds.</summary>
            /// <param name="offsetSeconds">The time in seconds.</param>
            public void SetOffsetSeconds(float offsetSeconds)
            {
                engine.animator.PlayInFixedTime(0, index, Mathf.Abs(offsetSeconds));
            }

            public AnimatorLayer(AnimatorEngine engine, string name, int index)
            {
                this.engine = engine;
                this.name = name;
                this.index = index;
            }

            /// <summary>Is called every frame for automation.</summary>
            public void Update()
            {
            }
        }

        private class AnimatorEngine
        {
            /// <summary>The underlying animator component.</summary>
            public Animator animator { get; private set; }

            /// <summary>The <see cref="AnimatorControllerParameter"/> by name.</summary>
            private Dictionary<string, AnimatorParameter> parametersByName;

            /// <summary>The layers in the animator by name.</summary>
            private Dictionary<string, AnimatorLayer> layersByName;

            /// <summary>
            /// Gets or sets the playback speed of the animator. 1.0 is the normal playback speed.
            /// </summary>
            public float speed
            {
                get => animator.speed;
                set => animator.speed = Mathf.Abs(value);
            }

            public AnimatorEngine(Animator animator)
            {
                this.animator = animator;

                InitializeAnimator();
                InitializeParametersByName();
                InitializeLayersByName();
            }

            /// <summary>Must be called every frame for automation.</summary>
            public void Update()
            {
                foreach (var parameter in ForEachParameter())
                    parameter.Update();

                foreach (var layer in ForEachLayer())
                    layer.Update();
            }

            /// <summary>Iterates over all parameters in the animation controller.</summary>
            public IEnumerable<AnimatorParameter> ForEachParameter()
            {
                foreach (var parameter in parametersByName)
                    yield return parameter.Value;
            }

            /// <summary>Iterates over all layers in the animation controller.</summary>
            public IEnumerable<AnimatorLayer> ForEachLayer()
            {
                foreach (var layer in layersByName)
                    yield return layer.Value;
            }

            /// <summary>Tries to get a layer by name.</summary>
            /// <param name="name">The name of the layer.</param>
            /// <param name="layer">The animator layer if found.</param>
            /// <returns>True when the layer was found else false.</returns>
            public bool TryGetLayer(string name, out AnimatorLayer layer) => layersByName.TryGetValue(name, out layer);

            /// <summary>Tries to get a parameter by name.</summary>
            /// <param name="name">The name of the parameter.</param>
            /// <param name="parameter">The animator parameter if found.</param>
            /// <returns>True when the parameter was found else false.</returns>
            public bool TryGetParameter(string name, out AnimatorParameter parameter) => parametersByName.TryGetValue(name, out parameter);

            /// <summary>
            /// Plays a state by name. The name of the parent layer should be included when
            /// specifying a state name. For example, if you have a 'Bounce' state in the 'Base
            /// Layer', the name would be 'Base Layer.Bounce'.
            /// </summary>
            /// <param name="name">The optional (recommended) layer and required name of the state.</param>
            public void Play(string name)
            {
                // the feature of starting over at 0.0 time without a name is not available here for clarity.
                if (string.IsNullOrEmpty(name))
                    return;

                animator.Play(name);
            }

            /// <summary>Initializes the <see cref="parametersByName"/> collection.</summary>
            private void InitializeParametersByName()
            {
                var animatorParameters = animator.parameters;
                parametersByName = new Dictionary<string, AnimatorParameter>(animatorParameters.Length);

                for (int i = 0; i < animatorParameters.Length; i++)
                {
                    var animatorParameter = animatorParameters[i];
                    parametersByName.Add(animatorParameter.name, new AnimatorParameter(this, animatorParameter));
                }
            }

            /// <summary>Initializes the <see cref="layersByName"/> collection.</summary>
            private void InitializeLayersByName()
            {
                var layerCount = animator.layerCount;
                layersByName = new Dictionary<string, AnimatorLayer>(layerCount);
                for (int i = 0; i < layerCount; i++)
                {
                    var layerName = animator.GetLayerName(i);
                    layersByName.Add(layerName, new AnimatorLayer(this, layerName, i));
                }
            }

            /// <summary>
            /// Fixes the "Animator is not playing an AnimatorController" warning in Unity Editor after
            /// changes are made to the animator controller. This warning causes all animator parameters
            /// to go missing in the C# API, so it's important to call this.
            /// </summary>
            private void InitializeAnimator()
            {
                animator.Rebind();
            }

            /// <summary>Sets the animator parameter (if found) to the reactive logic input parameter.</summary>
            /// <param name="name">The name of the parameter.</param>
            /// <param name="parameter">The parameter to be assigned.</param>
            public void SetParameter(string name, ReactiveParameter parameter)
            {
                if (TryGetParameter(name, out var animatorParameter))
                    animatorParameter.Set(parameter);
            }

            public void SetParameterDamping(string name, float damping)
            {
                if (TryGetParameter(name, out var animatorParameter))
                    animatorParameter.damping = damping;
            }

            /// <summary>Sets the weight of the layer (if found).</summary>
            /// <param name="name">The name of the layer.</param>
            /// <param name="weight">The weight of the layer.</param>
            public void SetLayerWeight(string name, float weight)
            {
                if (TryGetLayer(name, out var layer))
                    layer.weight = weight;
            }

            /// <summary>
            /// Sets the offset of the current animation in normalized time on the specified layer
            /// (if found).
            /// </summary>
            /// <param name="name">The name of the layer.</param>
            /// <param name="offset">The normalized time between 0.0 and 1.0.</param>
            public void SetLayerOffset(string name, float offset)
            {
                if (TryGetLayer(name, out var layer))
                    layer.SetOffset(offset);
            }

            /// <summary>
            /// Sets the offset of the current animation in seconds on the specified layer (if found).
            /// </summary>
            /// <param name="name">The name of the layer.</param>
            /// <param name="seconds">The offset in seconds.</param>
            public void SetLayerOffsetSeconds(string name, float seconds)
            {
                if (TryGetLayer(name, out var layer))
                    layer.SetOffsetSeconds(seconds);
            }
        }

        // as the outputs change depending on the animator, we create the metadata every time it's requested.
        public ReactiveMetadata reactiveMetadata
        {
            get
            {
                var interfaces = new List<MetaInterface>();
                interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "Play", "Plays a state whose name is passed by the string parameter. The name of the parent layer should be included when specifying a state name. For example, if you have a 'Bounce' state in the 'Base Layer', the name would be 'Base Layer.Bounce'.", "state", MetaParameterType.String, "The optional (recommended) layer and required name of the state."));
                interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "SetSpeed", "Sets the playback speed of the animator to the float parameter.", "speed", MetaParameterType.Float, "The playback speed of the animator; where 1.0 is normal playback."));

                var engine = animatorEngine;
                if (engine != null)
                {
                    foreach (var parameter in engine.ForEachParameter())
                    {
                        interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "SetParam:" + parameter.name, "Sets the animator parameter to the given parameter.", "parameter", MetaParameterType.String, "The parameter that was passed to the input."));
                        // only floats have the damping feature:
                        if (parameter.type == AnimatorControllerParameterType.Float)
                            interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "SetParamDamping:" + parameter.name, "Sets the damping applied to animator parameter changes.", "damping", MetaParameterType.Float, "The damping to be used on changes to this parameter, this roughly corresponds to the number of seconds."));
                    }

                    foreach (var layer in engine.ForEachLayer())
                    {
                        interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "SetLayerWeight:" + layer.name, "Sets the animator layer weight to the given parameter.", "weight", MetaParameterType.Float, "The weight of the layer between 0.0 and 1.0."));
                        interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "SetLayerOffset:" + layer.name, "Sets the normalized playback offset of the animator layer to the float parameter between zero and one.", "offset", MetaParameterType.Float, "The playback offset between 0.0 and 1.0."));
                        interfaces.Add(new MetaInterface(MetaInterfaceType.Input, "SetLayerOffsetSeconds:" + layer.name, "Sets the playback offset of the animator layer to the float parameter in seconds.", "seconds", MetaParameterType.Float, "The playback offset in exact seconds."));
                    }
                }

                return new ReactiveMetadata(interfaces.ToArray());
            }
        }

        /// <summary>The Unity animation system component.</summary>
        [Tooltip("The Unity animation system component.")]
        public Animator animator;

        /// <summary>The animator engine that helps to automate <see cref="Animator"/>.</summary>
        private AnimatorEngine _animatorEngine;

        /// <summary>
        /// Gets the animator engine that helps to automate <see cref="Animator"/> and creates or
        /// updates it as needed. If there is no valid <see cref="animator"/> then this returns null.
        /// </summary>
        private AnimatorEngine animatorEngine
        {
            get
            {
                // if there is no valid animator:
                if (!animator)
                {
                    _animatorEngine = null;
                    return _animatorEngine;
                }

                // at this point the animator is valid:

                // if we have not been initialized:
                if (_animatorEngine == null)
                    _animatorEngine = new AnimatorEngine(animator);

                // if the animator changed to another valid animator:
                else if (_animatorEngine.animator != animator)
                    _animatorEngine = new AnimatorEngine(animator);

                // the animator is valid and up to date.
                return _animatorEngine;
            }
        }

        public void OnReactiveInput(ReactiveInput input)
        {
            var engine = animatorEngine;
            if (engine == null)
                return; // nothing to do.

            if (input.name.StartsWith("SetParam:"))
            {
                engine.SetParameter(input.name.Substring("SetParam:".Length), input.parameter);
                return;
            }

            if (input.name.StartsWith("SetParamDamping:"))
            {
                engine.SetParameterDamping(input.name.Substring("SetParamDamping:".Length), input.parameter.GetFloat());
                return;
            }

            if (input.name.StartsWith("SetLayerWeight:"))
            {
                engine.SetLayerWeight(input.name.Substring("SetLayerWeight:".Length), input.parameter.GetFloat());
                return;
            }

            if (input.name.StartsWith("SetLayerOffset:"))
            {
                engine.SetLayerOffset(input.name.Substring("SetLayerOffset:".Length), input.parameter.GetFloat());
                return;
            }

            if (input.name.StartsWith("SetLayerOffsetSeconds:"))
            {
                engine.SetLayerOffsetSeconds(input.name.Substring("SetLayerOffsetSeconds:".Length), input.parameter.GetFloat());
                return;
            }

            switch (input.name)
            {
                case "Play": engine.Play(input.parameter.GetString()); break;
                case "SetSpeed": engine.speed = input.parameter.GetFloat(); break;
            }
        }

        private void Update()
        {
            animatorEngine?.Update();
        }
    }
}