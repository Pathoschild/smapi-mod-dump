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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using PlacementPlus.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Object = StardewValley.Object;

#endregion

namespace PlacementPlus
{
    // Lost code: https://pastebin.com/AyCbCMZv
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class PlacementPlus : Mod
    {
        // Flooring.cs for tile id's, Object.cs for item id's
        private static readonly Dictionary<int, int> TILE_ID_TO_ITEM = new Dictionary<int, int> {
            {0, 328},  // Wood Floor
            {1, 329},  // Stone Floor
            {2, 331},  // Weathered Floor
            {3, 333},  // Crystal Floor
            {4, 401},  // Straw Floor
            {5, 407},  // Gravel Path
            {6, 405},  // Rustic Plank Floor
            {7, 409},  // Crystal Path
            {8, 411},  // Cobblestone Path
            {9, 415},  // Stepping Stone Path
            {10, 293}, // Brick Floor
            {11, 840}, // Wood Path
            {12, 841}  // Stone Walkway Floor
        };

        private static readonly Dictionary<int, int> ITEM_TO_TILE_ID = TILE_ID_TO_ITEM.ToDictionary(tile => tile.Value, tile => tile.Key);

        internal static readonly List<int> CHEST_INFO_LIST = Enum.GetValues(typeof(ChestInfo)).Cast<ChestInfo>().Select(ci => (int) ci).ToList();

        internal static readonly List<int> FENCE_INFO_LIST = Enum.GetValues(typeof(FenceInfo)).Cast<FenceInfo>().Select(fi => (int) fi).ToList();
        
        internal ModState modState;

        // Static instance of entry class to allow for static mod classes (i.e. patch classes) to interact with entry class data.
        internal static PlacementPlus Instance { get; private set; }

        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper helper)
        {
            // Initialize static instance first as Harmony patches rely on it.
            Instance = this;
            
            // Ensure that modState.tileAtPlayerCursor is initialized for patches
            modState.tileAtPlayerCursor = new Vector2();
            modState.isHoldingActionButton = false;
            modState.isHoldingUseToolButton = false;
            
            var harmony = HarmonyInstance.Create(ModManifest.UniqueID); harmony.PatchAll();

            // We keep track of the time since the player last placed any flooring to counteract input spam.
            // TODO: THIS IS REALLY HACKY BUT I DUNNO HOW ELSE TO DO IT
            helper.Events.GameLoop.UpdateTicked += (o, e) => modState.timeSinceLastPlacement++;

            helper.Events.GameLoop.DayStarted   += (o, e) => modState.currentPlayer = Game1.player;

            helper.Events.Input.ButtonsChanged  += (o, e) => {
                if (!Context.IsWorldReady) return;
                
                // Update ModState fields
                modState.currentTerrainFeatures = modState.currentPlayer.currentLocation.terrainFeatures;
                modState.tileAtPlayerCursor     = e.Cursor.Tile;
                modState.currentlyHeldItem      = modState.currentPlayer.CurrentItem;

                modState.isHoldingUseToolButton = e.Held.Any(button => button.IsUseToolButton()); // (i.e. left click)
                modState.isHoldingActionButton  = e.Held.Any(button => button.IsActionButton());  // (i.e. right click)

                modState.objectAtTile           = modState.currentPlayer.currentLocation.getObjectAtTile((int) e.Cursor.Tile.X, (int) e.Cursor.Tile.Y);
                
                SwapTile(o, e);
            };
        }


        
        /*********
        ** Internal methods
        *********/
        internal bool FlooringAtTileIsPlayerItem(Item flooringItem, Vector2 flooringTile) 
        {
            return flooringItem.ParentSheetIndex.Equals(
                TILE_ID_TO_ITEM[((Flooring) modState.currentTerrainFeatures[flooringTile]).whichFloor]);
        }
 


        /*********
        ** Private methods
        *********/
        [SuppressMessage("ReSharper", "ConvertToLocalFunction")] // I don't want local functions, I want nested ones >:(
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private void SwapTile(object sender, ButtonsChangedEventArgs e)
        {
            Action<Item, Vector2> swapFlooring     = (i, t) => {
                Func<bool> tileIsValidToPlace = () => {
                    // * Begin tile is valid to place * //
                    var _ = ""; // Throwaway variable for Building.doesTileHaveProperty reference argument.
                    var p = new Point((int) t.X, (int) t.Y);

                    var objectAtTileIsInteractive = false;
                    var tileHasMailbox            = false;
                    var tileHasDoor               = false;
                    var tileHasShippingBin        = false;

                    // If the Object at the target tile subclasses StardewValley.Object (since those are interactable)
                    if (modState.objectAtTile != null && ReusablePatches_SkipMethod.OBJECT_SUBCLASSES.Contains(modState.objectAtTile.GetType()))
                    {
                        // If the Object is not a torch or the Object (as a Fence) is a gate.
                        if (modState.objectAtTile.GetType() != typeof(Torch) ||
                            modState.objectAtTile.GetType() == typeof(Fence) &&
                            (modState.objectAtTile as Fence).isGate)
                        {
                            objectAtTileIsInteractive = true;
                        }
                    }
                    
                    if (modState.currentPlayer.currentLocation is Farm f)
                    {
                        tileHasMailbox = new[] { p, new Point(p.X, p.Y + 1) }.Contains(f.GetMainMailboxPosition());
                        foreach (var b in f.buildings )
                        {
                            if (!tileHasMailbox) tileHasMailbox = b.doesTileHaveProperty((int) t.X, (int) t.Y, "Mailbox", "Buildings", ref _);
                            if (!tileHasDoor)    tileHasDoor    = b.getPointForHumanDoor() == p || b.getRectForAnimalDoor().Contains(p);
                            if (!tileHasShippingBin && b is ShippingBin sb) tileHasShippingBin = sb.occupiesTile(t);
                        }
                    }
                    
                    return !objectAtTileIsInteractive && !tileHasMailbox && !tileHasDoor && !tileHasShippingBin;
                };
                
                // * Begin swap flooring * //
                // Ensure the player cannot swap a flooring of the same type.
                if (FlooringAtTileIsPlayerItem(i, t) || !(modState.isHoldingUseToolButton || tileIsValidToPlace())) return;

                // performToolAction() drops the flooring at tile as an item and generates the respective destruction debris,
                // destruction sound, etc.
                modState.currentTerrainFeatures[t].performToolAction(null, 1, t, modState.currentPlayer.currentLocation);
                modState.currentTerrainFeatures.Remove(t);
                modState.currentTerrainFeatures.Add(t, new Flooring(ITEM_TO_TILE_ID[i.ParentSheetIndex]));

                modState.currentPlayer.reduceActiveItemByOne();
            };
            
            Action<Item, Chest, Vector2> swapChest = (i, o, t) => {
                // * Begin swap chest * //
                // Ensure the player cannot swap a chest of the same type AND that the currentPlayer is holding the
                // use tool button.
                if (o.ParentSheetIndex == i.ParentSheetIndex || !modState.isHoldingUseToolButton) return;
                
                var currentLocation  = modState.currentPlayer.currentLocation;

                // If the current location is not a valid location to replace chests.
                if (currentLocation is MineShaft || currentLocation is VolcanoDungeon) return;

                var chestToPlace = new Chest(true, t, i.ParentSheetIndex) { shakeTimer = 100 };
                // Setting chest coloring and inventory.
                chestToPlace.playerChoiceColor.Value = o.playerChoiceColor.Value;
                chestToPlace.items.Set(o.items.ToList());

                // Clearing out the object chest's inventory before 'dropping' it and playing its destruction sound.
                o.items.Clear(); o.clearNulls();
                currentLocation.debris.Add(new Debris(-o.ParentSheetIndex, t * 64f + new Vector2(32f, 32f), modState.currentPlayer.Position));
                currentLocation.playSound( o.ParentSheetIndex == (int) ChestInfo.Chest ? "axe" : "hammer");
                
                // Spawning broken particles to simulate chest breaking.
                Game1.createRadialDebris(currentLocation, o.ParentSheetIndex == (int) ChestInfo.Chest ? 12 : 14, 
                    (int) t.X, (int) t.Y, 4, false);
                
                currentLocation.Objects.Remove(t);
                currentLocation.objects.Add(t, chestToPlace);
                
                modState.currentPlayer.reduceActiveItemByOne();
            };

            Action<Item, Fence, Vector2> swapFence = (i, o, t) =>  {
                // * Begin swap fence * //
                // Ensure the player cannot swap a fence of the same type AND that if the Object at tile is a gate,
                // the currentPlayer is holding the use tool button.
                // ? ParentSheetIndex cannot be used because object value is negative?
                if (o.Name == i.Name || !modState.isHoldingUseToolButton &&
                    modState.objectAtTile.GetType() == typeof(Fence)     &&
                    (modState.objectAtTile as Fence).isGate
                ) {
                    return;
                }

                var currentLocation  = modState.currentPlayer.currentLocation;
                var objectHasTorch   = o.heldObject.Value is Torch;
                var itemIsGate       = (i as Fence)?.isGate ?? false;

                o.performToolAction(o.isGate ? new Axe() : null, currentLocation);
                // Item can be casted as object as it has been asserted beforehand.
                (i as Object).placementAction(currentLocation, (int) t.X * 64, (int) t.Y * 64);
                // Ensure that if the fence has a torch, it is preserved.
                if (objectHasTorch && !itemIsGate) 
                    currentLocation.getObjectAtTile((int) t.X, (int) t.Y).heldObject.Value = new Torch(t, 1);

                modState.currentPlayer.reduceActiveItemByOne();
            };
            
            Func<Vector2, bool> preliminaryChecks  = t => {
                // * Begin preliminary checks * //
                var currentlyHeldItemNotNull = modState.currentlyHeldItem != null;
                // Attempting to replicate the increased placement speed if both action and use buttons are held down.
                var validLastPlacementTime   = modState.isHoldingUseToolButton && modState.isHoldingActionButton ? 5 : 10;
                var isCursorInValidPosition  = Utility.tileWithinRadiusOfPlayer((int) t.X, (int) t.Y, 1, modState.currentPlayer);

                return (modState.isHoldingUseToolButton || modState.isHoldingActionButton) &&
                       modState.timeSinceLastPlacement > validLastPlacementTime            &&
                       modState.currentPlayer.CanMove && // TODO: THIS DOES NOT WORK SINCE PLAYER CAN MOVE AFTER CLICKING AND TILE IS STILL REPLACED
                       currentlyHeldItemNotNull       && 
                       isCursorInValidPosition;
            };
            
            // * Begin tile swap * //
            if (!preliminaryChecks(e.Cursor.Tile)) return; // Preliminary checks
            
            // * Flooring checks * //
            var tileAtCursorIsFlooring = modState.currentTerrainFeatures.ContainsKey(e.Cursor.Tile) && 
                                         modState.currentTerrainFeatures[e.Cursor.Tile] is Flooring;
            var isHoldingFlooring      = modState.currentlyHeldItem.category.Value == Object.furnitureCategory;

            // * Chest checks * //
            // The only valid chests we allow to swap are normal chests.
            // ? Apparently Stone Chests aren't chests so we need to check their parentSheetIndex instead?
            var objectAtTileIsChest    = modState.objectAtTile is Chest;
            var isHoldingChest         = CHEST_INFO_LIST.Contains(modState.currentlyHeldItem.ParentSheetIndex);

            // * Fence checks * //
            var objectAtTileIsFence    = modState.objectAtTile is Fence;
            var isHoldingFence         = FENCE_INFO_LIST.Contains(modState.currentlyHeldItem.parentSheetIndex);

            if (tileAtCursorIsFlooring && isHoldingFlooring)
                swapFlooring(modState.currentlyHeldItem, modState.tileAtPlayerCursor);
            else if (objectAtTileIsChest && isHoldingChest)
                swapChest(modState.currentlyHeldItem, (Chest) modState.objectAtTile, e.Cursor.Tile);
            else if (objectAtTileIsFence && isHoldingFence)
                swapFence(modState.currentlyHeldItem, (Fence) modState.objectAtTile, e.Cursor.Tile);
            
            modState.timeSinceLastPlacement = 0;
        }

        
        
        // struct to allow for other mod classes to interact with entry class data
        internal struct ModState
        {
            internal int     timeSinceLastPlacement;
            internal Farmer  currentPlayer;
            internal Vector2 tileAtPlayerCursor;
            internal Item    currentlyHeldItem;
            internal bool    isHoldingUseToolButton;
            internal bool    isHoldingActionButton;
            internal Object  objectAtTile;
            internal NetVector2Dictionary<TerrainFeature,NetRef<TerrainFeature>> currentTerrainFeatures;
        }
        
        // enum to link chest name with respective parentSheetIndex
        private enum ChestInfo
        {
            Chest       = 130, // Normal wood chest
            Stone_chest = 232
        }
        
        // enum to link fence name with respective parentSheetIndex
        private enum FenceInfo
        {
            Hardwood_fence = 298,
            Wood_fence     = 322,
            Stone_fence    = 323,
            Iron_fence     = 324,
            Gate           = 325
        }
    }
}