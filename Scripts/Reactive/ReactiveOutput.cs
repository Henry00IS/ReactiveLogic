namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Represents a user-configured output handler of an <see cref="IReactive"/>. These outputs are
    /// usually configured in Unity Editor by the level designer.
    /// </summary>
    [System.Serializable]
    public class ReactiveOutput
    {
        /// <summary>
        /// The output name of the <see cref="IReactive"/> that this output handler targets.
        /// </summary>
        public string name;

        /// <summary>
        /// The target name that yields one or more <see cref="IReactive"/> that this handler
        /// invokes an input on.
        /// </summary>
        public string targetName;

        /// <summary>The target input name that will be invoked.</summary>
        public string targetInput;

        /// <summary>The parameter that will be passed to the input of the target(s).</summary>
        public string targetInputParameter;

        /// <summary>The delay in seconds to wait before processing this output handler.</summary>
        public float delay;
    }
}