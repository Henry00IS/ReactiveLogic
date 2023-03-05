using System.Collections.Generic;
using AlpacaIT.ReactiveLogic.Internal;

namespace AlpacaIT.ReactiveLogic
{
    // this partial class handles everything related to unity editor integration.
    public partial class ReactiveLogicManager
    {
        /// <summary>
        /// Finds all of the reactive components in the scene and stores them in <see cref="reactives"/>.
        /// <para>This should only be called by editor code.</para>
        /// </summary>
        internal void EditorUpdateReactives()
        {
            reactives = new LinkedList<IReactive>(Utilities.FindObjectsOfTypeImplementing<IReactive>());

            // also refresh the data for all of them as they may have moved in the hierarchy.
            foreach (var reactive in reactives)
                RefreshReactiveData(reactive);
        }
    }
}