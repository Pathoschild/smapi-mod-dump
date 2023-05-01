/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Locations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    internal class TVInjections
    {
        private const string AP_WEATHER_REPORT = "Weather Report";
        private const string AP_FORTUNE_TELLER = " Fortune Teller";
        private const string AP_LIVING_OFF_THE_LAND = "Livin' Off The Land";
        private const string AP_QUEEN_OF_SAUCE = "The Queen of Sauce";
        private const string AP_FISHING = " Fishing Information Broadcasting Service";
        private const string AP_SINISTER_SIGNAL = "Sinister Signal";

        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        // public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        public static bool CheckForAction_TVChannels_Prefix(TV __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            try
            {
                if (justCheckingForActivity)
                {
                    __result = true;
                    return false; // don't run original logic
                }

                var channelsList = new List<Response>();
                AddWeatherChannel(channelsList);
                AddFortuneTellerChannel(channelsList);
                var dayOfWeek = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
                AddLivingOffTheLandChannel(dayOfWeek, channelsList);
                AddQueenOfSauceChannels(dayOfWeek, channelsList);
                AddSinisterTvChannel(who, channelsList);
                AddFishingChannel(channelsList);
                AddTurnOffTvChannel(channelsList);
                Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13120"), channelsList.ToArray(), __instance.selectChannel);
                Game1.player.Halt();

                __result = true;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckForAction_TVChannels_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void AddWeatherChannel(List<Response> channelsList)
        {
            if (!_archipelago.HasReceivedItem(AP_WEATHER_REPORT))
            {
                return;
            }

            channelsList.Add(CreateTvChannelLocalizedDialogue("Weather", "Strings\\StringsFromCSFiles:TV.cs.13105"));
        }

        private static void AddFortuneTellerChannel(List<Response> channelsList)
        {
            if (!_archipelago.HasReceivedItem(AP_FORTUNE_TELLER))
            {
                return;
            }

            channelsList.Add(CreateTvChannelLocalizedDialogue("Fortune","Strings\\StringsFromCSFiles:TV.cs.13107"));
        }

        private static void AddLivingOffTheLandChannel(string dayOfWeek, List<Response> channelsList)
        {
            if (!dayOfWeek.Equals("Mon") && !dayOfWeek.Equals("Thu"))
            {
                return;
            }

            if (!_archipelago.HasReceivedItem(AP_LIVING_OFF_THE_LAND))
            {
                return;
            }

            channelsList.Add(CreateTvChannelLocalizedDialogue("Livin'", "Strings\\StringsFromCSFiles:TV.cs.13111"));
        }

        private static void AddQueenOfSauceChannels(string str, List<Response> channelsList)
        {
            if (!_archipelago.HasReceivedItem(AP_QUEEN_OF_SAUCE))
            {
                return;
            }

            AddQueenOfSauceChannel(str, channelsList);
            AddQueenOfSauceRerunChannel(str, channelsList);
        }

        private static void AddQueenOfSauceChannel(string str, List<Response> channelsList)
        {
            if (!str.Equals("Sun"))
            {
                return;
            }
            channelsList.Add(CreateTvChannelLocalizedDialogue("The", "Strings\\StringsFromCSFiles:TV.cs.13114"));
        }

        private static void AddQueenOfSauceRerunChannel(string str, List<Response> channelsList)
        {
            if (!str.Equals("Wed") || Game1.stats.DaysPlayed <= 7U)
            {
                return;
            }
            channelsList.Add(CreateTvChannelLocalizedDialogue("The", "Strings\\StringsFromCSFiles:TV.cs.13117"));
        }

        private static void AddSinisterTvChannel(Farmer who, List<Response> channelsList)
        {
            if (Game1.currentSeason != "fall" || Game1.Date.DayOfMonth != 26 ||
                Game1.stats.getStat("childrenTurnedToDoves") <= 0U || who.mailReceived.Contains("cursed_doll"))
            {
                return;
            }

            if (!_archipelago.HasReceivedItem(AP_SINISTER_SIGNAL))
            {
                return;
            }

            channelsList.Add(CreateTvChannelDialogue("???", "???"));
        }

        private static void AddFishingChannel(List<Response> channelsList)
        {
            if (!_archipelago.HasReceivedItem(AP_FISHING))
            {
                return;
            }

            channelsList.Add(CreateTvChannelLocalizedDialogue("Fishing", "Strings\\StringsFromCSFiles:TV_Fishing_Channel"));
        }

        private static void AddTurnOffTvChannel(List<Response> channelsList)
        {
            channelsList.Add(CreateTvChannelLocalizedDialogue("(Leave)", "Strings\\StringsFromCSFiles:TV.cs.13118"));
        }

        private static Response CreateTvChannelLocalizedDialogue(string responseKey, string contentPath)
        {
            return CreateTvChannelDialogue(responseKey, Game1.content.LoadString(contentPath));
        }

        private static Response CreateTvChannelDialogue(string responseKey, string content)
        {
            return new Response(responseKey, content);
        }
    }
}
