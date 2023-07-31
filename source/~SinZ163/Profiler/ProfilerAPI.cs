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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Profiler
{
    public record EventMetadata(string ModId, string EventType, string Details, List<object> InnerDetails, string Type = "Base");
    internal record EventEntry(double At, EventMetadata Entry);
    public record EventDurationMetadata(string ModId, string EventType, string Details, double Duration, List<object> InnerDetails) : EventMetadata(ModId, EventType, Details, InnerDetails, "Duration");
    public record EventTraceMetadata(string ModId, string EventType, string Details, string ExceptionType, string ExceptionMessage, List<object> InnerDetails) : EventMetadata(ModId, EventType, Details, InnerDetails, "Trace");

    public interface IProfilerAPI
    {
        public void Push(EventMetadata eventDetails, bool important = false);
        public void Pop(Func<EventMetadata, EventMetadata> modifier);
        public void Pop();

        public IDisposable RecordSection(string ModId, string EventType, string Details);

        public MethodBase AddGenericDurationPatch(string type, string method, string detailsType = null);

        public HarmonyMethod GenericDurationPrefix { get; }
        public HarmonyMethod GenericDurationPrefixWithArgs { get; }
        public HarmonyMethod GenericDurationPostfix { get; }
        public HarmonyMethod GenericTraceFinalizer { get; }
        public HarmonyMethod GenericTraceFinalizerWithArgs { get; }
    }

    internal record ProfileLoggerRow(double OccuredAt, object Metadata);

    internal class RecordSectionBlock : IDisposable
    {
        private readonly IProfilerAPI api;
        private readonly Stopwatch timer;

        public RecordSectionBlock(IProfilerAPI api, EventMetadata eventMetadata)
        {
            this.api = api;
            this.timer = Stopwatch.StartNew();
            this.api.Push(eventMetadata);
        }

        public void Dispose()
        {
            timer.Stop();
            this.api.Pop(metadata =>
            {
                if (metadata is EventDurationMetadata durationMetadata)
                {
                    metadata = durationMetadata with { Duration = timer.Elapsed.TotalMilliseconds };
                }
                return metadata;
            });
        }
    }

    public class ProfilerAPI : IProfilerAPI
    {
        private ModConfig Config { get; }
        public Harmony Harmony { get; }
        public IMonitor Monitor { get; }
        public Stopwatch Timer { get; }

        internal ConcurrentStack<EventEntry> EventMetadata { get; private set; }
        internal bool EventHasImportance { get; private set; }

        internal ProfilerAPI(ModConfig config, Harmony harmony, Stopwatch timer, IMonitor monitor)
        {
            Config = config;
            Harmony = harmony;
            Monitor = monitor;
            Timer = timer;

            EventMetadata = new();
            EventHasImportance = false;
        }
        public HarmonyMethod GenericDurationPrefix => new HarmonyMethod(typeof(PublicPatches).GetMethod(nameof(PublicPatches.GenericDurationPrefix)));
        public HarmonyMethod GenericDurationPrefixWithArgs => new HarmonyMethod(typeof(PublicPatches).GetMethod(nameof(PublicPatches.GenericDurationPrefixWithArgs)));
        public HarmonyMethod GenericDurationPostfix => new HarmonyMethod(typeof(PublicPatches).GetMethod(nameof(PublicPatches.GenericDurationPostfix)));
        public HarmonyMethod GenericTraceFinalizer => new HarmonyMethod(typeof(PublicPatches).GetMethod(nameof(PublicPatches.GenericTraceFinalizer)));
        public HarmonyMethod GenericTraceFinalizerWithArgs => new HarmonyMethod(typeof(PublicPatches).GetMethod(nameof(PublicPatches.GenericTraceFinalizerWithArgs)));

        public void Write(EventMetadata eventDetails)
        {
            Write(Timer.Elapsed.TotalMilliseconds, eventDetails);
        }
        private void Write(double at, EventMetadata eventDetails)
        {
            // Finally back to the root event, log
            if (EventMetadata.IsEmpty)
            {
                if (eventDetails is EventDurationMetadata durationMetadata && durationMetadata.Duration < Config.LoggerDurationOuterThreshold)
                {
                    return;
                }
                Monitor.Log($"[RawLog] {JsonSerializer.Serialize(new ProfileLoggerRow(at, eventDetails))}", LogLevel.Trace);
                EventHasImportance = false;
            }
            else if (EventMetadata.TryPeek(out var outerMetadata))
            {
                if (!EventHasImportance && eventDetails is EventDurationMetadata durationMetadata && durationMetadata.Duration < Config.LoggerDurationInnerThreshold)
                {
                    return;
                }
                outerMetadata.Entry.InnerDetails.Add(new ProfileLoggerRow(at, eventDetails));
            }
        }

        public void Push(EventMetadata eventDetails, bool important = false)
        {
            EventMetadata.Push(new(Timer.Elapsed.TotalMilliseconds, eventDetails));
            if (important)
            {
                EventHasImportance = true;
            }
        }

        public void Pop(Func<EventMetadata, EventMetadata> modifier)
        {
            if (EventMetadata.TryPop(out var metadataPair))
            {
                var metadata = modifier(metadataPair.Entry);
                Write(metadataPair.At, metadata);
            }
        }
        public void Pop()
        {
            if (EventMetadata.TryPop(out var metadataPair))
            {
                Write(metadataPair.At, metadataPair.Entry);
            }
        }

        public IDisposable RecordSection(string ModId, string EventType, string Details)
        {
            var eventDetails = new EventDurationMetadata(ModId, EventType, Details, -1, new ());
            return new RecordSectionBlock(this, eventDetails);
        }

        public MethodBase AddGenericDurationPatch(string type, string method, string? detailType = null)
        {
            try
            {
                var typeInstance = Type.GetType(type);
                if (typeInstance == null)
                {
                    Monitor.Log($"Type {type} does not exist and therefore can't add duration monitoring.", LogLevel.Error);
                    return null;
                }
                var originalMethod = AccessTools.Method(typeInstance, method);
                if (originalMethod == null)
                {
                    Monitor.Log($"Method {method} on type {type} does not exist and therefore can't add duration monitoring.", LogLevel.Error);
                    return null;
                }
                HarmonyMethod prefix = detailType switch
                {
                    nameof(DetailsType.Argument) => new HarmonyMethod(typeof(PublicPatches), nameof(PublicPatches.GenericDurationPrefixWithArgs)),
                    _ => new HarmonyMethod(typeof(PublicPatches), nameof(PublicPatches.GenericDurationPrefix)),
                };
                Harmony.Patch(
                    original: originalMethod,
                    prefix,
                    postfix: new HarmonyMethod(typeof(PublicPatches), nameof(PublicPatches.GenericDurationPostfix))
                );
                return originalMethod;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Tried to add duration monitoring to {type}.{method} but {ex.GetType().Name} happened. ({ex.Message})", LogLevel.Error);
                return null;
            }
        }
        public MethodBase AddGenericTracePatch(string type, string method, string? detailType = null)
        {
            try
            {
                var typeInstance = Type.GetType(type);
                if (typeInstance == null)
                {
                    Monitor.Log($"Type {type} does not exist and therefore can't add duration monitoring.", LogLevel.Error);
                    return null;
                }
                var originalMethod = AccessTools.Method(typeInstance, method);
                if (originalMethod == null)
                {
                    Monitor.Log($"Type {type} does not exist and therefore can't add duration monitoring.", LogLevel.Error);
                    return null;
                }
                HarmonyMethod finalizer = detailType switch
                {
                    nameof(DetailsType.Argument) => new HarmonyMethod(typeof(PublicPatches), nameof(PublicPatches.GenericTraceFinalizerWithArgs)),
                    _ => new HarmonyMethod(typeof(PublicPatches), nameof(PublicPatches.GenericTraceFinalizer)),
                };
                Harmony.Patch(
                    original: originalMethod,
                    finalizer: finalizer
                );
                return originalMethod;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Tried to add trace monitoring to {type}.{method} but {ex.GetType().Name} happened. ({ex.Message})", LogLevel.Error);
                return null;
            }
        }
    }
}
