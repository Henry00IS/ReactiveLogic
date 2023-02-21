namespace AlpacaIT.ReactiveLogic
{
    /// <summary>Represents an output from a reactive logic component.</summary>
    [System.Serializable]
    public class ReactiveOutput
    {
        /// <summary>The name of the output that will cause this action.</summary>
        public string name;

        /// <summary>The reactive logic component that will receive this input.</summary>
        public string target;

        /// <summary>The name of the input that will be triggered.</summary>
        public string input;

        /// <summary>The delay in seconds before the input gets triggered.</summary>
        public float delay;

        /// <summary>The parameter that will be passed from this output to this input.</summary>
        public string parameter;
    }
}