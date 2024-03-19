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
using SpousesIsland.Additions;
using StardewModdingAPI.Events;
using xTile;
using static SpousesIsland.ModEntry;

namespace SpousesIsland.Events;

internal static class Asset
{
    private static readonly string[] Integrated = { "Abigail", "Alex", "Elliott", "Emily", "Haley", "Harvey", "Krobus", "Leah", "Maru", "Penny", "Sam", "Sebastian", "Shane", "Claire", "Lance", "Olivia", "Sophia", "Victor", "Wizard" };

    private static readonly List<string> MapNames = new() { "Island_FieldOffice", "Island_N", "Island_S", "Island_SE", "Island_SouthEastCave", "Island_W", "IslandEast", "IslandFarmCave", "IslandShrine", "IslandSouthEastCave_pirates", "IslandWestCave1" }; //"BusStop", 

    internal static void Requested(object sender, AssetRequestedEventArgs e)
    {
        //dialogue is added regardless of conditions
        if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/MarriageDialogueKrobus"))
        {
            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, string>().Data;
                data.Add("funLeave_Krobus", Help.Translation.Get("Krobus.GoOutside"));
            });
            return;
        }

        foreach (var character in Integrated)
        {
            if (!e.NameWithoutLocale.IsEquivalentTo($"Characters/Dialogue/{character}")) 
                continue;
            
            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, string>().Data;
                data["marriage_islandhouse"] = Translate($"Dialogue.{character}.arrival");
                data["marriage_loc1"] = Translate($"Dialogue.{character}.LocA");
                data["marriage_loc3"] = Translate($"Dialogue.{character}.LocB");
            });
            return;
        }
        
        if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Krobus"))
        {
            e.LoadFrom(
                () => new Dictionary<string, string>(),
                AssetLoadPriority.Low);
            return;
        }

        IslandMaps(e);
        
        if (ModInfo is null || ModInfo.LittleNpcs == false)
            return;
        
        if(e.NameWithoutLocale.IsEquivalentTo($"Characters/schedules/{Information.LittleNpcName(Children[0], "FirstLittleNPC")}"))
        {
            e.LoadFrom(
                () => new Dictionary<string, string>(),
                AssetLoadPriority.Low);
            return;
        }
        
        if(e.NameWithoutLocale.IsEquivalentTo($"Characters/schedules/{Information.LittleNpcName(Children[1], "SecondLittleNPC")}"))
        {
            e.LoadFrom(
                () => new Dictionary<string, string>(),
                AssetLoadPriority.Low);
        }
    }
    
    private static void IslandMaps(AssetRequestedEventArgs e)
    {
        Mon.LogOnce("Patching Island maps to allow NPC warping.");
        
        if (e.NameWithoutLocale.IsEquivalentTo("Maps/FishShop"))
        {
            e.Edit(asset =>
            {
                var editor = asset.AsMap();
                var map = editor.Data;
                CreateOrAddProperty(map, "NPCWarp", "4 3 IslandSouth 19 43");
            });
            return;
        }
        
        if (e.NameWithoutLocale.IsEquivalentTo("Maps/Beach"))
        {
            e.Edit(asset =>
            {
                var editor = asset.AsMap();
                var map = editor.Data;
                CreateOrAddProperty(map, "NPCWarp", "30 34 FishShop 5 9");
            });
            return;
        }
        
        if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandFarmHouse"))
        {
            e.Edit(asset =>
            {
                var editor = asset.AsMap();
                var map = editor.Data;
                CreateOrAddProperty(map, "NPCWarp", "14 18 IslandWest 77 40");
            });
            return;
        }
        
        var noPathName = e.NameWithoutLocale.BaseName.Remove(0, 5);
        if (MapNames.Contains(noPathName))
        {
            e.Edit(asset =>
            {
                var editor = asset.AsMap();
                var map = editor.Data;
                if (!map.Properties.TryGetValue("Warp", out var warpData))
                {
                    Mon.Log($"No warp data in {e.NameWithoutLocale}...NPCs won't warp there.", Level);
                    return;
                }
                
                if (noPathName.Equals("Island_W")) 
                    warpData += " 77 39 IslandFarmHouse 14 17";
                
                CreateOrAddProperty(map, "NPCWarp", warpData);

                if (!noPathName.Equals("Island_W")) 
                    return;
                
                var bridgeBarrier = map.GetLayer("Back").Tiles[62, 16];
                bridgeBarrier?.Properties.Remove("NPCBarrier");

                var a = map.GetLayer("Back").Tiles[60, 18];
                a?.Properties.Add("NPCBarrier", "T");

                var b = map.GetLayer("Back").Tiles[60, 17];
                b?.Properties.Add("NPCBarrier", "T");

                var c = map.GetLayer("Back").Tiles[60, 16];
                c?.Properties.Add("NPCBarrier", "T");

                var d = map.GetLayer("Back").Tiles[60, 15];
                d?.Properties.Add("NPCBarrier", "T");

                var tileE = map.GetLayer("Back").Tiles[60, 14];
                tileE?.Properties.Add("NPCBarrier", "T");

                var f = map.GetLayer("Back").Tiles[60, 13];
                f?.Properties.Add("NPCBarrier", "T");

                var g = map.GetLayer("Back").Tiles[60, 12];
                g?.Properties.Add("NPCBarrier", "T");
            });
        }
    }

    private static void CreateOrAddProperty(Map map, string which, string data)
    {
        if (!map.Properties.TryGetValue(which, out var propertyinfo))
            map.Properties.Add(which, data);
        else
        {
            var parsed = propertyinfo.ToString();
            map.Properties[which] = $"{parsed} {data}";
        }

        //log it just to be safe
        Mon.Log($"Updated {which} data.\nPrevious {which} = {propertyinfo}\nNew NPCWarp = {propertyinfo} {data}", Level);
    }
}
