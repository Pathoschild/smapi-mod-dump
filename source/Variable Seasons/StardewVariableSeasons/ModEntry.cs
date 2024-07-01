/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace StardewVariableSeasons
{
    internal sealed class ModEntry : Mod
    {
        public static int ChangeDate { get; set; }
        public static int CropSurvivalCounter { get; set; }
        public static Season SeasonByDay;
        public static string CurrentSeason => Utility.getSeasonKey(SeasonByDay);
        public static int SeasonIndex => (int)SeasonByDay;

        internal static MemberInfo NewDayAfterFadeMethod = null;
        internal static MethodInfo TenMinuteMethod = null;
        internal static MethodInfo GetBirthdaysMethod = null;
        
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), helper.Reflection.GetMethod(typeof(Game1), "_newDayAfterFade").MethodInfo.Name),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.ExtractNewDayAfterFadeMethod))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.performTenMinuteClockUpdate)),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.ExtractTenMinuteMethod))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), nameof(Billboard.GetBirthdays)),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.ExtractGetBirthdaysMethod))
            );

            harmony.Patch(
                original: AccessTools.Method(NewDayAfterFadeMethod.ReflectedType, "MoveNext"),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(TenMinuteMethod.ReflectedType, TenMinuteMethod.Name),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(TV), "getWeatherForecast"),
                postfix: new HarmonyMethod(typeof(CustomWeatherChannelMessage), nameof(CustomWeatherChannelMessage.Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "UpdateWeatherForNewDay"),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(WorldDate), "Now"),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), "isFestivalDay"),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Event), "tryToLoadFestival"),
                prefix: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.LoadFestPrefix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "warpFarmer",
                    new[] { typeof(LocationRequest), typeof(int), typeof(int), typeof(int) }),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "AreStoresClosedForFestival"),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), "getStartTimeOfFestival"),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "isBirthday"),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "get_IsSpring"),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "get_IsSummer"),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "get_IsFall"),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "get_IsWinter"),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Crop), "IsInSeason", new [] { typeof(GameLocation) }),
                prefix: new HarmonyMethod(typeof(CropDeathRandomizer), nameof(CropDeathRandomizer.Prefix)),
                postfix: new HarmonyMethod(typeof(CropDeathRandomizer), nameof(CropDeathRandomizer.Postfix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), "draw", new [] { typeof(SpriteBatch) }),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), nameof(Billboard.GetEventsForDay)),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );

            harmony.Patch(
                original: AccessTools.Method(GetBirthdaysMethod.ReflectedType, GetBirthdaysMethod.Name),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getDaysOfBooksellerThisSeason)),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );
            
            harmony.Patch(
                original: AccessTools.Constructor(typeof(Billboard), new [] { typeof(bool) }),
                transpiler: new HarmonyMethod(typeof(SvsPatches), nameof(SvsPatches.SeasonTranspiler))
            );

            helper.Events.GameLoop.GameLaunched +=
                (sender, e) => GameLaunchedActions.OnGameLaunched(Monitor, Helper, ModManifest, sender, e);
            helper.Events.GameLoop.DayEnding += (sender, e) => DayEndingActions.OnDayEnding(Monitor, Helper, sender, e);
            helper.Events.GameLoop.SaveLoaded +=
                (sender, e) => SaveLoadedActions.OnSaveLoaded(Monitor, Helper, sender, e);
        }
    }
}