/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

#nullable enable
using System.Collections;
using HarmonyLib;
using StardewModdingAPI;

namespace BetterArtisanGoodIconsForMeads;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    public static string ManifestUniqueID => "DaLion.Meads";

    /// <summary>Construct an instance.</summary>
    public ModEntry()
    {
        // add mead entry to BAGI's ContentSourceManager dictionary
        // this will fix a likely KeyNotFoundException
        var artisanGoodToSourceTypeDict = (IDictionary)"BetterArtisanGoodIcons.Content.ContentSourceManager".ToType()
            .RequireField("artisanGoodToSourceType").GetValue(null)!;
        artisanGoodToSourceTypeDict.Add(Globals.MeadAsArtisanGoodEnum, "Flowers");

        // apply patches
        var harmony = new Harmony(ManifestUniqueID);
        HarmonyPatcher.Apply(harmony);
    }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
    }
}