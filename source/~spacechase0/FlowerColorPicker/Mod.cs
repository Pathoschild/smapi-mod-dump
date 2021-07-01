/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace FlowerColorPicker
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod Instance;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            Log.Monitor = this.Monitor;

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || e.Button != SButton.MouseLeft)
                return;

            // get dirt under cursor
            Vector2 mouseTilePos = e.Cursor.Tile;
            if (!Game1.player.currentLocation.terrainFeatures.TryGetValue(mouseTilePos, out TerrainFeature terrainFeature) || !(terrainFeature is HoeDirt dirt))
                return;

            // get held colored object
            if (!(Game1.player.ActiveObject is ColoredObject held))
                return;

            // apply color
            if (dirt.crop != null && held.ParentSheetIndex == dirt.crop.indexOfHarvest.Value)
                dirt.crop.tintColor.Value = held.color.Value;
        }
    }
}
