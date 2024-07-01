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
using StardewArchipelago.Archipelago;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Characters;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.Festival
{
    public class WinterStarInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static Random _lastProvidedRandom;
        private static Random _random = null;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public void chooseSecretSantaGift(Item i, Farmer who)
        public static bool ChooseSecretSantaGift_SuccessfulGift_Prefix(Event __instance, Item i, Farmer who)
        {
            try
            {
                if (i is not Object gift || _archipelago.SlotData.FestivalLocations == FestivalLocations.Vanilla)
                {
                    return true; // don't run original logic
                }

                var recipient = __instance.getActorByName(__instance.secretSantaRecipient.Name);
                var taste = (GiftTaste)recipient.getGiftTasteForThisItem(gift);

                if (_archipelago.SlotData.FestivalLocations != FestivalLocations.Hard || taste == GiftTaste.Love)
                {
                    _locationChecker.AddCheckedLocation(FestivalLocationNames.SECRET_SANTA);
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ChooseSecretSantaGift_SuccessfulGift_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public static NPC GetRandomWinterStarParticipant(Func<string, bool> shouldIgnoreNpc = null)
        public static bool GetRandomWinterStarParticipant_ChooseBasedOnMonthNotYear_Prefix(Func<string, bool> ignoreNpc, ref NPC __result)
        {
            try
            {
                var random = Utility.CreateRandom((int)(Game1.uniqueIDForThisGame / 2UL), (int)(Game1.stats.DaysPlayed / 28), (double)Game1.player.UniqueMultiplayerID);
                __result = Utility.GetRandomNpc((name, data) => IsWinterStarParticipant(name, data, ignoreNpc), random);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetRandomWinterStarParticipant_ChooseBasedOnMonthNotYear_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static bool IsWinterStarParticipant(string name, CharacterData data, Func<string, bool> shouldIgnoreNpc)
        {
            if (shouldIgnoreNpc != null && shouldIgnoreNpc(name))
            {
                return false;
            }
            return data.WinterStarParticipant == null ? data.HomeRegion == "Town" : GameStateQuery.CheckConditions(data.WinterStarParticipant);
        }

        // public bool chooseResponse(Response response)
        public static void ChooseResponse_LegendOfTheWinterStar_Postfix(Dialogue __instance, Response response, ref bool __result)
        {
            try
            {
                if (__instance.speaker.Name != "Willy" || !Game1.CurrentEvent.isFestival || Game1.currentSeason != "winter" || Game1.dayOfMonth != 25)
                {
                    return;
                }

                if (response.responseKey == "quickResponse1")
                {
                    _locationChecker.AddCheckedLocation(FestivalLocationNames.LEGEND_OF_THE_WINTER_STAR);
                }
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ChooseResponse_LegendOfTheWinterStar_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
