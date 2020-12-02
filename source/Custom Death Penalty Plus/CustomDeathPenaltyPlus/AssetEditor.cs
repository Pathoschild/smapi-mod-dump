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
            if (config.DeathPenalty.WakeupNextDayinClinic == true)
            {
                // Yes, build string accordingly

                response.Insert(14, "Good you're finally awake. ");
            }

            // Is FriendshipPenalty greater than 0?
            if (config.DeathPenalty.FriendshipPenalty > 0)
            {
                // Yes, build string accordingly

                response.Replace("Be a little more careful next time, okay?", "You really need to be more careful, I don't like having to patch you up after you do something dangerous.");
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

            // Allow asset to be editted if name matches and any object references exist
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Strings\\StringsFromCSFiles") && PlayerStateRestorer.statedeath != null;
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
                    stringeditor["Event.cs.1068"] = $"Dr. Harvey charged me {(int)Math.Round(PlayerStateRestorer.statedeath.moneylost)}g for the hospital visit. ";
                    stringeditor["Event.cs.1058"] = $"I seem to have lost {(int)Math.Round(PlayerStateRestorer.statedeath.moneylost)}g";
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

                // Repeated strings in mail
                string incapacitated = "@,^Last night, a Joja team member found you incapacitated. A medical team was dispatched to bring you home safely.^We're glad you're okay!^^";
                string exhaustion = "@,^Someone dropped you off at the clinic last night. You'd passed out from exhaustion!^You've got to take better care of yourself and go to bed at a reasonable hour.^";
                string morris = "^^-Morris^Joja Customer Satisfaction Representative[#]Joja Invoice";
                string harvey = "^^-Dr. Harvey[#]From The Office Of Dr. Harvey";

                // Has player not lost any money?
                if (data.MoneyLostLastPassOut == 0)
                {
                    // Yes, edit strings to show this special case
                    maileditor["passedOut1_Billed_Male"] = $"Dear Mr. {incapacitated}(Be thankful you haven't been billed for this service){morris}";
                    maileditor["passedOut1_Billed_Female"] = $"Dear Ms. {incapacitated}(Be thankful you haven't been billed for this service){morris}";
                    maileditor["passedOut3_Billed"] = $"{exhaustion}I haven't billed you for your medical expenses this time.{harvey}";                  
                }

                else
                {
                    // No, edit strings to show amount lost
                    maileditor["passedOut1_Billed_Male"] = $"Dear Mr. {incapacitated}(You've been billed {data.MoneyLostLastPassOut}g for this service){morris}";
                    maileditor["passedOut1_Billed_Female"] = $"Dear Ms. {incapacitated}(You've been billed {data.MoneyLostLastPassOut}g for this service){morris}";
                    maileditor["passedOut3_Billed"] = $"{exhaustion}I've billed you {data.MoneyLostLastPassOut}g to cover your medical expenses.{harvey}";
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
                if (config.DeathPenalty.WakeupNextDayinClinic == true)
                {
                    eventedits["PlayerKilled"] = $"none/-100 -100/farmer 20 12 2 Harvey 21 12 3/changeLocation Hospital/pause 500/showFrame 5/message \" ...{Game1.player.Name}?\"/pause 1000/message \"Easy, now... take it slow.\"/viewport 20 12 true/pause 1000/{ResponseBuilder("{0}","in the mine")}/showFrame 0/pause 1000/emote farmer 28/hospitaldeath/end";
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

               eventedits["PlayerKilled"] = $"none/-100 -100/farmer 20 12 2 Harvey 21 12 3/pause 1500/showFrame 5/message \" ...{Game1.player.Name}?\"/pause 1000/message \"Easy, now... take it slow.\"/viewport 20 12 true/pause 1000/{ResponseBuilder("Someone","and battered")}/showFrame 0/pause 1000/emote farmer 28/hospitaldeath/end";
            }
        }
    }
}
