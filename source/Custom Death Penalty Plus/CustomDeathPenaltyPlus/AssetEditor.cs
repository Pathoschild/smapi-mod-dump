/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomDeathPenaltyPlus
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;
using StardewValley;
using System;
using System.Text;
using System.Linq;



namespace CustomDeathPenaltyPlus
{
    /// <summary>
    /// Edits game assets
    /// </summary>
    internal class AssetEditor
    {
        private static ModConfig config;
        private static IManifest manifest;
       
        // Allows the class to access the ModConfig properties
        public static void SetConfig(ModConfig config, IManifest manifest)
        {
            AssetEditor.config = config;
            AssetEditor.manifest = manifest;

        }

        /// <summary>
        /// Builds a response string based on config values
        /// </summary>
        /// <param name="person">The person who found the player if they died in the mine, else Someone</param>
        /// <param name="location">Response based on where the player died</param>
        /// <returns>The built string</returns>
        static string ResponseBuilder(string person, string location)
        {
            // Create new string to build on
            string basestring = string.Format(i18n.string_replacementdialogue(), person, location);
            StringBuilder finalresponse = new StringBuilder(basestring);

            // Is WakeupNextDayinClinic true?
            if (config.OtherPenalties.WakeupNextDayinClinic == true)
            {
                // Yes, build string accordingly

                finalresponse.Insert(0,i18n.string_finallyawake());
            }

            // Is FriendshipPenalty greater than 0?
            if (config.OtherPenalties.HarveyFriendshipChange < 0)
            {
                // Yes, build string accordingly
                finalresponse.Append(i18n.string_bereallycareful());
            }
            else if (config.OtherPenalties.HarveyFriendshipChange > 0)
            {
                finalresponse.Append(i18n.string_nicetoseeyou());
            }
            else
            {
                finalresponse.Append(i18n.string_becareful());
            }

            // Return the built string
            return finalresponse.ToString();
        }

        /// <summary>
        /// Edits content in Strings/StringsFromCSFiles
        /// </summary>
        public class StringsFromCSFilesFixes 
        {
            private IModHelper modHelper;

            public StringsFromCSFilesFixes(IModHelper helper)
            {
                modHelper = helper;
            }
            public static void EditEvent(IAssetData asset)
            {
                var stringeditor = asset.AsDictionary<string, string>().Data;

                if (config.OtherPenalties.MoreRealisticWarps == true
                    && (ModEntry.location.StartsWith("Farm") == true
                    || Game1.getLocationFromName(ModEntry.location) as FarmHouse != null
                    || ModEntry.location.StartsWith("UndergroundMine") == true
                    || ModEntry.location == "SkullCave")
                    && ModEntry.location.StartsWith("IslandFarm") == false)
                {
                    if (config.DeathPenalty.MoneyLossCap == 0 || config.DeathPenalty.MoneytoRestorePercentage == 1)
                    {
                        // Yes, edit strings to show this special case
                        stringeditor["Event.cs.1068"] = i18n.string_nomoneylost();
                        stringeditor["Event.cs.1058"] = i18n.string_nomoneylostmine();
                    }
                    else
                    {
                        // No, edit strings to show amount lost
                        stringeditor["Event.cs.1068"] = i18n.string_moneylost();
                        stringeditor["Event.cs.1058"] = stringeditor["Event.cs.1058"].Replace("{0}", $"{(int)Math.Round(PlayerStateRestorer.statedeathps.Value.moneylost)}");
                    }

                    stringeditor["Event.cs.1070"] = i18n.string_nomoney();
                }
                else
                {
                    // Has player not lost any money?
                    if (config.DeathPenalty.MoneyLossCap == 0 || config.DeathPenalty.MoneytoRestorePercentage == 1)
                    {
                        // Yes, edit strings to show this special case
                        stringeditor["Event.cs.1068"] = i18n.string_nocharge();
                        stringeditor["Event.cs.1058"] = i18n.string_nomoneylostmine();
                    }
                    else
                    {
                        // No, edit strings to show amount lost
                        stringeditor["Event.cs.1068"] = stringeditor["Event.cs.1068"].Replace("{0}", $"{(int)Math.Round(PlayerStateRestorer.statedeathps.Value.moneylost)}");
                        stringeditor["Event.cs.1058"] = stringeditor["Event.cs.1058"].Replace("{0}", $"{(int)Math.Round(PlayerStateRestorer.statedeathps.Value.moneylost)}");
                    }
                }

                // Is RestoreItems true?
                if (config.DeathPenalty.RestoreItems == true)
                {
                    // Yes, Remove unnecessary strings
                    stringeditor["Event.cs.1060"] = "";
                    stringeditor["Event.cs.1061"] = "";
                    stringeditor["Event.cs.1062"] = "";
                    stringeditor["Event.cs.1063"] = "";
                    stringeditor["Event.cs.1071"] = "";
                }
            }
        }
        /// <summary>
        /// Edits content in Data/mail
        /// </summary>
        public class MailDataFixes 
        {
            private IModHelper modHelper;

            public MailDataFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            public static void EditEvent(IAssetData asset)
            {
                var maileditor = asset.AsDictionary<string, string>().Data;

                var data = Game1.player.modData;

                // Has player not lost any money?
                if (data[$"{manifest.UniqueID}.MoneyLostLastPassOut"] != null && data[$"{manifest.UniqueID}.MoneyLostLastPassOut"] == "0")
                {
                    // Yes, edit strings to show this special case
                    maileditor["passedOut1_Billed_Male"] = maileditor["passedOut1_Billed_Male"].Replace(i18n.string_replacementmail1(), i18n.string_mailnocharge());
                    maileditor["passedOut1_Billed_Female"] = maileditor["passedOut1_Billed_Female"].Replace(i18n.string_replacementmail1(), i18n.string_mailnocharge());
                    maileditor["passedOut3_Billed"] = maileditor["passedOut3_Billed"].Replace(i18n.string_replacementmail2(), i18n.string_mailnochargeharvey());
                }

                else if(data[$"{manifest.UniqueID}.MoneyLostLastPassOut"] != null && data[$"{manifest.UniqueID}.MoneyLostLastPassOut"] != "0")
                {
                    // No, edit strings to show amount lost
                    maileditor["passedOut1_Billed_Male"] = maileditor["passedOut1_Billed_Male"].Replace("{0}", $"{data[$"{manifest.UniqueID}.MoneyLostLastPassOut"]}");
                    maileditor["passedOut1_Billed_Female"] = maileditor["passedOut1_Billed_Female"].Replace("{0}", $"{data[$"{manifest.UniqueID}.MoneyLostLastPassOut"]}"); ;
                    maileditor["passedOut3_Billed"] = maileditor["passedOut3_Billed"].Replace("{0}", $"{data[$"{manifest.UniqueID}.MoneyLostLastPassOut"]}");
                }
                else
                {
                    maileditor["passedOut1_Billed_Male"] = maileditor["passedOut1_Billed_Male"].Replace("{0}", "some amount");
                    maileditor["passedOut1_Billed_Female"] = maileditor["passedOut1_Billed_Female"].Replace("{0}", "some amount"); ;
                    maileditor["passedOut3_Billed"] = maileditor["passedOut3_Billed"].Replace("{0}", "some amount");
                }
            }
        }

        /// <summary>
        /// Edits content in Data/Events/Mine
        /// </summary>
        public class MineEventFixes
        {
            private static IModHelper modHelper;

            public MineEventFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            public static void EditEvent(IAssetData asset, IModHelper modHelper)
            {
                var eventedits = asset.AsDictionary<string, string>().Data;

                IDictionary<string, string> events = modHelper.ModContent.Load<Dictionary<string, string>>("assets\\Events.json");
                // Is WakeupNextDayinClinic true?
                if (config.OtherPenalties.WakeupNextDayinClinic == true)
                {
                    eventedits["PlayerKilled"] = string.Format(events["CDPP.PlayerKilledMine"], i18n.string_wakeplayer(), i18n.string_easynow1(), ResponseBuilder("{0}", i18n.string_responseappendix2()));
                    //$"none/-100 -100/farmer 20 12 2 Harvey 21 12 3/changeLocation Hospital/pause 500/showFrame 5/message \" ...{Game1.player.Name}?\"/pause 1000/message \"Easy, now... take it slow.\"/viewport 20 12 true/pause 1000/{ResponseBuilder("{0}", "in the mine")}/showFrame 0/pause 1000/emote farmer 28/hospitaldeath/end";

                }
            }
        }

        /// <summary>
        /// Edits content in Data/Events/IslandSouth
        /// </summary>
        public class IslandSouthEventFixes
        {
            private static IModHelper modHelper;

            public IslandSouthEventFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            public static void EditEvent(IAssetData asset, IModHelper modHelper)
            {
                var eventedits = asset.AsDictionary<string, string>().Data;

                IDictionary<string, string> events = modHelper.ModContent.Load<Dictionary<string, string>>("assets\\Events.json");

                // Is WakeupNextDayinClinic true?
                if (config.OtherPenalties.WakeupNextDayinClinic == true)
                {
                    eventedits["PlayerKilled"] = string.Format(events["CDPP.PlayerKilledIsland"], i18n.string_wakeplayer(), i18n.string_easynow1(), ResponseBuilder("{0}", i18n.string_responseappendix3()));
                    //$"none/-100 -100/farmer 20 12 2 Harvey 21 12 3/changeLocation Hospital/pause 500/showFrame 5/message \" ...{Game1.player.Name}?\"/pause 1000/message \"Easy, now... take it slow.\"/viewport 20 12 true/pause 1000/{ResponseBuilder("{0}", "on the island shore")}/showFrame 0/pause 1000/emote farmer 28/hospitaldeath/end";
                }
            }
        }

        /// <summary>
        /// Edits content in Data/Events/Hospital
        /// </summary>
        public class HospitalEventFixes
        {
            private static IModHelper modHelper;

            public HospitalEventFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            public static void EditEvent(IAssetData asset, IModHelper modHelper)
            {
                var eventedits = asset.AsDictionary<string, string>().Data;

                IDictionary<string, string> events = modHelper.ModContent.Load<Dictionary<string, string>>("assets\\Events.json");

                eventedits["PlayerKilled"] = string.Format(events["CDPP.PlayerKilledHospital"], i18n.string_wakeplayer(), i18n.string_easynow1(), ResponseBuilder(i18n.string_responseperson(), i18n.string_responseappendix1()));
                //$"none/-100 -100/farmer 20 12 2 Harvey 21 12 3/pause 1500/showFrame 5/message \" ...{Game1.player.Name}?\"/pause 1000/message \"Easy, now... take it slow.\"/viewport 20 12 true/pause 1000/{ResponseBuilder("Someone", "and battered")}/showFrame 0/pause 1000/emote farmer 28/hospitaldeath/end";

                if (ModEntry.location != null
                    && (ModEntry.location.StartsWith("UndergroundMine") == true
                    || ModEntry.location == "SkullCave")
                    && config.OtherPenalties.MoreRealisticWarps == true)
                {
                    eventedits["PlayerKilled"] = string.Format(events["CDPP.PlayerKilledSkullCave"], i18n.string_wakeplayer(), i18n.string_easynow2(), i18n.string_qi());
                    //$"none/-100 -100/farmer 3 5 2 MrQi 4 4 2/changeLocation SkullCave/pause 1500/showFrame 5/message \" ...{Game1.player.Name}?\"/pause 1000/message \"Hey, kid! You okay?\"/viewport 3 5 true/pause 1000/speak MrQi \"I found you battered and unconscious down there, kid... I hope you weren't doing something stupid.$1#$b#Just be more careful in the caverns next time, okay. There's still lots of potential in you, kid!\"/showFrame 0/pause 1000/emote farmer 28/hospitaldeath/end";

                }
                else if (ModEntry.location != null
                    && (ModEntry.location.StartsWith("Farm")
                    || Game1.getLocationFromName(ModEntry.location) as FarmHouse != null)
                    && config.OtherPenalties.MoreRealisticWarps == true
                    && ModEntry.location.StartsWith("IslandFarm") == false)
                {
                    var cabin = (Context.IsMainPlayer ? "FarmHouse" : Game1.player.homeLocation.Value) ?? "FarmHouse";
                    // Get tile where player should spawn, same as (doorX, doorY - 2) position  
                    int tileX = 27;
                    int tileY = 28;
                    if (Game1.player.HouseUpgradeLevel == 0)
                    {
                        tileX = 3;
                        tileY = 9;
                    }

                    eventedits["PlayerKilled"] = string.Format(events["CDPP.PlayerKilledFarm"], tileX, tileY, cabin, i18n.string_whathappened(), i18n.string_somethingbad());
                    //$"none/-100 -100/farmer {tileX} {tileY} 2/changeLocation {cabin}/pause 1500/showFrame 5/message \"...\"/pause 1000/message \"...What just happened?\"/viewport {tileX} {tileY} true/pause 1000/showFrame 0/pause 1000/emote farmer 28/message \"Something bad must have happened to me... I have no idea how I got here...\"/pause 500/hospitaldeath/end";
                }
            }
        }
    }
}
