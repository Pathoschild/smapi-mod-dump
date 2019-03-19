using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;

namespace Thor.Stardew.Mods.HealthBars
{
    /// <summary>
    /// Main class of the mod
    /// </summary>
    public class ModEntry : Mod
    {
        /// <summary>
        /// Texture that is used to draw lifebar
        /// </summary>
        private Texture2D _whitePixel;
        /// <summary>
        /// Contains the configuration of the mod
        /// </summary>
        private ModConfig _config;

        /// <summary>
        /// Border texture of the lifebar
        /// </summary>
        private static Texture2D lifebarBorder;

        /// <summary>
        /// Available colour schemes of the life bar
        /// </summary>
        private static readonly Color[][] ColourSchemes =
        {
            new Color[] { Color.LawnGreen, Color.YellowGreen, Color.Gold, Color.DarkOrange, Color.Crimson },
            new Color[] { Color.Crimson, Color.DarkOrange, Color.Gold, Color.YellowGreen, Color.LawnGreen },
        };

        /// <summary>
        /// Mod initialization method
        /// </summary>
        /// <param name="helper">helper provided by SMAPI</param>
        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<ModConfig>();
            EnsureCorrectConfig();
            lifebarBorder = helper.Content.Load<Texture2D>(@"assets/SDV_lifebar.png", ContentSource.ModFolder);
            helper.Events.Display.RenderedWorld += RenderLifeBars;
        }

        /// <summary>
        /// Method that ensure the configuration provided by user is correct and will not break the game
        /// </summary>
        private void EnsureCorrectConfig()
        {
            bool needUpdateConfig = false;
            if (_config.ColorScheme >= ColourSchemes.Length || _config.ColorScheme < 0)
            {
                _config.ColorScheme = 0;
                needUpdateConfig = true;
            }

            if (needUpdateConfig)
            {
                Helper.WriteConfig(_config);
            }
        }

        /// <summary>
        /// Handle the rendering of mobs life bars
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event parameters</param>
        private void RenderLifeBars(object sender, RenderedWorldEventArgs e)
        {
            
            if (!Context.IsWorldReady || Game1.currentLocation == null || Game1.gameMode == 11 || Game1.currentMinigame != null || Game1.showingEndOfNightStuff || Game1.gameMode == 6 || Game1.gameMode == 0 || Game1.menuUp || Game1.activeClickableMenu != null) return;

            if (_whitePixel == null)
            {
                _whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
                _whitePixel.SetData(new Color[] { Color.White });
            }

            // Iterate through all NPC
            foreach (NPC character in Game1.currentLocation.characters)
            {
                // We only care about monsters
                if (!(character is Monster))
                {
                    continue;
                }
                Monster monster = (Monster)character;
                // If monster is not visible, next
                if (monster.isInvisible || !Utility.isOnScreen(monster.position, 3 * Game1.tileSize))
                {
                    continue;
                }

                // Check if the current monster should not display life bar
                if (monster is RockCrab || monster is LavaCrab)
                {
                    if (monster.Sprite.CurrentFrame % 4 == 0) continue;
                }
                else if (monster is RockGolem)
                {
                    if (monster.health == monster.maxHealth) continue;
                }
                else if (monster is Bug)
                {
                    if (((Bug)monster).isArmoredBug) continue;
                }
                else if (monster is Grub)
                {
                    if (monster.Sprite.CurrentFrame == 19) continue;
                }


                // Get all infos about the monster
                int health = monster.Health;
                int maxHealth = monster.MaxHealth;
                if (health > maxHealth) maxHealth = health;
                
                // If monster has already been killed once by player, we get the number of kills, else it's 0
                int monsterKilledAmount = Game1.stats.specificMonstersKilled.ContainsKey(monster.name) ? Game1.stats.specificMonstersKilled[monster.name] : 0;
                String healthText = "???";

                // By default, color bar is grey
                Color barColor = Color.DarkSlateGray;
                // By default, color bar full
                float barLengthPercent = 1f;

                TextProps textProps = new TextProps()
                {
                    Font = Game1.smallFont,
                    Color = Color.AntiqueWhite,
                    Scale = Globals.TEXT_SPEC_CHAR_SCALE_LEVEL,
                    BottomOffset = Globals.TEXT_SPEC_CHAR_OFFSET
                };

                bool useAlternateSprite = true;

                // If level system is deactivated or the basic level is OK, we display the colours
                if (!_config.EnableXPNeeded || monsterKilledAmount + Game1.player.combatLevel > Globals.EXPERIENCE_BASIC_STATS_LEVEL)
                {
                    useAlternateSprite = false;
                    textProps.Color = Color.DarkSlateGray;
                    float monsterHealthPercent = (float)health / (float)maxHealth;
                    if (monsterHealthPercent > 0.9f) barColor = ColourSchemes[_config.ColorScheme][0];
                    else if (monsterHealthPercent > 0.65f) barColor = ColourSchemes[_config.ColorScheme][1];
                    else if (monsterHealthPercent > 0.35f) barColor = ColourSchemes[_config.ColorScheme][2];
                    else if (monsterHealthPercent > 0.15f) barColor = ColourSchemes[_config.ColorScheme][3];
                    else barColor = ColourSchemes[_config.ColorScheme][4];

                    // If level system is deactivated or the full level is OK, we display the stats
                    if (!_config.EnableXPNeeded || monsterKilledAmount + Game1.player.combatLevel * 4 > Globals.EXPERIENCE_FULL_STATS_LEVEL)
                    {
                        barLengthPercent = monsterHealthPercent;
                        // If it's a very strong monster, we hide the life counter
                        if (_config.EnableXPNeeded && monster.health > 999)
                        {
                            healthText = "!!!";
                        }
                        else
                        {
                            healthText = String.Format("{0:000}", health);
                            textProps.Font = Game1.tinyFont;
                            textProps.Scale = Globals.TEXT_DEFAUT_SCALE_LEVEL;
                            textProps.BottomOffset = Globals.TEXT_DEFAUT_OFFSET;
                        }
                    }
                }

                // Display the life bar
                Vector2 monsterLocalPosition = monster.getLocalPosition(Game1.viewport);
                Vector2 lifebarCenterPos = new Vector2(monsterLocalPosition.X + (float)monster.Sprite.SpriteWidth * Game1.pixelZoom / 2,(float)monsterLocalPosition.Y - ((float)monster.Sprite.SpriteHeight + 5) * Game1.pixelZoom / 2);

                // If we use alternate sprite (do not show life level)
                if (useAlternateSprite)
                {
                    //Display background of the bar
                    Game1.spriteBatch.Draw(
                        lifebarBorder,
                        lifebarCenterPos,
                        new Rectangle(0, Globals.SPRITE_HEIGHT * Globals.SPRITE_INDEX_DEACTIVATED, lifebarBorder.Width, Globals.SPRITE_HEIGHT),
                        Color.White * 1f,
                        0f,
                        new Vector2(lifebarBorder.Width / 2, Globals.SPRITE_HEIGHT / 2),
                        1f,
                        SpriteEffects.None,
                        0f);
                }
                else
                {
                    //Display background of the bar
                    Game1.spriteBatch.Draw(
                        lifebarBorder,
                        lifebarCenterPos,
                        new Rectangle(0, Globals.SPRITE_HEIGHT * Globals.SPRITE_INDEX_BACK, lifebarBorder.Width, Globals.SPRITE_HEIGHT),
                        Color.White * 1f,
                        0f,
                        new Vector2(lifebarBorder.Width / 2, Globals.SPRITE_HEIGHT / 2),
                        1f,
                        SpriteEffects.None,
                        0f);

                    //Calculate size of the lifebox
                    Rectangle lifeBox = new Rectangle(0, 0, (int)((lifebarBorder.Width - Globals.LIFEBAR_MARGINS * 2) * barLengthPercent), Globals.SPRITE_HEIGHT - Globals.LIFEBAR_MARGINS * 2);
                    Vector2 internalLifebarPos = new Vector2(lifebarCenterPos.X - lifebarBorder.Width / 2 + Globals.LIFEBAR_MARGINS, lifebarCenterPos.Y);
                    //Display life bar
                    Game1.spriteBatch.Draw(
                        _whitePixel,
                        internalLifebarPos,
                        lifeBox,
                        barColor,
                        0f,
                        new Vector2(0, lifeBox.Height / 2f),
                        1f,
                        SpriteEffects.None,
                        0f);
                }

                // Display life count
                Color textColor = (barColor == Color.DarkSlateGray || barLengthPercent < 0.35f) ? Color.AntiqueWhite : Color.DarkSlateGray;
                // Draw text
                Vector2 textsize = textProps.Font.MeasureString(healthText);
                Game1.spriteBatch.DrawString(
                    textProps.Font,
                    healthText,
                    lifebarCenterPos,
                    textProps.Color,
                    0f,
                    new Vector2(textsize.X / 2, textsize.Y / 2 + textProps.BottomOffset),
                    textProps.Scale,
                    SpriteEffects.None,
                    0f);

                // If we display alternate sprite, there is no foreground
                if (!useAlternateSprite)
                {
                    //Display foreground of the bar
                    Game1.spriteBatch.Draw(
                    lifebarBorder,
                    lifebarCenterPos,
                    new Rectangle(0, Globals.SPRITE_HEIGHT * Globals.SPRITE_INDEX_FRONT, lifebarBorder.Width, Globals.SPRITE_HEIGHT),
                    Color.White * 1.0f,
                    0f,
                    new Vector2(lifebarBorder.Width / 2f, Globals.SPRITE_HEIGHT / 2f),
                    1f,
                    SpriteEffects.None,
                    0f);
                }
            }
        }
    }
}
