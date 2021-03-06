/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace TehPers.DependencyResolver {
    public class ModDependencyResolver : Mod {

        public override void Entry(IModHelper helper) {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Get loaded versions
            this.Monitor.Log("Getting versions of currently loaded assemblies...", LogLevel.Trace);
            Dictionary<string, Version> loadedAssemblyVersions = new Dictionary<string, Version>();
            foreach (Assembly loadedAssembly in loadedAssemblies) {
                AssemblyName assemblyName = loadedAssembly.GetName();
                if (loadedAssemblyVersions.TryGetValue(assemblyName.Name, out Version version)) {
                    if (version.CompareTo(assemblyName.Version) != 0) {
                        this.Monitor.Log($" - {assemblyName.Name}@{version} and {assemblyName.Name}@{assemblyName.Version} have the same name and different versions", LogLevel.Trace);
                    }
                } else {
                    loadedAssemblyVersions.Add(assemblyName.Name, assemblyName.Version);
                }
            }

            // Check each loaded mod's dependencies
            Dictionary<IManifest, HashSet<AssemblyName>> outdatedDependencies = new Dictionary<IManifest, HashSet<AssemblyName>>();
            this.Monitor.Log("Checking mod dependencies...", LogLevel.Trace);
            foreach (Assembly assembly in loadedAssemblies) {
                // Check if the assembly has any exported mod classes
                if (!assembly.DefinedTypes.Any(t => typeof(Mod).IsAssignableFrom(t))) {
                    continue;
                }

                // Check if the assembly has a manifest associated with it
                AssemblyName assemblyName = assembly.GetName();
                this.Monitor.Log($" - Found potential mod {assemblyName.Name}", LogLevel.Trace);
                IManifest manifest = helper.ModRegistry.GetAll().FirstOrDefault(m => string.Equals(m.Manifest.EntryDll, $"{assemblyName.Name}.dll", StringComparison.OrdinalIgnoreCase))?.Manifest;

                if (manifest == null) {
                    this.Monitor.Log($"    - No manifest was found for it. Skipping...", LogLevel.Trace);
                    continue;
                } else {
                    this.Monitor.Log($" - Scanning dependencies for {manifest.Name}@{manifest.Version}", LogLevel.Trace);
                }

                // Get the assembly's dependencies
                AssemblyName[] dependencies = assembly.GetReferencedAssemblies();

                // Check each dependency to see if an outdated version is loaded
                foreach (AssemblyName dependency in dependencies) {
                    if (loadedAssemblyVersions.TryGetValue(dependency.Name, out Version loadedVersion)) {
                        if (loadedVersion.CompareTo(dependency.Version) < 0) {
                            this.Monitor.Log($"    - Outdated dependency {dependency.Name}@{loadedVersion} is loaded. Version {dependency.Version} is requested.", LogLevel.Trace);

                            // Track outdated dependency
                            if (outdatedDependencies.TryGetValue(manifest, out HashSet<AssemblyName> outdatedNames)) {
                                outdatedNames.Add(dependency);
                            } else {
                                outdatedDependencies.Add(manifest, new HashSet<AssemblyName> { dependency });
                            }
                        }
                    } else {
                        this.Monitor.Log($"    - Unable to find loaded version of {dependency.Name}. Requested version is {dependency.Version}", LogLevel.Trace);

                        // Track unknown dependency
                        if (outdatedDependencies.TryGetValue(manifest, out HashSet<AssemblyName> outdatedNames)) {
                            outdatedNames.Add(dependency);
                        } else {
                            outdatedDependencies.Add(manifest, new HashSet<AssemblyName> { dependency });
                        }
                    }
                }
            }

            // Print off outdated depedencies
            if (outdatedDependencies.Any()) {
                this.Monitor.Log("Outdated versions of dependencies have been loaded:", LogLevel.Info);
                foreach (KeyValuePair<IManifest, HashSet<AssemblyName>> kv in outdatedDependencies) {
                    this.Monitor.Log($" - {kv.Key.Name}@{kv.Key.Version}", LogLevel.Info);
                    foreach (AssemblyName dependency in kv.Value) {
                        if (loadedAssemblyVersions.TryGetValue(dependency.Name, out Version loadedVersion)) {
                            this.Monitor.Log($"    - Depends on {dependency.Name}@{dependency.Version}, loaded version is {loadedVersion}", LogLevel.Info);
                        } else {
                            this.Monitor.Log($"    - Depends on {dependency.Name}@{dependency.Version}, no loaded version found", LogLevel.Info);
                        }
                    }
                }
            }
        }
    }
}
