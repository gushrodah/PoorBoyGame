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
    public class Vector4Converter : JsonConverter<Vector4>, IJsonSchemaConverter
    {
        public string Id => typeof(Vector4).GetTypeId();
        public JsonNode GetScheme() => new JsonObject
        {
            [JsonUtils.Schema.Id] = Id,
            [JsonUtils.Schema.Type] = JsonUtils.Schema.Object,
            [JsonUtils.Schema.Properties] = new JsonObject
            {
                ["x"] = new JsonObject { [JsonUtils.Schema.Type] = JsonUtils.Schema.Number },
                ["y"] = new JsonObject { [JsonUtils.Schema.Type] = JsonUtils.Schema.Number },
                ["z"] = new JsonObject { [JsonUtils.Schema.Type] = JsonUtils.Schema.Number },
                ["w"] = new JsonObject { [JsonUtils.Schema.Type] = JsonUtils.Schema.Number }
            },
            [JsonUtils.Schema.Required] = new JsonArray { "x", "y", "z", "w" }
        };
        public JsonNode GetSchemeRef() => new JsonObject
        {
            [JsonUtils.Schema.Ref] = Id
        };

        public override Vector4 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            float x = 0, y = 0, z = 0, w = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return new Vector4(x, y, z, w);

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
                        case "z":
                            z = reader.GetSingle();
                            break;
                        case "w":
                            w = reader.GetSingle();
                            break;
                        default:
                            throw new JsonException($"Unexpected property name: {propertyName}. "
                                + "Expected 'x', 'y', 'z', or 'w'.");
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Vector4 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.x);
            writer.WriteNumber("y", value.y);
            writer.WriteNumber("z", value.z);
            writer.WriteNumber("w", value.w);
            writer.WriteEndObject();
        }
    }
}
