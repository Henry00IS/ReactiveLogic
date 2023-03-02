using System.Collections.Generic;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    public static class MetaInterfaceExtensions
    {
        /// <summary>Gets all interfaces that were marked as outputs.</summary>
        /// <param name="interfaces">The extension this reference.</param>
        /// <returns>The list of interfaces that were marked as outputs.</returns>
        public static List<MetaInterface> GetOutputs(this ICollection<MetaInterface> interfaces)
        {
            var results = new List<MetaInterface>(interfaces.Count);
            foreach (var iface in interfaces)
                if (iface.type == MetaInterfaceType.Output)
                    results.Add(iface);
            return results;
        }

        /// <summary>Gets all interfaces that were marked as inputs.</summary>
        /// <param name="interfaces">The extension this reference.</param>
        /// <returns>The list of interfaces that were marked as inputs.</returns>
        public static List<MetaInterface> GetInputs(this ICollection<MetaInterface> interfaces)
        {
            var results = new List<MetaInterface>(interfaces.Count);
            results.Add(new MetaInterface(MetaInterfaceType.Input, "Enable", "Enables the reactive logic component allowing it to receive inputs and invoke outputs."));
            results.Add(new MetaInterface(MetaInterfaceType.Input, "Disable", "Disables the reactive logic component so that it can't receive inputs nor invoke outputs."));

            foreach (var iface in interfaces)
                if (iface.type == MetaInterfaceType.Input)
                    results.Add(iface);
            return results;
        }

        /// <summary>Gets all outputs as <see cref="GUIContent"/> for inspector dropdowns.</summary>
        /// <param name="interfaces">The extension this reference.</param>
        /// <returns>The list of <see cref="GUIContent"/> outputs.</returns>
        public static void GetOutputsAsGUIContents(this ICollection<MetaInterface> interfaces, List<GUIContent> results)
        {
            foreach (var iface in interfaces)
                if (iface.type == MetaInterfaceType.Output)
                    results.Add(new GUIContent(iface.name, iface.description));
        }

        /// <summary>Gets all inputs as <see cref="GUIContent"/> for inspector dropdowns.</summary>
        /// <param name="interfaces">The extension this reference.</param>
        /// <returns>The list of <see cref="GUIContent"/> inputs.</returns>
        public static void GetInputsAsGUIContents(this ICollection<MetaInterface> interfaces, List<GUIContent> results)
        {
            results.Add(new GUIContent("Enable", "Enables the reactive logic component allowing it to receive inputs and invoke outputs."));
            results.Add(new GUIContent("Disable", "Disables the reactive logic component so that it can't receive inputs nor invoke outputs."));

            foreach (var iface in interfaces)
                if (iface.type == MetaInterfaceType.Input)
                    results.Add(new GUIContent(iface.name, iface.description));
        }
    }
}