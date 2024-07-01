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
        public static void Initialize()
        {
            //Working around a Harmony Bug where PlacementPlus also Prefixed Furniture.checkForAction, but this for some reason removed my patch from the result
            Helper.Events.GameLoop.DayStarted += DayStarted;
        }

        private static void DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            Helper.Events.GameLoop.DayStarted -= DayStarted;
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Furniture), nameof(Furniture.checkForAction), new[] { typeof(Farmer), typeof(bool) }),
                prefix: new HarmonyMethod(typeof(Door).GetMethod(nameof(checkForAction_Prefix)), priority: Priority.First)
            );
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Furniture), nameof(Furniture.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                postfix: new HarmonyMethod(typeof(Door).GetMethod(nameof(draw_Postfix)))
            );
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
                if (!isDimensionDoor(__instance.ItemId, out var model))
                    return true;

                __result = true;
                checkWarp(model, __instance);
                return false;
            } catch (Exception err) {
                Monitor.LogOnce("Error at Door.checkForAction_Prefix:\n" + err.Message, LogLevel.Error);
                return true;
            }
        }

        public static bool isDimensionDoor(string itemId, out DoorAssetModel model)
        {
            var asset = Helper.GameContent.Load<Dictionary<string, DoorAssetModel>>(AssetRequested.DoorsAsset);
            if (asset.TryGetValue(itemId, out model)) {
                model.DoorId = String.IsNullOrEmpty(model.DoorId) ? itemId : model.DoorId;
                return true;
            }

            model = null;
            return false;
        }

        public static Farmer getOwner(Furniture door)
        {
            var rule = Enum.Parse<DoorOwnerEnum>(
            Game1.currentLocation is FarmHouse ? Config.OwnerFarmhouse : Config.OwnerOutside,
            true);

            if (rule == DoorOwnerEnum.None)
                return null;

            else if (rule == DoorOwnerEnum.Host)
                return Game1.MasterPlayer;

            else if (rule == DoorOwnerEnum.CurrentPlayer)
                return Game1.player;

            else if (rule == DoorOwnerEnum.Owner)
                return Game1.currentLocation is FarmHouse fh && fh.HasOwner ? Game1.getFarmerMaybeOffline(fh.OwnerId) : null;

            else if (rule == DoorOwnerEnum.PlacedBy && door is not null)
                return Game1.getFarmerMaybeOffline(door.owner.Value);

            return null;
        }

        public static Farmer checkOwner(Furniture door, string doorId, string pidKey)
        {
            Farmer owner = getOwner(door);

            if (owner is null) {
                Game1.showRedMessage(Helper.Translation.Get("Door.Empty"));
                return null;
            }

            var isOwner = owner.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID;
            if (isOwner && Game1.player.CurrentItem?.QualifiedItemId == Key.QualifiedItemId) {
                Key.useOnDoor(Game1.player, doorId);
                return null;
            }

            var lockStatus = Key.getDoorLocked(owner, doorId);
            if (!isOwner && lockStatus == DoorLockEnum.Locked) {
                Game1.showRedMessage(Helper.Translation.Get("Door.Locked"));
                return null;
            }

            if (!isOwner && lockStatus == DoorLockEnum.LockedWhenOffline && !Game1.getOnlineFarmers().Any(e => e.UniqueMultiplayerID == owner.UniqueMultiplayerID)) {
                Game1.showRedMessage(Helper.Translation.Get("Door.Offline"));
                return null;
            }

            if (!isOwner && !owner.modData.ContainsKey(pidKey)) {
                Game1.showRedMessage(Helper.Translation.Get("Door.Empty"));
                return null;
            }

            return owner;
        }

        public static void checkWarp(DoorAssetModel doorModel, Furniture door)
        {
            var pidKey = PersonalFarm.generateFarmerPIDKey(doorModel.DoorId);
            var owner = checkOwner(door, doorModel.DoorId, pidKey);

            if (owner is null)
                return;

            if (Game1.currentLocation.NameOrUniqueName.StartsWith(PersonalFarm.BaseLocationKey)) {
                Game1.showRedMessage(Helper.Translation.Get("Door.Empty"));
                return;
            }

            var models = Helper.GameContent.Load<Dictionary<string, PersonalFarmModel>>(AssetRequested.FarmsAsset);
            if (models.Count == 0) {
                Monitor.LogOnce("No Content Packs installed message thrown");
                Game1.showRedMessage(Helper.Translation.Get("NoContentPacks"));
                return;
            }

            if (!owner.modData.TryGetValue(pidKey, out var pid)) {
                var farmSelection = new SelectionMenu();
                farmSelection.exitFunction = () => {
                    if (!farmSelection.Confirmed)
                        return;

                    var pid = farmSelection.ConfirmedModel.Key;

                    owner.modData.Add(pidKey, pid);
                    var location = PersonalFarm.createLocation(pid, Game1.player, doorModel.DoorId);
                    Helper.Multiplayer.SendMessage(new ShareLocationModel(pid, doorModel.DoorId, Game1.player.UniqueMultiplayerID), "shareLocation", new[] { ModManifest.UniqueID });
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

            playDoorSound(doorModel);

            var target = PersonalFarm.getArrivalTile(Game1.getFarmerMaybeOffline(owner.UniqueMultiplayerID), model);
            Game1.player.warpFarmer(new Warp(0, 0, PersonalFarm.generateLocationKey(pid, owner.UniqueMultiplayerID, doorModel.DoorId), target.X, target.Y, false));
            var location = Game1.getLocationFromName(PersonalFarm.generateLocationKey(pid, owner.UniqueMultiplayerID, doorModel.DoorId)); //For debugging :)
        }

        private static void playDoorSound(DoorAssetModel doorModel)
        {
            if (doorModel.Sound == DoorSoundEnum.Silent)
                return;

            else if (doorModel.Sound == DoorSoundEnum.Door)
                Game1.currentLocation.playSound("doorOpen", Game1.player.Tile);

            else if (doorModel.Sound == DoorSoundEnum.WoodStep) {
                Game1.currentLocation.playSound("woodyStep", Game1.player.Tile);
                DelayedAction.playSoundAfterDelay("woodyStep", 300, Game1.currentLocation, Game1.player.Tile);
                DelayedAction.playSoundAfterDelay("woodyStep", 600);

            }
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
            if (Game1.player.CurrentItem?.QualifiedItemId != Key.QualifiedItemId)
                return;

            if (!isDimensionDoor(__instance.ItemId, out var doorModel))
                return;

            var owner = getOwner(__instance);

            if (owner is null || owner.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
                return;

            Key.drawOverlay(spriteBatch, __instance, doorModel.DoorId, owner);
        }
    }
}
