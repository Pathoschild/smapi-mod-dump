/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Xml.Serialization;

namespace BNC.Twitch
{
    class TwitchFly : Fly, ITwitchMonster
    {
        public TwitchFly() { }

        public TwitchFly(Vector2 position, bool hard) : base(position, hard)
        {
        }

        [XmlIgnore]
        public string TwitchName { get; set; } = "null";

        public string GetTwitchName()
        {
            return TwitchName;
        }

        public void setTwitchName(string username)
        {
            TwitchName = username;
            this.Name = username;
            this.displayName = username;
        }

        public override void draw(SpriteBatch b)
        {
            drawAboveAllLayers(b);
        }

        public override void drawAboveAllLayers(SpriteBatch b)
        {
            if (!Utility.isOnScreen(this.Position, 128))
                return;
            b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 32f), new Rectangle?(this.Sprite.SourceRect), Color.White, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.92f);
            SpriteBatch spriteBatch = b;
            Texture2D shadowTexture = Game1.shadowTexture;
            Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, 64f);
            Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
            Color white = Color.White;
            double num1 = 0.0;
            Rectangle bounds = Game1.shadowTexture.Bounds;
            double x = (double)bounds.Center.X;
            bounds = Game1.shadowTexture.Bounds;
            double y = (double)bounds.Center.Y;
            Vector2 origin = new Vector2((float)x, (float)y);
            double num2 = 4.0;
            int num3 = 0;
            //double num4 = this.wildernessFarmMonster ? 9.99999974737875E-05 : (double)(this.getStandingY() - 1) / 10000.0;
            double num4 = 1f;
            spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, (float)num1, origin, (float)num2, (SpriteEffects)num3, (float)num4);
            if (!this.isGlowing)
                return;
            b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 32f), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.99f : (float)((double)this.getStandingY() / 10000.0 + 1.0 / 1000.0)));
        }
    }
}
