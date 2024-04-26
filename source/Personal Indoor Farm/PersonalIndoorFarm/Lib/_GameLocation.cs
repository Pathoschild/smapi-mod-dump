/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PersonalIndoorFarm.ModEntry;

namespace PersonalIndoorFarm.Lib
{
    internal class _GameLocation
    {
        public static void Initialize()
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(GameLocation), nameof(GameLocation.DayUpdate), new[] { typeof(int) }),
                prefix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.DayUpdate_Prefix))
            );

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(GameLocation), nameof(GameLocation.GetSeason)),
                prefix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.GetSeason_Prefix))
            );

            harmony.Patch( //this runs before Cabin.DeleteFarmhand
                original: AccessTools.DeclaredMethod(typeof(Cabin), nameof(Cabin.demolish)),
                prefix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.demolish_Prefix))
            );
        }

        private static void demolish_Prefix(Cabin __instance)
        {
            if (Context.ScreenId > 0)
                return;

            if (!__instance.HasOwner)
                return;

            var sendData = new RemoveFarmhandModel(__instance.OwnerId, __instance.NameOrUniqueName);
            var farmer = Game1.getFarmerMaybeOffline(__instance.OwnerId);
            foreach (var doorId in Door.getDoorIds(farmer)) {
                if (!farmer.modData.TryGetValue(PersonalFarm.generateFarmerPIDKey(doorId), out var pid))
                    continue;

                var locationKey = PersonalFarm.generateLocationKey(pid, __instance.OwnerId, doorId);
                PersonalFarm.removeLocation(locationKey, __instance.NameOrUniqueName);
                sendData.PifLocations.Add(locationKey);
            }

            Helper.Multiplayer.SendMessage(sendData, "removeFarmHand", new[] { ModManifest.UniqueID });
        }

        public static bool DayUpdate_Prefix(GameLocation __instance)
        {
            if (__instance.NameOrUniqueName?.StartsWith(PersonalFarm.BaseLocationKey) == false)
                return true;

            return DayUpdate.CanRunDayUpdate;
        }

        public static bool GetSeason_Prefix(GameLocation __instance, ref Season __result)
        {
            if (__instance.NameOrUniqueName?.StartsWith(PersonalFarm.BaseLocationKey) == false)
                return true;

            __result = PersonalFarm.getSeason(__instance);
            return false;
        }

    }
}
