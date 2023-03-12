using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    public static class MetaInterfaceExtensions
    {
        /// <summary>Iterates over all interfaces that were marked as outputs.</summary>
        /// <param name="interfaces">The extension this reference.</param>
        public static IEnumerable<MetaInterface> ForEachOutput(this ICollection<MetaInterface> interfaces)
        {
            foreach (var iface in interfaces)
                if (iface.type == MetaInterfaceType.Output)
                    yield return iface;
        }

        /// <summary>Iterates over all interfaces that were marked as inputs.</summary>
        /// <param name="interfaces">The extension this reference.</param>
        public static IEnumerable<MetaInterface> ForEachInput(this ICollection<MetaInterface> interfaces)
        {
            yield return new MetaInterface(MetaInterfaceType.Input, "Enable", "Enables the reactive logic component allowing it to receive inputs and invoke outputs.");
            yield return new MetaInterface(MetaInterfaceType.Input, "Disable", "Disables the reactive logic component so that it can't receive inputs nor invoke outputs.");
            yield return new MetaInterface(MetaInterfaceType.Input, "Cancel", "Cancels all delayed pending outputs on the reactive logic component.");

            foreach (var iface in interfaces)
                if (iface.type == MetaInterfaceType.Input)
                    yield return iface;
        }

        /// <summary>Gets all outputs as <see cref="GUIContent"/> for inspector dropdowns.</summary>
        /// <param name="interfaces">The extension this reference.</param>
        /// <returns>The list of <see cref="GUIContent"/> outputs.</returns>
        public static void GetOutputsAsGUIContents(this ICollection<MetaInterface> interfaces, List<GUIContent> results)
        {
            foreach (var iface in ForEachOutput(interfaces))
                results.Add(new GUIContent(iface.name, iface.description));
        }
    }
}