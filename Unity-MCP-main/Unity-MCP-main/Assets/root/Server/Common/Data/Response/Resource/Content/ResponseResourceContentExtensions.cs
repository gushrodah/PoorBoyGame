#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using Microsoft.Extensions.Logging;

namespace com.IvanMurzak.ReflectorNet.Model
{
    public static class ResponseResourceContentExtensions
    {
        public static ResponseResourceContent[] Log(this ResponseResourceContent[] target, ILogger logger, Exception? ex = null)
        {
            if (!logger.IsEnabled(LogLevel.Information))
                return target;

            foreach (var item in target)
                logger.LogInformation(ex, "Resource: {0}", item.uri);

            return target;
        }


        public static IResponseData<ResponseResourceContent[]> Pack(this ResponseResourceContent[] target, string requestId, string? message = null)
            => ResponseData<ResponseResourceContent[]>.Success(requestId, message ?? "List Tool execution completed.")
                .SetData(target);
    }
}