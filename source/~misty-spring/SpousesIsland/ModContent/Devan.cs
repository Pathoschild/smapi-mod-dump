/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SpousesIsland.ModContent
{
    internal static class Devan
    {
        /// <summary>
        /// Changes Devan's schedule depending on how many kids there are.
        /// </summary>
        /// <param name="who">Devan.</param>
        internal static void CorrectSchedule(string who = "Devan")
        {
            if (BedCode.HasCrib(Game1.player)) return; //no crib
            ModEntry.Mon.Log("no cribs found. reloading schedule...", LogLevel.Debug);

            var npc = Game1.getCharacterFromName(who);

            const string nocrib = "0 FarmHouse 6 6 0/630 FarmHouse 8 5 1/640 FarmHouse 17 6 1/700 FarmHouse 12 13 2/730 FarmHouse 9 14 0/830 FarmHouse 5 14 0 Devan_spoon/900 FarmHouse 5 14 2 Devan_bottle/1000 FarmHouse 8 14 0 Devan_washing/1100 FarmHouse 8 15 2/1150 FarmHouse 5 14 0 Devan_cook/1250 FarmHouse 5 14 2 Devan_plate/1400 FarmHouse 8 14 0 Devan_washing/1700 FarmHouse 11 16 0 Devan_broom/2130 FarmHouse 12 13 3/2200 FarmHouse 10 14 2/a2300 FarmHouse 12 19 2";

            //stop any npc action
            npc.Halt();
            npc.followSchedule = false;
            npc.clearSchedule();

            //set data
            var schedule = npc.parseMasterSchedule(nocrib);
            npc.Schedule = schedule;
            npc.followSchedule = true;
        }

        internal static void WalkTo(Character who)
        {
            var devan = Utility.fuzzyCharacterSearch("Devan", false);

            var position = who.Position.ToPoint();
            position.X++;

            devan.controller = new PathFindController(devan, devan.currentLocation, position, 0);
        }

        internal static void Wander(int maxDistance)
        {
            var devan = Utility.fuzzyCharacterSearch("Devan", false);

            var randomlocation = Point.Zero;
            Point difference;

            //can try at max. 10 times to fix position
            for (int i = 0; i < 10; i++)
            {
                randomlocation = new Point(Game1.random.Next(devan.currentLocation.Map.Layers[0].LayerWidth), Game1.random.Next(devan.currentLocation.Map.Layers[0].LayerHeight));

                difference = new Point(
                (int)(randomlocation.X - devan.Position.X),
                (int)(randomlocation.Y - devan.Position.Y));

                if (Math.Abs(difference.X) <= maxDistance && Math.Abs(difference.Y) <= maxDistance)
                {
                    break;
                }
            }
            
            devan.controller = new PathFindController(devan, devan.currentLocation, randomlocation, Game1.random.Next(0, 4));
        }

        /* Required for devan to load 
         * All content here has been made international (by adding TL keys)! */
        internal static void MainData(AssetRequestedEventArgs e)
        {
            //general
            if (e.NameWithoutLocale.IsEquivalentTo("Data/animationDescriptions"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan_washing", "18/16 16 16 17 17 17/18");
                    data.Add("Devan_plate", "23/20/23");
                    data.Add("Devan_cook", "18/21 21 21 21 22 22 22 22/18");
                    data.Add("Devan_bottle", "23/19/23");
                    data.Add("Devan_spoon", "18/24 24 24 24 25 25 25 25/18");
                    data.Add("Devan_broom", "0 23 23 31 31/26 26 26 27 27 27 28 28 28 29 29 29 28 28 28 27 27 27/31 31 23 23 0");
                    data.Add("Devan_sleep", "34 34 34 34 35 35 35/32/35 35 35 35 35 34 34 34");
                    data.Add("Devan_think", "34 34 34/33/34 34 34 34");
                    data.Add("Devan_sit", "0 0 38 38 38 37 37/36/37 37 38 38 38 0 0");
                    data.Add("shane_charlie", "29/28/29");
                    data.Add("harvey_excercise_island", "24/24 24 25 25 26 27 27 27 27 27 26 25 25 24 24 24/24");
                    data.Add("krobus_napping", "17/17/17");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCExclusions"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", "MovieInvite Socialize IslandEvent");
                });
            }
        }

        /* Add to festivals*/
        internal static void AppendFestivalData(AssetRequestedEventArgs e)
        {
            if (e.Name.StartsWith("Data/Festivals/spring", true, false))
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/spring13"))
                    e.Edit(asset =>
                    {
                        var data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out var spring13Setup);
                            data["Set-Up_additionalCharacters"] = spring13Setup + "/Devan 25 69 up";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 25 69 up";
                        }

                        if (data.ContainsKey("MainEvent_additionalCharacters"))
                        {
                            data.TryGetValue("MainEvent_additionalCharacters", out var spring13Main);
                            data["MainEvent_additionalCharacters"] = spring13Main + "/Devan 25 73 up";
                        }
                        else
                        {
                            data["MainEvent_additionalCharacters"] = "Devan 25 73 up";
                        }


                    });
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/spring24"))
                    e.Edit(asset =>
                    {
                        var data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out var spring24Setup);
                            data["Set-Up_additionalCharacters"] = spring24Setup + "/Devan 9 34 down";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 9 34 down";
                        }
                        if (data.ContainsKey("MainEvent_additionalCharacters"))
                        {
                            data.TryGetValue("MainEvent_additionalCharacters", out var spring24Main);
                            data["MainEvent_additionalCharacters"] = spring24Main + "/Devan 8 30 up";
                        }
                        else
                        {
                            data["MainEvent_additionalCharacters"] = "Devan 8 30 up";
                        }
                    });
            }
            if (e.Name.StartsWith("Data/Festivals/summer", true, false))
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/summer11"))
                    e.Edit(asset =>
                    {
                        var data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out var summer11Setup);
                            data["Set-Up_additionalCharacters"] = summer11Setup + "/Devan 13 9 down";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 13 9 down";
                        }
                        if (data.ContainsKey("MainEvent_additionalCharacters"))
                        {
                            data.TryGetValue("MainEvent_additionalCharacters", out var summer11Main);
                            data["MainEvent_additionalCharacters"] = summer11Main + "/Devan 30 14 right";
                        }
                        else
                        {
                            data["MainEvent_additionalCharacters"] = "Devan 30 14 right";
                        }
                    });
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/summer28"))
                    e.Edit(asset =>
                    {
                        var data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out var summer28Setup);
                            data["Set-Up_additionalCharacters"] = summer28Setup + "/Devan 11 18 left";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 11 18 left";
                        }
                    });
            }
            if (e.Name.StartsWith("Data/Festivals/fall", true, false))
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/fall16"))
                    e.Edit(asset =>
                    {
                        var data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out var fall16Setup);
                            data["Set-Up_additionalCharacters"] = fall16Setup + "/Devan 66 65 down";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 66 65 down";
                        }
                    });
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/fall27"))
                    e.Edit(asset =>
                    {
                        var data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out var fall27Setup);
                            data["Set-Up_additionalCharacters"] = fall27Setup + "/Devan 27 68 up";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 27 68 up";
                        }
                    });
            }
            if (e.Name.StartsWith("Data/Festivals/winter", true, false))
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/winter8"))
                    e.Edit(asset =>
                    {
                        var data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out var winter8Setup);
                            data["Set-Up_additionalCharacters"] = winter8Setup + "/Devan 66 14 right";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 66 14 right";
                        }
                        if (data.ContainsKey("MainEvent_additionalCharacters"))
                        {
                            data.TryGetValue("MainEvent_additionalCharacters", out string winter8Main);
                            data["MainEvent_additionalCharacters"] = winter8Main + "/Devan 69 27 down";
                        }
                        else
                        {
                            data["MainEvent_additionalCharacters"] = "Devan 69 27 down";
                        }
                    });
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/winter25"))
                    e.Edit(asset =>
                    {
                        var data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out var winter25Setup);
                            data["Set-Up_additionalCharacters"] = winter25Setup + "/Devan 23 74 up";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 23 74 up";
                        }
                    });
            }
        }
        /*
         * not doing this since it'll be a pain in the ass for translators AND me.
         * however im leaving the idea here just in case
         * 
        internal static Dictionary<string, string> GetTranslatedDialogue()
        {
            Dictionary<string, string> result = new Dictionary<string, string>()
            {
                { "intro", tl.Get("") },
                { "Mon", tl.Get("") },
                {"","" },
                {"","" },

                // (...) and so on would've been for all 55 dialogues

                {"","" }
            };

            return result;
        }*/
    }
}