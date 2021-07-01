/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/2Retr0/PlacementPlus
**
*************************************************/

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

#endregion

namespace PlacementPlus.Patches
{
    [HarmonyPatch]
    internal class ReusablePatches_SkipMethod
    {
        private static PlacementPlus.ModState modState => PlacementPlus.Instance.modState;
        private static IMonitor Monitor                => PlacementPlus.Instance.Monitor;
        
        public static readonly IEnumerable<Type> OBJECT_SUBCLASSES   = Assembly.Load("Stardew Valley").GetTypes()
                                                                               .Where(t => t.IsSubclassOf(typeof(Object)));

        public static readonly IEnumerable<Type> BUILDING_SUBCLASSES = Assembly.Load("Stardew Valley").GetTypes()
                                                                               .Where(t => t.IsSubclassOf(typeof(Building)));

        private static IEnumerable<MethodBase> TargetMethods()
        {
            // Get MethodInfo for each overwritten CheckForAction() in subclasses of StardewValley.Object
            var checkForActionIEnumerable = OBJECT_SUBCLASSES.SelectMany(t => t.GetMethods())
                                                             .Where(m => m.Name.Equals("checkForAction"));
            
            // Get MethodInfo for each overwritten doAction() in subclasses of StardewValley.Building
            var doAcionIEnumerable        = BUILDING_SUBCLASSES.SelectMany(t => t.GetMethods())
                                                             .Where(m => m.Name.Equals("doAction"));
            
            // Including other classes with StardewValley.Object subclasses.
            return checkForActionIEnumerable.Concat(
                   doAcionIEnumerable.Concat(new [] {
                       AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                       AccessTools.Method(typeof(ShippingBin),  nameof(ShippingBin.leftClicked)),
                       AccessTools.Method(typeof(Building),     nameof(Building.doAction))
                   }));
        }

        
        
        private static bool Prefix(ref bool __result, MethodBase __originalMethod)
        {
            try 
            {
                Func<NetVector2Dictionary<TerrainFeature,NetRef<TerrainFeature>>, Vector2, bool> flooringTileChecks = (tf, t) => {
                    // We need to use Game1.didPlayerJustLeftClick() since modState.isHoldingUseToolButton does not
                    // cover the initial button press.
                    var preliminaryChecks = modState.currentlyHeldItem != null                      &&
                                            // If the method's class subclasses Object or Building.
                                            (__originalMethod.DeclaringType == typeof(ShippingBin)  && 
                                             __originalMethod.DeclaringType == typeof(GameLocation) ||
                                             // If the Player is holding left click.
                                             Game1.didPlayerJustLeftClick() || modState.isHoldingUseToolButton);
                    
                    // * Begin flooring tile checks * //
                    if (!preliminaryChecks) return false; // Preliminary checks.

                    var tileAtCursorIsFlooring = tf.ContainsKey(t) && tf[t] is Flooring;
                    var isHoldingFlooring      = modState.currentlyHeldItem.category.Value == Object.furnitureCategory;

                    // If the player-held item is the flooring they are looking at OR the player holding flooring.
                    return tileAtCursorIsFlooring && !PlacementPlus.Instance.FlooringAtTileIsPlayerItem(modState.currentlyHeldItem, t) || 
                           isHoldingFlooring;
                };
                
                Func<GameLocation, Vector2, bool> fenceTileChecks = (gl, t) => {
                    // * Begin fence tile checks * //
                    var objectAtTile   = gl.getObjectAtTile((int) t.X, (int) t.Y);
                    var isHoldingFence = modState.currentlyHeldItem is Fence;
                    
                    // If the fence the player is looking at is a gate AND the player-held item is a fence.
                    return __originalMethod.DeclaringType == typeof(Fence) &&
                           objectAtTile is Fence fence && fence.isGate     && 
                           isHoldingFence;
                };

                // * Begin Prefix * //
                if (
                    !fenceTileChecks(modState.currentPlayer.currentLocation, modState.tileAtPlayerCursor) &&
                    !flooringTileChecks(modState.currentTerrainFeatures, modState.tileAtPlayerCursor)
                ) {
                    return true;
                }

                __result = false; // Original method will now return false.
                return false;     // Skip original logic.

            } catch (Exception e) {
                Monitor.Log($"Failed in {nameof(ReusablePatches_SkipMethod)}:\n{e}", LogLevel.Error);
                return true; // Run original logic.
            }
        }
    }
}