#if !UNITY_5_3_OR_NEWER
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using NLog;
using com.IvanMurzak.ReflectorNet.Model;
using com.IvanMurzak.ReflectorNet.Utils;

namespace com.IvanMurzak.Unity.MCP.Server
{
    public static partial class ToolRouter
    {
        public static async ValueTask<ListToolsResult> ListAll(RequestContext<ListToolsRequestParams> request, CancellationToken cancellationToken)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Trace("{0}.ListAll", nameof(ToolRouter));

            var mcpServerService = McpServerService.Instance;
            if (mcpServerService == null)
                return new ListToolsResult().SetError($"[Error] '{nameof(mcpServerService)}' is null");

            var toolRunner = mcpServerService.ToolRunner;
            if (toolRunner == null)
                return new ListToolsResult().SetError($"[Error] '{nameof(toolRunner)}' is null");

            logger.Trace("Using ToolRunner: {0}", toolRunner.GetType().Name);

            var requestData = new RequestListTool();
            var response = await toolRunner.RunListTool(requestData, cancellationToken: cancellationToken);
            if (response == null)
                return new ListToolsResult().SetError($"[Error] '{nameof(response)}' is null");

            if (response.IsError)
                return new ListToolsResult().SetError(response.Message ?? "[Error] Got an error during reading resources");

            if (response.Value == null)
                return new ListToolsResult().SetError("[Error] Resource value is null");

            var result = new ListToolsResult()
            {
                Tools = response.Value
                    .Where(x => x != null)
                    .Select(x => x!.ToTool())
                    .ToList()
            };

            if (logger.IsTraceEnabled)
                logger.Trace("ListAll, result: {0}", JsonUtils.ToJson(result));

            return result;
        }
    }
}
#endif