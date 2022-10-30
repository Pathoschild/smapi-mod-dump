/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI;
using System.Collections.Generic;
using xTile;
using Microsoft.Xna.Framework;
using System;

namespace SpousesIsland
{
    internal class Devan
    {
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
                    data.Add("Babysitter", "All");
                });
            }

            //make translatable
            var tl = ModEntry.TL; //set here for ease of changing in the future (if any)

            if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCGiftTastes"))
            {
                e.Edit(asset =>
                {
                    var lovedgift = tl.Get("devan_lovedgift");
                    var likedgift = tl.Get("devan_likedgift");
                    var dislikedgift = tl.Get("devan_dislikedgift");
                    var hatedgift = tl.Get("devan_hatedgift");
                    var neutralgift = tl.Get("devan_neutralgift");

                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", $"{lovedgift}/395 432 424 296/{likedgift}/399 410 403 240/{dislikedgift}/86 84 80 446/{hatedgift}/287 288 348 346 303 459 873/{neutralgift}/82 440 349 246/");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCDispositions"))
            {
                e.Edit(asset =>
                {
                    var GusTitle = tl.Get("gus_title");

                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", $"adult/polite/outgoing/neutral/undefined/not-datable/null/Town/fall 3/Gus '{GusTitle}' Leah '' Elliott ''/Saloon 44 5/Devan");
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail"))
            {
                e.Edit(asset =>
                {
                    var Mail = tl.Get("Devanmail") + "   ^   -Devan%item object 270 1 424 1 256 2 419 1 264 1 400 1 254 1 %%[#]" + tl.Get("Devanmail_title");
                    
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", Mail);
                });
            }
        }

        /* Map stuff */
        internal static void PictureInRoom(IAssetData asset)
        {
            var editor = asset.AsMap();
            Map sourceMap = ModEntry.Help.ModContent.Load<Map>("assets/Maps/z_Devan_post4h.tbin");
            editor.PatchMap(sourceMap, sourceArea: new Rectangle(0, 0, 2, 2), targetArea: new Rectangle(40, 1, 2, 2), PatchMapMode.ReplaceByLayer);
        }
        internal static void SVESaloon(IAssetData asset)
        {
            var editor = asset.AsMap();
            Map sourceMap = ModEntry.Help.ModContent.Load<Map>("assets/Maps/z_DevanRoom_comp.tbin");
            editor.PatchMap(sourceMap, sourceArea: new Rectangle(3, 0, 8, 11), targetArea: new Rectangle(38, 0, 8, 11), PatchMapMode.ReplaceByLayer);
            editor.PatchMap(sourceMap, sourceArea: new Rectangle(0, 8, 3, 2), targetArea: new Rectangle(35, 8, 3, 2), PatchMapMode.ReplaceByLayer);
            Map map = editor.Data;
            map.Properties["NPCWarp"] = "14 25 Town 45 71";

            xTile.ObjectModel.PropertyValue doors;
            map.Properties.TryGetValue("Doors", out doors);
            map.Properties.Remove("Doors");
            map.Properties.Add("Doors", $"{doors} 39 8 1 120");

        }
        internal static void VanillaSaloon(IAssetData asset)
        {
            var editor = asset.AsMap();
            Map sourceMap = ModEntry.Help.ModContent.Load<Map>("assets/Maps/z_DevanRoom.tbin");
            editor.PatchMap(sourceMap, sourceArea: new Rectangle(0, 0, 11, 11), targetArea: new Rectangle(35, 0, 11, 11), PatchMapMode.ReplaceByLayer);
            Map map = editor.Data;
            map.Properties["NPCWarp"] = "14 25 Town 45 71";

            xTile.ObjectModel.PropertyValue doors;
            map.Properties.TryGetValue("Doors", out doors);
            map.Properties.Remove("Doors");
            map.Properties.Add("Doors", $"{doors} 39 8 1 120");
            /*
            Map PatchFix = ModEntry.Help.ModContent.Load<Map>("assets/Maps/z_DevanRoom_FIX.tbin");
            editor.PatchMap(PatchFix, sourceArea: new Rectangle(0, 0, 4, 4), targetArea: new Rectangle(35, 7, 4, 4), patchMode: (PatchMapMode)PatchMode.Replace);*/
        }

        /* Add to festivals*/
        internal static void AppendFestivalData(AssetRequestedEventArgs e)
        {
            if (e.Name.StartsWith("Data/Festivals/spring", true, false))
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/spring13"))
                    e.Edit(asset =>
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out string spring13_setup);
                            data["Set-Up_additionalCharacters"] = spring13_setup + "/Devan 25 69 up";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 25 69 up";
                        }

                        if (data.ContainsKey("MainEvent_additionalCharacters"))
                        {
                            data.TryGetValue("MainEvent_additionalCharacters", out string spring13_main);
                            data["MainEvent_additionalCharacters"] = spring13_main + "/Devan 25 73 up";
                        }
                        else
                        {
                            data["MainEvent_additionalCharacters"] = "Devan 25 73 up";
                        }


                    });
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/spring24"))
                    e.Edit(asset =>
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out string spring24_setup);
                            data["Set-Up_additionalCharacters"] = spring24_setup + "/Devan 9 34 down";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 9 34 down";
                        }
                        if (data.ContainsKey("MainEvent_additionalCharacters"))
                        {
                            data.TryGetValue("MainEvent_additionalCharacters", out string spring24_main);
                            data["MainEvent_additionalCharacters"] = spring24_main + "/Devan 8 30 up";
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
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out string summer11_setup);
                            data["Set-Up_additionalCharacters"] = summer11_setup + "/Devan 13 9 down";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 13 9 down";
                        }
                        if (data.ContainsKey("MainEvent_additionalCharacters"))
                        {
                            data.TryGetValue("MainEvent_additionalCharacters", out string summer11_main);
                            data["MainEvent_additionalCharacters"] = summer11_main + "/Devan 30 14 right";
                        }
                        else
                        {
                            data["MainEvent_additionalCharacters"] = "Devan 30 14 right";
                        }
                    });
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/summer28"))
                    e.Edit(asset =>
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out string summer28_setup);
                            data["Set-Up_additionalCharacters"] = summer28_setup + "/Devan 11 18 left";
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
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out string fall16_setup);
                            data["Set-Up_additionalCharacters"] = fall16_setup + "/Devan 66 65 down";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 66 65 down";
                        }
                    });
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/fall27"))
                    e.Edit(asset =>
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out string fall27_setup);
                            data["Set-Up_additionalCharacters"] = fall27_setup + "/Devan 27 68 up";
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
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out string winter8_setup);
                            data["Set-Up_additionalCharacters"] = winter8_setup + "/Devan 66 14 right";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 66 14 right";
                        }
                        if (data.ContainsKey("MainEvent_additionalCharacters"))
                        {
                            string winter8_main;
                            data.TryGetValue("MainEvent_additionalCharacters", out winter8_main);
                            data["MainEvent_additionalCharacters"] = winter8_main + "/Devan 69 27 down";
                        }
                        else
                        {
                            data["MainEvent_additionalCharacters"] = "Devan 69 27 down";
                        }
                    });
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/winter25"))
                    e.Edit(asset =>
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                        if (data.ContainsKey("Set-Up_additionalCharacters"))
                        {
                            data.TryGetValue("Set-Up_additionalCharacters", out string winter25_setup);
                            data["Set-Up_additionalCharacters"] = winter25_setup + "/Devan 23 74 up";
                        }
                        else
                        {
                            data["Set-Up_additionalCharacters"] = "Devan 23 74 up";
                        }
                    });
            }
        }

        /* Add events */
        internal static void EventsInternational(AssetRequestedEventArgs e)
        {
            var tl = ModEntry.TL;
            
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Railroad"))
                e.Edit(asset =>
                {
                    var a1 = tl.Get("Event.Railroad_start-a1");
                    var a2 = tl.Get("Event.Railroad_start-a2");
                    var s1 = tl.Get("Event.Railroad_start-s1");
                    var s2 = tl.Get("Event.Railroad_start-s2");
                    var mid = tl.Get("Event.Railroad_talk");
                    var q1 = tl.Get("Event.Railroad_mid-a1");
                    var q2 = tl.Get("Event.Railroad_mid-a2");
                    var q3 = tl.Get("Event.Railroad_mid-a3");
                    var r1 = tl.Get("Event.Railroad_mid-s1");
                    var r2 = tl.Get("Event.Railroad_mid-s2");
                    var r3 = tl.Get("Event.Railroad_mid-s3");
                    var final = tl.Get("Event.Railroad_end");

                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("11037500/F/f Devan 500/p Devan", $"breezy/32 45/Devan 39 41 2 farmer 29 47 0/skippable/pause 500/move farmer 0 -6 0/move farmer 5 0 1 continue/viewport move 1 -1 3000/pause 500/emote Devan 16/pause 1000/quickQuestion @?#{a1}#{a2}#(break)speak Devan \"{s2}\"(break)speak Devan \"{s2}\"/speak Devan \"{mid}\"/quickQuestion #{q1}#{q2}#{q3}(break)speak Devan \"{r1}\"\\friendship Devan 20(break)speak Devan \"{r2}\"(break)speak Devan \"{r3}\"\\friendship Devan -50/pause 1000/emote Devan 40/speak Devan \"{final}\"/emote farmer 28/end");
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Forest"))
                e.Edit(asset =>
                {
                    var greet = tl.Get("Event.Forest_1");
                    var two = tl.Get("Event.Forest_2");
                    var three = tl.Get("Event.Forest_3");
                    var four = tl.Get("Event.Forest_4");
                    var five = tl.Get("Event.Forest_5");
                    var question = tl.Get("Event.Forest_q");
                    var a1 = tl.Get("Event.Forest_a1");
                    var a2 = tl.Get("Event.Forest_a2");
                    var a3 = tl.Get("Event.Forest_a3");
                    var s1 = tl.Get("Event.Forest_s1");
                    var s2 = tl.Get("Event.Forest_s2");
                    var s3 = tl.Get("Event.Forest_s3");
                    var end = tl.Get("Event.Forest_end");

                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("110371500/F/f Devan 1500/p Devan", $"continue/34 25/Devan 34 25 2 farmer 33 25 2/skippable/pause 4000/speak Devan \"{greet}\"/pause 2000/speak Devan \"{two}\"/stopMusic/pause 500/playMusic desolate/speak Devan \"{three}\"/pause 500/speak devan \"{four}\"/pause 2000/speak Devan \"{five}\"/stopMusic/pause 500/playMusic spring_night_ambient/pause 2000/quickQuestion {question}#{a1}#{a2}#{a3}(break)speak Devan \"{s1}\"\\friendship Devan 50(break)speak Devan \"{s2}\"(break)speak Devan \"{s3}\"\\friendship Devan -20(break)speak Devan \"{end}\"/end");
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/LeahHouse"))
                e.Edit(asset =>
                {
                    var one = tl.Get("Event.LeahHouse_1");
                    var two = tl.Get("Event.LeahHouse_2");
                    var three = tl.Get("Event.LeahHouse_3");
                    var four = tl.Get("Event.LeahHouse_4");
                    var five = tl.Get("Event.LeahHouse_5");
                    var six = tl.Get("Event.LeahHouse_6");
                    var seven = tl.Get("Event.LeahHouse_7");
                    var end = tl.Get("Event.LeahHouse_end");

                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("110371000/F/f Devan 1000/p Leah", $"jaunty/7 6/Leah 9 5 2 Devan 10 7 2 farmer 7 20 0/skippable/animate Leah false true 500 32 33 34/animate Devan false true 5000 20/pause 5000/speak Leah \"{one}\"/speak Devan \"{two}\"/speak Leah \"{three}\"/emote Devan 28/warp farmer 7 9/playSound doorClose/pause 500/speak Leah \"{four}\"/speak Devan \"{five}\"/speak Leah \"{six}\"/stopAnimation Leah 35/pause 500/animate Leah false false 500 8 9 10 11/playSound pickUpItem/pause 1000/stopAnimation Leah 0/addObject 8 5 99/pause 100/stopAnimation Devan 8/pause 600/speak Devan \"{seven}\"/speak Leah \"{end}\"/emote Devan 20/end");
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Saloon"))
                e.Edit(asset =>
                {
                    var base1 = tl.Get("Event.Saloon.base1");
                    var base2 = tl.Get("Event.Saloon.base2");
                    var base3 = tl.Get("Event.Saloon.base3");
                    var basechoice = tl.Get("Event.Saloon.baseChoice");


                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("110372000/f Devan 2000/p Devan/t 2000 2600", $"Saloon1/7 20/Devan 3 19 2 Elliott 2 20 1 Gus 10 18 2 Emily 15 17 0 Clint 19 23 3 Shane 21 17 2 Willy 17 22 2 farmer 4 21 0/skippable/showFrame Devan 36/pause 3000/speak Devan \"{base1}\"/pause 100/emote farmer 32/pause 500/speak Elliott \"{base2}\"/pause 500/speak Devan \"{base3}\"/question fork goWithDevan staywithElliott \"{basechoice}\"/fork goWithDevan 2000_goWithDevan/fork staywithElliott 2000_staywithElliott");

                    var a1 = tl.Get("Event.Saloon.A-1");
                    var a2 = tl.Get("Event.Saloon.A-2");
                    var a3 = tl.Get("Event.Saloon.A-3");
                    var a4 = tl.Get("Event.Saloon.A-4");
                    var aEnd = tl.Get("Event.Saloon.A-end");

                    /* skippable/stopMusic/ */
                    data.Add("2000_goWithDevan", $"changeLocation Town/viewport 45 72/globalFadeToClear 2000/pause 500/warp farmer 45 71 2/playSound doorClose/move farmer 0 1/move farmer -2 0/faceDirection farmer 2 continue/pause 1000/warp Devan 45 71/move Devan 0 1/playSound doorClose/pause 1500/speak Devan \"{a1}\"/pause 500/speak Devan \"{a2}\"/pause 1000/speak Devan \"{a3}\"/emote Devan 20/speak Devan \"{a4}\"/end dialogue Devan \"{aEnd}\"");

                    var b1 = tl.Get("Event.Saloon.B-1");
                    var b2 = tl.Get("Event.Saloon.B-2");
                    var b3 = tl.Get("Event.Saloon.B-3");
                    var b4 = tl.Get("Event.Saloon.B-4");
                    var b5 = tl.Get("Event.Saloon.B-5");
                    var b6 = tl.Get("Event.Saloon.B-6");
                    var b7 = tl.Get("Event.Saloon.B-7");
                    var bEnd = tl.Get("Event.Saloon.B-end");

                    //skippable/
                    data.Add("2000_staywithElliott", $"speak Devan \"{b1}\"/pause 500/move Devan 3 0/move Devan 0 4/move Devan 8 0/move Devan 0 1/playSound doorClose/warp Devan -20 -20/pause 500/stopMusic/speak Elliott \"{b2}\"/pause 1000/speak Elliott \"{b3}.\"/pause 500/emote Elliott 40/pause 500/speak Elliott \"{b4}\"/pause 300/speak Elliott \"{b5}\"/pause 800/speak Elliott \"{b6}\"/pause 200/playSound doorClose/warp Devan 14 24/move Devan 0 -2/speak Devan \"{b7}\"/pause 100/emote farmer 40/emote Elliott 40/pause 500/speak Elliott \"{bEnd}\"/emote Devan 20/pause 200/move Devan 0 -1/move Devan -5 0/end");
                });
        }
        /* Festival data */
        internal static void FesInternational(AssetRequestedEventArgs e)
        {
            var tl = ModEntry.TL;

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/spring13"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", tl.Get("Festivals.spring13"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/spring24"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", tl.Get("Festivals.spring24"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/summer11"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", tl.Get("Festivals.summer11"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/summer28"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", tl.Get("Festivals.summer28"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/fall16"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", tl.Get("Festivals.fall16"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/fall27"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", tl.Get("Festivals.fall27"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/winter8"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", tl.Get("Festivals.winter8"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/winter25"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", tl.Get("Festivals.winter25"));
                });
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