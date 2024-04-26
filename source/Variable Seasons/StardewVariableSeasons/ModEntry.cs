/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace StardewVariableSeasons
{
    internal sealed class ModEntry : Mod
    {
        public static int ChangeDate;
        public static int CropSurvivalCounter;
        public static Season SeasonByDay;
        
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "_newDayAfterFade"),
                transpiler: new HarmonyMethod(typeof(NewDayAfterFadeTranspiler), nameof(NewDayAfterFadeTranspiler.Transpiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Objects.TV), "getWeatherForecast"),
                postfix: new HarmonyMethod(typeof(CustomWeatherChannelMessage), nameof(CustomWeatherChannelMessage.Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), "isFestivalDay"),
                prefix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPrefix)),
                postfix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPostfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Event), "tryToLoadFestival"),
                prefix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.LoadFestPrefix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "performTenMinuteClockUpdate"),
                prefix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPrefix)),
                postfix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPostfix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "warpFarmer",
                    new[] { typeof(LocationRequest), typeof(int), typeof(int), typeof(int) }),
                prefix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPrefix)),
                postfix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPostfix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "AreStoresClosedForFestival"),
                prefix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPrefix)),
                postfix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPostfix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), "getStartTimeOfFestival"),
                prefix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPrefix)),
                postfix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPostfix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "isBirthday"),
                prefix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPrefix)),
                postfix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPostfix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Crop), "IsInSeason", new [] { typeof(GameLocation) }),
                prefix: new HarmonyMethod(typeof(CropDeathRandomizer), nameof(CropDeathRandomizer.Prefix)),
                postfix: new HarmonyMethod(typeof(CropDeathRandomizer), nameof(CropDeathRandomizer.Postfix))
            );

            /*harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Locations.SeedShop), "addStock"),
                prefix: new HarmonyMethod(typeof(ShopStockPatches), nameof(ShopStockPatches.Prefix))
            );*/
            
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.Billboard), "draw", new [] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPrefix)),
                postfix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPostfix))
            );
            
            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.Menus.Billboard), new [] { typeof(bool) }),
                prefix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPrefix)),
                postfix: new HarmonyMethod(typeof(FestivalDayFixes), nameof(FestivalDayFixes.ResetSeasonPostfix))
            );
            
            helper.Events.GameLoop.DayEnding += (sender, e) => DayEndingActions.OnDayEnding(Monitor, Helper, sender, e);
            helper.Events.GameLoop.SaveLoaded +=
                (sender, e) => SaveLoadedActions.OnSaveLoaded(Monitor, Helper, sender, e);
        }
    }
}