/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using StardewModdingAPI;
using Version = StardewModdingAPI.Version;

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
                Authour = assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company,
                Version = GetVersion(assembly),
                Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description,
                UniqueID = assembly.GetCustomAttribute<GuidAttribute>().Value,
                PerSaveConfigs = false,
                EntryDll = Path.GetFileName(assemblyPath),
            };

            var json = JsonConvert.SerializeObject(manifest, new JsonSerializerSettings {Formatting = Formatting.Indented});
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(assemblyPath), "manifest.json"), json);
        }

        private static Version GetVersion(Assembly assembly)
        {
            var version = assembly.GetName().Version;
            return new Version
            {
                MajorVersion = version.Major,
                MinorVersion = version.Minor,
                PatchVersion = version.Revision,
                Build = version.Build.ToString(CultureInfo.InvariantCulture),
            };
        }
    }
}
