using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;


namespace TwilightShards.LunarDisturbances
{
    class TCHUDMessage : HUDMessage
    {
        private MoonPhase messagePhase;

        public TCHUDMessage(string message, MoonPhase mPhase) : base(message)
        {
            this.messagePhase = mPhase;
            this.message = message;
            this.noIcon = false;
            color = Color.SeaGreen;
            timeLeft = 3500f;
        }

        private Rectangle GetMoonSpritePhase()
        {
            if (messagePhase == MoonPhase.NewMoon)
                return new Rectangle(0, 208, 16, 16);
            if (messagePhase == MoonPhase.WaxingCrescent)
                return new Rectangle(16,208, 16, 16);
            if (messagePhase == MoonPhase.FirstQuarter)
                return new Rectangle(64, 208, 16, 16);
            if (messagePhase == MoonPhase.WaxingGibbeous)
                return new Rectangle(80, 208, 16, 16);
            if (messagePhase == MoonPhase.FullMoon)
                return new Rectangle(98, 208, 16, 16);
            if (messagePhase == MoonPhase.BloodMoon)
                return new Rectangle(116, 208, 16, 16);
            if (messagePhase == MoonPhase.WaningGibbeous)
                return new Rectangle(134, 208, 16, 16);
            if (messagePhase == MoonPhase.ThirdQuarter)
                return new Rectangle(152, 208, 16, 16);
            if (messagePhase == MoonPhase.BlueMoon)
                return new Rectangle(188, 208, 16, 16);
            if (messagePhase == MoonPhase.HarvestMoon)
                return new Rectangle(207, 208, 16, 16);

            return new Rectangle(0, 180, 16, 16);
        }

#pragma warning disable IDE1006 // Naming Styles
        public new void draw(SpriteBatch b, int i)
#pragma warning restore IDE1006 // Naming Styles
        {
            Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
                Vector2 vector2 = new Vector2((float)(titleSafeArea.Left + 16), (float)(titleSafeArea.Bottom - (i + 1) * 64 * 7 / 4 - 64));
                if (Game1.isOutdoorMapSmallerThanViewport())
                    vector2.X = (float)Math.Max(titleSafeArea.Left + 16, -Game1.viewport.X + 16);
                if (Game1.viewport.Width < 1400)
                    vector2.Y -= 48f;

                float x = Game1.smallFont.MeasureString(this.message).X;
                b.Draw(Game1.mouseCursors, new Vector2(vector2.X + 104f, vector2.Y), new Rectangle?(new Rectangle(319, 360, 1, 24)), Color.White * this.transparency, 0.0f, Vector2.Zero, new Vector2(x, 4f), SpriteEffects.None, 1f);
                b.Draw(Game1.mouseCursors, new Vector2(vector2.X + 104f + x, vector2.Y), new Rectangle?(new Rectangle(323, 360, 6, 24)), Color.White * this.transparency, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                vector2.X += 16f;
                vector2.Y += 16f;
                b.Draw(LunarDisturbances.OurIcons.MoonSource, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(GetMoonSpritePhase()), Color.White * this.transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float)(((double)this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
                vector2.X += 51f;
                vector2.Y += 51f;
                if (this.number > 1)
                    Utility.drawTinyDigits(this.number, b, vector2, 3f, 1f, Color.White * this.transparency);
                vector2.X += 32f;
                vector2.Y -= 33f;
                Utility.drawTextWithShadow(b, this.message, Game1.smallFont, vector2, Game1.textColor * this.transparency, 1f, 1f, -1, -1, this.transparency, 3);
        }
    }
} 
