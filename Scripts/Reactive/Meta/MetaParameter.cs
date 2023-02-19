using System;
using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>Represents metadata for a reactive logic component parameter.</summary>
    public class MetaParameter
    {
        /// <summary>The name of the parameter.</summary>
        public readonly string name;

        /// <summary>The parameter description with notes and tips.</summary>
        public readonly string description;

        /// <summary>The value type expected by the parameter.</summary>
        public readonly MetaParameterType type;

        /// <summary>Creates a new meta parameter description.</summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The value type expected by the parameter.</param>
        /// <param name="description">The parameter description with notes and tips.</param>
        public MetaParameter(string name, MetaParameterType type, string description = "")
        {
            this.name = name;
            this.type = type;
            this.description = description;
        }
    }
}