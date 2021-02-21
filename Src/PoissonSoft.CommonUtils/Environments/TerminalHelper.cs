using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PoissonSoft.CommonUtils.Environments
{
    /// <summary>
    /// Helper for work with console terminal
    /// </summary>
    public static class TerminalHelper
    {
        /// <summary>
        /// Run command in terminal
        /// </summary>
        /// <param name="command">Name of the command</param>
        /// <param name="arguments">Arguments of the command</param>
        /// <returns>Стандартный вывод выполнения команды</returns>
        public static TerminalResponse RunCommand(string command, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = arguments
            };
            var res = Process.Start(startInfo);
            var resp = new TerminalResponse
            {
                ErrorOutput = res.StandardError.ReadToEnd().Trim(),
                StandardOutput = res.StandardOutput.ReadToEnd().Trim()
            };
            return resp;
        }
    }

    /// <summary>
    /// Response from executed command
    /// </summary>
    public class TerminalResponse
    {
        /// <summary>
        /// Standard output
        /// </summary>
        public string StandardOutput { get; set; }
        /// <summary>
        /// Error output
        /// </summary>
        public string ErrorOutput { get; set; }
        /// <summary>
        /// Has error
        /// </summary>
        public bool HasError => string.IsNullOrWhiteSpace(ErrorOutput) == false;
    }
}
