using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rimirin.Bestdori.JsonConverters
{
    public class UnixTimeStringJsonConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(reader.GetString())).LocalDateTime;
            }
            return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).LocalDateTime;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(new DateTimeOffset(value.Value).ToUnixTimeMilliseconds().ToString());
            }
        }
    }
}