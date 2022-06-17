using System.Text.Json;
using System.Text.Json.Serialization;

namespace nvm.Serialization.Json.Converters
{
    internal class StringBoolConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True) { return "true"; }
            if (reader.TokenType == JsonTokenType.False) { return "false"; }

            var converter = options.GetConverter(typeof(JsonElement)) as JsonConverter<JsonElement>;
            if (converter != null)
            {
                var elem = converter.Read(ref reader, typeToConvert, options);
                return elem.ToString();
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
