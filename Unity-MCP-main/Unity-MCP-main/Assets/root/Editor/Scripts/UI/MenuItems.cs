#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using com.IvanMurzak.Unity.MCP.Common;
using System.Threading.Tasks;

namespace com.IvanMurzak.Unity.MCP.Editor
{
    public static class MenuItems
    {
        [MenuItem("Window/AI Connector (Unity-MCP)", priority = 1006)]
        public static void ShowWindow() => MainWindowEditor.ShowWindow();

        [MenuItem("Tools/AI Connector (Unity-MCP)/DotNet/Get Version", priority = 1007)]
        public static async void GetDotNetVersion() => await Startup.IsDotNetInstalled();

        [MenuItem("Tools/AI Connector (Unity-MCP)/DotNet/Install", priority = 1008)]
        public static async void InstallDotNet() => await Startup.InstallDotNetIfNeeded(force: true);

        [MenuItem("Tools/AI Connector (Unity-MCP)/MCP Plugin/Build and Start", priority = 1009)]
        public static void BuildAndStart() => McpPluginUnity.BuildAndStart();

        [MenuItem("Tools/AI Connector (Unity-MCP)/MCP Server/Build", priority = 1010)]
        public static Task BuildMcpServer() => Startup.BuildServer();

        [MenuItem("Tools/AI Connector (Unity-MCP)/MCP Server/Logs/Open Logs", priority = 1011)]
        public static void OpenLogs() => OpenFile(Startup.ServerLogsPath);

        [MenuItem("Tools/AI Connector (Unity-MCP)/MCP Server/Open Error Logs", priority = 1012)]
        public static void OpenErrorLogs() => OpenFile(Startup.ServerErrorLogsPath);
        static void OpenFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true // Ensures the file opens with the default application
                });
            }
            else
            {
                Debug.LogError($"{Consts.Log.Tag} Log file not found at: {filePath}");
            }
        }
    }
}
#endif