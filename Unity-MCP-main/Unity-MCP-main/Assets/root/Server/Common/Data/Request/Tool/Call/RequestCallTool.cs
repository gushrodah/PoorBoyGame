#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace com.IvanMurzak.ReflectorNet.Model
{
    public class RequestCallTool : IRequestCallTool
    {
        public string RequestID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public IReadOnlyDictionary<string, JsonElement> Arguments { get; set; } = new Dictionary<string, JsonElement>();

        public RequestCallTool() { }
        public RequestCallTool(string name, IReadOnlyDictionary<string, JsonElement> arguments)
            : this(Guid.NewGuid().ToString(), name, arguments) { }
        public RequestCallTool(string requestId, string name, IReadOnlyDictionary<string, JsonElement> arguments)
        {
            RequestID = requestId ?? throw new ArgumentNullException(nameof(requestId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        public virtual void Dispose()
        {
            // Arguments.Clear();
        }
        ~RequestCallTool() => Dispose();
    }
}