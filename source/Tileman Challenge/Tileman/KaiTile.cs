/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spicykai/StardewValley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;


namespace Tileman
{
    public class KaiTile 
    {

        public int tileX = 0;
        public int tileY = 0;
        public int tileW = Game1.tileSize;
        public int tileH = Game1.tileSize;


        public string tileIsWhere = null;
        public KaiTile(int TileX,int TileY, string TileIsWhere)
        {
            this.tileX = TileX;
            this.tileY = TileY;
            this.tileIsWhere = TileIsWhere;
        }


        public void DrawTile(Texture2D texture, SpriteBatch spriteBatch) 
        {

                float offsetX = Game1.viewport.X;
                float offsetY = Game1.viewport.Y;


                spriteBatch.Draw(texture,
                    Utility.getRectangleCenteredAt(new Vector2((this.tileX + 1) * 64 - offsetX - 32, (this.tileY + 1) * 64 - offsetY - 32), 64),
                    Color.White);



        }

        

        public bool IsSpecifiedTile(int TileX, int TileY, string TileIsWhere) {
            if (TileX == tileX && TileY == tileY && TileIsWhere == tileIsWhere) return true;
            return false;
        }


    }
}