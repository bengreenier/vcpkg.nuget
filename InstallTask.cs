using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VcpkgBuildTask
{
    public class InstallTask : ToolTask
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
        /// The vcpkg triplet to default to
        /// </summary>
        public string VcpkgTriplet { get; set; }

        /// <summary>
        /// The packages to install
        /// </summary>
        [Required]
        public string[] Packages { get; set; }

        protected override string ToolName => "Vcpkg";

        public InstallTask() : base()
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
            Timeout = 10 * 60 * 1000;
        }

        protected override string GenerateFullPathToTool()
        {
            return VcpkgExe;
        }

        protected override string GenerateCommandLineCommands()
        {
            var builder = new CommandLineBuilder();

            builder.AppendTextUnquoted("install");
            
            foreach (var package in Packages)
            {
                try
                {
                    var pkg = new PackageInfo(package);
                    var triplet = pkg.Triplet;

                    if (string.IsNullOrEmpty(triplet))
                    {
                        triplet = VcpkgTriplet;
                    }

                    if (!string.IsNullOrEmpty(triplet))
                    {
                        triplet = PackageInfo.InfoSeparator + triplet;
                    }

                    builder.AppendTextUnquoted(" " + pkg.Name + triplet);
                }
                catch (Exception ex)
                {
                    this.BuildEngine.LogErrorEvent(new BuildErrorEventArgs("PackageNameParsing",
                        "InvalidPackageName",
                        "",
                        0,
                        0,
                        0,
                        0,
                        package + "\n" + ex.ToString(),
                        "PackageNameParsing",
                        "Vcpkg.PackageNameParsing"));
                }
            }

            if (!string.IsNullOrEmpty(VcpkgRoot))
            {
                builder.AppendSwitch("--vcpkg-root");
                builder.AppendFileNameIfNotNull(VcpkgRoot);
            }

            return builder.ToString();
        }
    }
}
