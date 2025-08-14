using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using com.IvanMurzak.ReflectorNet;
using com.IvanMurzak.ReflectorNet.Json;
using com.IvanMurzak.ReflectorNet.Utils;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Common.Json.Converters
{
    public class Vector2Converter : JsonConverter<Vector2>, IJsonSchemaConverter
    {
        public string Id => typeof(Vector2).GetTypeId();
        public JsonNode GetScheme() => new JsonObject
        {
            [JsonUtils.Schema.Id] = Id,
            [JsonUtils.Schema.Type] = JsonUtils.Schema.Object,
            [JsonUtils.Schema.Properties] = new JsonObject
            {
                ["x"] = new JsonObject { [JsonUtils.Schema.Type] = JsonUtils.Schema.Number },
                ["y"] = new JsonObject { [JsonUtils.Schema.Type] = JsonUtils.Schema.Number }
            },
            [JsonUtils.Schema.Required] = new JsonArray { "x", "y" }
        };
        public JsonNode GetSchemeRef() => new JsonObject
        {
            [JsonUtils.Schema.Ref] = Id
        };

        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            float x = 0, y = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return new Vector2(x, y);

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "x":
                            x = reader.GetSingle();
                            break;
                        case "y":
                            y = reader.GetSingle();
                            break;
                        default:
                            throw new JsonException($"Unexpected property name: {propertyName}. "
                                + "Expected 'x' or 'y'.");
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.x);
            writer.WriteNumber("y", value.y);
            writer.WriteEndObject();
        }
    }
}
