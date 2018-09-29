using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Objects;
using Revitalize.Persistence;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Magic
{
    class MagicMonitor
    {
        public static string magicalTexturesPath = Path.Combine("Revitalize", "MagicSystem", "Graphics");
        public static string magicalDataPath = Path.Combine("Revitalize", "MagicSystem", "Data");
        public static int penaltyCost = 5;
        public static void drawMagicMeter()
        {
            // Log.AsyncR("I AM BEING CALLED");
            //Draw Stamina Text
            //  Game1.drawWithBorder((int)Math.Max(0f, Game1.player.Stamina) + "/" + Game1.player.MaxStamina, Color.Black, Color.White, vector + new Vector2(-Game1.dialogueFont.MeasureString("999/999").X - (float)(Game1.tileSize / 4) - (float)((Game1.currentLocation.Name.Equals("UndergroundMine") || PlayerVariables.Magic < 100) ? Game1.tileSize : 0), (float)Game1.tileSize));

            //Draw health and bar
            if (!Game1.eventUp)
            {
                string path = Path.Combine("Revitalize", "MagicSystem", "Graphics", "magicMeterTop");
                Texture2D magicMeterTexture = Game1.content.Load<Texture2D>(path);

                Color magicColor = Color.Aqua;

                var offset = Game1.graphics.GraphicsDevice.Viewport.Width * .8f;
                float num = 0.625f;

                Vector2 vector = new Vector2((float)(Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 48 - Game1.tileSize / 8) - offset, (float)(Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 224 - Game1.tileSize / 4 - (int)((float)(Game1.player.MaxStamina - 270) * num)));


                if (PlayerVariables.CurrentMagic <= PlayerVariables.MaxMagic)
                {
                    // Log.AsyncC("YUP");
                    vector.X = (float)(48 + Game1.tileSize / 2 + offset - ((Game1.hitShakeTimer > 0) ? Game1.random.Next(-3, 4) : 0));
                    //  vector.X = (float)(48 + Game1.tileSize / 8 + ((Game1.hitShakeTimer > 0) ? Game1.random.Next(-3, 4) : 0));

                    vector.Y = (float)(Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 224 - (PlayerVariables.MaxMagic - 100) - Game1.tileSize / 4 + 4);
                    //prob the health symbol
                    if (PlayerVariables.CurrentMagic > 0)
                    {
                        Game1.spriteBatch.Draw(magicMeterTexture, vector, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 12, 16)), (PlayerVariables.CurrentMagic < 20) ? (magicColor * ((float)Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / (double)((float)PlayerVariables.CurrentMagic * 50f)) / 4f + 0.9f)) : Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                        Game1.spriteBatch.Draw(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle((int)vector.X, (int)(vector.Y + (float)Game1.tileSize), 48, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - Game1.tileSize - Game1.tileSize / 4 - (int)(vector.Y + (float)Game1.tileSize)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(268, 424, 12, 16)), (PlayerVariables.CurrentMagic < 20) ? (magicColor * ((float)Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / (double)((float)PlayerVariables.CurrentMagic * 50f)) / 4f + 0.9f)) : Color.White);
                        Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(vector.X, vector.Y + 220f + (float)(PlayerVariables.MaxMagic - 100) - 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(268, 448, 12, 16)), (PlayerVariables.CurrentMagic < 20) ? (magicColor * ((float)Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / (double)((float)PlayerVariables.CurrentMagic * 50f)) / 4f + 0.9f)) : Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                    }
                    else
                    {
                        Game1.spriteBatch.Draw(magicMeterTexture, vector, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 12, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                        Game1.spriteBatch.Draw(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle((int)vector.X, (int)(vector.Y + (float)Game1.tileSize), 48, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - Game1.tileSize - Game1.tileSize / 4 - (int)(vector.Y + (float)Game1.tileSize - 8f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(256, 424, 12, 16)), Color.White);
                        Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(vector.X, vector.Y + 224f + (float)((int)((float)(PlayerVariables.MaxMagic - 100) * num)) - (float)Game1.tileSize), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(256, 448, 12, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                    }
                    int num2 = (int)((float)PlayerVariables.CurrentMagic / (float)PlayerVariables.MaxMagic * (float)(168 + (PlayerVariables.MaxMagic - 100)));
                    //     redToGreenLerpColor = Utility.getRedToGreenLerpColor((float)PlayerVariables.CurrentMagic / (float)PlayerVariables.MaxMagic);

                    //The actual color bar
                    Game1.spriteBatch.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int)vector.X + 12, (int)vector.Y + Game1.tileSize / 2 + (Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - Game1.tileSize - Game1.tileSize / 4 - (int)vector.Y + 24 - num2), 24, num2), new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), Util.invertColor(Resources.LightColors.Aqua), 0f, Vector2.Zero, SpriteEffects.None, 1f);
                    //  redToGreenLerpColor.R = (byte)Math.Max(0, (int)(redToGreenLerpColor.R - 50));
                    //   redToGreenLerpColor.G = (byte)Math.Max(0, (int)(redToGreenLerpColor.G - 50));
                    if ((float)Game1.getOldMouseX() >= vector.X && (float)Game1.getOldMouseY() >= vector.Y && (float)Game1.getOldMouseX() < vector.X + (float)(Game1.tileSize / 2))
                    {
                        Game1.drawWithBorder(Math.Max(0, PlayerVariables.CurrentMagic) + "/" + PlayerVariables.MaxMagic, Color.Black, magicColor, vector + new Vector2(-Game1.dialogueFont.MeasureString("999/999").X - (float)(Game1.tileSize / 2), (float)Game1.tileSize));
                    }
                    Game1.spriteBatch.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int)vector.X + 12, (int)vector.Y + Game1.tileSize / 2 + (Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - Game1.tileSize - Game1.tileSize / 4 - (int)vector.Y + 24 - num2), 24, Game1.pixelZoom), new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), Util.invertColor(Resources.LightColors.Aqua), 0f, Vector2.Zero, SpriteEffects.None, 1f);
                }
            }

        }

        public static void consumeMagic(int magicToConsume)
        {
            if (PlayerVariables.CurrentMagic < magicToConsume)
            {
                int i = (magicToConsume - PlayerVariables.CurrentMagic) *penaltyCost;

                if (Game1.player.stamina < i)
                {
                    int j =(int) ((i) - Game1.player.stamina);
                    if (Game1.player.health < j)
                    {
                        Game1.farmerShouldPassOut = true;
                        Game1.player.health = 0;
                        Game1.player.stamina = 0;
                        Game1.player.exhausted = true;
                        MagicMonitor.gainMagicExperience(PlayerVariables.CurrentMagic);
                        PlayerVariables.CurrentMagic = 0;
                    }
                    else
                    {
                        MagicMonitor.gainMagicExperience(PlayerVariables.CurrentMagic);
                        PlayerVariables.CurrentMagic = 0;
                        Game1.player.stamina = 0;
                        Game1.player.health -= j;
                    }

                }
                else
                {
                    MagicMonitor.gainMagicExperience(PlayerVariables.CurrentMagic);
                    PlayerVariables.CurrentMagic = 0;
                    Game1.player.stamina -= i;
                }

            }
            else
            {
                MagicMonitor.gainMagicExperience(magicToConsume);
                PlayerVariables.CurrentMagic -= magicToConsume;
            }


        }

        public static void consumeUsage(Spell s)
        {
            if (s.usesRemaining == -1) return;
            else
            {
                s.usesRemaining -= 1;
                if (s.usesRemaining == 0)
                {
                    Game1.player.reduceActiveItemByOne();
                }
                else
                {
                    s.description = s.vanillaDescription += "\nUses Remaining: " + s.usesRemaining;
                }
            }
        }

        public static void gainMagicExperience(int f)
        {
            PlayerVariables.MagicExperience += f;
            while (PlayerVariables.MagicExperience >= PlayerVariables.MagicExperienceToLevelUp)
            {
               // Log.AsyncR("WTF");
                magicLevelUp();
            }
        }

        public static void magicLevelUp()
        {
            PlayerVariables.MagicLevel++;
            PlayerVariables.MagicProficiency += PlayerVariables.magicProficiencyToGainUponLevelUp;
            PlayerVariables.MagicExperienceToLevelUp +=(int)( (PlayerVariables.MagicExperienceToLevelUp * PlayerVariables.ExpCurve));
            PlayerVariables.MaxMagic += PlayerVariables.MagicToGainUponLevelUp;

            Game1.addHUDMessage(new HUDMessage("Magic Leveled Up!", 1));

        }

        public static void castMagic()
        {
            if (Game1.player.ActiveObject as Spell != null) (Game1.player.ActiveObject as Spell).castMagic();
        }

    }
}
