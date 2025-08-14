#if !UNITY_5_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using NLog;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.ReflectorNet.Model;
using com.IvanMurzak.ReflectorNet.Utils;

namespace com.IvanMurzak.Unity.MCP.Server
{
    public static partial class ToolRouter
    {
        public static async ValueTask<CallToolResult> Call(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Trace("{0}.Call", nameof(ToolRouter));

            if (request == null)
                return new CallToolResult().SetError("[Error] Request is null");

            if (request.Params == null)
                return new CallToolResult().SetError("[Error] Request.Params is null");

            if (request.Params.Arguments == null)
                return new CallToolResult().SetError("[Error] Request.Params.Arguments is null");

            var mcpServerService = McpServerService.Instance;
            if (mcpServerService == null)
                return new CallToolResult().SetError($"[Error] '{nameof(mcpServerService)}' is null");

            var toolRunner = mcpServerService.McpRunner.HasTool(request.Params.Name)
                ? mcpServerService.McpRunner
                : mcpServerService.ToolRunner;

            logger.Trace("Using ToolRunner: {0}", toolRunner?.GetType().Name);

            if (toolRunner == null)
                return new CallToolResult().SetError($"[Error] '{nameof(toolRunner)}' is null");

            // while (RemoteApp.FirstConnectionId == null)
            //     await Task.Delay(100, cancellationToken);

            var requestData = new RequestCallTool(request.Params.Name, request.Params.Arguments);
            if (logger.IsTraceEnabled)
                logger.Trace("Call remote tool '{0}':\n{1}", request.Params.Name, requestData.ToJsonOrEmptyJsonObject());

            var response = await toolRunner.RunCallTool(requestData, cancellationToken: cancellationToken);
            if (response == null)
                return new CallToolResult().SetError($"[Error] '{nameof(response)}' is null");

            if (logger.IsTraceEnabled)
                logger.Trace("Call tool response:\n{0}", response.ToJsonOrEmptyJsonObject());

            if (response.IsError)
                return new CallToolResult().SetError(response.Message ?? "[Error] Got an error during running tool");

            if (response.Value == null)
                return new CallToolResult().SetError("[Error] Tool returned null value");

            return response.Value.ToCallToolResult();
        }

        public static ValueTask<CallToolResult> Call(string name, Action<Dictionary<string, object>>? configureArguments = null)
        {
            var arguments = new Dictionary<string, object>();
            configureArguments?.Invoke(arguments);

            return CallWithJson(name, args =>
            {
                foreach (var kvp in arguments)
                    args[kvp.Key] = kvp.Value.ToJsonElement();
            });
        }

        public static ValueTask<CallToolResult> CallWithJson(string name, Action<Dictionary<string, JsonElement>>? configureArguments = null)
        {
            var mcpServer = McpServerService.Instance?.McpServer;
            if (mcpServer == null)
                throw new InvalidOperationException("[Error] 'McpServer' is null");

            var arguments = new Dictionary<string, JsonElement>();
            configureArguments?.Invoke(arguments);

            var request = new RequestContext<CallToolRequestParams>(mcpServer)
            {
                Params = new CallToolRequestParams()
                {
                    Name = name,
                    Arguments = arguments
                }
            };
            return Call(request, default);

            // Do we need to return the 'response'? It may work even better.

            // var response = await Call(request, default);
            // return response;

            // if (response == null)
            //     return "[Error] Tool response is null";

            // if (response.IsError)
            //     return response.Content?.FirstOrDefault()?.Text ?? "[Error] Got an error during running tool";

            // var result = response.Content?.FirstOrDefault()?.Text;
            // if (result == null)
            //     return "[Error] Tool returned null value";

            // // logger.Trace("Call, result: {0}", JsonSerializer.Serialize(response.Value));
            // return response.Value.ToCallToolResult();
        }
    }
}
#endif