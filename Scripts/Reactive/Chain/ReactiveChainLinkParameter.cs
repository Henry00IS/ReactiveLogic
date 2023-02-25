using System.Globalization;

namespace AlpacaIT.ReactiveLogic
{
    /// <summary>
    /// Represents a parameter that is passed from <see cref="IReactive"/> outputs to inputs.
    /// </summary>
    public class ReactiveChainLinkParameter
    {
        /// <summary>The internal parameter value.</summary>
        private readonly string parameter = "";

        /// <summary>Creates a new empty parameter.</summary>
        public ReactiveChainLinkParameter()
        {
        }

        /// <summary>Creates a new instance with the specified parameter.</summary>
        /// <param name="parameter">The parameter to be stored in this instance.</param>
        public ReactiveChainLinkParameter(object parameter)
        {
            // booleans are stored as either "1" or "0".
            if (parameter is bool @bool)
            {
                this.parameter = @bool ? "1" : "0";
                return;
            }

            // integers are simply converted to a string.
            if (parameter is int @int)
            {
                this.parameter = @int.ToString(CultureInfo.InvariantCulture);
                return;
            }

            // floats are simply converted to a string.
            if (parameter is float @float)
            {
                this.parameter = @float.ToString(CultureInfo.InvariantCulture);
                return;
            }

            // strings are stored directly.
            if (parameter is string @string)
            {
                this.parameter = @string;
                return;
            }

            // chain link parameters are copied directly.
            if (parameter is ReactiveChainLinkParameter reactiveChainLinkParameter)
            {
                this.parameter = reactiveChainLinkParameter.parameter;
                return;
            }

            throw new System.Exception("Unable to create a parameter of type '" + parameter.GetType().Name + "'!");
        }

        /// <summary>Gets the parameter interpreted as a <see cref="bool"/>.</summary>
        /// <param name="defaultValue">If the value could not be interpreted this is returned instead.</param>
        /// <returns>The parameter interpreted as a <see cref="bool"/>.</returns>
        public bool GetBool(bool defaultValue = false)
        {
            if (parameter == "1")
                return true;

            if (parameter == "0")
                return false;

            return defaultValue;
        }

        /// <summary>Gets the parameter interpreted as an <see cref="int"/>.</summary>
        /// <param name="defaultValue">If the value could not be interpreted this is returned instead.</param>
        /// <returns>The parameter interpreted as an <see cref="int"/>.</returns>
        public int GetInt(int defaultValue = 0)
        {
            if (int.TryParse(parameter, NumberStyles.Integer, CultureInfo.InvariantCulture, out int @int))
                return @int;
            return defaultValue;
        }

        /// <summary>Gets the parameter interpreted as a <see cref="float"/>.</summary>
        /// <param name="defaultValue">If the value could not be interpreted this is returned instead.</param>
        /// <returns>The parameter interpreted as a <see cref="float"/>.</returns>
        public float GetFloat(float defaultValue = 0.0f)
        {
            if (float.TryParse(parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out float @float))
                return @float;
            return defaultValue;
        }

        /// <summary>Gets the parameter interpreted as a <see cref="string"/>.</summary>
        /// <param name="defaultValue">If the value is an empty string this is returned instead.</param>
        /// <returns>The parameter interpreted as a <see cref="string"/>.</returns>
        public string GetString(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(parameter))
                return parameter;
            return defaultValue;
        }

        /// <summary>Gets whether the parameter is empty and does not have a value.</summary>
        public bool isEmpty => string.IsNullOrEmpty(parameter);

        /// <summary>Converts the parameter to a <see cref="bool"/>.</summary>
        public static explicit operator bool(ReactiveChainLinkParameter parameter) => parameter.GetBool();

        /// <summary>Converts the parameter to an <see cref="int"/>.</summary>
        public static explicit operator int(ReactiveChainLinkParameter parameter) => parameter.GetInt();

        /// <summary>Converts the parameter to a <see cref="float"/>.</summary>
        public static explicit operator float(ReactiveChainLinkParameter parameter) => parameter.GetFloat();

        /// <summary>Converts the parameter to a <see cref="string"/>.</summary>
        public static explicit operator string(ReactiveChainLinkParameter parameter) => parameter.GetString();

        /// <summary>
        /// Returns a string that represents the current parameter. Same as calling <see cref="GetString"/>.
        /// </summary>
        /// <returns>A string that represents the current parameter.</returns>
        public override string ToString()
        {
            return GetString();
        }
    }
}