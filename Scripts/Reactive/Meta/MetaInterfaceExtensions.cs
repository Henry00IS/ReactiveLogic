﻿using System.Collections;
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
            foreach (var iface in interfaces)
                if (iface.type == MetaInterfaceType.Input)
                    results.Add(iface);
            return results;
        }

        /// <summary>Gets all outputs as <see cref="GUIContent"/> for inspector dropdowns.</summary>
        /// <param name="interfaces">The extension this reference.</param>
        /// <returns>The list of <see cref="GUIContent"/> outputs.</returns>
        public static List<GUIContent> GetOutputsAsGUIContents(this ICollection<MetaInterface> interfaces)
        {
            var results = new List<GUIContent>(interfaces.Count);
            foreach (var iface in interfaces)
                if (iface.type == MetaInterfaceType.Output)
                    results.Add(new GUIContent(iface.name, iface.description));
            return results;
        }

        /// <summary>Gets all inputs as <see cref="GUIContent"/> for inspector dropdowns.</summary>
        /// <param name="interfaces">The extension this reference.</param>
        /// <returns>The list of <see cref="GUIContent"/> inputs.</returns>
        public static List<GUIContent> GetInputsAsGUIContents(this ICollection<MetaInterface> interfaces)
        {
            var results = new List<GUIContent>(interfaces.Count);
            foreach (var iface in interfaces)
                if (iface.type == MetaInterfaceType.Input)
                    results.Add(new GUIContent(iface.name, iface.description));
            return results;
        }
    }
}