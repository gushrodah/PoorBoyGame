#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.IvanMurzak.Unity.MCP.Common
{
    public static class ExtensionsObject
    {
        public static Task<T> TaskFromResult<T>(this T response)
            => Task.FromResult(response);

        public static T[] MakeArray<T>(this T item) => new T[] { item };
        public static List<T> MakeList<T>(this T item) => new List<T> { item };

        public static string JoinString(this IEnumerable<string> items, string seperator)
            => string.Join(seperator, items);
        public static string JoinString(this IEnumerable<int> items, string seperator)
            => string.Join(seperator, items);
    }
}