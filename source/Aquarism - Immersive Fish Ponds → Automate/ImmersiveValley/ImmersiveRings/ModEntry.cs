/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Rings;

#region using directives

using Common;
using Common.Events;
using Common.Harmony;
using StardewModdingAPI;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal static ModConfig Config { get; set; } = null!;

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;

    internal static bool HasLoadedBetterRings { get; private set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(Monitor);

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // hook events
        new EventManager(helper.Events).HookAll();

        // apply patches
        new Harmonizer(ModManifest.UniqueID).ApplyAll();

        // check for Better Rings mod
        HasLoadedBetterRings = helper.ModRegistry.IsLoaded("BBR.BetterRings");
    }
}