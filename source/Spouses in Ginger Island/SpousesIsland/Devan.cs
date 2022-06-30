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
        /* Required for the little meow meow to load */
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
            if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCGiftTastes"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", ModEntry.ModHelper.Translation.Get("Devan.GiftTaste"));
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCDispositions"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", ModEntry.ModHelper.Translation.Get("Devan.Disposition"));
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail"))
            {
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", ModEntry.ModHelper.Translation.Get("Devan.Mail"));
                });
            }
        }

        /* Map stuff */
        internal static void PictureInRoom(IAssetData asset)
        {
            var editor = asset.AsMap();
            Map sourceMap = ModEntry.ModHelper.ModContent.Load<Map>("assets/Maps/z_Devan_post4h.tbin");
            editor.PatchMap(sourceMap, sourceArea: new Rectangle(0, 0, 2, 2), targetArea: new Rectangle(40, 1, 2, 2), PatchMapMode.ReplaceByLayer);
        }
        internal static void SVESaloon(IAssetData asset)
        {
            var editor = asset.AsMap();
            Map sourceMap = ModEntry.ModHelper.ModContent.Load<Map>("assets/Maps/z_DevanRoom_comp.tbin");
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
            Map sourceMap = ModEntry.ModHelper.ModContent.Load<Map>("assets/Maps/z_DevanRoom.tbin");
            editor.PatchMap(sourceMap, sourceArea: new Rectangle(0, 0, 11, 11), targetArea: new Rectangle(35, 0, 11, 11), PatchMapMode.ReplaceByLayer);
            Map map = editor.Data;
            map.Properties["NPCWarp"] = "14 25 Town 45 71";

            xTile.ObjectModel.PropertyValue doors;
            map.Properties.TryGetValue("Doors", out doors);
            map.Properties.Remove("Doors");
            map.Properties.Add("Doors", $"{doors} 39 8 1 120");
            /*
            Map PatchFix = ModEntry.ModHelper.ModContent.Load<Map>("assets/Maps/z_DevanRoom_FIX.tbin");
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
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Railroad"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("11037500/F/f Devan 500/p Devan", ModEntry.ModHelper.Translation.Get("Event.Railroad"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Forest"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("110371500/F/f Devan 1500/p Devan", ModEntry.ModHelper.Translation.Get("Event.Forest"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/LeahHouse"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("110371000/F/f Devan 1000/p Leah", ModEntry.ModHelper.Translation.Get("Event.LeahHouse"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Saloon"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("110372000/f Devan 2000/p Devan/t 2000 2600", ModEntry.ModHelper.Translation.Get("Event.Saloon"));

                    /* skippable/stopMusic/ */
                    data.Add("2000_goWithDevan", ModEntry.ModHelper.Translation.Get("Event.Saloon.A"));

                    //skippable/
                    data.Add("2000_staywithElliott", ModEntry.ModHelper.Translation.Get("Event.Saloon.B"));
                });
        }
        internal static void FesInternational(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/spring13"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", ModEntry.ModHelper.Translation.Get("Festivals.spring13"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/spring24"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", ModEntry.ModHelper.Translation.Get("Festivals.spring24"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/summer11"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", ModEntry.ModHelper.Translation.Get("Festivals.summer11"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/summer28"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", ModEntry.ModHelper.Translation.Get("Festivals.summer28"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/fall16"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", ModEntry.ModHelper.Translation.Get("Festivals.fall16"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/fall27"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", ModEntry.ModHelper.Translation.Get("Festivals.fall27"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/winter8"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", ModEntry.ModHelper.Translation.Get("Festivals.winter8"));
                });
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Festivals/winter25"))
                e.Edit(asset =>
                {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Devan", ModEntry.ModHelper.Translation.Get("Festivals.winter25"));
                });
        }

    }
}