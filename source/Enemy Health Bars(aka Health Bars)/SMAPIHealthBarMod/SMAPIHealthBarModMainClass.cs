/*
    Copyright 2016 Maurício Gomes (Speeder)

    Storm is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Speeder's SDV Mods are distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Speeder's SDV Mods.  If not, see <http://www.gnu.org/licenses/>.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;

namespace SMAPIHealthBarMod
{
    public class SMAPIHealthBarModMainClass : Mod
    {
        static Texture2D whitePixel;
        //static RenderTarget2D renderTarget;
        //static float lastZoomLevel;

        public static HealthBarConfig ModConfig { get; protected set; }

        static readonly Color[][] ColourSchemes =
        {
            new Color[] { Color.LawnGreen, Color.YellowGreen, Color.Gold, Color.DarkOrange, Color.Crimson },
            new Color[] { Color.Crimson, Color.DarkOrange, Color.Gold, Color.YellowGreen, Color.LawnGreen },
        };

        public override void Entry(params object[] objects)
        {            
            ModConfig = new HealthBarConfig().InitializeConfig(BaseConfigPath);
            if (ModConfig.ColourScheme < 0) ModConfig.ColourScheme = 0;
            if (ModConfig.ColourScheme >= ColourSchemes.Length) ModConfig.ColourScheme = ColourSchemes.Length-1;

            GraphicsEvents.DrawTick += drawTickEvent;
            whitePixel = null;
        }

        static void drawTickEvent(object sender, EventArgs e)
        {          
            if (Game1.currentLocation == null || Game1.gameMode == 11 || Game1.currentMinigame != null || Game1.showingEndOfNightStuff || Game1.gameMode == 6 || Game1.gameMode == 0 || Game1.menuUp || Game1.activeClickableMenu != null) return;

            if (whitePixel == null)
            {
                whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
                whitePixel.SetData(new Color[] { Color.White });
            }

            /*if (renderTarget == null || lastZoomLevel != Game1.options.zoomLevel)
            {
                try
                {
                    lastZoomLevel = Game1.options.zoomLevel;
                    renderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, Math.Min(4096, (int)((float)Program.gamePtr.Window.ClientBounds.Width * (1f / Game1.options.zoomLevel))), Math.Min(4096, (int)((float)Program.gamePtr.Window.ClientBounds.Height * (1f / Game1.options.zoomLevel))), false, SurfaceFormat.Color, DepthFormat.None);
                }
                catch (Exception)
                {
                }
            }*/

            /*if (Game1.options.zoomLevel != 1f)
            {
                Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget);
                Game1.graphics.GraphicsDevice.Clear(Color.Transparent);                
            }
            else
            {
                Game1.graphics.GraphicsDevice.SetRenderTarget(null);                
            }*/
            Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            foreach (NPC character in Game1.currentLocation.characters)
            {
                Monster monster;
                GreenSlime slime;                
                Rectangle monsterBox;
                Rectangle lifeBox;
                Vector2 monsterLocalPosition;
                Color barColor;
                String healthText;
                float monsterHealthPercent;
                float barLengthPercent;
                int monsterKilledAmount;
                if (character is Monster)
                {
                    monster = (Monster)character;                    

                    if(!monster.isInvisible && Utility.isOnScreen(monster.position, 3 * Game1.tileSize))
                    {
                        if (monster.health > monster.maxHealth) monster.maxHealth = monster.health;

                        if (Game1.stats.specificMonstersKilled.ContainsKey(monster.name))
                        {
                            monsterKilledAmount = Game1.stats.specificMonstersKilled[monster.name];
                        }
                        else
                        {
                            monsterKilledAmount = 0;
                        }

                        healthText = "???";
                        if (monsterKilledAmount + Game1.player.combatLevel > 15)
                        {
                            //basic stats
                            monsterHealthPercent = (float)monster.health / (float)monster.maxHealth;
                            barLengthPercent = 1f;
                            if (monsterHealthPercent > 0.9f) barColor = ColourSchemes[ModConfig.ColourScheme][0];
                            else if (monsterHealthPercent > 0.65f) barColor = ColourSchemes[ModConfig.ColourScheme][1];
                            else if (monsterHealthPercent > 0.35f) barColor = ColourSchemes[ModConfig.ColourScheme][2];
                            else if (monsterHealthPercent > 0.15f) barColor = ColourSchemes[ModConfig.ColourScheme][3];
                            else barColor = ColourSchemes[ModConfig.ColourScheme][4];

                            if (monsterKilledAmount + Game1.player.combatLevel * 4 > 45)
                            {
                                barLengthPercent = monsterHealthPercent;
                                if (monster.health > 999) healthText = "!!!";
                                else healthText = String.Format("{0:000}", monster.health);
                            }
                        }
                        else
                        {
                            barLengthPercent = 1f;                            
                            barColor = Color.DarkSlateGray;
                        }

                        monsterLocalPosition = monster.getLocalPosition(Game1.viewport);
                        monsterBox = new Rectangle((int)monsterLocalPosition.X, (int)monsterLocalPosition.Y-monster.sprite.spriteHeight/2 * Game1.pixelZoom, monster.sprite.spriteWidth * Game1.pixelZoom, 12);
                        if (monster is GreenSlime)
                        {
                            slime = (GreenSlime)monster;
                            if (slime.hasSpecialItem)
                            {
                                monsterBox.X -= 5;
                                monsterBox.Width += 10;
                            }
                            else if(slime.cute)
                            {
                                monsterBox.X -= 2;
                                monsterBox.Width += 4;
                            }
                            else
                            {
                                monsterBox.Y += 5 * Game1.pixelZoom;
                            }
                        }
                        else if (monster is RockCrab || monster is LavaCrab)
                        {
                            if(monster.sprite.CurrentFrame % 4 == 0) continue;                            
                        }
                        else if (monster is RockGolem)
                        {
                            if (monster.health == monster.maxHealth) continue;
                            monsterBox.Y = (int)monsterLocalPosition.Y - monster.sprite.spriteHeight * Game1.pixelZoom * 3 / 4;
                        }
                        else if(monster is Bug)
                        {
                            if (((Bug)monster).isArmoredBug) continue;
                            monsterBox.Y -= 15 * Game1.pixelZoom;
                        }
                        else if(monster is Grub)
                        {
                            if (monster.sprite.CurrentFrame == 19) continue;
                            monsterBox.Y = (int)monsterLocalPosition.Y - monster.sprite.spriteHeight * Game1.pixelZoom * 4 / 7;
                        }
                        else if(monster is Fly)
                        {
                            monsterBox.Y = (int)monsterLocalPosition.Y - monster.sprite.spriteHeight * Game1.pixelZoom * 5 / 7;
                        }
                        else if(monster is DustSpirit)
                        {
                            monsterBox.X += 3;
                            monsterBox.Width -= 6;
                            monsterBox.Y += 5 * Game1.pixelZoom;
                        }
                        else if(monster is Bat)
                        {
                            if (monster.sprite.CurrentFrame == 4) continue;
                            monsterBox.X -= 1;
                            monsterBox.Width -= 2;
                            monsterBox.Y += 1 * Game1.pixelZoom;
                        }
                        else if(monster is MetalHead || monster is Mummy)
                        {
                            monsterBox.Y -= 2 * Game1.pixelZoom;
                        }
                        else if(monster is Skeleton || monster is ShadowBrute || monster is ShadowShaman || monster is SquidKid)
                        {
                            if (monster.health == monster.maxHealth) continue;
                            monsterBox.Y -= 7 * Game1.pixelZoom;
                        }
                        monsterBox.X = (int)((float)monsterBox.X*Game1.options.zoomLevel);
                        monsterBox.Y = (int)((float)monsterBox.Y * Game1.options.zoomLevel);
                        monsterBox.Width = (int)((float)monsterBox.Width * Game1.options.zoomLevel);
                        monsterBox.Height = (int)((float)monsterBox.Height * Game1.options.zoomLevel);
                        lifeBox = monsterBox;
                        ++lifeBox.X;
                        ++lifeBox.Y;
                        lifeBox.Height = monsterBox.Height - 2;
                        lifeBox.Width = monsterBox.Width - 2;
                        Game1.spriteBatch.Draw(whitePixel, monsterBox, Color.BurlyWood);
                        Game1.spriteBatch.Draw(whitePixel, lifeBox, Color.SaddleBrown);
                        lifeBox.Width = (int)((float)lifeBox.Width*barLengthPercent);
                        Game1.spriteBatch.Draw(whitePixel, lifeBox, barColor);
                        if(barColor == Color.DarkSlateGray || barLengthPercent < 0.35f)
                            Utility.drawTextWithShadow(Game1.spriteBatch, healthText, Game1.smallFont, new Vector2(monsterBox.X+(float)monsterBox.Width/2-9*Game1.options.zoomLevel, monsterBox.Y+2), Color.AntiqueWhite, Game1.options.zoomLevel*0.4f, -1, 0, 0, 0, 0);
                        else
                            Utility.drawTextWithShadow(Game1.spriteBatch, healthText, Game1.smallFont, new Vector2(monsterBox.X + (float)monsterBox.Width / 2 - 9 * Game1.options.zoomLevel, monsterBox.Y + 2), Color.DarkSlateGray, Game1.options.zoomLevel * 0.4f, -1, 0, 0, 0, 0);
                    }
                }
            }
           
            Game1.spriteBatch.End();

            /*if (Game1.options.zoomLevel != 1f)
            {
                Game1.graphics.GraphicsDevice.SetRenderTarget(null);
                Game1.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Game1.spriteBatch.Draw(renderTarget, Vector2.Zero, new Microsoft.Xna.Framework.Rectangle?(renderTarget.Bounds), new Color(255, 255, 255, 127), 0f, Vector2.Zero, Game1.options.zoomLevel, SpriteEffects.None, 1f);
                Game1.spriteBatch.End();
            }*/
        }
    }

    public class HealthBarConfig : Config
    {
        public int ColourScheme { get; set; }

        public override T GenerateDefaultConfig<T>()
        {            
            ColourScheme = 0;
            return this as T;
        }        
    }
}
