/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Profiler
{
    internal record InternalDetailsEntry(DetailsType Type, string Name);
    internal enum DetailsType
    {
        Property,
        Field
    }

    public static class PublicPatches
    {
        public static IProfilerAPI API { get; private set; }

        private static Dictionary<string, string> AssemblyMap = new();

        private static Dictionary<MethodBase, InternalDetailsEntry> DetailsMap = new();

        public static void Initialize(IProfilerAPI api, Harmony harmony)
        {
            API = api;
        }

        public static void AddAssemblyMap(string assemblyName, string modId)
        {
            AssemblyMap.TryAdd(assemblyName, modId);
        }
        public static void AddDetailsEntry(MethodBase method, ProfilerContentPackDetailEntry detailsEntry)
        {
            if (Enum.TryParse<DetailsType>(detailsEntry.Type, out var detailsType))
            {
                DetailsMap.Add(method, new(detailsType, detailsEntry.Name));
            }
        }

        public static string GetGenericDurationDetails(MethodBase __originalMethod, object __instance)
        {
            if (DetailsMap.TryGetValue(__originalMethod, out var details))
            {
                switch (details.Type)
                {
                    case DetailsType.Property:
                        return __originalMethod.DeclaringType.GetProperty(details.Name)?.GetValue(__instance)?.ToString();
                    case DetailsType.Field:
                        return __originalMethod.DeclaringType.GetField(details.Name)?.GetValue(__instance)?.ToString();
                    default:
                        return string.Empty;
                }
            }
            return string.Empty;
        }

        public static void GenericDurationPrefix(out Stopwatch __state, MethodBase __originalMethod, object __instance)
        {
            __state = Stopwatch.StartNew();
            var assembly = Assembly.GetAssembly(__originalMethod.DeclaringType);
            if (!AssemblyMap.TryGetValue(assembly.GetName().Name, out var modId))
            {
                modId = assembly.GetName().Name;
                // TODO: Warn that assembly was not mapped
            }
            API.Push(new EventDurationMetadata(modId, String.Join(String.Empty, modId, '/', __originalMethod.DeclaringType.Name, '.', __originalMethod.Name), GetGenericDurationDetails(__originalMethod, __instance), -1, new()));
        }
        public static void GenericDurationPostfix(Stopwatch __state)
        {
            __state.Stop();
            API.Pop(metadata =>
            {
                if (metadata is EventDurationMetadata durationMetadata)
                {
                    metadata = durationMetadata with { Duration = __state.Elapsed.TotalMilliseconds };
                }
                return metadata;
            });
        }
    }
}
