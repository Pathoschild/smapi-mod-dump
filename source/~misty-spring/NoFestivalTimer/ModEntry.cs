/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using NoFestivalTimer.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace NoFestivalTimer;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
        helper.Events.Content.AssetRequested += this.AssetRequested;
        helper.Events.Content.AssetsInvalidated += this.AssetInvalidated;
        
        Mon = this.Monitor;
        Help = this.Helper;

        var harmony = new Harmony(this.ModManifest.UniqueID);

        EventPatches.Apply(harmony);
    }

    private void SaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        Exclusions =Helper.GameContent.Load<Dictionary<string, ExclusionData>>($"Mods/{Help.ModRegistry.ModID}/Exclusions");
    }

    private void AssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo($"Mods/{Help.ModRegistry.ModID}/Exclusions", true))
        {
            e.LoadFrom(
                () => new Dictionary<string, ExclusionData>()
                {
                    {"iceFishing", new(true,5,false)},
                    {"eggHunt", new(true,0,true)}
                },
                AssetLoadPriority.Low
            );
        }
    }
    
    private void AssetInvalidated(object sender, AssetsInvalidatedEventArgs e)
    {
        if (!e.NamesWithoutLocale.Any(a => a.Name.Equals($"Mods/{Help.ModRegistry.ModID}/Exclusions"))) 
            return;
        
        Exclusions =Helper.GameContent.Load<Dictionary<string, ExclusionData>>($"Mods/{Help.ModRegistry.ModID}/Exclusions");
    }

    public static Dictionary<string, ExclusionData> Exclusions { get; set; } = new();

    internal static IModHelper Help { get; set; }
    internal static IMonitor Mon { get; set; }
}