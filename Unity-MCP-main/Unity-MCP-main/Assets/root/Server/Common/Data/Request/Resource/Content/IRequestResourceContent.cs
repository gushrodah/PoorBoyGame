#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;

namespace com.IvanMurzak.ReflectorNet.Model
{
    public interface IRequestResourceContent : IRequestID, IDisposable
    {
        public string Uri { get; set; }
    }
}