using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace VcpkgBuildTask
{
    [RequiredRuntime("v2.0")]
    public class VcpkgPackages : Task
    {
        /// <summary>
        /// The vcpkg root directory path
        /// </summary>
        public string VcpkgRoot { get; set; }

        /// <summary>
        /// The vcpkg exe path
        /// </summary>
        public string VcpkgExe { get; set; }

        /// <summary>
        /// The time in ms after which we time out an install
        /// </summary>
        public int TimeoutMs { get; set; }

        /// <summary>
        /// The packages to install
        /// </summary>
        [Required]
        public string[] Packages { get; set; }
        
        /// <summary>
        /// Default ctor, sets default values
        /// </summary>
        public VcpkgPackages() : base()
        {
            VcpkgExe = Path.Combine(
                Path.GetDirectoryName(
                    Uri.UnescapeDataString(
                        new UriBuilder(
                            Assembly.GetExecutingAssembly()
                                .CodeBase
                        )
                        .Path
                    )
                ),
                "vcpkg.exe");
            TimeoutMs = 10 * 60 * 1000;
        }

        /// <summary>
        /// Actual msbuild task work
        /// </summary>
        /// <returns>status flag</returns>
        public override bool Execute()
        {
            if (Packages.Length == 0)
            {
                LogMessage("No packages to install");
                return true;
            }

            // start the install process
            var installProc = InstallPackages(Packages);

            System.Threading.Tasks.Task.Run(() =>
            {
                using (installProc.StandardOutput)
                {
                    while (!installProc.StandardOutput.EndOfStream)
                    {
                        LogMessage(installProc.StandardOutput.ReadLine(), dropPrefix: true);
                    }
                }

                using (installProc.StandardError)
                {
                    while (!installProc.StandardOutput.EndOfStream)
                    {
                        LogMessage(installProc.StandardError.ReadLine(), dropPrefix: true);
                    }
                }
            });
            
            var installDone = installProc.WaitForExit(TimeoutMs);
            
            if (!installDone)
            {
                installProc.Kill();
                LogError("install timed out at " + TimeoutMs);
                return false;
            }
            else
            {
                if (installProc.ExitCode == 0)
                {
                    LogMessage("installed " + string.Join(", ", Packages));
                    return true;
                }
                else
                {
                    LogError("install failed with code " + installProc.ExitCode);
                    return false;
                }
            }
        }

        /// <summary>
        /// Helper to create the install process
        /// </summary>
        /// <param name="pkgs">packages to install</param>
        /// <returns>install process</returns>
        private Process InstallPackages(string[] pkgs)
        {
            var args = "install " +
                    string.Join(" ", pkgs) +
                    (string.IsNullOrEmpty(VcpkgRoot) ? "" : " --vcpkg-root \"" + VcpkgRoot + "\"");

            LogMessage(VcpkgExe + " " + args, dropPrefix: true);

            return Process.Start(new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = VcpkgExe,
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            });
        }

        /// <summary>
        /// Apparently Log is broken, so we use this
        /// </summary>
        /// <param name="message"></param>
        private void LogMessage(string message, bool dropPrefix = false)
        {
            var logName = "Vcpkg";
            var messagePrefix = logName + ": ";

            if (dropPrefix)
            {
                messagePrefix = "";
            }

            this.BuildEngine.LogMessageEvent(new BuildMessageEventArgs(messagePrefix + message, logName, logName, MessageImportance.High));
        }

        /// <summary>
        /// Apparently Log is broken, so we use this
        /// </summary>
        /// <param name="message"></param>
        private void LogError(string error)
        {
            var logName = "Vcpkg";
            var logErr = 0;
            var messagePrefix = logName + ": ";
            this.BuildEngine.LogErrorEvent(new BuildErrorEventArgs(
                logName,
                "",
                logName,
                logErr,
                logErr,
                logErr,
                logErr,
                messagePrefix + error,
                logName,
                logName));
        }
    }
}
