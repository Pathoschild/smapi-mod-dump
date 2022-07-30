/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using DecidedlyShared.Logging;
using SmartBuilding.UI;
using SmartBuilding.Utilities;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace SmartBuilding
{
    public class ModState
    {
        private Logger logger;
        private bool inBuildMode = false;
        private bool blockMouseInteractions = false;
        private ButtonId activeTool = ButtonId.None;
        private TileFeature selectedLayer = TileFeature.None;
        private PlayerUtils playerUtils;
        private IdentificationUtils identificationUtils;
        private WorldUtils worldUtils;
        private PlacementUtils placementUtils;
        private bool buildingMode;
        
        // All of our drawing variables.
        private Dictionary<Vector2, ItemInfo> tilesSelected = new Dictionary<Vector2, ItemInfo>();
        private Vector2? startTile = null;
        private Vector2? endTile = null;
        private List<Vector2> rectTiles = new List<Vector2>();
        private Item rectangleItem;
        
        #region Properties
        
        public bool BuildingMode
        {
            get { return buildingMode; }
            set
            {
                buildingMode = value;
                HarmonyPatches.Patches.CurrentlyInBuildMode = value;

                if (!buildingMode) // If this is now false, we want to clear the tiles list, refund everything, and kill our UI.
                {
                    
                }
                else
                {
                    
                }
            }
        }
        
        public Dictionary<Vector2, ItemInfo> TilesSelected
        {
            get { return tilesSelected; }
            set { tilesSelected = value ?? throw new ArgumentNullException(nameof(value)); }
        }
        
        public List<Vector2> RectTiles
        {
            get { return rectTiles; }
            set { rectTiles = value ?? throw new ArgumentNullException(nameof(value)); }
        }
        
        public Vector2? StartTile
        {
            get { return startTile; }
            set { startTile = value; }
        }
        public Vector2? EndTile
        {
            get { return endTile; }
            set { endTile = value; }
        }
        
        public Item RectangleItem
        {
            get { return rectangleItem; }
            set { rectangleItem = value; /*?? throw new ArgumentNullException(nameof(value));*/ }
        }
        
        public TileFeature SelectedLayer
        {
            get => selectedLayer;
            set
            {
                selectedLayer = value;
            }
        }
        
        #endregion

        public ModState(Logger logger, PlayerUtils playerUtils, IdentificationUtils identificationUtils, WorldUtils worldUtils, PlacementUtils placementUtils)
        {
            this.logger = logger;
            this.playerUtils = playerUtils;
            this.identificationUtils = identificationUtils;
            this.worldUtils = worldUtils;
            this.placementUtils = placementUtils;
        }
        
        
        
        public void EnterBuildMode()
        {
            
        }

        public void LeaveBuildMode()
        {
            ActiveTool = ButtonId.None;
        }

        public ButtonId ActiveTool
        {
            get { return activeTool; }
            set { activeTool = value; }
        }

        public bool InBuildMode
        {
            get => inBuildMode;
        }

        /// <summary>
        /// Whether or not mouse buttons should apply to our UI only.
        /// </summary>
        public bool BlockMouseInteractions
        {
            get => blockMouseInteractions;
            set { blockMouseInteractions = value; }
        }

        /// <summary>
        /// Resets the *volatile*, temporary tiles used to draw the rectangle on-screen while drawing.
        /// </summary>
        public void ResetVolatileTiles()
        {
            rectTiles.Clear();
            startTile = null;
            endTile = null;
        }

        public void ResetState()
        {
            // First, we clear all tiles.
            ClearPaintedTiles();
            
            // Reset all of our bools.
            buildingMode = false;
            blockMouseInteractions = false;
            
            // And reset our Harmony patch bools.
            HarmonyPatches.Patches.CurrentlyInBuildMode = false;
            HarmonyPatches.Patches.AllowPlacement = false;
        }

        public void EraseTile(Vector2 tile, ModEntry modEntry)
        {
            Vector2 flaggedForRemoval = new Vector2();

            foreach (var item in tilesSelected)
            {
                if (item.Key == tile)
                {
                    // If we're over a tile in _tilesSelected, remove it and refund the item to the player.
                    Game1.player.addItemToInventoryBool(item.Value.Item.getOne(), false);
                    logger.Log($"{item.Value.Item.Name} {I18n.SmartBuilding_Info_RefundedIntoPlayerInventory()}");

                    // And flag it for removal from the queue, since we can't remove from within the foreach.
                    flaggedForRemoval = tile;
                }
            }

            tilesSelected.Remove(flaggedForRemoval);
        }

        /// <summary>
        /// There is no queue for item insertion, as the only method available to determine whether an item can be inserted is to insert it.
        /// </summary>
        /// <param name="targetTile"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryToInsertHere(Vector2 targetTile, Item item, ModEntry modEntry)
        {
            // First, we need to ensure there's an SObject here.
            if (Game1.currentLocation.objects.ContainsKey(targetTile))
            {
                // There is one, so we grab a reference to it.
                SObject o = Game1.currentLocation.objects[targetTile];

                // We also need to know what type of producer we're looking at, if any.
                ProducerType type = identificationUtils.IdentifyProducer(o);

                // Whether or not we need to manually deduct the item.
                bool needToDeduct = false;

                // If this isn't a producer, we return immediately.
                if (type == ProducerType.NotAProducer)
                {
                    return false;
                }
                else if (type == ProducerType.ManualRemoval)
                {
                    // If this requires manual removal, we mark that we do need to manually deduct the item.
                    needToDeduct = true;
                }
                else if (type == ProducerType.AutomaticRemoval)
                {
                    // The producer in question removes automatically, so we don't need to manually deduct the item.
                    needToDeduct = false;
                }
                else if (type == ProducerType.TechnicallyNotAProducerButIsATorch)
                {
                    // And then we attempt to shove the held gem into the torch.
                    worldUtils.ShoveGemIntoTorch(item, o, targetTile);

                    return true;
                }

                worldUtils.InsertItem(item, o, needToDeduct);
            }

            // There was no object here, so we return false.
            return false;
        }

        public void AddTile(Item item, Vector2 v, ModEntry modEntry)
        {
            // If we're not in building mode, we do nothing.
            if (!buildingMode)
                return;

            // If the player isn't holding an item, we do nothing.
            if (Game1.player.CurrentItem == null)
                return;
            
            // If the item is not an SObject, we don't want to do anything with it.
            if (item is not SObject)
                return;

            // If the item cannot be placed here according to our own rules, we do nothing. This is to allow for slightly custom placement logic.
            if (!placementUtils.CanBePlacedHere(v, item))
                return;

            ItemInfo itemInfo = identificationUtils.GetItemInfo((SObject)item);

            // We only want to add the tile if the Dictionary doesn't already contain it. 
            if (!tilesSelected.ContainsKey(v))
            {
                // We then want to check if the item can even be placed in this spot.
                if (placementUtils.CanBePlacedHere(v, item))
                {
                    tilesSelected.Add(v, itemInfo);
                    Game1.player.reduceActiveItemByOne();
                }
            }
        }

        /// <summary>
        /// Clear the tiles in the drawn queue.
        /// </summary>
        public void ClearPaintedTiles()
        {
            // To clear the painted tiles, we want to iterate through our Dictionary, and refund every item contained therein.
            foreach (var t in tilesSelected)
            {
                playerUtils.RefundItem(t.Value.Item, I18n.SmartBuilding_Info_BuildCancelled(), LogLevel.Trace, false);
            }

            // And, finally, clear it.
            tilesSelected.Clear();

            // We also want to clear the rect tiles. No refunding necessary here, however, as items are only deducted when added to tilesSelected.
            rectTiles.Clear();
            startTile = null;
            endTile = null;
        }
    }
}