namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Represents metadata for an <see cref="IReactive"/>. This contains a list of inputs, outputs,
    /// parameters and descriptions among other things. This is primarily used to support the game
    /// developer within Unity Editor and is not a requirement for function.
    /// </summary>
    public class ReactiveMetadata
    {
        /// <summary>The collection of inputs and outputs.</summary>
        public readonly MetaInterface[] interfaces;

        /// <summary>Creates new metadata that describes all interfaces of an <see cref="IReactive"/>.</summary>
        /// <param name="interfaces">The input and output interfaces of the <see cref="IReactive"/>.</param>
        public ReactiveMetadata(params MetaInterface[] interfaces)
        {
            this.interfaces = interfaces;
        }
    }
}