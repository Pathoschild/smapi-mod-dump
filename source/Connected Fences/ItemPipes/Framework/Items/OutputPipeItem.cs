/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Util;


namespace ItemPipes.Framework.Items
{
    public abstract class OutputPipeItem : IOPipeItem
    {
        public OutputPipeItem() : base()
        {

        }

        public OutputPipeItem(Vector2 position) : base(position)
        {

        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            base.draw(spriteBatch, x, y);
            DataAccess DataAccess = DataAccess.GetDataAccess();
            if (DataAccess.LocationNodes.ContainsKey(Game1.currentLocation))
            {
                List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
                Node node = nodes.Find(n => n.Position.Equals(TileLocation));
                if (node != null && node is IOPipeNode)
                {
                    IOPipeNode IONode = (IOPipeNode)node;
                    if (IONode.Signal != null && !IONode.PassingItem)
                    {
                        UpdateSignal(IONode.Signal);
                        float transparency = 1f;
                        if (IONode.Passable)
                        {
                            Passable = true;
                            transparency = 0.5f;
                        }
                        else
                        {
                            Passable = false;
                            transparency = 1f;
                        }
                        Rectangle srcRect = new Rectangle(0, 0, 16, 16);
                        spriteBatch.Draw(SignalTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64)), srcRect, Color.White * transparency, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64 + 32) / 10000f) + 0.002f);
                    }
                    if (Globals.IOPipeStatePopup && IONode.Signal != null)
                    {
                        if (IONode.Signal.Equals("nochest"))
                        {
                            Texture2D sprite;
                            if (((int)Game1.currentGameTime.TotalGameTime.TotalSeconds) % 2 == 0)
                            {
                                sprite = DataAccess.Sprites["nochest_state"];
                            }
                            else
                            {
                                sprite = DataAccess.Sprites["nochest1_state"];
                            }
                            Popup.Draw(spriteBatch, sprite, x, y);
                        }
                        else if (IONode.Signal.Equals("unconnected"))
                        {
                            Texture2D sprite;
                            if (((int)Game1.currentGameTime.TotalGameTime.TotalSeconds) % 2 == 0)
                            {
                                sprite = DataAccess.Sprites["InserterPipe_Item"];
                            }
                            else
                            {
                                sprite = DataAccess.Sprites["unconnected1_state"];
                            }
                            Popup.Draw(spriteBatch, sprite, x, y);
                        }
                    }
                }
            }
        }
    }
}
