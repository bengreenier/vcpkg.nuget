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
            TimeoutMs = 10 * 1000;
        }

        /// <summary>
        /// Actual msbuild task work
        /// </summary>
        /// <returns>status flag</returns>
        public override bool Execute()
        {
            // start the install process
            var installProc = InstallPackages(Packages);

            // log stdout and stderr
            Log.LogMessagesFromStream(installProc.StandardOutput,
                MessageImportance.Low);
            Log.LogMessagesFromStream(installProc.StandardError,
                MessageImportance.Normal);

            var installDone = installProc.WaitForExit(TimeoutMs);

            if (!installDone)
            {
                installProc.Kill();
                Log.LogMessage(MessageImportance.High,
                    "Vcpkg install timed out at " + TimeoutMs);
                return false;
            }
            else
            {
                Log.LogMessage(MessageImportance.Normal,
                    "Installed " +
                    string.Join(",", Packages));
                return true;
            }
        }

        /// <summary>
        /// Helper to create the install process
        /// </summary>
        /// <param name="pkgs">packages to install</param>
        /// <returns>install process</returns>
        private Process InstallPackages(string[] pkgs)
        {
            return Process.Start(VcpkgExe,
                "install " +
                string.Join(" ", pkgs) +
                (string.IsNullOrEmpty(VcpkgRoot) ? "" : "--vcpkg-root " + VcpkgRoot));
        }
    }
}
