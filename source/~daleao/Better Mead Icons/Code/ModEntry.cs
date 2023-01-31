/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

using System.Collections;
using StardewModdingAPI;

namespace DaLion.Meads;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    /// <summary>Construct an instance.</summary>
    public ModEntry()
    {
        try
        {
            // add a mead entry to BAGI's ContentSourceManager dictionary to prevent a likely KeyNotFoundException
            var artisanGoodToSourceTypeDict = (IDictionary)"BetterArtisanGoodIcons.Content.ContentSourceManager".ToType().RequireField("artisanGoodToSourceType").GetValue(null)!;
            artisanGoodToSourceTypeDict.Add(Globals.MeadAsArtisanGoodEnum, "Flowers");

            // patches must be applied in the constructor in order to take effect before BAGI loads its content packs
            HarmonyPatcher.Apply(new("DaLion.Meads"));
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
    }
}