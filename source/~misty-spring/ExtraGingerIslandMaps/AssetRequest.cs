/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace ExtraGingerIslandMaps
{
    internal static class Asset
    {
        internal static void Request(object sender, AssetRequestedEventArgs e)
        {
            //ModEntry.Mon.Log("Requested " + e.NameWithoutLocale, StardewModdingAPI.LogLevel.Info);

            if (e.NameWithoutLocale.Name.Equals("Maps/Cloudy_Ocean_BG"))
            {
                e.LoadFrom(
                    () => ModEntry.CloudyBg ?? ModEntry.Help.GameContent.Load<Texture2D>("LooseSprites/Cloudy_Ocean_BG"),
                    AssetLoadPriority.Medium
                    );
            }

            if (e.NameWithoutLocale.Name.Equals("Maps/Cloudy_Ocean_BG_Night"))
            {
                e.LoadFrom(
                    () => ModEntry.CloudyBgNight ?? ModEntry.Help.GameContent.Load<Texture2D>("LooseSprites/Cloudy_Ocean_BG_Night"),
                    AssetLoadPriority.Medium
                    );
            }

            if (e.NameWithoutLocale.Name.Equals("Maps/Ostrich"))
            {
                e.LoadFrom(
                    () => ModEntry.Ostrich ?? ModEntry.Help.GameContent.Load<Texture2D>("Animals/Ostrich"),
                    AssetLoadPriority.Medium
                    );
            }
            
            if (e.NameWithoutLocale.Name.Equals("Maps/Cursors"))
            {
                e.LoadFrom(
                    () => ModEntry.Cursors ?? ModEntry.Help.GameContent.Load<Texture2D>("LooseSprites/Cursors"),
                    AssetLoadPriority.Medium
                );
            }

            if (ModEntry.HasSgi)
            {
                return;
            }

            ModEntry.Mon.LogOnce("Spouses in Ginger Island not found, patching Island maps to allow NPC warping.");

            if (!e.Name.StartsWith("Maps/Island", true, false)) return;
            
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_FieldOffice"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    map.Properties.Add("NPCWarp", "4 11 IslandNorth 46 46");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_N"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    map.Properties.Add("NPCWarp", "35 90 IslandSouth 17 0 36 90 IslandSouth 18 0 37 90 IslandSouth 19 0 43 90 IslandSouth 27 1 44 90 IslandSouth 27 1 46 45 IslandFieldOffice 4 10 39 20 VolcanoEntrance 1 1 40 20 VolcanoEntrance 1 1 41 20 VolcanoEntrance 1 1 42 20 VolcanoEntrance 1 1 12 30 VolcanoEntrance 6 48 21 45 IslandNorthCave1 6 11 22 45 IslandNorthCave1 6 11");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_S"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    map.Properties.Remove("NPCWarp");
                    map.Properties.Add("NPCWarp", "17 44 FishShop 3 4 36 11 IslandEast 0 45 36 12 IslandEast 0 46 36 13 IslandEast 0 47 -1 11 IslandWest 105 40 -1 10 IslandWest 105 40 -1 12 IslandWest 105 40 -1 13 IslandWest 105 40 17 -1 IslandNorth 35 89 18 -1 IslandNorth 36 89 19 -1 IslandNorth 37 89 27 -1 IslandNorth 43 89 28 -1 IslandNorth 43 89 43 28 IslandSouthEast 0 29 43 29 IslandSouthEast 0 29 43 30 IslandSouthEast 0 29");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_SE"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    map.Properties.Add("NPCWarp", "-1 28 IslandSouth 43 29 -1 29 IslandSouth 43 29 -1 30 IslandSouth 43 29 31 18 IslandSouthEastCave 1 8 31 19 IslandSouthEastCave 1 8");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_SouthEastCave"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    map.Properties.Add("NPCWarp", "0 7 IslandSouthEast 30 19 0 8 IslandSouthEast 30 19");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandSouthEastCave_pirates"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    ModEntry.Mon.VerboseLog("Editing IslandSouthEastCave_pirates...");
                    var map = editor.Data;
                    map.Properties.Add("NPCWarp", "0 7 IslandSouthEast 30 19 0 8 IslandSouthEast 30 19");
                });
            }
            //↑ all proofread
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandWestCave1"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    map.Properties.Add("NPCWarp", "6 12 IslandWest 61 5");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandEast"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    map.Properties.Add("NPCWarp", "-1 45 IslandSouth 35 11 -1 46 IslandSouth 35 12 -1 47 IslandSouth 35 13 -1 48 IslandSouth 35 13 22 9 IslandHut 7 13 34 30 IslandShrine 13 28");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandShrine"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    map.Properties.Add("NPCWarp", "12 27 IslandEast 33 30 12 28 IslandEast 33 30 12 29 IslandEast 33 30");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandFarmCave"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    map.Properties.Add("NPCWarp", "4 10 IslandSouth 97 35");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_W"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    /*  77 41 IslandFarmHouse 14 16 was taken out, because that's the coord NPCs warp to (from islandfarmhouse). */
                    map.Properties.Add("NPCWarp", "106 39 IslandSouth 0 10 106 40 IslandSouth 0 11 106 41 IslandSouth 0 12 106 42 IslandSouth 0 12 61 3 IslandWestCave1 6 11 96 32 IslandFarmCave 4 10 60 92 CaptainRoom 0 5 77 40 IslandFarmHouse 14 16");

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

                    var ee = map.GetLayer("Back").Tiles[60, 14];
                    ee?.Properties.Add("NPCBarrier", "T");

                    var f = map.GetLayer("Back").Tiles[60, 13];
                    f?.Properties.Add("NPCBarrier", "T");

                    var g = map.GetLayer("Back").Tiles[60, 12];
                    g?.Properties.Add("NPCBarrier", "T");
                });
            }
        }
    }
}