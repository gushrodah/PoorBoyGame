#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using com.IvanMurzak.ReflectorNet;
using com.IvanMurzak.ReflectorNet.Json;
using com.IvanMurzak.ReflectorNet.Model.Unity;
using com.IvanMurzak.ReflectorNet.Utils;

namespace com.IvanMurzak.Unity.MCP.Common.Json
{
    public class ObjectRefConverter : JsonConverter<ObjectRef>, IJsonSchemaConverter
    {
        public string Id => typeof(ObjectRef).GetTypeId();
        public JsonNode GetScheme() => new JsonObject
        {
            [JsonUtils.Schema.Id] = Id,
            [JsonUtils.Schema.Type] = JsonUtils.Schema.Object,
            [JsonUtils.Schema.Properties] = new JsonObject
            {
                [nameof(ObjectRef.instanceID)] = new JsonObject { [JsonUtils.Schema.Type] = JsonUtils.Schema.Integer },
                [nameof(ObjectRef.assetPath)] = new JsonObject { [JsonUtils.Schema.Type] = JsonUtils.Schema.String },
                [nameof(ObjectRef.assetGuid)] = new JsonObject { [JsonUtils.Schema.Type] = JsonUtils.Schema.String }
            },
            [JsonUtils.Schema.Required] = new JsonArray { nameof(ObjectRef.instanceID) }
        };
        public JsonNode GetSchemeRef() => new JsonObject
        {
            [JsonUtils.Schema.Ref] = Id
        };

        public override ObjectRef? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            var instanceID = new ObjectRef();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read(); // Move to the value token

                    switch (propertyName)
                    {
                        case nameof(ObjectRef.instanceID):
                            instanceID.instanceID = reader.GetInt32();
                            break;
                        case nameof(ObjectRef.assetPath):
                            instanceID.assetPath = reader.GetString();
                            break;
                        case nameof(ObjectRef.assetGuid):
                            instanceID.assetGuid = reader.GetString();
                            break;
                        default:
                            throw new JsonException($"Unexpected property name: {propertyName}. "
                                + $"Expected '{nameof(ObjectRef.instanceID)}', '{nameof(ObjectRef.assetPath)}', or '{nameof(ObjectRef.assetGuid)}'.");
                    }
                }
            }

            return instanceID;
        }

        public override void Write(Utf8JsonWriter writer, ObjectRef value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteStartObject();

                // Write the "instanceID" property
                writer.WritePropertyName(nameof(ObjectRef.instanceID));
                writer.WriteNumberValue(0);

                writer.WriteEndObject();
                return;
            }

            writer.WriteStartObject();

            // Write the "instanceID" property
            writer.WritePropertyName(nameof(ObjectRef.instanceID));
            writer.WriteNumberValue(value.instanceID);

            // Write the "assetPath" property
            if (!string.IsNullOrEmpty(value.assetPath))
            {
                writer.WritePropertyName(nameof(ObjectRef.assetPath));
                writer.WriteStringValue(value.assetPath);
            }

            // Write the "assetGuid" property
            if (!string.IsNullOrEmpty(value.assetGuid))
            {
                writer.WritePropertyName(nameof(ObjectRef.assetGuid));
                writer.WriteStringValue(value.assetGuid);
            }

            writer.WriteEndObject();
        }
    }
}