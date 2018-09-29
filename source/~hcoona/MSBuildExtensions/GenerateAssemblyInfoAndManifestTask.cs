using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuildExtensions
{
    public class GenerateAssemblyInfoAndManifestTask : Task
    {
        private static readonly DataContractJsonSerializer ManifestSerializer =
            new DataContractJsonSerializer(typeof(Manifest));

        [Required]
        public string Name { get; set; }

        [Required]
        public string TargetFileName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string GUID { get; set; }

        [Required]
        public string ManifestUniqueID { get; set; }

        [Required]
        public string Version { get; set; }

        [Required]
        public string OutputAssemblyInfoFilePath { get; set; }

        [Required]
        public string OutputManifestFilePath { get; set; }

        private Guid GUID0 { get { return Guid.Parse(GUID); } }

        private Version Version0 { get { return System.Version.Parse(Version); } }

        public override bool Execute()
        {
            try
            {
                GenerateAssemblyInfoFile();
                GenerateManifestFile();
                return true;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
        }

        private void GenerateAssemblyInfoFile()
        {
            Log.LogMessage("Begin generate assembly info file to {0}", OutputAssemblyInfoFilePath);

            Directory.CreateDirectory(Path.GetDirectoryName(OutputAssemblyInfoFilePath));

            File.WriteAllText(OutputAssemblyInfoFilePath, $@"using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(""{Name}"")]
[assembly: AssemblyDescription(""{Description}"")]
[assembly: AssemblyProduct(""{Name}"")]
[assembly: ComVisible(false)]
[assembly: Guid(""{GUID0}"")]
[assembly: AssemblyVersion(""{Version0}"")]
[assembly: AssemblyFileVersion(""{Version0}"")]
");
            Log.LogMessage("Finished generate assembly info file to {0}", OutputAssemblyInfoFilePath);
        }

        private void GenerateManifestFile()
        {
            Log.LogMessage("Begin generate manifest file to {0}", OutputManifestFilePath);
            var manifest = new Manifest
            {
                Name = Name,
                Author = Author,
                Description = Description,
                EntryDll = TargetFileName,
                UniqueID = ManifestUniqueID,
                Version = new ModVersion
                {
                    MajorVersion = Version0.Major,
                    MinorVersion = Version0.Minor,
                    PatchVersion = Version0.Build,
                    Build = Version0.Revision
                }
            };

            Directory.CreateDirectory(Path.GetDirectoryName(OutputManifestFilePath));
            using (var fs = File.OpenWrite(OutputManifestFilePath))
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                try
                {
                    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(fs, Encoding.UTF8, true, true, "    "))
                    {
                        ManifestSerializer.WriteObject(writer, manifest);
                    }
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }
            }
            Log.LogMessage("Finish generate manifest file to {0}", OutputManifestFilePath);
        }
    }
}
