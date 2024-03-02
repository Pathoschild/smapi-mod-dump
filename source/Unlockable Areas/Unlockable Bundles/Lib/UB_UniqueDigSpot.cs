/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using Unlockable_Bundles.Lib.AdvancedPricing;

namespace Unlockable_Bundles.Lib
{
    public class UB_SharedDigSpot
    {
        public static Mod Mod;
        public static IMonitor Monitor;
        public static IModHelper Helper;

        public const string SHAREDIGGSPOT = "UB_SharedDigSpot";
        public const string INDIVIDUALDIGGSPOT = "UB_IndividualDigSpot";

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = ModEntry._Monitor;
            Helper = ModEntry._Helper;

            var harmony = new Harmony(Mod.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkForBuriedItem)),
                prefix: new HarmonyMethod(typeof(UB_SharedDigSpot), nameof(UB_SharedDigSpot.checkForBuriedItem_Prefix))
            );

            Helper.Events.Multiplayer.ModMessageReceived += modMessageReceived;
        }

        public static bool checkForBuriedItem_Prefix(GameLocation __instance, ref string __result, int xLocation, int yLocation, bool explosion, bool detectOnly, Farmer who)
        {
            if (detectOnly)
                return true;

            var property = __instance.doesTileHavePropertyNoNull(xLocation, yLocation, SHAREDIGGSPOT, "Back").Trim();
            var shared = true;
            if (property == "") {
                shared = false;
                property = __instance.doesTileHavePropertyNoNull(xLocation, yLocation, INDIVIDUALDIGGSPOT, "Back").Trim();
            }

            if (property == "")
                return true;

            var spot = __instance.NameOrUniqueName + ":" + xLocation + "," + yLocation;
            if (!ModData.Instance.FoundUniqueDigSpots.ContainsKey(spot))
                ModData.Instance.FoundUniqueDigSpots.Add(spot, new());
            else if (shared)
                return true;

            if (ModData.Instance.FoundUniqueDigSpots[spot].Contains(who.UniqueMultiplayerID))
                return true;

            Helper.Multiplayer.SendMessage(new KeyValuePair<string, long>(spot, who.UniqueMultiplayerID), "DugUpUniqueItem", modIDs: new[] { ModEntry.Mod.ModManifest.UniqueID });
            ModData.Instance.FoundUniqueDigSpots[spot].Add(who.UniqueMultiplayerID);

            var itemId = property;
            if (property.Contains(' ')) {
                var split = property.Split(' ');
                itemId = split.First();

                if (shared)
                    Game1.addMailForTomorrow(split[1], noLetter: true, sendToEveryone: true);
                else
                    who.mailReceived.Add(split[1]);
            }
            var quality = Unlockable.getQualityFromReqSplit(itemId);
            itemId = Unlockable.getIDFromReqSplit(itemId);

            var item = Unlockable.parseItem(itemId, 1, quality);

            if (item is AdvancedPricingItem apItem) {
                if (apItem.UsesFlavoredSyntax) {
                    apItem.ItemCopy.Quality = quality;
                    apItem.ItemCopy.Stack = 1;
                    item = apItem.ItemCopy;
                } else {
                    Monitor.Log($"UB Digspots do not accept advanced pricing syntax apart from auto generated flavored Items!", LogLevel.Error);
                    return true;
                }
            }

            Debris debris = new(item, new Vector2(xLocation * 64 + 32, yLocation * 64 + 32));

            Game1.currentLocation.debris.Add(debris);
            return false;
        }

        private static void modMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != Mod.ModManifest.UniqueID)
                return;

            if (e.Type == "DugUpUniqueItem") {
                var spot = e.ReadAs<KeyValuePair<string, long>>();

                if (!ModData.Instance.FoundUniqueDigSpots.ContainsKey(spot.Key))
                    ModData.Instance.FoundUniqueDigSpots.Add(spot.Key, new());

                if (!ModData.Instance.FoundUniqueDigSpots[spot.Key].Contains(spot.Value))
                    ModData.Instance.FoundUniqueDigSpots[spot.Key].Add(spot.Value);
            }
        }
    }
}
