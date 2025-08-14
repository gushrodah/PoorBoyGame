#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using com.IvanMurzak.ReflectorNet;
using com.IvanMurzak.ReflectorNet.Model;
using com.IvanMurzak.ReflectorNet.Utils;

namespace com.IvanMurzak.Unity.MCP.Common
{
    /// <summary>
    /// Provides functionality to execute methods dynamically, supporting both static and instance methods.
    /// Allows for parameter passing by position or by name, with support for default parameter values.
    /// </summary>
    public partial class RunResourceContent : MethodWrapper, IRunResourceContent
    {
        /// <summary>
        /// Initializes the Command with the target method information.
        /// </summary>
        /// <param name="type">The type containing the static method.</param>
        public static RunResourceContent CreateFromStaticMethod(Reflector reflector, ILogger? logger, MethodInfo methodInfo)
            => new RunResourceContent(reflector, logger, methodInfo);

        /// <summary>
        /// Initializes the Command with the target instance method information.
        /// </summary>
        /// <param name="targetInstance">The instance of the object containing the method.</param>
        /// <param name="methodInfo">The MethodInfo of the instance method to execute.</param>
        public static RunResourceContent CreateFromInstanceMethod(Reflector reflector, ILogger? logger, object targetInstance, MethodInfo methodInfo)
            => new RunResourceContent(reflector, logger, targetInstance, methodInfo);

        /// <summary>
        /// Initializes the Command with the target instance method information.
        /// </summary>
        /// <param name="targetInstance">The instance of the object containing the method.</param>
        /// <param name="methodInfo">The MethodInfo of the instance method to execute.</param>
        public static RunResourceContent CreateFromClassMethod(Reflector reflector, ILogger? logger, Type targetType, MethodInfo methodInfo)
            => new RunResourceContent(reflector, logger, targetType, methodInfo);

        public RunResourceContent(Reflector reflector, ILogger? logger, MethodInfo methodInfo) : base(reflector, logger, methodInfo) { }
        public RunResourceContent(Reflector reflector, ILogger? logger, object targetInstance, MethodInfo methodInfo) : base(reflector, logger, targetInstance, methodInfo) { }
        public RunResourceContent(Reflector reflector, ILogger? logger, Type targetType, MethodInfo methodInfo) : base(reflector, logger, targetType, methodInfo) { }

        /// <summary>
        /// Executes the target static method with the provided arguments.
        /// </summary>
        /// <param name="parameters">The arguments to pass to the method.</param>
        /// <returns>The result of the method execution, or null if the method is void.</returns>
        public async Task<ResponseResourceContent[]> Run(params object?[] parameters)
        {
            var result = await Invoke(parameters);

            if (_logger?.IsEnabled(LogLevel.Trace) ?? false)
                _logger.LogTrace("Result: {result}", JsonUtils.ToJson(result));

            return result as ResponseResourceContent[] ?? throw new InvalidOperationException($"The method did not return a valid {nameof(ResponseResourceContent)}[]. Instead returned {result?.GetType().Name}.");
        }

        /// <summary>
        /// Executes the target method with named parameters.
        /// Missing parameters will be filled with their default values or the type's default value if no default is defined.
        /// </summary>
        /// <param name="namedParameters">A dictionary mapping parameter names to their values.</param>
        /// <returns>The result of the method execution, or null if the method is void.</returns>
        public async Task<ResponseResourceContent[]> Run(IDictionary<string, object?>? namedParameters)
        {
            var result = await InvokeDict(namedParameters?.ToImmutableDictionary(x => x.Key, x => x.Value));

            if (_logger?.IsEnabled(LogLevel.Trace) ?? false)
                _logger.LogTrace("Result: {result}", JsonUtils.ToJson(result));

            return result as ResponseResourceContent[] ?? throw new InvalidOperationException($"The method did not return a valid {nameof(ResponseResourceContent)}[]. Instead returned {result?.GetType().Name}.");
        }
    }
}