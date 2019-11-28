using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace AutoToolSelector
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += ButtonPressed;
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.IsDown(SButton.NumPad6))
            {
                GameLocation location = Game1.getLocationFromName("BusStop");

                int tileX = 11;
                int tileY = 23;

                location.removeTile(tileX, tileY, "Back");
                var seas = Game1.currentSeason;
                Layer layer = location.map.GetLayer("Back");
                TileSheet tileSheet = location.map.GetTileSheet($"{seas}_outdoorsTileSheet");
                layer.Tiles[tileX, tileY] = new StaticTile(layer, tileSheet, BlendMode.Alpha, tileIndex: 75);
            }

            if (e.IsDown(SButton.NumPad5))
            {
                string[] layers = {"Back", "Buildings", "Front", "AlwaysFront"};
                if (!layers.Contains(stryLayer))
                {
                    parseError = true;
                    //do more stuff
                }
            }
        }

        private bool CustomAction(GameLocation loc, int xCoord, int yCoord, string cAction)
        {
            GameLocation location = loc;
            int x = xCoord;
            int y = yCoord;
            Tile tile = location.Map.GetLayer("Buildings").Tiles[x, y];

            if (tile != null && tile.TileIndex != -1)
            {
                tile.TileIndexProperties.TryGetValue("Action", out var propertyVal);
                if (propertyVal == null)
                    tile.Properties.TryGetValue("Action", out propertyVal);
                if (propertyVal == null)
                    return true;
                string str = propertyVal.ToString();
                if (!str.Contains(cAction))
                    return true;
            }

            return false;
        }
    }
}
