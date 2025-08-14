#if !UNITY_5_3_OR_NEWER
using System;
using com.IvanMurzak.ReflectorNet.Model;
using ModelContextProtocol.Protocol;

namespace com.IvanMurzak.Unity.MCP.Server
{
    public static class ExtensionsListResources
    {
        public static ListResourcesResult SetError(this ListResourcesResult target, string message)
        {
            throw new Exception(message);
        }

        public static Resource ToResource(this IResponseListResource response)
        {
            return new Resource()
            {
                Uri = response.uri,
                Name = response.name,
                Description = response.description,
                MimeType = response.mimeType
            };
        }
    }
}
#endif