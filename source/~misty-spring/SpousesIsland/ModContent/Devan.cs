/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
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