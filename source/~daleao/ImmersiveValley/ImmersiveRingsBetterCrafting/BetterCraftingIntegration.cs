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

using DaLion.Common.Integrations;

#endregion using directives

internal class BetterCraftingIntegration : BaseIntegration<IBetterCraftingAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="log">Encapsulates monitoring and logging.</param>
    public BetterCraftingIntegration(IModRegistry modRegistry, Action<string, LogLevel> log)
        : base("Better Crafting", "leclair.bettercrafting", "1.0.0", modRegistry, log)
    {
        ModEntry.Api = ModApi;
    }

    /// <summary>Register the ring recipe provider.</summary>
    public void Register()
    {
        AssertLoaded();

        ModApi!.AddRecipeProvider(new RingRecipeProvider());
    }
}