/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/


using DeepWoodsMod.API.Impl;
using DeepWoodsMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;

namespace DeepWoodsMod.Stuff
{
    public class CuteSign : LargeTerrainFeature
    {
        private NetInt row = new NetInt(0);

        public CuteSign()
           : base(false)
        {
            InitNetFields();
            this.row.Value = new Random().Next(0, 4);
        }

        public CuteSign(Vector2 tileLocation)
            : this()
        {
            this.tilePosition.Value = tileLocation;
        }

        private void InitNetFields()
        {
            this.NetFields.AddFields(this.row);
        }

        public override bool isActionable()
        {
            return true;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Rectangle((int)tileLocation.X * 64 + 8, (int)tileLocation.Y * 64, 128 - 24, 64);
        }

        public override bool isPassable(Character c = null)
        {
            return false;
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            DeepWoodsQuestMenu.OpenQuestMenu(I18N.EntrySignMessage, new Response[1]
            {
                new Response("No", I18N.MessageBoxClose).SetHotKey(Keys.Escape)
            });

            return true;
        }

        public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
        {
            return false;
        }

        public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
        {
        }

        public override bool seasonUpdate(bool onLoad)
        {
            return false;
        }

        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            return false;
        }

        public override void performPlayerEntryAction(Vector2 tileLocation)
        {
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Vector2 globalPosition = tileLocation * 64f;

            int column;
            if (Game1.IsWinter) 
            {
                column = 2;
            }
            else if (Game1.IsSpring)
            {
                column = 3;
            }
            else if (Game1.IsFall)
            {
                column = 1;
            }
            else//if (Game1.IsSummer)
            {
                column = 0;
            }

            Rectangle bottomSourceRectangle = new Rectangle(column * 32, row.Value * 37 + 21, 32, 16);
            Vector2 globalBottomPosition = new Vector2(globalPosition.X, globalPosition.Y);

            Rectangle topSourceRectangle = new Rectangle(column * 32, row.Value * 37, 32, 21);
            Vector2 globalTopPosition = new Vector2(globalPosition.X, globalPosition.Y - 84);

            spriteBatch.Draw(DeepWoodsTextures.Textures.CuteSign, Game1.GlobalToLocal(Game1.viewport, globalTopPosition), topSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
            spriteBatch.Draw(DeepWoodsTextures.Textures.CuteSign, Game1.GlobalToLocal(Game1.viewport, globalBottomPosition), bottomSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
        }
    }
}
