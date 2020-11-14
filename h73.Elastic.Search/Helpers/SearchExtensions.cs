using System;
using System.Linq;
using System.Reflection;
using h73.Elastic.Core.Helpers;
using h73.Elastic.Core.Json;
using Newtonsoft.Json;

namespace h73.Elastic.Search.Helpers
{
    /// <summary>
    /// Extending search classes
    /// </summary>
    public static class SearchExtensions
    {
        /// <summary>
        /// To json.
        /// </summary>
        /// <param name="aq">The aq.</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>Json string</returns>
        public static string ToJson(this object aq, bool debug = false, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (jsonSerializerSettings.IsNull()) jsonSerializerSettings = JsonHelpers.CreateSerializerSettings();
            return debug ? 
                JsonConvert.SerializeObject(aq, Formatting.Indented, jsonSerializerSettings) : 
                JsonConvert.SerializeObject(aq, jsonSerializerSettings);
        }

        /// <summary>
        /// To json.
        /// </summary>
        /// <param name="aq">The aq.</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <returns>Json string</returns>
        public static string ToJson<T>(this Query<T> aq, bool debug = false)
        {
            var jsonSerializerSettings = JsonHelpers.CreateSerializerSettings();
            return debug ? 
                JsonConvert.SerializeObject(aq, Formatting.Indented, jsonSerializerSettings) : 
                JsonConvert.SerializeObject(aq, jsonSerializerSettings);
        }

        /// <summary>
        /// Determines whether [is castable to] [the specified to].
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="implicitly">if set to <c>true</c> [implicitly].</param>
        /// <returns>
        ///   <c>true</c> if [is castable to] [the specified to]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCastableTo(this Type from, Type to, bool implicitly = false)
        {
            return to.IsAssignableFrom(from) || from.HasCastDefined(to, implicitly);
        }

        private static bool HasCastDefined(this Type from, Type to, bool implicitly)
        {
            if ((from.IsPrimitive || from.IsEnum) && (to.IsPrimitive || to.IsEnum))
            {
                if (!implicitly)
                {
                    return from == to || (from != typeof(bool) && to != typeof(bool));
                }

                Type[][] typeHierarchy =
                {
                    new[] { typeof(byte),  typeof(sbyte), typeof(char) },
                    new[] { typeof(short), typeof(ushort) },
                    new[] { typeof(int), typeof(uint) },
                    new[] { typeof(long), typeof(ulong) },
                    new[] { typeof(float) },
                    new[] { typeof(double) }
                };
                var lowerTypes = Enumerable.Empty<Type>();
                foreach (var types in typeHierarchy)
                {
                    if (types.Any(t => t == to))
                    {
                        return lowerTypes.Any(t => t == from);
                    }

                    lowerTypes = lowerTypes.Concat(types);
                }

                return false;
            }

            return IsCastDefined(to, m => m.GetParameters()[0].ParameterType, _ => from, implicitly, false)
                   || IsCastDefined(from, _ => to, m => m.ReturnType, implicitly, true);
        }

        /// <summary>
        /// Determines whether [is cast defined] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="derivedType">Type of the derived.</param>
        /// <param name="implicitly">if set to <c>true</c> [implicitly].</param>
        /// <param name="lookInBase">if set to <c>true</c> [look in base].</param>
        /// <returns>
        ///   <c>true</c> if [is cast defined] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCastDefined(Type type, Func<MethodInfo, Type> baseType, Func<MethodInfo, Type> derivedType, bool implicitly, bool lookInBase)
        {
            var bindinFlags = BindingFlags.Public | BindingFlags.Static
                              | (lookInBase ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);
            return type.GetMethods(bindinFlags).Any(
                m => (m.Name == "op_Implicit" || (!implicitly && m.Name == "op_Explicit"))
                     && baseType(m).IsAssignableFrom(derivedType(m)));
        }
    }
}
