/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BetterGiantCrops
{
    public class DoGiantCrop : ResourceClump
    {

        public DoGiantCrop(int indexOfSmallerVersion, Vector2 tile)
        {
            this.tile.Value = tile;
            this.parentSheetIndex.Value = indexOfSmallerVersion;
            
            this.width.Value = 3;
            this.height.Value = 3;
            this.health.Value = 3f;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            spriteBatch.Draw(Game1.cropSpriteSheet, Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f - new Vector2((double)this.shakeTimer > 0.0 ? (float)(Math.Sin(2.0 * Math.PI / (double)this.shakeTimer) * 2.0) : 0.0f, 64f)), new Rectangle?(new Rectangle(112 * 48, 512, 48, 63)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(((double)tileLocation.Y + 2.0) * 64.0 / 10000.0));
        }

        public override bool performToolAction(
            Tool t,
            int damage,
            Vector2 tileLocation,
            GameLocation location)
        {
            /*
            if (t == null || !(t is Axe))
                return false;
            location.playSound("axchop");
            this.health.Value -= (float)(t.getLastFarmerToUse().toolPower + 1);
            Game1.createRadialDebris(Game1.currentLocation, 12, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(4, 9), false, -1, false, -1);
            if ((double)this.shakeTimer <= 0.0)
                this.shakeTimer = 100f;
            if ((double)(float)((NetFieldBase<float, NetFloat>)this.health) > 0.0)
                return false;
            t.getLastFarmerToUse().gainExperience(5, 50 * (((int)((NetFieldBase<int, NetInt>)t.getLastFarmerToUse().luckLevel) + 1) / 2));
            if (t.getLastFarmerToUse().hasMagnifyingGlass)
            {
                StardewValley.Object unseenSecretNote = location.tryToCreateUnseenSecretNote(t.getLastFarmerToUse());
                if (unseenSecretNote != null)
                    Game1.createItemDebris((Item)unseenSecretNote, tileLocation * 64f, -1, (GameLocation)null, -1);
            }
            Random random;
            if (Game1.IsMultiplayer)
            {
                Game1.recentMultiplayerRandom = new Random((int)tileLocation.X * 1000 + (int)tileLocation.Y);
                random = Game1.recentMultiplayerRandom;
            }
            else
                random = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)tileLocation.X * 7 + (int)tileLocation.Y * 11);
            int num = random.Next(15, 22);
            if (Game1.IsMultiplayer)
                Game1.createMultipleObjectDebris((int)((NetFieldBase<int, NetInt>)this.parentSheetIndex), (int)tileLocation.X + 1, (int)tileLocation.Y + 1, num, t.getLastFarmerToUse().UniqueMultiplayerID);
            else
                Game1.createRadialDebris(Game1.currentLocation, (int)((NetFieldBase<int, NetInt>)this.parentSheetIndex), (int)tileLocation.X, (int)tileLocation.Y, num, false, -1, true, -1);
            Game1.setRichPresence("giantcrop", (object)new StardewValley.Object(Vector2.Zero, (int)((NetFieldBase<int, NetInt>)this.parentSheetIndex), 1).Name);
            Game1.createRadialDebris(Game1.currentLocation, 12, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(4, 9), false, -1, false, -1);
            location.playSound("stumpCrack");
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, tileLocation * 64f, Color.White, 8, false, 100f, 0, -1, -1f, -1, 0));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 0.0f)) * 64f, Color.White, 8, false, 110f, 0, -1, -1f, -1, 0));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 1f)) * 64f, Color.White, 8, true, 80f, 0, -1, -1f, -1, 0));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(0.0f, 1f)) * 64f, Color.White, 8, false, 90f, 0, -1, -1f, -1, 0));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, tileLocation * 64f + new Vector2(32f, 32f), Color.White, 8, false, 70f, 0, -1, -1f, -1, 0));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, tileLocation * 64f, Color.White, 8, false, 100f, 0, -1, -1f, -1, 0));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(2f, 0.0f)) * 64f, Color.White, 8, false, 110f, 0, -1, -1f, -1, 0));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(2f, 1f)) * 64f, Color.White, 8, true, 80f, 0, -1, -1f, -1, 0));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(2f, 2f)) * 64f, Color.White, 8, false, 90f, 0, -1, -1f, -1, 0));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, tileLocation * 64f + new Vector2(96f, 96f), Color.White, 8, false, 70f, 0, -1, -1f, -1, 0));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(0.0f, 2f)) * 64f, Color.White, 8, false, 110f, 0, -1, -1f, -1, 0));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 2f)) * 64f, Color.White, 8, true, 80f, 0, -1, -1f, -1, 0));
            return true;*/
            return false;
        }

    }
}
