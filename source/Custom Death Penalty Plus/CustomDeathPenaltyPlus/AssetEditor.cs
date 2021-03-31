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
using StardewValley;
using System;
using System.Text;

namespace CustomDeathPenaltyPlus
{
    /// <summary>
    /// Edits game assets
    /// </summary>
    internal class AssetEditor
    {
        private static ModConfig config;

        // Allows the class to access the ModConfig properties
        public static void SetConfig(ModConfig config)
        {
            AssetEditor.config = config;
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
            StringBuilder response = new StringBuilder($"speak Harvey \"{person} found you unconscious {location}... I had to perform an emergency surgery on you!#$b#Be a little more careful next time, okay?$s\"");

            // Is WakeupNextDayinClinic true?
            if (config.OtherPenalties.WakeupNextDayinClinic == true)
            {
                // Yes, build string accordingly

                response.Insert(14, "Good you're finally awake. ");
            }

            // Is FriendshipPenalty greater than 0?
            if (config.OtherPenalties.HarveyFriendshipChange < 0)
            {
                // Yes, build string accordingly

                response.Replace("Be a little more careful next time, okay?", "You really need to be more careful, I don't like having to patch you up after you do something dangerous.");
            }
            else if(config.OtherPenalties.HarveyFriendshipChange > 0)
            {
                response.Replace("Be a little more careful next time, okay?$s", "While it's nice to see you, I hate having to patch you up...#$b#Please be a little more careful next time, okay?$s");
            }

            // Return the built string
            return response.ToString();
        }

        /// <summary>
        /// Edits content in Strings/StringsFromCSFiles
        /// </summary>
        public class StringsFromCSFilesFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public StringsFromCSFilesFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            // Allow asset to be edited if name matches and any object references exist
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Strings\\StringsFromCSFiles") && PlayerStateRestorer.statedeathps.Value != null;
            }

            // Edit asset
            public void Edit<T>(IAssetData asset)
            {
                var stringeditor = asset.AsDictionary<string, string>().Data;

                // Has player not lost any money?
                if (config.DeathPenalty.MoneyLossCap == 0 || config.DeathPenalty.MoneytoRestorePercentage == 1)
                {
                    // Yes, edit strings to show this special case
                    stringeditor["Event.cs.1068"] = "Dr. Harvey didn't charge me for the hospital visit, how nice. ";
                    stringeditor["Event.cs.1058"] = "Fortunately, I still have all my money";
                }
                else
                {
                    // No, edit strings to show amount lost
                    stringeditor["Event.cs.1068"] = stringeditor["Event.cs.1068"].Replace("{0}",$"{(int)Math.Round(PlayerStateRestorer.statedeathps.Value.moneylost)}");
                    stringeditor["Event.cs.1058"] = stringeditor["Event.cs.1058"].Replace("{0}", $"{(int)Math.Round(PlayerStateRestorer.statedeathps.Value.moneylost)}");
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
        public class MailDataFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public MailDataFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            // Allow asset to be editted if name matches
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Data\\mail");
            }

            // Edit asset
            public void Edit<T>(IAssetData asset)
            {
                var maileditor = asset.AsDictionary<string, string>().Data;

                var data = ModEntry.PlayerData;

                

                // Has player not lost any money?
                if (data.MoneyLostLastPassOut == 0)
                {
                    // Yes, edit strings to show this special case
                    maileditor["passedOut1_Billed_Male"] = maileditor["passedOut1_Billed_Male"].Replace("You've been billed {0}g for this service", "Be thankful you haven't been billed for this service");
                    maileditor["passedOut1_Billed_Female"] = maileditor["passedOut1_Billed_Female"].Replace("You've been billed {0}g for this service", "Be thankful you haven't been billed for this service");
                    maileditor["passedOut3_Billed"] = maileditor["passedOut3_Billed"].Replace("I've billed you {0}g to cover your medical expenses.", "I haven't billed you for your medical expenses this time.");                  
                }

                else
                {
                    // No, edit strings to show amount lost
                    maileditor["passedOut1_Billed_Male"] = maileditor["passedOut1_Billed_Male"].Replace("{0}", $"{data.MoneyLostLastPassOut}");
                    maileditor["passedOut1_Billed_Female"] = maileditor["passedOut1_Billed_Female"].Replace("{0}", $"{data.MoneyLostLastPassOut}"); ;
                    maileditor["passedOut3_Billed"] = maileditor["passedOut3_Billed"].Replace("{0}", $"{data.MoneyLostLastPassOut}");
                }
            }
        }

        /// <summary>
        /// Edits content in Data/Events/Mine
        /// </summary>
        public class MineEventFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public MineEventFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            // Allow asset to be editted if name matches
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Data\\Events\\Mine");
            }

            // Edit asset
            public void Edit<T>(IAssetData asset)
            {
                var eventedits = asset.AsDictionary<string, string>().Data;

                // Is WakeupNextDayinClinic true?
                if (config.OtherPenalties.WakeupNextDayinClinic == true)
                {
                    eventedits["PlayerKilled"] = $"none/-100 -100/farmer 20 12 2 Harvey 21 12 3/changeLocation Hospital/pause 500/showFrame 5/message \" ...{Game1.player.Name}?\"/pause 1000/message \"Easy, now... take it slow.\"/viewport 20 12 true/pause 1000/{ResponseBuilder("{0}","in the mine")}/showFrame 0/pause 1000/emote farmer 28/hospitaldeath/end";
                }
            }
        }

        /// <summary>
        /// Edits content in Data/Events/IslandSouth
        /// </summary>
        public class IslandSouthEventFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public IslandSouthEventFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            // Allow asset to be editted if name matches
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Data\\Events\\IslandSouth");
            }

            // Edit asset
            public void Edit<T>(IAssetData asset)
            {
                var eventedits = asset.AsDictionary<string, string>().Data;

                // Is WakeupNextDayinClinic true?
                if (config.OtherPenalties.WakeupNextDayinClinic == true)
                {
                    eventedits["PlayerKilled"] = $"none/-100 -100/farmer 20 12 2 Harvey 21 12 3/changeLocation Hospital/pause 500/showFrame 5/message \" ...{Game1.player.Name}?\"/pause 1000/message \"Easy, now... take it slow.\"/viewport 20 12 true/pause 1000/{ResponseBuilder("{0}", "on the island shore")}/showFrame 0/pause 1000/emote farmer 28/hospitaldeath/end";
                }
            }
        }

        /// <summary>
        /// Edits content in Data/Events/Hospital
        /// </summary>
        public class HospitalEventFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public HospitalEventFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            // Allow asset to be editted if name matches
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Data\\Events\\Hospital");
            }

            // Edit asset
            public void Edit<T>(IAssetData asset)
            {
               var eventedits = asset.AsDictionary<string, string>().Data;

               eventedits["PlayerKilled"] = $"none/-100 -100/farmer 20 12 2 Harvey 21 12 3/pause 1500/showFrame 5/message \" ...{Game1.player.Name}?\"/pause 1000/message \"Easy, now... take it slow.\"/viewport 20 12 true/pause 1000/{ResponseBuilder("Someone", "and battered")}/showFrame 0/pause 1000/emote farmer 28/hospitaldeath/end";
            }
        }
    }
}
