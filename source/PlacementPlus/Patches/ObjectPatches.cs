/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/2Retr0/PlacementPlus
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

using static PlacementPlus.ModState;
using static PlacementPlus.Utility.Utility;

using Object = StardewValley.Object;

namespace PlacementPlus.Patches
{
    [HarmonyPatch(typeof(Object), nameof(Object.placementAction))]
    internal class ObjectPatches
    {
        /// <summary>
        /// Adds additional logic when attempting to place an object to determine if some tile object should be swapped.
        /// Additionally prevents the appearance of the 'Unsuitable Location' dialogue when attempting to place a chest
        /// where a chest already exists.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
        private static bool Prefix(GameLocation location, int x, int y, Farmer who, ref bool __result, Object __instance)
        {
            try
            {
                var tilePos = new Vector2(x / 64, y / 64);
                var tileObject = location.getObjectAt(x, y);
                var terrainFeatures = location.terrainFeatures; 
                var holdingToolButton = Game1.didPlayerJustLeftClick() || ModState.holdingToolButton;

                bool SwapFlooring(Flooring flooring) 
                {
                    bool tileHasInteractable()  
                    {
                        // If the tile object is a fence gate OR not a torch/fence we consider it interactable.
                        if (IsItemGate(tileObject) || tileObject is not (Torch or Fence)) 
                            return true;
                        
                        // Otherwise we check for interactable components on buildings if the location is the farm.
                        return location is Farm farm && IsTileOnBuildingInteractable(farm, tilePos);
                    }
                    
                    
                    // We do not swap if:
                    //  - Both the player-held flooring and tile flooring are the same.
                    //  - The player is not holding the use tool button and the tile has an interactable object/feature
                    //      (i.e. mailbox, shipping bin, etc.) (we want their actions to still be accessible).
                    if (IsItemTargetFlooring(__instance, flooring) || (!holdingToolButton && tileHasInteractable())) 
                        return false;

                    
                    // We use performToolAction() drops the flooring at tile as an item and generates the respective
                    // destruction debris, destruction sound, etc. Axes can destroy all flooring.
                    terrainFeatures[tilePos].performToolAction(new Axe(), 0, tilePos, location);
                    terrainFeatures.Remove(tilePos);
                    
                    terrainFeatures.Add(tilePos, new Flooring(FlooringInfoMap[__instance.ParentSheetIndex]));

                    who.reduceActiveItemByOne();
                    return true;
                }
                
                bool SwapChest(Chest chest) 
                {
                    // Do not swap if the player is not holding the use tool button (we want their inventory to still be
                    // accessible).
                    if (!holdingToolButton) return false;
                    
                    
                    var chestToPlace = new Chest(true, tilePos, __instance.ParentSheetIndex) {
                        shakeTimer        = 100,
                        playerChoiceColor = { Value = chest.playerChoiceColor.Value } };
                    chestToPlace.items.Set(chest.items); // Fill the new chest with the items of the old chest.

                    var isWoodenChest = chest.ParentSheetIndex == (int) ChestType.Chest;
                    // We clear out the chest's inventory before simulating its destruction.
                    chest.items.Clear(); chest.clearNulls();
                    location.debris.Add(new Debris(-chest.ParentSheetIndex, new Vector2(x + 32, y + 32), who.Position));
                    location.playSound(isWoodenChest ? "axe" : "hammer");
                    
                    // Spawning broken particles to simulate chest breaking.
                    Game1.createRadialDebris(location, isWoodenChest ? 12 : 14, 64 * x, 64 * y, 4, false);
                    location.Objects.Remove(tilePos);
                    
                    location.Objects.Add(tilePos, chestToPlace);

                    who.reduceActiveItemByOne();
                    return true;
                }
                
                bool SwapFence(Fence fence) 
                {
                    // We do not swap if:
                    //  - Both the player-held fence and tile fence are the same (unless the tile fence has been turned
                    //      into a gate).
                    //  - The tile fence is a gate and the player is not holding the use tool button (i.e. right clicks
                    //      can still be used to open gates even if the player-held item is a fence.
                    //  - The player-held item is a gate--we don't want to overwrite the gate's functionality to convert
                    //      fences.
                    if (IsItemTargetFence(__instance, fence) 
                        || (!holdingToolButton && fence.isGate.Value) 
                        || IsItemGate(__instance)) 
                    { return false; }

                    
                    // We must use the correct tool to destroy the fence as if the fence has been converted to a gate,
                    // it needs to drop the gate as well as the original fence.
                    var usePickaxe = (FenceType) fence.whichType.Value is
                        FenceType.Stone_fence or FenceType.Iron_fence;
                    fence.performToolAction(usePickaxe ? new Pickaxe() : new Axe(), location);
                
                    __instance.placementAction(location, x, y);
                    // Ensure that if the original fence has a torch, it is preserved.
                    if (fence.heldObject.Value is Torch) tileObject.heldObject.Value = new Torch(tilePos, 1);

                    who.reduceActiveItemByOne();
                    return true;
                }
                
                
                if (IsItemFlooring(__instance) && DoesTileHaveFlooring(terrainFeatures, tilePos))
                    __result = SwapFlooring((Flooring) terrainFeatures[tilePos]);
                else if (IsItemChest(__instance) && IsItemChest(tileObject))
                    // We still skip the original logic if the chests are equal as to prevent the 'Unsuitable Location'
                    // dialogue from appearing when trying to place a chest where a chest already exists.
                    __result = __instance.ParentSheetIndex == tileObject.ParentSheetIndex || 
                               SwapChest(tileObject as Chest);
                else if (IsItemFence(__instance) && IsItemFence(tileObject))
                    __result = SwapFence(tileObject as Fence);

                return !__result; // Skip original logic if there was a successful swap.
            } catch (Exception e) {
                Monitor.Log($"Failed in {nameof(ObjectPatches)}:\n{e}", LogLevel.Error);
                return true; // Run original logic.
            }
        }
    }
}