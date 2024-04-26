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
using DecidedlyShared.Logging;
using Microsoft.Xna.Framework;
using SmartBuilding.HarmonyPatches;
using SmartBuilding.UI;
using SmartBuilding.Utilities;
using StardewValley;

namespace SmartBuilding
{
    public class ModState
    {
        private readonly ModConfig config;
        private readonly IdentificationUtils identificationUtils;
        private readonly Logger logger;
        private readonly PlacementUtils placementUtils;
        private readonly PlayerUtils playerUtils;
        private readonly WorldUtils worldUtils;
        private bool buildingMode;
        private List<Vector2> rectTiles = new();

        // All of our drawing variables.
        private Dictionary<Vector2, ItemInfo> tilesSelected = new();

        public ModState(Logger logger, ModConfig config, PlayerUtils playerUtils, IdentificationUtils identificationUtils,
            WorldUtils worldUtils, PlacementUtils placementUtils)
        {
            this.config = config;
            this.logger = logger;
            this.playerUtils = playerUtils;
            this.identificationUtils = identificationUtils;
            this.worldUtils = worldUtils;
            this.placementUtils = placementUtils;
        }

        public ButtonId ActiveTool { get; set; } = ButtonId.None;

        public bool InBuildMode { get; } = false;

        /// <summary>
        ///     Whether or not mouse buttons should apply to our UI only.
        /// </summary>
        public bool BlockMouseInteractions { get; set; }


        public void EnterBuildMode()
        {
        }

        public void LeaveBuildMode()
        {
            this.ActiveTool = ButtonId.None;
        }

        /// <summary>
        ///     Resets the *volatile*, temporary tiles used to draw the rectangle on-screen while drawing.
        /// </summary>
        public void ResetVolatileTiles()
        {
            this.rectTiles.Clear();
            this.StartTile = null;
            this.EndTile = null;
        }

        public void ResetState()
        {
            // First, we clear all tiles.
            this.ClearPaintedTiles();

            // Reset all of our bools.
            this.buildingMode = false;
            this.BlockMouseInteractions = false;

            // And reset our Harmony patch bools.
            Patches.CurrentlyInBuildMode = false;
            Patches.AllowPlacement = false;
        }

        public void EraseTile(Vector2 tile, ModEntry modEntry)
        {
            var flaggedForRemoval = new Vector2();

            foreach (var item in this.tilesSelected)
                if (item.Key == tile)
                {
                    // If we're over a tile in _tilesSelected, remove it and refund the item to the player.
                    Game1.player.addItemToInventoryBool(item.Value.Item.getOne());
                    this.logger.Log($"{item.Value.Item.Name} {I18n.SmartBuilding_Info_RefundedIntoPlayerInventory()}");

                    // And flag it for removal from the queue, since we can't remove from within the foreach.
                    flaggedForRemoval = tile;
                }

            this.tilesSelected.Remove(flaggedForRemoval);
        }

        /// <summary>
        ///     There is no queue for item insertion, as the only method available to determine whether an item can be inserted is
        ///     to insert it.
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
                var o = Game1.currentLocation.objects[targetTile];

                // We also need to know what type of producer we're looking at, if any.
                var type = this.identificationUtils.IdentifyProducer(o);

                // Whether or not we need to manually deduct the item.
                bool needToDeduct = false;

                // If this isn't a producer, we return immediately.
                if (type == ProducerType.NotAProducer)
                    return false;

                if (type == ProducerType.AutomaticRemoval)
                    // The producer in question removes automatically, so we don't need to manually deduct the item.
                    needToDeduct = false;
                else if (type == ProducerType.TechnicallyNotAProducerButIsATorch)
                {
                    // And then we attempt to shove the held gem into the torch.
                    this.worldUtils.ShoveGemIntoTorch(item, o, targetTile);

                    return true;
                }

                return this.worldUtils.TryInsertItem(item, o, needToDeduct);
            }

            // There was no object here, so we return false.
            return false;
        }

        public void AddTile(Item item, Vector2 v, ModEntry modEntry)
        {
            // If we're not in building mode, we do nothing.
            if (!this.buildingMode)
                return;

            // If the player isn't holding an item, we do nothing.
            if (Game1.player.CurrentItem == null)
                return;

            // If the item is not an SObject, we don't want to do anything with it.
            if (item is not SObject)
                return;

            // If the item cannot be placed here according to our own rules, we do nothing. This is to allow for slightly custom placement logic.
            if (!this.placementUtils.CanBePlacedHere(v, item))
                return;

            var itemInfo = this.identificationUtils.GetItemInfo((SObject)item);

            // We only want to add the tile if the Dictionary doesn't already contain it.
            if (!this.tilesSelected.ContainsKey(v))
                // We then want to check if the item can even be placed in this spot.
                if (this.placementUtils.CanBePlacedHere(v, item))
                {
                    this.tilesSelected.Add(v, itemInfo);

                    // If we're not in creative mode, we reduce the item by one.
                    if (!this.config.CreativeMode)
                        Game1.player.reduceActiveItemByOne();
                }
        }

        /// <summary>
        ///     Clear the tiles in the drawn queue.
        /// </summary>
        public void ClearPaintedTiles()
        {
            // To clear the painted tiles, we want to iterate through our Dictionary, and refund every item contained therein.
            foreach (var t in this.tilesSelected)
                this.playerUtils.RefundItem(t.Value.Item, I18n.SmartBuilding_Info_BuildCancelled());

            // And, finally, clear it.
            this.tilesSelected.Clear();

            // We also want to clear the rect tiles. No refunding necessary here, however, as items are only deducted when added to tilesSelected.
            this.rectTiles.Clear();
            this.StartTile = null;
            this.EndTile = null;
        }

        #region Properties

        public bool BuildingMode
        {
            get => this.buildingMode;
            set
            {
                this.buildingMode = value;
                Patches.CurrentlyInBuildMode = value;

                if (!this.buildingMode) // If this is now false, we want to clear the tiles list, refund everything, and kill our UI.
                {
                }
            }
        }

        public Dictionary<Vector2, ItemInfo> TilesSelected
        {
            get => this.tilesSelected;
            set => this.tilesSelected = value ?? throw new ArgumentNullException(nameof(value));
        }

        public List<Vector2> RectTiles
        {
            get => this.rectTiles;
            set => this.rectTiles = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Vector2? StartTile { get; set; }
        public Vector2? EndTile { get; set; }

        public Item RectangleItem { get; set; }

        public TileFeature SelectedLayer { get; set; } = TileFeature.None;

        #endregion
    }
}
