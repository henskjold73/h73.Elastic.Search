using h73.Elastic.Core.Json;
using Newtonsoft.Json;

namespace h73.Elastic.Search.Helpers
{
    public static class Cloner
    {
        /// <summary>
        /// Clones the specified source.
        /// </summary>
        /// <typeparam name="T">Type of T</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>A clone of the object.</returns>
        public static T Clone<T>(T source)
            where T : new()
        {
            var jsonSerializerSettings = JsonHelpers.CreateSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;
            var serialized = JsonConvert.SerializeObject(source, jsonSerializerSettings);
            return JsonConvert.DeserializeObject<T>(serialized, jsonSerializerSettings);
        }
    }
}
