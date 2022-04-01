/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.AssetLoaders;

#region using directives

using System;
using System.IO;
using StardewModdingAPI;
using StardewValley;

using Common.Extensions;

#endregion using directives

/// <summary>Loads custom mod textures.</summary>
public class TextureLoader : IAssetLoader
{
    /// <inheritdoc />
    public bool CanLoad<T>(IAssetInfo asset)
    {
        return asset.AssetName.Contains(ModEntry.Manifest.UniqueID) &&
               asset.AssetName.ContainsAnyOf("UltimateMeter", "SkillBars", "PrestigeRibbons", "MaxFishSizeIcon",
                   "HudPointer");
    }

    /// <inheritdoc />
    public T Load<T>(IAssetInfo asset)
    {
        var textureName = asset.AssetName.Split('/')[1];
        return textureName switch
        {
            "UltimateMeter" => ModEntry.ModHelper.Content.Load<T>(Path.Combine("assets", "hud",
                Context.IsWorldReady && ModEntry.ModHelper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP") &&
                !ModEntry.Config.DisableGaldoranTheme &&
                (Game1.currentLocation.NameOrUniqueName.IsAnyOf("Custom_CastleVillageOutpost", "Custom_CrimsonBadlands",
                    "Custom_IridiumQuarry", "Custom_TreasureCave") || ModEntry.Config.UseGaldoranThemeAllTimes)
                    ?
                    "gauge_galdora.png"
                    : ModEntry.Config.UseVintageInterface
                        ? "gauge_vintage.png"
                        : "gauge.png")),
            "SkillBars" => ModEntry.ModHelper.Content.Load<T>(Path.Combine("assets", "menus",
                ModEntry.Config.UseVintageInterface ? "skillbars_vintage.png" : "skillbars.png")),
            "PrestigeRibbons" => ModEntry.ModHelper.Content.Load<T>(Path.Combine("assets", "sprites", "ribbons.png")),
            "MaxFishSizeIcon" => ModEntry.ModHelper.Content.Load<T>(Path.Combine("assets", "menus", "max.png")),
            "HudPointer" => ModEntry.ModHelper.Content.Load<T>(Path.Combine("assets", "hud", "pointer.png")),
            _ => throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.")
        };
    }
}