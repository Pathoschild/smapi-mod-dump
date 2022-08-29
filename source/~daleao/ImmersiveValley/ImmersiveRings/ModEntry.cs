/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Rings;

#region using directives

using Common;
using Common.Events;
using Common.Harmony;
using Common.Integrations.JsonAssets;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    public static int GarnetIndex { get; set; }
    public static int GarnetRingIndex { get; set; }

    internal static ModEntry Instance { get; private set; } = null!;
    internal static ModConfig Config { get; set; } = null!;
    internal static EventManager Events { get; private set; } = null!;
    internal static PerScreen<int> SavageExcitedness { get; } = new(() => 0);

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;

    internal static IJsonAssetsAPI? JsonAssetsApi { get; set; }
    internal static bool IsBetterRingsLoaded { get; private set; }
    internal static bool IsImmersiveProfessionsLoaded { get; private set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(Monitor);

        // check for Better Rings
        IsBetterRingsLoaded = helper.ModRegistry.IsLoaded("BBR.BetterRings");
        IsImmersiveProfessionsLoaded = helper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions");

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // enable events
        Events = new(helper.Events);

        // apply patches
        new Harmonizer(helper.ModRegistry, ModManifest.UniqueID).ApplyAll();
    }
}