#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Collections.Generic;
using com.IvanMurzak.ReflectorNet.Utils;

namespace com.IvanMurzak.ReflectorNet.Model
{
    public class ResponseCallTool : IResponseCallTool
    {
        public bool IsError { get; set; }
        public List<ResponseCallToolContent> Content { get; set; } = new List<ResponseCallToolContent>();

        public ResponseCallTool() { }
        public ResponseCallTool(bool isError, List<ResponseCallToolContent> content)
        {
            IsError = isError;
            Content = content;
        }

        public static ResponseCallTool Error(Exception exception)
            => Error($"[Error] {exception?.Message}\n{exception?.StackTrace}");

        public static ResponseCallTool Error(string? message)
            => new ResponseCallTool(isError: true, new List<ResponseCallToolContent>
            {
                new ResponseCallToolContent()
                {
                    Type = "text",
                    Text = message,
                    MimeType = Consts.MimeType.TextPlain
                }
            });

        public static ResponseCallTool Success(string? message)
            => new ResponseCallTool(isError: false, new List<ResponseCallToolContent>
            {
                new ResponseCallToolContent()
                {
                    Type = "text",
                    Text = message,
                    MimeType = Consts.MimeType.TextPlain
                }
            });
    }
}