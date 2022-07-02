using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CostSplitter
{
    internal static class Common
    {
        private readonly static JsonSerializerSettings _settings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public static (T?, Exception?) DeserializeJsonString<T>(this string serializedString, bool convertSnakeCaseToPascalCase = false)
            where T : class?
        {
            if (string.IsNullOrEmpty(serializedString))
                return (default, new ArgumentNullException(nameof(serializedString)));

            var settings = convertSnakeCaseToPascalCase ? _settings : new JsonSerializerSettings();
            try
            {
                return (JsonConvert.DeserializeObject<T>(serializedString, settings), default);
            }
            catch (Exception e) when (e is JsonReaderException || e is JsonSerializationException)
            {
                return (default, e);
            }
        }

        public static string FormatAsCurrency(this decimal d, NumberFormatInfo formatInfo, string formattingString = "")
            => string.IsNullOrEmpty(formattingString) ? string.Format(formatInfo, "{0:C}", d) : string.Format(formatInfo, formattingString, d);
    }
}
