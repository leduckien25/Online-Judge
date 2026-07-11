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
            string parentDir = Path.Combine(FindSolutionDirectory(), "OnlineJudge.Sandbox");
            string filePath = Path.Combine(parentDir, "solution.py");
            
            await File.WriteAllTextAsync(filePath, sourceCode);

            string arguments = $@"run --rm -i --network none -v ""{parentDir}:/app:ro"" -m 50m --cpus=""0.5"" python:3.10-alpine /usr/bin/time -f ""EXEC_TIME:%e"" python /app/solution.py";

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

                File.WriteAllText(filePath, string.Empty);

                return (executionTime, rawOutput, cleanError);
            }
        }

        private static string FindSolutionDirectory()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                try
                {
                    var slnExists = dir.GetFiles("*.sln").Any() || dir.GetFiles("*.slnx").Any();
                    if (slnExists)
                    {
                        return dir.FullName;
                    }
                }
                catch
                {
                    // Ignore and continue up the directory tree
                }

                dir = dir.Parent;
            }

            // Fallback to previous behavior if no solution file found
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        }
    }
}