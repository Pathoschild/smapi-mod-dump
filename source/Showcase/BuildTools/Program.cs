using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Igorious.BuildManifest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var assemblyPath = args[0];
            var assembly = Assembly.LoadFrom(assemblyPath);

            var manifest = new Manifest
            {
                Name = assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title,
                Author = assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company,
                Version = GetVersion(assembly),
                Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description,
                UniqueID = assembly.GetCustomAttribute<GuidAttribute>().Value,
                EntryDll = Path.GetFileName(assemblyPath),
            };

            var json = JsonConvert.SerializeObject(manifest, new JsonSerializerSettings { Formatting = Formatting.Indented });
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(assemblyPath), "manifest.json"), json);
        }

        private static ModVersion GetVersion(Assembly assembly)
        {
            var version = assembly.GetName().Version;
            return new ModVersion
            {
                MajorVersion = version.Major,
                MinorVersion = version.Minor,
                PatchVersion = version.Build,
                Build = version.Revision.ToString(CultureInfo.InvariantCulture),
            };
        }
    }
}