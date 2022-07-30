/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using PrismaticSlime.Framework;
using StardewModdingAPI.Events;

namespace PrismaticSlime;

/// <inheritdoc/>
internal sealed class ModEntry : Mod
{
    private static IJsonAssetsAPI? jsonAssets;

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

#pragma warning disable SA1201 // Elements should appear in the correct order - keeping fields near their accessors.
    private static IWearMoreRingsAPI? wearMoreRingsAPI;

    /// <summary>
    /// Gets the WearMoreRings API, if available.
    /// </summary>
    internal static IWearMoreRingsAPI? WearMoreRingsAPI => wearMoreRingsAPI;

    private static int prismaticSlimeEgg = -1;

    /// <summary>
    /// Gets the integer ID of the Prismatic Slime Egg. -1 if not found/not loaded yet.
    /// </summary>
    internal static int PrismaticSlimeEgg
    {
        get
        {
            if (prismaticSlimeEgg == -1)
            {
                prismaticSlimeEgg = jsonAssets?.GetObjectId("atravita.PrismaticSlime Egg") ?? -1;
            }
            return prismaticSlimeEgg;
        }
    }

    private static int prismaticSlimeRing = -1;

    /// <summary>
    /// Gets the integer ID of the Prismatic Slime Ring. -1 if not found/not loaded yet.
    /// </summary>
    internal static int PrismaticSlimeRing
    {
        get
        {
            if (prismaticSlimeRing == -1)
            {
                prismaticSlimeRing = jsonAssets?.GetObjectId("atravita.PrismaticSlimeRing") ?? -1;
            }
            return prismaticSlimeRing;
        }
    }
#pragma warning restore SA1201 // Elements should appear in the correct order

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        I18n.Init(helper.Translation);

        helper.Events.Content.AssetRequested += this.OnAssetRequested;
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        {
            IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Warn);
            if (!helper.TryGetAPI("spacechase0.JsonAssets", "1.10.3", out jsonAssets))
            {
                this.Monitor.Log("Packs could not be loaded! This mod will probably not function.", LogLevel.Error);
                return;
            }
            jsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets", "json-assets"), this.Helper.Translation);
        }

        {
            IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Trace);
            _ = helper.TryGetAPI("bcmpinc.WearMoreRings", "5.1.0", out wearMoreRingsAPI);
        }
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        => AssetManager.Apply(e);

    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            // handle patches from annotations.
            harmony.PatchAll();
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }

        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }
}
