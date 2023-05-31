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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using xTile;

namespace SpousesIsland.ModContent
{
    internal static class Integrated
    {
        /* Maps */
        internal static void Maps(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/FishShop"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
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
        internal static void IslandMaps(AssetRequestedEventArgs e)
        {
            if (!e.Name.StartsWith("Maps/Island", true, false)) return;
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/IslandFarmHouse"))
            {

                if (ModEntry.Config.Allow_Children)
                {
                    ModEntry.Mon.Log("Patching child room onto IslandFarmHouse...");
                    e.LoadFromModFile<Map>("assets/Maps/Kidroom_addition.tbin", AssetLoadPriority.Medium);
                }

                if (ModEntry.Config.Allow_Children && ModEntry.Config.UseFurnitureBed == false)
                {
                    e.Edit(asset =>
                    {
                        ModEntry.Mon.Log("Patching child bed onto IslandFarmHouse...");
                        var editor = asset.AsMap();
                        var sourceMap = ModEntry.Help.ModContent.Load<Map>($"assets/Maps/kidbeds/z_kidbed_{ModEntry.Config.Childbedcolor}.tbin");
                        editor.PatchMap(sourceMap, sourceArea: new Rectangle(0, 0, 2, 4), targetArea: new Rectangle(35, 13, 2, 4), patchMode: PatchMapMode.Overlay);

                    });
                }

                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    map.Properties.Add("NPCWarp", "14 17 IslandWest 77 41");
                });
            }
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
                    //map.Properties.Add("NPCWarp", "22 22 IslandFarmHouse 14 16 35 89 IslandSouth 18 1 46 45 IslandFieldOffice 4 10 40 21 VolcanoEntrance 1 1 40 22 VolcanoEntrance 1 1");
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
                    //map.Properties.Add("NPCWarp", "17 44 FishShop 3 4 36 11 IslandEast 0 45 36 12 IslandEast 0 46 36 13 IslandEast 0 47 -1 11 IslandWest 105 40 -1 10 IslandWest 105 40 -1 12 IslandWest 105 40 -1 13 IslandWest 105 40 17 0 IslandNorth 35 89 18 0 IslandNorth 36 89 19 0 IslandNorth 37 89 27 0 IslandNorth 43 89 28 -1 IslandNorth 43 89 43 28 IslandSouthEast 0 29 43 29 IslandSouthEast 0 29 43 30 IslandSouthEast 0 29");
                    map.Properties.Add("NPCWarp", "17 44 FishShop 3 4 36 11 IslandEast 0 45 36 12 IslandEast 0 46 36 13 IslandEast 0 47 -1 11 IslandWest 105 40 -1 10 IslandWest 105 40 -1 12 IslandWest 105 40 -1 13 IslandWest 105 40 17 -1 IslandNorth 35 89 18 -1 IslandNorth 36 89 19 -1 IslandNorth 37 89 27 -1 IslandNorth 43 89 28 -1 IslandNorth 43 89 43 28 IslandSouthEast 0 29 43 29 IslandSouthEast 0 29 43 30 IslandSouthEast 0 29");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_SE"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    //map.Properties.Add("NPCWarp", "0 29 IslandSouth 43 29 29 18 IslandSouthEastCave 1 8");
                    map.Properties.Add("NPCWarp", "-1 28 IslandSouth 43 29 -1 29 IslandSouth 43 29 -1 30 IslandSouth 43 29 31 18 IslandSouthEastCave 1 8 31 19 IslandSouthEastCave 1 8");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_SouthEastCave"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    //map.Properties.Add("NPCWarp", "0 7 IslandSouthEast 30 19");
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
                    //map.Properties.Add("NPCWarp", "0 7 IslandSouthEast 30 19");
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

                    var tileE = map.GetLayer("Back").Tiles[60, 14];
                    tileE?.Properties.Add("NPCBarrier", "T");

                    var f = map.GetLayer("Back").Tiles[60, 13];
                    f?.Properties.Add("NPCBarrier", "T");

                    var g = map.GetLayer("Back").Tiles[60, 12];
                    g?.Properties.Add("NPCBarrier", "T");
                });
            }
        }

        internal static void Dialogues(AssetRequestedEventArgs e)
        {
            /*var targetspan =
        System.IO.Path.GetFileName(e.NameWithoutLocale.Name.ToCharArray());
            var targetwithoutpath = targetspan.ToString();
            */
            var tl = ModEntry.TL;

            //ALL translation keys are placeholders
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Abigail"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Abigail.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Abigail.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Abigail.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Alex"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Alex.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Alex.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Alex.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Elliott"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Elliott.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Elliott.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Elliott.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Emily"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Emily.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Emily.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Emily.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Haley"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Haley.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Haley.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Haley.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Harvey"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Harvey.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Harvey.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Harvey.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Krobus"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Krobus.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Krobus.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Krobus.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Leah"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Leah.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Leah.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Leah.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Maru"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Maru.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Maru.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Maru.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Penny"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Penny.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Penny.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Penny.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Sam"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Sam.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Sam.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Sam.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Sebastian"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Sebastian.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Sebastian.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Sebastian.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Shane"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Shane.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Shane.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Shane.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Claire"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Claire.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Claire.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Claire.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Lance"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Lance.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Lance.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Lance.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Wizard"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Wizard.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Wizard.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Wizard.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Olivia"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Olivia.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Olivia.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Olivia.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Sophia"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Sophia.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Sophia.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Sophia.LocB");
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Victor"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data["marriage_islandhouse"] = tl.Get("Dialogue.Victor.arrival");
                    data["marriage_loc1"] = tl.Get("Dialogue.Victor.LocA");
                    data["marriage_loc3"] = tl.Get("Dialogue.Victor.LocB");
                });
            }
        }
        internal static void KidSchedules(AssetRequestedEventArgs e)
        {
            //var targetspan = System.IO.Path.GetFileName(e.NameWithoutLocale.Name.ToCharArray());
            //var targetwithoutpath = targetspan.ToString();
            foreach (var kid in ModEntry.Children)
            {
                if(e.NameWithoutLocale.IsEquivalentTo($"Characters/schedules/{kid.Name}"))
                {
                    e.LoadFrom(
                        () => new Dictionary<string, string>(),
                        AssetLoadPriority.Low);
                }
            }
        }
        internal static Point GetPosition(string spouse)
        {
            var result = spouse switch
            {
                "Abigail" => new Point(16, 9),
                "Alex" => new Point(19, 6),
                "Elliott" => new Point(9, 9),
                "Emily" => new Point(12, 10),
                "Haley" => new Point(8, 6),
                "Harvey" => new Point(16, 13),
                "Krobus" => new Point(15, 10),
                "Leah" => new Point(21, 13),
                "Maru" => new Point(18, 15),
                "Penny" => new Point(9, 12),
                "Sam" => new Point(22, 6),
                "Sebastian" => new Point(25, 14),
                "Shane" => new Point(20, 5),
                "Claire" => new Point(5, 6),
                "Lance" => new Point(13, 13),
                "Magnus" => new Point(26, 6),
                "Olivia" => new Point(7, 11),
                "Sophia" => new Point(3, 5),
                "Victor" => new Point(14, 9),
                _ => new Point(0, 0) //GetCustomPosition(spouse)
            };

            return result;
        }

        internal static int GetFacing(string spouse)
        {
            var result = spouse switch
            {

                "Abigail" => 0,
                "Alex" => 0,
                "Elliott" => 2,
                "Emily" => 2,
                "Haley" => 0,
                "Harvey" => 0,
                "Krobus" => 0,
                "Leah" => 1,
                "Maru" => 2,
                "Penny" => 1,
                "Sam" => 2,
                "Sebastian" => 3,
                "Shane" => 0,
                "Claire" => 0,
                "Lance" => 1,
                "Magnus" => 0,
                "Olivia" => 1,
                "Sophia" => 2,
                "Victor" => 2,
                _ => 0 //GetCustomFacing(spouse)
            };

            return result;
        }
        /* These are here for brevity */
        internal static Texture2D KbcSamples() => ModEntry.Help.ModContent.Load<Texture2D>("assets/kbcSamples.png");
    }
}
