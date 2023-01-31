/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Collections.Immutable;

#if DEBUG
using System.Diagnostics;
#endif

using System.Reflection;

using AtraCore.Framework.Harmonizer.HarmonyAttributes;

using AtraShared.Utils.Extensions;

using CommunityToolkit.Diagnostics;

using HarmonyLib;

namespace AtraCore.Framework.Harmonizer;
public sealed class Harmonizer
{
    private readonly IReadOnlyCollection<string>? excludedCategories;
    private readonly IMonitor logger;
    private readonly IModRegistry registry;

    private readonly Dictionary<string, Harmony> cache = new();

    private readonly Dictionary<HarmonyPatchType, (int, int)> count = new();

    public Harmonizer(IMonitor logger, IModRegistry registry, string uniqueID)
        : this(logger, registry, uniqueID, null) { }

    public Harmonizer(IMonitor logger, IModRegistry registry, string uniqueID, IEnumerable<string>? excludedCategories)
    {
        Guard.IsNotNull(logger);
        Guard.IsNotNull(registry);
        Guard.IsNotNull(uniqueID);

        this.logger = logger;
        this.registry = registry;
        this.UniqueID = uniqueID;

        this.excludedCategories = excludedCategories?.ToImmutableArray();
    }

    public string UniqueID { get; init; }

    public void PatchAll(Assembly assembly)
    {
#if DEBUG
        Stopwatch sw = Stopwatch.StartNew();
#endif
        foreach (Type type in assembly.GetTypes())
        {
            if (type.GetCustomAttribute<HarmonyPatch>() is not HarmonyPatch patch)
            {
                continue;
            }

            if (type.GetCustomAttribute<RequireModAttribute>() is RequireModAttribute required
                && !this.CheckHasModAttribute(required))
            {
                this.logger.DebugOnlyLog($"Skipping {type.FullName} - missing required mod {required.UniqueID}");
                continue;
            }

            if (type.GetCustomAttribute<NotWithModAttribute>() is NotWithModAttribute notWith
                && !this.CheckWithoutModAttribute(notWith))
            {
                this.logger.DebugOnlyLog($"Skipping {type.FullName} - patch skipped when mod {notWith.UniqueID} is installed");
                continue;
            }

            if (type.GetCustomAttribute<GameVersionAttribute>() is GameVersionAttribute gameVersion
                && !this.CheckGameVersionAttribute(gameVersion))
            {
                this.logger.DebugOnlyLog($"Skipping {type.FullName} - patch skipped for Game version {Game1.version}");
                continue;
            }

            // roughly three bins here.
            if (type.GetMethod("TargetMethod", AccessTools.all) is MethodInfo targetMethodGetter)
            {
            }
            else if (type.GetMethod("TargetMethods", AccessTools.all) is MethodInfo targetMethodsGetter)
            {
            }
            else if (patch.info.declaringType is Type typeToPatch)
            {

            }
        }

#if DEBUG
        sw.Stop();
        this.logger.Log($"Took {sw.ElapsedMilliseconds} to apply harmony patches");
#endif
    }

    #region helpers

    private bool CheckHasModAttribute(RequireModAttribute attr)
    {
        if (this.registry.Get(attr.UniqueID) is not IModInfo info)
        {
            return false;
        }

        return (attr.MinVersion is null || !info.Manifest.Version.IsOlderThan(attr.MinVersion))
               && (attr.MaxVersion is null || !info.Manifest.Version.IsNewerThan(attr.MaxVersion));
    }

    private bool CheckWithoutModAttribute(NotWithModAttribute attr)
    {
        if (this.registry.Get(attr.UniqueID) is not IModInfo info)
        {
            return true;
        }

        return (attr.MinVersion is not null && info.Manifest.Version.IsOlderThan(attr.MinVersion))
               || (attr.MaxVersion is not null && info.Manifest.Version.IsNewerThan(attr.MaxVersion));
    }

    private bool CheckGameVersionAttribute(GameVersionAttribute attr)
    {
        SemanticVersion? gameVersion = new(Game1.version);
        return (attr.MinVersion is null || !gameVersion.IsOlderThan(attr.MinVersion))
               && (attr.MaxVersion is null || !gameVersion.IsNewerThan(attr.MaxVersion));
    }
    #endregion
}
