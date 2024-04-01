/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Object = StardewValley.Object;

namespace AnythingAnywhere.Framework.Patches.StandardObjects    
{
    internal class FurniturePatch : PatchTemplate
    {
        private readonly Type _object = typeof(Furniture);

        internal FurniturePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Furniture.GetAdditionalFurniturePlacementStatus), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(GetAdditionalFurniturePlacementStatusPostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Furniture.canBePlacedHere), new[] { typeof(GameLocation), typeof(Vector2), typeof(CollisionMask), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(CanBePlacedHerePostfix)));
            //DISABLE TABLE AND RUG CHECKS FOR NOW
            //harmony.Patch(AccessTools.Method(_object, nameof(Furniture.canBeRemoved), new[] { typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(CanBeRemovedPostfix)));
            //harmony.Patch(AccessTools.Method(_object, nameof(Furniture.placementAction), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(PlacementActionPrefix)));
            //harmony.Patch(AccessTools.Method(_object, nameof(Furniture.clicked), new[] { typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(ClickedPrefix)));
        }

        // Enables disabling wall furniture in all places in decortable locations. It can be annoying indoors.
        private static void GetAdditionalFurniturePlacementStatusPostfix(Furniture __instance, GameLocation location, int x, int y, Farmer who, ref int __result)
        {
            // Check if the furniture is wall furniture
            bool isWallFurniture =
                (__instance.furniture_type.Value == 6 ||
                __instance.furniture_type.Value == 17 ||
                __instance.furniture_type.Value == 13 ||
                __instance.QualifiedItemId == "(F)1293");

            // Check conditions for running the code inside
            if (!ModEntry.modConfig.EnableWallFurnitureIndoors && location is DecoratableLocation decoratableLocation && !ModEntry.modConfig.EnableFreeBuild)
            {
                // Conditions met, but skip if it's not wall furniture
                if (!isWallFurniture)
                {
                    __result = 0;
                }
                return;
            }
            // If EnableFreeBuild or EnableWallFurnitureIndoors are true
            else
            {
                __result = 0;
            }
        }

        //Enable placing furniture in walls
        private static void CanBePlacedHerePostfix(Furniture __instance, GameLocation l, Vector2 tile, ref bool __result, CollisionMask collisionMask = CollisionMask.All, bool showError = false)
        {
            if (ModEntry.modConfig.EnableFreePlace)
                __result = true;
        }

        // Enable picking up rugs with furniture on top.
/*        private static void CanBeRemovedPostfix(Furniture __instance, Farmer who, ref bool __result)
        {
            if (!ModEntry.modConfig.EnableRugTweaks || __instance.furniture_type.Value != 12)
                return;

            foreach (Furniture f in who.currentLocation.furniture)
            {
                if (f.furniture_type.Value == 12)
                {
                    __result = true;
                }
            }
        }

        // Table Tweak place item on table check
        private static bool PlacementActionPrefix(Furniture __instance, GameLocation location, int x, int y, ref bool __result, Farmer who = null)
        {
            if (!ModEntry.modConfig.EnablePlacing || !ModEntry.modConfig.EnableTableTweak)
                return true;

            if (!__instance.isGroundFurniture())
                y = __instance.GetModifiedWallTilePosition(location, x / 64, y / 64) * 64;

            if (__instance.GetAdditionalFurniturePlacementStatus(location, x, y, who) != 0)
            {
                __result = false;
                return false;
            }

            Vector2 tile = new Vector2(x / 64, y / 64);
            if (__instance.TileLocation != tile)
            {
                __instance.TileLocation = tile;
            }
            else
            {
                __instance.RecalculateBoundingBox();
            }

            foreach (Furniture f in location.furniture)
            {
                if (f.furniture_type.Value == 11 && f.heldObject.Value == null && f.GetBoundingBox().Intersects(__instance.boundingBox.Value))
                {
                    if (ModEntry.modConfig.TableTweakBind.IsDown())
                    {
                        if (__instance.furniture_type.Value == 12)
                            continue;
                        f.performObjectDropInAction(__instance, probe: false, who ?? Game1.player);
                        __result = true;
                        return false;
                    }
                    else
                    {
                        string message = I18n.Message_AnythingAnywhere_TableAddition(keybind: ModEntry.modConfig.TableTweakBind);
                        Game1.addHUDMessage(new HUDMessage(message, HUDMessage.error_type) { timeLeft = HUDMessage.defaultTime });
                        __result = false;
                        return false; //skip original method
                    }
                }
            }

            return true;
        }


        // Table Tweak pick up item check
        private static bool ClickedPrefix(Furniture __instance, Farmer who, ref bool __result)
        {
            if (!ModEntry.modConfig.EnablePlacing || !ModEntry.modConfig.EnableTableTweak)
                return true;

            Game1.haltAfterCheck = false;
            if ((int)__instance.furniture_type.Value == 11 && who.ActiveObject != null && __instance.heldObject.Value == null)
            {
                __result = false;
                return false;
            }
            if (__instance.heldObject.Value != null && ModEntry.modConfig.TableTweakBind.IsDown())
            {
                StardewValley.Object item = __instance.heldObject.Value;
                __instance.heldObject.Value = null;
                if (who.addItemToInventoryBool(item))
                {
                    item.performRemoveAction();
                    Game1.playSound("coin");
                    __result = true;
                    return false;
                }
                __instance.heldObject.Value = item;
            }
            else
            {
                string message = I18n.Message_AnythingAnywhere_TableRemoval(keybind: ModEntry.modConfig.TableTweakBind);
                Game1.addHUDMessage(new HUDMessage(message, HUDMessage.error_type) { timeLeft = HUDMessage.defaultTime });
                __result = false;
            }
            __result = false;
            return false;
        }
*/

    }
}
