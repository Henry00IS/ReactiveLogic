namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Represents the different parameter types that can be passed between <see cref="IReactive"/>
    /// inputs and outputs.
    /// </summary>
    public enum MetaParameterType
    {
        /// <summary>The parameter takes or outputs a boolean value.</summary>
        Boolean,
        /// <summary>The parameter takes or outputs a string value.</summary>
        String,
        /// <summary>The parameter takes or outputs an integer value.</summary>
        Integer,
        /// <summary>The parameter takes or outputs a float value.</summary>
        Float,
    }
}