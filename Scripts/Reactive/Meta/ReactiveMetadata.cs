namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Represents metadata for a reactive logic component. This contains a list of inputs, outputs,
    /// parameters and descriptions among other things. This is primarily used to support the game
    /// developer within Unity Editor and is not a requirement for function.
    /// </summary>
    public class ReactiveMetadata
    {
        /// <summary>The collection of inputs and outputs.</summary>
        public readonly MetaInterface[] interfaces;

        /// <summary>Creates new metadata that describes all interfaces of a reactive logic component.</summary>
        /// <param name="interfaces">The input and output interfaces of the reactive logic component.</param>
        public ReactiveMetadata(params MetaInterface[] interfaces)
        {
            this.interfaces = interfaces;
        }
    }
}