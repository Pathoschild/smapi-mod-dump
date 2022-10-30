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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;

namespace DeepWoodsMod.Stuff
{
    public class MaxHut : LargeTerrainFeature
    {
        int height = 112;
        int width = 80;

        int distToTop = 64;
        int distToBottom = 16;

        float alpha = 1f;

        public MaxHut()
           : base(true)
        {
        }

        public MaxHut(Vector2 tileLocation)
            : this()
        {
            this.tilePosition.Value = tileLocation;
        }

        public override bool isActionable()
        {
            return false;
        }

        // 80px x 124px

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Rectangle((int)tileLocation.X * 16 * 4, ((int)tileLocation.Y * 16 + distToTop) * 4, width * 4, (height - distToTop - distToBottom) * 4);
        }

        public override bool isPassable(Character c = null)
        {
            return false;
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            return false;
        }

        public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
        {
            alpha = Math.Min(1f, alpha + 0.05f);
            if (Game1.player.GetBoundingBox().Intersects(new Rectangle((int)tileLocation.X * 16 * 4, (int)tileLocation.Y * 16 * 4, width * 4, distToTop * 4)))
            {
                alpha = Math.Max(0.4f, alpha - 0.09f);
            }
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
            var globalPosition = tileLocation * 16 * 4;

            int column;
            if (Game1.IsWinter)
            {
                column = 3;
            }
            else if (Game1.IsSpring)
            {
                column = 1;
            }
            else if (Game1.IsFall)
            {
                column = 0;
            }
            else//if (Game1.IsSummer)
            {
                column = 2;
            }

            int row;
            if (Game1.timeOfDay >= Game1.getStartingToGetDarkTime() || Game1.isRaining)
            {
                row = 1;
            }
            else
            {
                row = 0;
            }

            var sourceRectangle = new Rectangle(column * 80, row * 112, 80, 112);

            spriteBatch.Draw(DeepWoodsTextures.Textures.MaxHut, Game1.GlobalToLocal(Game1.viewport, globalPosition), sourceRectangle, Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 4) * 64f / 10000f + tileLocation.X / 100000f));
        }

        public bool doAction(Vector2 tileLocation, Farmer who)
        {
            if (who.mount != null)
            {
                return false;
            }

            Vector2 doorPosition = this.tilePosition.Value + new Vector2(2, 5);

            if (tileLocation == doorPosition)
            {
                who.currentLocation.playSoundAt("doorClose", tileLocation);
                // for some reason in multiplayer, the game deletes this game location on clients
                // i can't for the life of me figure out where, when, or why, so i am just readding it here
                DeepWoodsManager.AddMaxHut();
                Game1.warpFarmer("DeepWoodsMaxHouse", 19, 24, Game1.player.FacingDirection, isStructure: false);
                return true;
            }

            return false;
        }

        public bool isActionableTile(Vector2 tileLocation, Farmer who)
        {
            Vector2 doorPosition = this.tilePosition.Value + new Vector2(2, 5);

            return doorPosition == tileLocation;
        }
    }
}
