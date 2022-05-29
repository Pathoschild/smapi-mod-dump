/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Rings.BetterCrafting;

#region using directives

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static Action<string, LogLevel> Log { get; private set; }
    internal static IBetterCraftingAPI Api { get; set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        // store reference to logger
        Log = Monitor.Log;

        // hook events
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }

    /// <summary>The event called after the first game update, once all mods are loaded.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        new BetterCraftingIntegration(Helper.ModRegistry, Log).Register();
    }
}