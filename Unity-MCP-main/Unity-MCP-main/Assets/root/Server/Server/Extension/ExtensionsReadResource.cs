#if !UNITY_5_3_OR_NEWER
using System;
using System.Collections.Generic;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.ReflectorNet.Model;
using ModelContextProtocol.Protocol;

namespace com.IvanMurzak.Unity.MCP.Server
{
    public static class ExtensionsReadResource
    {
        public static ReadResourceResult SetError(this ReadResourceResult target, string uri, string message)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var error = new TextResourceContents()
            {
                Uri = uri,
                MimeType = Consts.MimeType.TextPlain,
                Text = message
            };

            target.Contents ??= new List<ResourceContents>(1);
            target.Contents.Clear();
            target.Contents.Add(error);

            return target;
        }

        public static ResourceContents ToResourceContents(this IResponseResourceContent response)
        {
            if (response!.text != null)
                return new TextResourceContents()
                {
                    Uri = response.uri,
                    MimeType = response.mimeType,
                    Text = response.text
                };

            if (response!.blob != null)
                return new BlobResourceContents()
                {
                    Uri = response.uri,
                    MimeType = response.mimeType,
                    Blob = response.blob
                };

            throw new InvalidOperationException("Resource contents is null");
        }
    }
}
#endif