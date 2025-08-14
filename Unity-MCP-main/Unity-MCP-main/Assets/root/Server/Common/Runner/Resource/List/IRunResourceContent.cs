#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.Collections.Generic;
using System.Threading.Tasks;
using com.IvanMurzak.ReflectorNet.Model;

namespace com.IvanMurzak.Unity.MCP.Common
{
    public interface IRunResourceContent
    {
        /// <summary>
        /// Executes the target static method with the provided arguments.
        /// </summary>
        /// <param name="parameters">The arguments to pass to the method.</param>
        /// <returns>The result of the method execution, or null if the method is void.</returns>
        Task<ResponseResourceContent[]> Run(params object?[] parameters);

        /// <summary>
        /// Executes the target method with named parameters.
        /// Missing parameters will be filled with their default values or the type's default value if no default is defined.
        /// </summary>
        /// <param name="namedParameters">A dictionary mapping parameter names to their values.</param>
        /// <returns>The result of the method execution, or null if the method is void.</returns>
        Task<ResponseResourceContent[]> Run(IDictionary<string, object?>? namedParameters);
    }
}