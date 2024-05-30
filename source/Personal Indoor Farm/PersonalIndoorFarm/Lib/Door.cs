/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using static PersonalIndoorFarm.ModEntry;

namespace PersonalIndoorFarm.Lib
{
    public class Door
    {
        public const string ItemId = "DLX.PIF_Door";
        public const string QualifiedItemId = "(F)" + ItemId;

        public const string LastDoorLocationKey = "DLX.PIF_LastDoorLocation";

        public const string SoundDoor = "";
        public const string SoundSilent = "Silent_";
        public const string SoundWoodStep = "WoodStep_";

        public static readonly List<string> VanillaDoors = new() {
            "DecorativeJojaDoor",
            "DecorativeWizardDoor",
            "DecorativeJunimoDoor",
            "DecorativeRetroDoor",
            "DecorativeDoor1",
            "DecorativeDoor2",
            "DecorativeDoor3",
            "DecorativeDoor4",
            "DecorativeDoor5",
            "DecorativeDoor6"
        };

        public static readonly List<string> VMVDoors = new() {
            "Lumisteria.MtVapius_FurnitureDeluxeSet_Door01",
            "Lumisteria.MtVapius_FurnitureDarkDeluxeSet_Door01"
        };
        public static void Initialize()
        {
            //Working around a Harmony Bug where PlacementPlus also Prefixed Furniture.checkForAction, but this for some reason removed my patch from the result
            Helper.Events.GameLoop.DayStarted += DayStarted;
        }

        private static void DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Furniture), nameof(Furniture.checkForAction), new[] { typeof(Farmer), typeof(bool) }),
                prefix: new HarmonyMethod(typeof(Door).GetMethod(nameof(checkForAction_Prefix)), priority: Priority.First)
            );
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Furniture), nameof(Furniture.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                postfix: new HarmonyMethod(typeof(Door).GetMethod(nameof(draw_Postfix)))
            );

            Helper.Events.GameLoop.DayStarted -= DayStarted;
        }

        public static Warp getWarpToLast(Farmer who)
        {
            who ??= Game1.player;

            if (who.modData.TryGetValue(LastDoorLocationKey, out var last)) {
                var args = last.Split(";");

                return new Warp(0, 0, args[0], int.Parse(args[1]), int.Parse(args[2]), false);
            }

            return new Warp(0, 0, who.homeLocation.Value, (int)who.mostRecentBed.X / 64, 1 + (int)who.mostRecentBed.Y / 64, false);
        }
        public static bool checkForAction_Prefix(Furniture __instance, ref bool __result)
        {
            try {
                if (!isDimensionDoor(__instance.ItemId, out string doorId))
                    return true;

                __result = true;
                checkWarp(doorId);
                return false;
            } catch (Exception err) {
                Monitor.LogOnce("Error at Door.checkForAction_Prefix:\n" + err.Message, LogLevel.Error);
                return true;
            }
        }

        public static bool isDimensionDoor(string itemId, out string doorId)
        {
            if (itemId.StartsWith(ItemId)) {
                doorId = itemId.Substring(ItemId.Length + 1);
                return true;

            } else if (Config.UseVanillaDoors && VanillaDoors.Contains(itemId)) {
                doorId = "Vanilla." + itemId;
                return true;

            } else if (Config.UseVMVDoors && VMVDoors.Contains(itemId)) {
                doorId = itemId;
                return true;
            }

            doorId = null;
            return false;
        }

        public static void checkWarp(string doorId)
        {
            if (Game1.currentLocation is not FarmHouse fh || !fh.HasOwner) {
                Game1.showRedMessage(Helper.Translation.Get("Door.Empty"));
                return;
            }

            var owner = Game1.getFarmerMaybeOffline(fh.OwnerId);
            if (fh.IsOwnedByCurrentPlayer && Game1.player.CurrentItem?.QualifiedItemId == Key.QualifiedItemId) {
                Key.useOnDoor(owner, doorId);
                return;
            }

            if (!fh.IsOwnedByCurrentPlayer && Key.isDoorLocked(owner, doorId)) {
                Game1.showRedMessage(Helper.Translation.Get("Door.Locked"));
                return;
            }

            if (!fh.IsOwnedByCurrentPlayer && !Game1.getOnlineFarmers().Contains(owner)) {
                Game1.showRedMessage(Helper.Translation.Get("Door.Offline"));
                return;
            }

            var models = Helper.GameContent.Load<Dictionary<string, PersonalFarmModel>>(AssetRequested.FarmsAsset);
            if (models.Count == 0) {
                Monitor.LogOnce("No Content Packs installed message thrown");
                Game1.showRedMessage(Helper.Translation.Get("NoContentPacks"));
                return;
            }

            if (!owner.modData.TryGetValue(PersonalFarm.generateFarmerPIDKey(doorId), out var pid)) {
                if (!fh.IsOwnedByCurrentPlayer) {
                    Game1.showRedMessage(Helper.Translation.Get("Door.Empty"));
                    return;
                }

                var farmSelection = new SelectionMenu();
                farmSelection.exitFunction = () => {
                    if (!farmSelection.Confirmed)
                        return;

                    var pid = farmSelection.ConfirmedModel.Key;

                    owner.modData.Add(PersonalFarm.generateFarmerPIDKey(doorId), pid);
                    var location = PersonalFarm.createLocation(pid, Game1.player, doorId);
                    Helper.Multiplayer.SendMessage(new ShareLocationModel(pid, doorId, Game1.player.UniqueMultiplayerID), "shareLocation", new[] { ModManifest.UniqueID });
                    PersonalFarm.setInitialDayAndSeason(location);
                };
                Game1.activeClickableMenu = farmSelection;
                return;

            }

            Game1.player.modData[LastDoorLocationKey] = $"{Game1.player.currentLocation.NameOrUniqueName};{Game1.player.TilePoint.X};{Game1.player.TilePoint.Y}";

            var model = PersonalFarm.getModel(pid);
            if (model is null) {
                Game1.showRedMessage(Helper.Translation.Get("Door.Empty"));
                return;
            }

            playDoorSound(doorId);

            var target = PersonalFarm.getArrivalTile(Game1.getFarmerMaybeOffline(fh.OwnerId), model);
            Game1.player.warpFarmer(new Warp(0, 0, PersonalFarm.generateLocationKey(pid, fh.OwnerId, doorId), target.X, target.Y, false));
            var location = Game1.getLocationFromName(PersonalFarm.generateLocationKey(pid, fh.OwnerId, doorId)); //For debugging :)
        }

        private static void playDoorSound(string doorId)
        {
            if (doorId.StartsWith(SoundSilent))
                return;
            else if (doorId.StartsWith(SoundWoodStep)) {
                Game1.currentLocation.playSound("woodyStep", Game1.player.Tile);
                DelayedAction.playSoundAfterDelay("woodyStep", 300, Game1.currentLocation, Game1.player.Tile);
                DelayedAction.playSoundAfterDelay("woodyStep", 600);
            } else
                Game1.currentLocation.playSound("doorOpen", Game1.player.Tile);
        }

        public static List<string> getDoorIds(Farmer who)
        {
            var ret = new List<string>();
            var startKey = PersonalFarm.BaseFarmerPidKey + "_";

            foreach (var e in who.modData.Pairs) {
                if (!e.Key.StartsWith(startKey))
                    continue;

                ret.Add(e.Key.Substring(startKey.Length));
            }

            return ret;
        }

        public static void draw_Postfix(Furniture __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (!isDimensionDoor(__instance.ItemId, out string doorId))
                return;

            if (Game1.currentLocation is FarmHouse fh && Game1.player.CurrentItem?.QualifiedItemId == Key.QualifiedItemId)
                Key.drawOverlay(spriteBatch, __instance, doorId, Game1.getFarmerMaybeOffline(fh.OwnerId));
        }
    }
}
