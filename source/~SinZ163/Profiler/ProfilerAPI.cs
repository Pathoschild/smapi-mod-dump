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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Profiler
{
    public record EventMetadata(string ModId, string EventType, string Details, List<object> InnerDetails);
    public record EventDurationMetadata(string ModId, string EventType, string Details, double Duration, List<object> InnerDetails) : EventMetadata(ModId, EventType, Details, InnerDetails);

    public interface IProfilerAPI
    {
        public void Push(EventMetadata eventDetails);
        public void Pop(Func<EventMetadata, EventMetadata> modifier);

        public MethodBase AddGenericDurationPatch(string type, string method);
    }

    public class ProfilerAPI : IProfilerAPI
    {
        private ProfilerLogger Logger { get; }
        private ModConfig Config { get; }
        public Harmony Harmony { get; }
        public IMonitor Monitor { get; }

        internal ProfilerAPI(ProfilerLogger logger, ModConfig config, Harmony harmony, IMonitor monitor)
        {
            Logger = logger;
            Config = config;
            Harmony = harmony;
            Monitor = monitor;
        }

        public void Push(EventMetadata eventDetails)
        {
            Logger.EventMetadata.Push(eventDetails);
        }

        public void Pop(Func<EventMetadata, EventMetadata> modifier)
        {
            if (Logger.EventMetadata.TryPop(out var metadata))
            {
                metadata = modifier(metadata);
                // Finally back to the root event, log
                if (Logger.EventMetadata.IsEmpty)
                {
                    if (metadata is EventDurationMetadata durationMetadata)
                    {
                        if (durationMetadata.Duration > Config.LoggerDurationThreshold)
                        {
                            Logger.AddRow(new ProfileLoggerRow(DateTimeOffset.Now, metadata));
                        }
                    }
                }
                else
                {
                    if (Logger.EventMetadata.TryPeek(out var outerMetadata))
                    {
                        // AssetRequested as a nested event is *way* too noisy, removing all together
                        if (metadata.EventType != "Content.AssetRequested" && metadata.EventType != "Content.AssetReady" && metadata.EventType != "Content.AssetsInvalidated")
                        {
                            outerMetadata.InnerDetails.Add(metadata);
                        }
                    }
                }
            }
        }

        public MethodBase AddGenericDurationPatch(string type, string method)
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
                Harmony.Patch(
                    original: originalMethod,
                    prefix: new HarmonyMethod(typeof(PublicPatches), nameof(PublicPatches.GenericDurationPrefix)),
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
    }
}
