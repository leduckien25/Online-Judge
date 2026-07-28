using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineJudge.Sandbox.Services
{
    public class SandboxService : ISandboxService
    {
        public SandboxService() { }

        public async Task<(double Time, string Output, string? Error)> RunPythonCodeAsync(string sourceCode, string inputData)
        {
            string localContainerDir = Path.Combine(AppContext.BaseDirectory, "sandbox");

            string filePath = Path.Combine(localContainerDir, "solution.py");

            await File.WriteAllTextAsync(filePath, sourceCode);

            string hostMountDir = Environment.GetEnvironmentVariable("HOST_SANDBOX_DIR")
                                  ?? Path.GetFullPath(localContainerDir).Replace("\\", "/");

            string arguments = $@"run --rm -i --network none -v ""{hostMountDir}:/app:ro"" -m 50m --cpus=""0.5"" python:3.10-alpine python /app/solution.py";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = arguments,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();

                if (!string.IsNullOrEmpty(inputData))
                {
                    using (var writer = process.StandardInput)
                    {
                        await writer.WriteAsync(inputData);
                    }
                }
                else
                {
                    process.StandardInput.Close();
                }

                Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                Task<string> errorTask = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                string rawOutput = await outputTask;
                string rawError = await errorTask;

                double executionTime = 0;

                string? cleanError = null;

                if (!string.IsNullOrEmpty(rawError))
                {
                    var lines = rawError.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

                    var timeLine = lines.FirstOrDefault(l => l.StartsWith("EXEC_TIME:"));
                    if (timeLine != null)
                    {
                        string timeString = timeLine.Replace("EXEC_TIME:", "").Trim();
                        if (double.TryParse(timeString, out double seconds))
                        {
                            executionTime = seconds * 1000;
                        }
                    }

                    var crashLines = lines.Where(l => !l.StartsWith("EXEC_TIME:")).ToList();

                    if (crashLines.Any())
                    {
                        cleanError = string.Join(Environment.NewLine, crashLines);
                    }
                }

                await File.WriteAllTextAsync(filePath, string.Empty);

                return (executionTime, rawOutput, cleanError);
            }
        }
    }
}