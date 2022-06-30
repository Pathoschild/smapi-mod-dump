/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpousesIsland.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using xTile;
using xTile.Tiles;

namespace SpousesIsland
{
    internal class AssetRequest
    {
        /* Maps */
        internal static void Maps(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/FishShop"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    Map map = editor.Data;
                    map.Properties.Add("NPCWarp", "4 3 IslandSouth 19 43");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Custom_IslandShell"))
            {
                e.LoadFromModFile<Map>("assets/Maps/z_SpouseRoomShell.tbin", AssetLoadPriority.Medium);
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Custom_IslandShell_freelove"))
            {
                e.LoadFromModFile<Map>("assets/Maps/z_SpouseRoomShell_fl.tbin", AssetLoadPriority.Medium);
            }
        }
        internal static void IslandMaps(ModEntry modEntry, AssetRequestedEventArgs e, ModConfig Config)
        {
            if (e.Name.StartsWith("Maps/Island", true, false))
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandFarmHouse"))
                {
                    if (Config.CustomRoom == true && (Config.Allow_Children == false || modEntry.Children.Count is 0))
                    {
                        e.LoadFromModFile<Map>("assets/Maps/FarmHouse_Custom.tbin", AssetLoadPriority.Medium);
                    }
                    if (Config.Allow_Children == true && modEntry.Children.Count >= 1 && Config.ChildbedType is "mod")
                    {
                        e.LoadFromModFile<Map>($"assets/Maps/FarmHouse_kid_custom{Config.CustomRoom}.tbin", AssetLoadPriority.Medium);
                        e.Edit(asset =>
                        {
                            ModEntry.ModMonitor.Log("Patching child bed onto IslandFarmHouse...", LogLevel.Trace);
                            var editor = asset.AsMap();
                            Map sourceMap = ModEntry.ModHelper.ModContent.Load<Map>($"assets/Maps/kidbeds/z_kidbed_{Config.Childbedcolor}.tbin");
                            editor.PatchMap(sourceMap, sourceArea: new Rectangle(0, 0, 2, 4), targetArea: new Rectangle(35, 13, 2, 4), patchMode: PatchMapMode.Overlay);

                        });
                    }
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        Map map = editor.Data;
                        map.Properties.Add("NPCWarp", "14 17 IslandWest 77 41");
                    });
                }
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_FieldOffice"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        Map map = editor.Data;
                        map.Properties.Add("NPCWarp", "4 11 IslandNorth 46 46");
                    });
                }
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_N"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        Map map = editor.Data;
                        map.Properties.Add("NPCWarp", "22 22 IslandFarmHouse 14 16 35 89 IslandSouth 18 0 46 45 IslandFieldOffice 4 10 40 21 VolcanoEntrance 1 1 40 22 VolcanoEntrance 1 1");
                    });
                }
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_S"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        Map map = editor.Data;
                        map.Properties.Remove("NPCWarp");
                        map.Properties.Add("NPCWarp", "17 44 FishShop 3 4 36 11 IslandEast 0 45 36 12 IslandEast 0 46 36 13 IslandEast 0 47 -1 11 IslandWest 105 40 -1 10 IslandWest 105 40 -1 12 IslandWest 105 40 -1 13 IslandWest 105 40 17 -1 IslandNorth 35 89 18 -1 IslandNorth 36 89 19 -1 IslandNorth 37 89 27 -1 IslandNorth 43 89 28 -1 IslandNorth 43 89 43 28 IslandSouthEast 0 29 43 29 IslandSouthEast 0 29 43 30 IslandSouthEast 0 29");
                    });
                }
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_SE"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        Map map = editor.Data;
                        map.Properties.Add("NPCWarp", "0 29 IslandSouth 43 29 29 18 IslandSouthEastCave 1 8");
                    });
                }
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_SouthEastCave"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        Map map = editor.Data;
                        map.Properties.Add("NPCWarp", "0 7 IslandSouthEast 30 19");
                    });
                }
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandSouthEastCave_pirates"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        ModEntry.ModMonitor.VerboseLog("Editing IslandSouthEastCave_pirates...");
                        Map map = editor.Data;
                        map.Properties.Add("NPCWarp", "0 7 IslandSouthEast 30 19");
                    });
                }
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandWestCave1"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        Map map = editor.Data;
                        map.Properties.Add("NPCWarp", "6 12 IslandWest 61 5");
                    });
                }
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandEast"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        Map map = editor.Data;
                        map.Properties.Add("NPCWarp", "-1 45 IslandSouth 35 11 -1 46 IslandSouth 35 12 -1 47 IslandSouth 35 13 -1 48 IslandSouth 35 13 22 9 IslandHut 7 13 34 30 IslandShrine 13 28");
                    });
                }
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandShrine"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        Map map = editor.Data;
                        map.Properties.Add("NPCWarp", "12 27 IslandEast 33 30 12 28 IslandEast 33 30 12 29 IslandEast 33 30");
                    });
                }
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandFarmCave"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        Map map = editor.Data;
                        map.Properties.Add("NPCWarp", "4 10 IslandSouth 97 35");
                    });
                }
                if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_W"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsMap();
                        Map map = editor.Data;
                        /*  77 41 IslandFarmHouse 14 16 was taken out, because that's the coord NPCs warp to (from islandfarmhouse). */
                        map.Properties.Add("NPCWarp", "106 39 IslandSouth 0 10 106 40 IslandSouth 0 11 106 41 IslandSouth 0 12 106 42 IslandSouth 0 12 61 3 IslandWestCave1 6 11 96 32 IslandFarmCave 4 10 60 92 CaptainRoom 0 5 77 40 IslandFarmHouse 14 16");

                        Tile BridgeBarrier = map.GetLayer("Back").Tiles[62, 16];
                        if (BridgeBarrier is not null)
                            BridgeBarrier.Properties.Remove("NPCBarrier");

                        Tile a = map.GetLayer("Back").Tiles[60, 18];
                        if (a is not null)
                            a.Properties.Add("NPCBarrier", "T");

                        Tile b = map.GetLayer("Back").Tiles[60, 17];
                        if (b is not null)
                            b.Properties.Add("NPCBarrier", "T");

                        Tile c = map.GetLayer("Back").Tiles[60, 16];
                        if (c is not null)
                            c.Properties.Add("NPCBarrier", "T");

                        Tile d = map.GetLayer("Back").Tiles[60, 15];
                        if (d is not null)
                            d.Properties.Add("NPCBarrier", "T");

                        Tile e = map.GetLayer("Back").Tiles[60, 14];
                        if (e is not null)
                            e.Properties.Add("NPCBarrier", "T");

                        Tile f = map.GetLayer("Back").Tiles[60, 13];
                        if (f is not null)
                            f.Properties.Add("NPCBarrier", "T");

                        Tile g = map.GetLayer("Back").Tiles[60, 12];
                        if (g is not null)
                            g.Properties.Add("NPCBarrier", "T");
                    });
                }
            }
        }

        /* Schedules */
        internal static void ChangeSchedulesIntegrated(ModEntry modEntry, AssetRequestedEventArgs e, ModConfig Config)
        {
            /*this is set at the top so it doesn't overwrite krobus data by accident*/
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Krobus") && Config.Allow_Krobus == true)
            {
                e.LoadFromModFile<Dictionary<string, string>>("assets/Spouses/Empty.json", AssetLoadPriority.Low);
            }
            //integrated data
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Abigail") && Config.Allow_Abigail == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 16 9 0 \"Strings\\schedules\\Abigail:marriage_islandhouse\"/1100 IslandNorth 44 28 0 \"Strings\\schedules\\Abigail:marriage_loc1\"/{SGIValues.RandomMap_nPos("Abigail", modEntry.HasExGIM, Config.ScheduleRandom)}/2000 IslandWest 39 41 0 \"Strings\\schedules\\Abigail:marriage_loc3\"/a2200 IslandFarmHouse 16 9 0";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Alex") && Config.Allow_Alex == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 19 6 0 \"Strings\\schedules\\Alex:marriage_islandhouse\"/1100 IslandWest 85 39 2 alex_lift_weights \"Strings\\schedules\\Alex:marriage_loc1\"/{SGIValues.RandomMap_nPos("Alex", modEntry.HasExGIM, Config.ScheduleRandom)}/a1900 IslandSouth 12 27 2 \"Strings\\schedules\\Alex:marriage_loc3\"/a2200 IslandFarmHouse 19 6 0";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Elliott") && Config.Allow_Elliott == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 9 9 2 \"Strings\\schedules\\Elliott:marriage_islandhouse\"/1100 IslandWest 102 77 2 elliott_read/1400 IslandWest 73 83 2 \"Strings\\schedules\\Elliott:marriage_loc2\"/{SGIValues.RandomMap_nPos("Elliott", modEntry.HasExGIM, Config.ScheduleRandom)}/a2200 IslandFarmHouse 9 9 2";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Emily") && Config.Allow_Emily == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 12 10 \"Strings\\schedules\\Emily:marriage_islandhouse\"/1100 IslandWest 53 52 2 \"Strings\\schedules\\Emily:marriage_loc1\"/1400 IslandWest 89 79 2 emily_exercise/{SGIValues.RandomMap_nPos("Emily", modEntry.HasExGIM, Config.ScheduleRandom)}/a2200 IslandFarmHouse 12 10";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Haley") && Config.Allow_Haley == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 8 6 2 \"Strings\\schedules\\Haley:marriage_islandhouse\"/1100 IslandNorth 32 74 0 \"Strings\\schedules\\Haley:marriage_loc1\"/{SGIValues.RandomMap_nPos("Haley", modEntry.HasExGIM, Config.ScheduleRandom)}/1900 IslandWest 80 45 2/a2200 IslandFarmHouse 8 6 0";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Harvey") && Config.Allow_Harvey == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 16 13 0 \"Strings\\schedules\\Harvey:marriage_islandhouse\"/1100 IslandFarmHouse 3 5 0 \"Strings\\schedules\\Harvey:marriage_loc1\"/1400 IslandWest 89 75 2 harvey_excercise/{SGIValues.RandomMap_nPos("Harvey", modEntry.HasExGIM, Config.ScheduleRandom)}/a2100 IslandFarmHouse 16 13";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Krobus") && Config.Allow_Krobus == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 15 10 0 \"Characters\\Dialogue\\Krobus:marriage_islandhouse\"/1100 IslandFarmHouse 10 10 3 \"Characters\\Dialogue\\Krobus:marriage_loc1\"/1130 IslandFarmHouse 9 8 0/1200 IslandFarmHouse 9 11 2/1400 IslandWestCave1 8 8 0 \"Characters\\Dialogue\\Krobus:marriage_loc3\"/1500 IslandWestCave1 9 8 0/1600 IslandWestCave1 9 6 3/{SGIValues.RandomMap_nPos("Krobus", modEntry.HasExGIM, Config.ScheduleRandom)}/a2200 IslandFarmHouse 15 10 0";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Leah") && Config.Allow_Leah == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 21 13 1 \"Strings\\schedules\\Leah:marriage_islandhouse\"/1100 IslandNorth 50 25 0 leah_draw \"Strings\\schedules\\Leah:marriage_loc1\"/1400 IslandNorth 21 16 0/{SGIValues.RandomMap_nPos("Leah", modEntry.HasExGIM, Config.ScheduleRandom)}/a2200 IslandFarmHouse 21 13 1";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Maru") && Config.Allow_Maru == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 18 15 2 \"Strings\\schedules\\Maru:marriage_islandhouse\"/1100 IslandWest 95 45 2/1400 IslandNorth 50 25 0 \"Strings\\schedules\\Maru:marriage_loc1\"/{SGIValues.RandomMap_nPos("Maru", modEntry.HasExGIM, Config.ScheduleRandom)}/a2200 IslandFarmHouse 18 15 2";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                }
            );
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Penny") && Config.Allow_Penny == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 9 12 1 \"Strings\\schedules\\Penny:marriage_islandhouse\"/1100 IslandFarmHouse 3 6 0/1400 IslandWest 83 37 3 penny_read \"Strings\\schedules\\Penny:marriage_loc1\"/{SGIValues.RandomMap_nPos("Penny", modEntry.HasExGIM, Config.ScheduleRandom)}/a2200 IslandFarmHouse 9 12 1";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Sam") && Config.Allow_Sam == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 22 6 2 \"Strings\\schedules\\Sam:marriage_islandhouse\"/1100 IslandFarmHouse 8 9 0 sam_guitar/1400 IslandNorth 36 27 0 sam_skateboarding \"Strings\\schedules\\Sam:marriage_loc1\"/{SGIValues.RandomMap_nPos("Sam", modEntry.HasExGIM, Config.ScheduleRandom)}/a2200 IslandFarmHouse 22 6 2";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Sebastian") && Config.Allow_Sebastian == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 25 14 3 \"Strings\\schedules\\Sebastian:marriage_islandhouse\"/1100 IslandWest 88 14 0/1400 IslandWestCave1 6 4 0 \"Strings\\schedules\\Sebastian:marriage_loc1\"/{SGIValues.RandomMap_nPos("Sebastian", modEntry.HasExGIM, Config.ScheduleRandom)}/a2200 IslandFarmHouse 25 14 3";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Shane") && Config.Allow_Shane == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 20 5 0 \"Strings\\schedules\\Shane:marriage_islandhouse\"/1100 IslandWest 87 52 0 shane_charlie \"Strings\\schedules\\Shane:marriage_loc1\"/a1420 IslandWest 77 39 0/1430 IslandFarmHouse 15 9 0 shane_drink/{SGIValues.RandomMap_nPos("Shane", modEntry.HasExGIM, Config.ScheduleRandom)}/a2150 IslandWest 82 43/2200 IslandFarmHouse 20 5 0";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            //sve
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Claire") && Config.Allow_Claire == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 5 6 0 \"Characters\\Dialogue\\Claire:marriage_islandhouse\"/1100 IslandFarmHouse 17 12 2 Claire_Read \"Characters\\Dialogue\\Claire:marriage_loc1\"/1400 IslandEast 19 40 0 \"Characters\\Dialogue\\Claire:marriage_loc3\"/{SGIValues.RandomMap_nPos("Claire", modEntry.HasExGIM, Config.ScheduleRandom)}/a2150 IslandFarmHouse 5 6 0";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Lance") && Config.Allow_Lance == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 13 13 1 \"Characters\\Dialogue\\Lance:marriage_islandhouse\"/1100 IslandNorth 37 30 0/1400 Caldera 24 23 2 \"Characters\\Dialogue\\Lance:marriage_loc2\"/{SGIValues.RandomMap_nPos("Lance", modEntry.HasExGIM, Config.ScheduleRandom)}/a2150 IslandFarmHouse 13 13 1";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Wizard") && Config.Allow_Magnus == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 26 6 0 \"Characters\\Dialogue\\Wizard:marriage_islandhouse\"/1100 IslandWest 38 38 0 \"Characters\\Dialogue\\Wizard:marriage_loc1\"/1400 IslandSouthEast 28 26 2/{SGIValues.RandomMap_nPos("Magnus", modEntry.HasExGIM, Config.ScheduleRandom)}/a2150 IslandFarmHouse 26 6 0";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Olivia") && Config.Allow_Olivia == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 7 11 1\"Characters\\Dialogue\\Olivia:marriage_islandhouse\"/1100 IslandFarmHouse 14 9 0 Olivia_Wine1 \"Characters\\Dialogue\\Olivia:marriage_loc1\"/1400 IslandSouth 31 24 2 Olivia_Yoga/{SGIValues.RandomMap_nPos("Olivia", modEntry.HasExGIM, Config.ScheduleRandom)}/a2150 IslandFarmHouse 7 11 1";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Sophia") && Config.Allow_Sophia == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse 3 5 0 \"Characters\\Dialogue\\Sophia:marriage_islandhouse\"/1100 IslandWest 82 48 2/1400 IslandNorth 17 36 3 \"Characters\\Dialogue\\Sophia:marriage_loc2_scenery\"/{SGIValues.RandomMap_nPos("Sophia", modEntry.HasExGIM, Config.ScheduleRandom)}/a2150 IslandFarmHouse 3 5 2";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Victor") && Config.Allow_Victor == true)
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 FishShop 4 7 0/900 IslandSouth 1 11/940 IslandWest 77 43 0/1020 IslandFarmHouse 14 9 2 \"Characters\\Dialogue\\Victor:marriage_islandhouse\"/1100 IslandSouth 11 23 2 Victor_Wine2/1400 IslandFieldOffice 5 4 0 \"Characters\\Dialogue\\Victor:marriage_loc2\"/{SGIValues.RandomMap_nPos("Victor", modEntry.HasExGIM, Config.ScheduleRandom)}/a2150 IslandFarmHouse 14 9 2";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                });
        }
        internal static void ContentPackSchedule(ModEntry modEntry, AssetRequestedEventArgs e, ContentPackData cpd)
        {
            if (e.NameWithoutLocale.IsEquivalentTo($"Characters/schedules/{cpd.Spousename}"))
            {
                string temp_loc3 = Commands.IsLoc3Valid(cpd);
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_Mon"] = $"620 IslandFarmHouse {cpd.ArrivalPosition} \"Characters\\Dialogue\\{cpd.Spousename}:marriage_islandhouse\"/{cpd.Location1.Time} {cpd.Location1.Name} {cpd.Location1.Position} \"Characters\\Dialogue\\{cpd.Spousename}:marriage_loc1\"/{cpd.Location2.Time} {cpd.Location2.Name} {cpd.Location2.Position} \"Characters\\Dialogue\\{cpd.Spousename}:marriage_loc2\"/{temp_loc3}a2150 IslandFarmHouse {cpd.ArrivalPosition}";
                    data["marriage_Tue"] = "GOTO marriage_Mon";
                    data["marriage_Wed"] = "GOTO marriage_Mon";
                    data["marriage_Thu"] = "GOTO marriage_Mon";
                    data["marriage_Fri"] = "GOTO marriage_Mon";
                    data["marriage_Sat"] = "GOTO marriage_Mon";
                    data["marriage_Sun"] = "GOTO marriage_Mon";
                }
              );
                ModEntry.ModMonitor.LogOnce($"Edited the marriage schedule of {cpd.Spousename}.", LogLevel.Debug);
                if (!modEntry.SchedulesEdited.Contains(cpd.Spousename))
                {
                    modEntry.SchedulesEdited.Add(cpd.Spousename);
                }
            }
        }

        /* ContentPack dialogue */
        internal static void ContentPackDialogue(ModEntry modEntry, AssetRequestedEventArgs e, ContentPackData cpd)
        {
            /*first check which file its calling for:
            * If dialogue, check whether translations exist. (If they don't, just patch file. If they do, check the specific file being requested.)
            */
            if (cpd.Translations.Count is 0 || cpd.Translations is null)
            {
                ModEntry.ModMonitor.LogOnce($"No translations found in {cpd.Spousename} contentpack. Patching all dialogue files with default dialogue", LogLevel.Trace);
                e.Edit(asset => Commands.EditDialogue(cpd, asset, ModEntry.ModMonitor));
                ModEntry.ModMonitor.Log($"Added Ginger Island dialogue to {cpd.Spousename} data.", LogLevel.Debug);
                if (!modEntry.DialoguesEdited.Contains(cpd.Spousename))
                {
                    modEntry.DialoguesEdited.Add(cpd.Spousename);
                }
            }
            else
            {
                if (e.NameWithoutLocale.IsEquivalentTo($"Characters/Dialogue/{cpd.Spousename}"))
                {
                    e.Edit(asset => Commands.EditDialogue(cpd, asset, ModEntry.ModMonitor));
                    ModEntry.ModMonitor.Log($"Added Ginger Island dialogue to {cpd.Spousename} data.", LogLevel.Debug);
                    if (!modEntry.DialoguesEdited.Contains(cpd.Spousename))
                    {
                        modEntry.DialoguesEdited.Add(cpd.Spousename);
                    }
                }
                foreach (DialogueTranslation kpv in cpd.Translations)
                {
                    if (e.NameWithoutLocale.IsEquivalentTo($"Characters/schedules/{cpd?.Spousename}{Commands.ParseLangCode(kpv?.Key)}") && Commands.IsListValid(kpv) is true)
                    {
                        ModEntry.ModMonitor.LogOnce($"Found '{kpv.Key}' translation for {cpd.Spousename} dialogue!", LogLevel.Trace);
                        e.Edit(asset =>
                        {
                            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                            data["marriage_islandhouse"] = kpv.Arrival;
                            data["marriage_loc1"] = kpv.Location1;
                            data["marriage_loc2"] = kpv.Location2;
                            data["marriage_loc3"] = kpv.Location3?.ToString();
                        });
                        if (!modEntry.TranslationsAdded.Contains($"{cpd.Spousename} ({kpv.Key})"))
                        {
                            modEntry.TranslationsAdded.Add($"{cpd.Spousename} ({kpv.Key})");
                        }
                    }
                }
            }
        }

        /* Character sheets */
        internal static void CharacterSheetsByConfig(ModEntry modEntry, AssetRequestedEventArgs e, ModConfig Conf)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Harvey") && Conf.Allow_Harvey == true && Conf.CustomChance >= modEntry.RandomizedInt)
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsImage();
                    Texture2D Harvey = ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/Spouses/Harvey_anim.png");
                    editor.PatchImage(Harvey, new Rectangle(0, 192, 64, 32), new Rectangle(0, 192, 64, 32), PatchMode.Replace);
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Krobus") && Conf.Allow_Krobus == true && Conf.CustomChance >= modEntry.RandomizedInt)
            {
                e.LoadFromModFile<Texture2D>("assets/Spouses/Krobus_Outside_Character.png", AssetLoadPriority.Medium);
            }
        }
    }
}