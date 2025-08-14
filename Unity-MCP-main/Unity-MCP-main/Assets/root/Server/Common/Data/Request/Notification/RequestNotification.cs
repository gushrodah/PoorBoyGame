#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Collections.Generic;

namespace com.IvanMurzak.ReflectorNet.Model
{
    public class RequestNotification : IRequestNotification
    {
        public string RequestID { get; set; } = string.Empty;
        public string? Path { get; set; }
        public string? Name { get; set; }
        public IDictionary<string, object?>? Parameters { get; set; } = new Dictionary<string, object?>();

        public RequestNotification() { }
        public RequestNotification(string path, string name)
        : this(Guid.NewGuid().ToString(), path, name) { }
        public RequestNotification(string requestId, string path, string name) : this()
        {
            RequestID = requestId ?? throw new ArgumentNullException(nameof(requestId));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public virtual void Dispose()
        {
            Parameters?.Clear();
        }
        ~RequestNotification() => Dispose();
    }
}