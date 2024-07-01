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
using StardewValley.Triggers;
using StardewValley.Delegates;
using static Unlockable_Bundles.ModEntry;
using Unlockable_Bundles.NetLib;

namespace Unlockable_Bundles.Lib.MapFeatures
{
    public class DigSpot
    {
        public const string SHAREDIGGSPOT = "UB_SharedDigSpot";
        public const string INDIVIDUALDIGGSPOT = "UB_IndividualDigSpot";

        public static void Initialize()
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkForBuriedItem)),
                prefix: new HarmonyMethod(typeof(DigSpot), nameof(DigSpot.checkForBuriedItem_Prefix))
            );

            Helper.Events.Multiplayer.ModMessageReceived += modMessageReceived;

            TriggerActionManager.RegisterAction("UB_ResetDigSpot", ResetAction);
        }

        private static bool ResetAction(string[] args, TriggerActionContext context, out string error)
        {
            if (!ArgUtility.TryGet(args, 1, out string who, out error, allowBlank: false))
                return false;

            if (!new string[] { "all", "current", "host" }.Contains(who.ToLower())) {
                error = $"Invalid Target Player {who}. Must be one of 'All', 'Current', 'Host'";
                return false;

            }
            who = who.ToLower();

            if (!ArgUtility.TryGet(args, 2, out string where, out error, allowBlank: false))
                return false;

            var location = Game1.getLocationFromName(where);
            if (location is null) {
                error = $"Unknown gamelocation {where}";
                return false;

            }

            if (!ArgUtility.TryGet(args, 3, out string xString, out error, allowBlank: false))
                return false;

            if (!int.TryParse(xString, out int x)) {
                error = $"X coordinates do not parse to number: {xString}";
                return false;

            }

            if (!ArgUtility.TryGet(args, 4, out string yString, out error, allowBlank: false))
                return false;

            if (!int.TryParse(yString, out int y)) {
                error = $"Y coordinates do not parse to number: {yString}";
                return false;

            }

            if (!ArgUtility.TryGet(args, 5, out string resetMailflagString, out _, allowBlank: true))
                resetMailflagString = "false";

            var resetMailFlag = resetMailflagString.ToLower() == "true" ? true : false;

            var transferData = new DigSpotTransferData() {
                Location = where,
                X = x,
                Y = y,
                ResetMailFlag = resetMailFlag
            };
            if (who == "current") {
                transferData.Who = Game1.player.UniqueMultiplayerID;
                resetDigspot(Game1.player, location, x, y, resetMailFlag);

            }  else if (who == "host") {
                transferData.Who = Game1.MasterPlayer.UniqueMultiplayerID;
                resetDigspot(Game1.MasterPlayer, location, x, y, resetMailFlag);

            } else foreach (var farmer in Game1.getAllFarmers())
                    resetDigspot(farmer, location, x, y, resetMailFlag);

            Helper.Multiplayer.SendMessage(transferData, "ResetDigSpot", modIDs: new[] { ModManifest.UniqueID });

            return true;
        }

        public static void resetDigspot(Farmer who, GameLocation location, int x, int y, bool resetMailFlag)
        {
            if (resetMailFlag) {
                var property = location.doesTileHavePropertyNoNull(x, y, SHAREDIGGSPOT, "Back").Trim();
                if (property == "")
                    property = location.doesTileHavePropertyNoNull(x, y, INDIVIDUALDIGGSPOT, "Back").Trim();

                var splitProperty = property.Split(" ");
                if (splitProperty.Length >= 2)
                    who.mailReceived.Remove(splitProperty[1]);
                else
                    Monitor.LogOnce($"TriggerAction requested resetting MailFlag of DropSpot in {location.Name}: X {x} Y {y}, but the DigSpot contains no MailKey");
            }

            var spot = location.NameOrUniqueName + ":" + x + "," + y;
            if (!ModData.Instance.FoundUniqueDigSpots.ContainsKey(spot))
                return;

            ModData.Instance.FoundUniqueDigSpots[spot].Remove(who.UniqueMultiplayerID);
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

            Helper.Multiplayer.SendMessage(new KeyValuePair<string, long>(spot, who.UniqueMultiplayerID), "DugUpUniqueItem", modIDs: new[] { ModManifest.UniqueID });
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
            if (e.FromModID != ModManifest.UniqueID)
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
