using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace Mizzion.Common
{
    internal class CommonHelper
    {
        //Grab Locations
        public static IEnumerable<GameLocation> GetLocations()
        {
            foreach(GameLocation locations in Game1.locations)
            {
                //Return current location
                yield return location;

                //Return Buildings
                if(location is BuildableGameLocation buildables)
                {
                    foreach(Building building in buildables.buildings)
                    {
                        if (building.indoors != null)
                            yield return building.indoors;
                    }
                }
            }
        }

        //Draw Hover Box
        public static Vector2 DrawHoverBox(SpriteBatch spriteBatch, string label, Vector2 position, float wrapWidth)
        {
            const int paddingSize = 27;
            const int gutterSize = 20;

            Vector2 labelSize = spriteBatch.DrawTextBlock(Game1.smallFont, label, position + new Vector2(gutterSize), wrapWidth); // draw text to get wrapped text dimensions
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)position.X, (int)position.Y, (int)labelSize.X + paddingSize + gutterSize, (int)labelSize.Y + paddingSize, Color.White);
            spriteBatch.DrawTextBlock(Game1.smallFont, label, position + new Vector2(gutterSize), wrapWidth); // draw again over texture box

            return labelSize + new Vector2(paddingSize);
        }
    }
}
