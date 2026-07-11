using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineJudge.Sandbox.Services
{
    public interface ISandboxService
    {
        Task<(double Time, string Output, string? Error)> RunPythonCodeAsync(string sourceCode, string inputData);
    }
}
