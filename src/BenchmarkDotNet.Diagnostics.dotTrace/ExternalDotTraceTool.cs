using System;
using System.Diagnostics;
using System.Threading;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Loggers;
using JetBrains.Profiler.SelfApi;
using ILogger = BenchmarkDotNet.Loggers.ILogger;

namespace BenchmarkDotNet.Diagnostics.dotTrace
{
    internal class ExternalDotTraceTool : DotTraceToolBase
    {
        public ExternalDotTraceTool(ILogger logger, Uri nugetUrl = null, NuGetApi nugetApi = NuGetApi.V3, string downloadTo = null) :
            base(logger, nugetUrl, nugetApi, downloadTo) { }

        protected override bool AttachOnly => true;

        protected override void Attach(DiagnoserActionParameters parameters, string snapshotFile)
        {
            var logger = parameters.Config.GetCompositeLogger();

            string runnerPath = GetRunnerPath();
            int pid = parameters.Process.Id;
            string arguments = $"attach {pid} --save-to=\"{snapshotFile}\"";

            logger.WriteLineInfo($"Starting process: '{runnerPath} {arguments}'");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = runnerPath,
                WorkingDirectory = "",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            using (new ConsoleExitHandler(process, NullLogger.Instance))
            {
                try
                {
                    process.OutputDataReceived += (_, args) => logger.WriteLineInfo(args.Data);
                    process.ErrorDataReceived += (_, args) => logger.WriteLineError(args.Data);
                    process.Start();
                }
                catch (Exception e)
                {
                    logger.WriteLineError(e.ToString());
                }
            }
            Thread.Sleep(1_000);
        }

        protected override void StartCollectingData() { }

        protected override void SaveData() { }

        protected override void Detach() { }
    }
}