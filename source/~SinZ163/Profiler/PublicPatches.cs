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
using StardewModdingAPI;
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
    internal record InternalArgumentDetailsEntry(DetailsType Type, string Name, int Index) : InternalDetailsEntry(Type, Name);
    internal enum DetailsType
    {
        Property,
        Field,
        Argument,
    }

    public static class PublicPatches
    {
        public static IProfilerAPI API { get; private set; }

        private static Dictionary<string, string> AssemblyMap = new();

        private static Dictionary<MethodBase, InternalDetailsEntry> DetailsMap = new();
        private static Dictionary<MethodBase, bool> ExceptionSupressMap = new();

        private static IMonitor Monitor;

        public static void Initialize(IProfilerAPI api, Harmony harmony, IMonitor monitor)
        {
            Monitor = monitor;
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
                if (detailsType == DetailsType.Argument && detailsEntry.Index != null)
                {
                    DetailsMap.TryAdd(method, new InternalArgumentDetailsEntry(detailsType, detailsEntry.Name, detailsEntry.Index.Value));
                }
                else
                {
                    DetailsMap.TryAdd(method, new(detailsType, detailsEntry.Name));
                }
            }
            if (detailsEntry.Supress.HasValue)
            {
                ExceptionSupressMap.TryAdd(method, detailsEntry.Supress.Value);
            }
        }

        public static string GetGenericDetails(MethodBase __originalMethod, object __instance, object[]? __args = null)
        {
            if (DetailsMap.TryGetValue(__originalMethod, out var details))
            {
                switch (details.Type)
                {
                    case DetailsType.Property:
                        return __originalMethod.DeclaringType.GetProperty(details.Name)?.GetValue(__instance)?.ToString();
                    case DetailsType.Field:
                        return __originalMethod.DeclaringType.GetField(details.Name)?.GetValue(__instance)?.ToString();
                    case DetailsType.Argument:
                        if (__args?.Length > 0 && details is InternalArgumentDetailsEntry argDetails && argDetails.Index < __args.Length)
                        {
                            return __args[argDetails.Index]?.ToString() ?? "<null>";
                        }
                        goto case default;
                    default:
                        return string.Empty;
                }
            }
            return string.Empty;
        }
        public static void GenericDurationPrefixWithArgs(out Stopwatch __state, MethodBase __originalMethod, object __instance, object[] __args)
        {
            __state = Stopwatch.StartNew();
            var assembly = Assembly.GetAssembly(__originalMethod.DeclaringType);
            if (!AssemblyMap.TryGetValue(assembly.GetName().Name, out var modId))
            {
                modId = assembly.GetName().Name;
                // TODO: Warn that assembly was not mapped
            }
            API.Push(new EventDurationMetadata(modId, String.Join(String.Empty, modId, '/', __originalMethod.DeclaringType.Name, '.', __originalMethod.Name), GetGenericDetails(__originalMethod, __instance, __args), -1, new()));
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
            API.Push(new EventDurationMetadata(modId, String.Join(String.Empty, modId, '/', __originalMethod.DeclaringType.Name, '.', __originalMethod.Name), GetGenericDetails(__originalMethod, __instance), -1, new()));
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

        public static Exception GenericTraceFinalizer(Exception __exception, MethodBase __originalMethod, object __instance)
        {
            // Finalizers run even when no exceptions happen, we only care about the bad case
            if (__exception == null) return null;
            try
            {
                if (ExceptionSupressMap.TryGetValue(__originalMethod, out var supress) && supress)
                {
                    return null;
                }
                var assembly = Assembly.GetAssembly(__originalMethod.DeclaringType);
                if (!AssemblyMap.TryGetValue(assembly.GetName().Name, out var modId))
                {
                    modId = assembly.GetName().Name;
                    // TODO: Warn that assembly was not mapped
                }
                API.Push(new EventTraceMetadata(modId, String.Join(String.Empty, modId, '/', __originalMethod.DeclaringType.Name, '.', __originalMethod.Name), GetGenericDetails(__originalMethod, __instance), __exception.GetType().Name, __exception.Message, new()), true);
                API.Pop();
                return __exception;
            } catch (Exception ex)
            {
                Monitor.Log($"Profiler itself errored out while trying to trace {__originalMethod.Name}, {ex.GetType().Name} {ex.Message}", LogLevel.Error);
                return __exception;
            }

        }

        internal static object GenericTraceFinalizerWithArgs(Exception __exception, MethodBase __originalMethod, object __instance, object[] __args)
        {
            // Finalizers run even when no exceptions happen, we only care about the bad case
            if (__exception == null) return null;
            try
            {
                // TODO: Make a seperate config for Supress Logs, or implement an equivilent to LogOnce
                if (ExceptionSupressMap.TryGetValue(__originalMethod, out var supress) && supress)
                {
                    return null;
                }
                var assembly = Assembly.GetAssembly(__originalMethod.DeclaringType);
                if (!AssemblyMap.TryGetValue(assembly.GetName().Name, out var modId))
                {
                    modId = assembly.GetName().Name;
                    // TODO: Warn that assembly was not mapped
                }
                API.Push(new EventTraceMetadata(modId, String.Join(String.Empty, modId, '/', __originalMethod.DeclaringType.Name, '.', __originalMethod.Name), GetGenericDetails(__originalMethod, __instance, __args), __exception.GetType().Name, __exception.Message, new()), true);
                API.Pop();
                return __exception;
            } catch (Exception ex)
            {
                Monitor.Log($"Profiler itself errored out while trying to trace {__originalMethod.Name}, {ex.GetType().Name} {ex.Message}", LogLevel.Error);
                return __exception;
            }
        }
    }
}
