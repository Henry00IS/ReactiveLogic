using UnityEngine;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>Represents metadata for an <see cref="IReactive"/> input or output.</summary>
    public class MetaInterface
    {
        /// <summary>The interface type which can either be an input or an output.</summary>
        public readonly MetaInterfaceType type;

        /// <summary>The name of the input or output.</summary>
        public readonly string name;

        /// <summary>The description of the input or output with notes and tips.</summary>
        public readonly string description = "";

        /// <summary>The parameter that the input or output expects (or null).</summary>
        public readonly MetaParameter parameter = null;

        /// <summary>Describes an input or output that does not take a parameter.</summary>
        /// <param name="type">Whether this interface is an input or an output.</param>
        /// <param name="name">The name of the input or output.</param>
        public MetaInterface(MetaInterfaceType type, string name)
        {
            this.type = type;
            this.name = name;
        }

        /// <summary>Describes an input or output that does not take a parameter.</summary>
        /// <param name="type">Whether this interface is an input or an output.</param>
        /// <param name="name">The name of the input or output.</param>
        /// <param name="description">The description of the input or output with notes and tips.</param>
        public MetaInterface(MetaInterfaceType type, string name, string description)
        {
            this.type = type;
            this.name = name;
            this.description = description;
        }

        /// <summary>Describes an input or output that takes a parameter.</summary>
        /// <param name="type">Whether this interface is an input or an output.</param>
        /// <param name="name">The name of the input or output.</param>
        /// <param name="parameter">The parameter that the input or output expects.</param>
        public MetaInterface(MetaInterfaceType type, string name, MetaParameter parameter)
        {
            this.type = type;
            this.name = name;
            this.parameter = parameter;
        }

        /// <summary>Describes an input or output that takes a parameter.</summary>
        /// <param name="type">Whether this interface is an input or an output.</param>
        /// <param name="name">The name of the input or output.</param>
        /// <param name="description">The description of the input or output with notes and tips.</param>
        /// <param name="parameter">The parameter that the input or output expects.</param>
        public MetaInterface(MetaInterfaceType type, string name, string description, MetaParameter parameter)
        {
            this.type = type;
            this.name = name;
            this.description = description;
            this.parameter = parameter;
        }

        /// <summary>Describes an input or output that takes a parameter.</summary>
        /// <param name="type">Whether this interface is an input or an output.</param>
        /// <param name="name">The name of the input or output.</param>
        /// <param name="parameterName">The name of the parameter that the input or output expects.</param>
        /// <param name="parameterType">The type of the parameter that the input or output expects.</param>
        public MetaInterface(MetaInterfaceType type, string name, string parameterName, MetaParameterType parameterType)
        {
            this.type = type;
            this.name = name;
            this.parameter = new MetaParameter(parameterName, parameterType);
        }

        /// <summary>Describes an input or output that takes a parameter.</summary>
        /// <param name="type">Whether this interface is an input or an output.</param>
        /// <param name="name">The name of the input or output.</param>
        /// <param name="description">The description of the input or output with notes and tips.</param>
        /// <param name="parameterName">The name of the parameter that the input or output expects.</param>
        /// <param name="parameterType">The type of the parameter that the input or output expects.</param>
        public MetaInterface(MetaInterfaceType type, string name, string description, string parameterName, MetaParameterType parameterType)
        {
            this.type = type;
            this.name = name;
            this.description = description;
            this.parameter = new MetaParameter(parameterName, parameterType);
        }

        /// <summary>Describes an input or output that takes a parameter.</summary>
        /// <param name="type">Whether this interface is an input or an output.</param>
        /// <param name="name">The name of the input or output.</param>
        /// <param name="description">The description of the input or output with notes and tips.</param>
        /// <param name="parameterName">The name of the parameter that the input or output expects.</param>
        /// <param name="parameterType">The type of the parameter that the input or output expects.</param>
        /// <param name="parameterDescription">The description of the parameter that the input or output expects.</param>
        public MetaInterface(MetaInterfaceType type, string name, string description, string parameterName, MetaParameterType parameterType, string parameterDescription)
        {
            this.type = type;
            this.name = name;
            this.description = description;
            this.parameter = new MetaParameter(parameterName, parameterType, parameterDescription);
        }
    }
}