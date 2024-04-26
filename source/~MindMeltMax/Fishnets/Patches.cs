/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Reflection.Emit;
using System.Reflection;
using StardewValley.ItemTypeDefinitions;

namespace Fishnets
{
    internal static class Patches
    {
        internal static void Patch(string id)
        {
            Harmony harmony = new(id);

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.canBePlacedHere)),
                postfix: new(typeof(Patches), nameof(Object_CanBePlacedHere_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.draw), [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]),
                prefix: new(typeof(Patches), nameof(Object_Draw_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.DayUpdate)),
                postfix: new(typeof(Patches), nameof(Object_DayUpdate_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.placementAction)),
                prefix: new(typeof(Patches), nameof(Object_PlacementAction_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.performToolAction)),
                prefix: new(typeof(Patches), nameof(Object_PerformToolAction_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.performObjectDropInAction)),
                prefix: new(typeof(Patches), nameof(Object_PerformObjectDropInAction_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.checkForAction)),
                prefix: new(typeof(Patches), nameof(Object_CheckForAction_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.actionOnPlayerEntry)),
                postfix: new(typeof(Patches), nameof(Object_ActionOnPlayerEntry_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.updateWhenCurrentLocation)),
                postfix: new(typeof(Patches), nameof(Object_UpdateWhenCurrentLocation_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.isActionable)),
                postfix: new(typeof(Patches), nameof(Object_IsActionable_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ParsedItemData), nameof(ParsedItemData.GetSourceRect)),
                postfix: new(typeof(Patches), nameof(ParsedItemData_GetSourceRect_Postfix))
            );
        }

        private static void Object_CanBePlacedHere_Postfix(Object __instance, GameLocation l, Vector2 tile, ref bool __result)
        {
            if (__instance.ItemId != ModEntry.ObjectInfo.Id)
                return;
            __result = CrabPot.IsValidCrabPotLocationTile(l, (int)tile.X, (int)tile.Y);
        }

        private static bool Object_Draw_Prefix(Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            if (__instance.ItemId != ModEntry.ObjectInfo.Id)
                return true;
            Statics.Draw(__instance, spriteBatch, x, y, alpha);
            return false;
        }

        private static void Object_DayUpdate_Postfix(Object __instance)
        {
            if (__instance.ItemId != ModEntry.ObjectInfo.Id && __instance.Location != null)
                return;
            Statics.DoDayUpdate(__instance);
        }

        private static bool Object_PlacementAction_Prefix(Object __instance, GameLocation location, int x, int y, Farmer who, ref bool __result) //Works \\Solution: a reference from the original object in the players inventory was placed instead of a new object
        {
            if (__instance.ItemId != ModEntry.ObjectInfo.Id)
                return true;
            Vector2 tileLocation = new((int)Math.Floor(x / 64f), (int)Math.Floor(y / 64f));
            if (!CrabPot.IsValidCrabPotLocationTile(location, (int)tileLocation.X, (int)tileLocation.Y))
            {
                __result = false;
                return false;
            }
            Object o = (Object)__instance.getOne();
            o.Location = location;
            o.setHealth(10);
            o.TileLocation = tileLocation;
            o.owner.Value = (who ?? Game1.player).UniqueMultiplayerID;
            location.Objects.Add(tileLocation, o);
            if (!ModEntry.NoSound)
            {
                location.playSound("waterSlosh");
                DelayedAction.playSoundAfterDelay("slosh", 150);
            }
            Statics.OnPlace(location, tileLocation);
            __result = true;
            return false;
        }

        private static bool Object_PerformToolAction_Prefix(Object __instance)
        {
            if (__instance.ItemId != ModEntry.ObjectInfo.Id)
                return true;
            return false;
        }

        private static bool Object_PerformObjectDropInAction_Prefix(Object __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
        {
            if (__instance.ItemId != ModEntry.ObjectInfo.Id || probe)
                return true;
            var modData = Statics.GetModDataAt(__instance.Location, __instance.TileLocation);
            if (dropInItem is not Object o || o.Category != Object.baitCategory || !string.IsNullOrWhiteSpace(modData?.BaitId) || (who ?? Game1.player).professions.Contains(11) || __instance.heldObject.Value is not null)
            {
                __result = false;
                return false;
            }
            if (!probe)
            {
                Statics.SetModDataAt(__instance.Location, __instance.TileLocation, (modData ?? new(Statics.SetDirectionOffset(__instance.Location, __instance.TileLocation))) with { BaitId = o.ItemId, BaitQuality = o.Quality });
                __instance.modData[ModEntry.ModDataTileIndexKey] = "0,0,60";
                __instance.Location.playSound("Ship");
            }
            __result = true;
            return false;
        }

        private static bool Object_CheckForAction_Prefix(Object __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            if (__instance.ItemId != ModEntry.ObjectInfo.Id || justCheckingForActivity)
                return true;

            var modData = Statics.GetModDataAt(__instance.Location, __instance.TileLocation);
            if (__instance.heldObject.Value is not null)
            {
                Object o = __instance.heldObject.Value;
                if (who.IsLocalPlayer && !who.addItemToInventoryBool(o))
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                    return false;
                }
                __instance.heldObject.Value = null;
                Dictionary<string, string> fishData = ModEntry.IHelper.GameContent.Load<Dictionary<string, string>>("Data\\Fish");
                if (fishData.ContainsKey(o.ItemId))
                    who.caughtFish(o.ItemId, -1, numberCaught: o.Stack);
                __instance.readyForHarvest.Value = false;
                Statics.SetModDataAt(__instance.Location, __instance.TileLocation, (modData ?? new(Statics.SetDirectionOffset(__instance.Location, __instance.TileLocation))) with { BaitId = "", BaitQuality = 0 });
                who.animateOnce(279 + who.FacingDirection);
                Statics.SetTileIndexData(__instance, false, 5, 60);
                __instance.Location.playSound("fishingRodBend");
                DelayedAction.playSoundAfterDelay("coin", 500);
                who.gainExperience(1, 5);
                __result = true;
                return false;
            }
            if (string.IsNullOrWhiteSpace(modData?.BaitId))
            {
                if (Game1.didPlayerJustClickAtAll(true))
                {
                    if (who.addItemToInventoryBool(__instance.getOne()))
                    {
                        if (who.isMoving())
                            Game1.haltAfterCheck = false;
                        Game1.playSound("coin");
                        Statics.OnRemove(__instance.Location, __instance.TileLocation);
                        __instance.Location.Objects.Remove(__instance.TileLocation);
                        __result = true;
                        return false;
                    }
                    else
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                }
            }
            __result = false;
            return false;
        }

        private static void Object_ActionOnPlayerEntry_Postfix(Object __instance)
        {
            if (__instance.ItemId != ModEntry.ObjectInfo.Id)
                return;
            var modData = Statics.GetModDataAt(__instance.Location, __instance.TileLocation);
            if (modData is null)
                Statics.SetModDataAt(__instance.Location, __instance.TileLocation, modData = new(Statics.SetDirectionOffset(__instance.Location, __instance.TileLocation)));
            Statics.AddOverlayTiles(__instance.Location, __instance.TileLocation, modData!.Offset);
        }

        private static void Object_UpdateWhenCurrentLocation_Postfix(Object __instance, GameTime time)
        {
            if (__instance.ItemId != ModEntry.ObjectInfo.Id || !Statics.TryGetTileIndexData(__instance, out var data) || data.complete)
                return;
            int curTileIndex = data.tileIndex;
            int timer = data.timer;
            timer -= time.ElapsedGameTime.Milliseconds;
            if (timer <= 0)
            {
                curTileIndex++;
                timer = 60;
            }
            if (curTileIndex == 3)
                Statics.SetTileIndexData(__instance, true, curTileIndex, 0);
            else if (curTileIndex >= 8)
                Statics.ClearTileIndexData(__instance);
            else
                Statics.SetTileIndexData(__instance, false, curTileIndex, timer);
        }

        private static void Object_IsActionable_Postfix(Object __instance, ref bool __result)
        {
            if (__instance.ItemId != ModEntry.ObjectInfo.Id)
                return;
            var modData = Statics.GetModDataAt(__instance.Location, __instance.TileLocation);
            if (!string.IsNullOrWhiteSpace(modData?.BaitId))
            {
                __result = false;
                return;
            }
            __result = true;
            return;
        }

        private static void ParsedItemData_GetSourceRect_Postfix(ParsedItemData __instance, ref Rectangle __result)
        {
            if (__instance.ItemId != ModEntry.ObjectInfo.Id)
                return;
            __result = Statics.GetSourceRectAtTileIndex(-1);
        }
    }
}
