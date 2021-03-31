/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Abilities-Experience-Bars
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.BellsAndWhistles;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;

namespace AbilitiesExperienceBars
{
    public class ModEntry : StardewModdingAPI.Mod
    {
        #region //Variables
        //PRIVATE VARS
        private int configButtonPosX = 25, configButtonPosY = 10;
        private int defaultButtonPosX = 25, defaultButtonPosY = 10;
        private int maxLevel = 10;
        private int levelUpPosY;
        private bool inConfigMode;
        private int expAdvicePositionX;

        //PLAYER INFO VARS
        private int[] playerExperience;
        private int[] oldPlayerExperience = new int[10];
        private int[] playerModdedExperience = new int[3];
        private int[] oldPlayerModdedExperience = new int[3];
        private int[] playerLevels;
        private int[] oldPlayerLevels;
        private int[] oldPlayerModdedLevels = new int[3];

        //SPRITE VARS
        private Texture2D[] icons;
        private Texture2D backgroundTop,
            backgroundBottom,
            backgroundFiller,
            backgroundBar,
            backgroundExp,
            barFiller,
            backgroundBoxConfig,
            backgroundLevelUp,
            buttonConfig,
            buttonDecreaseSize,
            buttonIncreaseSize,
            backgroundButton,
            levelUpButton,
            experienceButton,
            buttonConfigApply,
            buttonVisibility,
            buttonHidden,
            buttonReset;

        //COLOR VARS
        private Color[] colors;
        private Color[] finalColors;
        private Color[] colorsRestoration;
        private Color globalChangeColor = Color.White;
        private Color decreaseSizeButtonColor = Color.White,
            increaseSizeButtonColor = Color.White,
            backgroundButtonColor = Color.White,
            levelUpButtonColor = Color.White,
            experienceButtonColor = Color.White;

        //GLOBAL INFO VARS
        public int barQuantity = 5;
        private int barSpacement = 10;
        public bool luckCompatibility, cookingCompatibility, magicCompatibility, loveCookingCompatibility,
            luckCheck, cookingCheck, magicCheck, loveCookingCheck;

        //ANIMATION VARS
        public bool animatingBox, animatingLevelUp;
        public Vector2 animDestPosBox, animDestPosLevelUp;
        public string animBoxDir, animLevelUpDir;

        //CONTROL VARS
        private bool draggingBox;
        private bool canShowLevelUp;
        private bool canCountTimer;
        private bool[] animateSkill = new bool[9];
        private bool[] expIncreasing = new bool[9];
        private bool[] actualExpGainedMessage = new bool[9];
        private int[] expGained = new int[9];
        private byte[] expAlpha = new byte[9];
        private bool[] inIncrease = new bool[9], inWait = new bool[9], inDecrease = new bool[9];
        
        //DATA VARS
        public ModEntry instance;
        private ModConfig config;

        //TIMER VARS
        private float timeLeft;
        private int[] timeExpMessageLeft = new int[9];

        //LEVEL UP VARS
        Texture2D levelUpIcon;
        string levelUpMessage;
        #endregion

        public override void Entry(IModHelper helper)
        {
            instance = this;
            getInfo();
            loadTextures();
            getCompatibleMods();
            loadColors();

            helper.Events.Display.RenderedHud += onRenderedHud;
            helper.Events.GameLoop.UpdateTicked += onUpdate;
            helper.Events.Input.ButtonPressed += onButtonPressed;
            helper.Events.Input.ButtonReleased += onButtonReleased;
            helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
            helper.Events.Player.Warped += onPlayerWarped;

            helper.ConsoleCommands.Add("abilities_change_size", "Changes the box size, only accepts integer values between 1 and 6.\nUsage: abilities_change_size <size>", cm_ChangeSize);
            helper.ConsoleCommands.Add("abilities_change_levelup_duration", "Changes the level up message duration.\nUsage: abilities_change_levelup_duration <duration>", cm_MessageDuration);
            helper.ConsoleCommands.Add("abilities_toggle_background", "Switch the box background.\nUsage: abilities_toggle_background <true/false>", cm_ToggleBackground);
            helper.ConsoleCommands.Add("abilities_toggle_levelup", "Switch the level up messages.\nUsage: abilities_toggle_levelup <true/false>", cm_ToggleLevelUpMessage);
            helper.ConsoleCommands.Add("abilities_toggle_experience", "Switch the experience infos.\nUsage: abilities_toggle_experience <true/false>", cm_ToggleExperience);
            helper.ConsoleCommands.Add("abilities_toggle_buttons", "Switch the main buttons.\nUsage: abilities_toggle_buttons <true/false>", cm_ToggleButtons);
            helper.ConsoleCommands.Add("abilities_reset", "Resets the config.\nUsage: abilities_reset", cm_Reset);
        }

        private void cm_ChangeSize(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            int size = Int32.Parse(args[0]);
            if (size > 0 && size < 7)
            {
                config.mainScale = size;
                this.Monitor.Log($"Size changed to: {size}.", LogLevel.Info);
            }
            else this.Monitor.Log($"Command invalid, please use integer values between 1 and 6.", LogLevel.Error);
        }
        private void cm_MessageDuration(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            float duration = float.Parse(args[0]);
            config.LevelUpMessageDuration = duration;
            this.Monitor.Log($"Duration changed to: {duration}.", LogLevel.Info);
        }
        private void cm_ToggleBackground(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            bool state = bool.Parse(args[0]);
            config.ShowBoxBackground = state;
            if (state) this.Monitor.Log($"Background enabled.", LogLevel.Info);
            else this.Monitor.Log($"Background disabled.", LogLevel.Info);

        }
        private void cm_ToggleLevelUpMessage(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            bool state = bool.Parse(args[0]);
            config.ShowLevelUp = state;
            if (state) this.Monitor.Log($"Level up message enabled.", LogLevel.Info);
            else this.Monitor.Log($"Level up message disabled.", LogLevel.Info);
        }
        private void cm_ToggleExperience(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            bool state = bool.Parse(args[0]);
            config.ShowExperienceInfo = state;
            if (state) this.Monitor.Log($"Experience info enabled.", LogLevel.Info);
            else this.Monitor.Log($"Experience info disabled.", LogLevel.Info);
        }
        private void cm_ToggleButtons(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            bool state = bool.Parse(args[0]);
            config.ShowButtons = state;
            if (state) this.Monitor.Log($"Experience info enabled.", LogLevel.Info);
            else this.Monitor.Log($"Experience info disabled.", LogLevel.Info);
        }
        private void cm_Reset(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            resetInfos();
            this.Monitor.Log($"Mod configurations resetted.", LogLevel.Info);
        }

        private void repositionExpInfo()
        {
            if (!Context.IsWorldReady) return;

            int rightPosX = this.config.mainPosX + backgroundTop.Width * this.config.mainScale;
            if (rightPosX >= Game1.uiViewport.Width - (backgroundExp.Width * this.config.mainScale))
            {
                expAdvicePositionX = -(backgroundExp.Width * this.config.mainScale + (10 * this.config.mainScale));
            }
            else
            {
                expAdvicePositionX = backgroundTop.Width * this.config.mainScale + 1;
            }
        }

        private void onPlayerWarped(object sender, WarpedEventArgs e)
        {
            configButtonPosX = MyHelper.AdjustPositionMineLevelWidth(configButtonPosX, e.NewLocation, defaultButtonPosX);
        }

        private void getInfo()
        {
            this.config = this.Helper.ReadConfig<ModConfig>();
            ajustInfos();
        }
        private void saveInfo()
        {
            this.Helper.WriteConfig(config);
        }

        private void ajustInfos()
        {
            //Adjust Decrease Size Button color and value
            if (this.config.mainScale < 1 || this.config.mainScale == 1)
            {
                this.config.mainScale = 1;
                decreaseSizeButtonColor = MyHelper.ChangeColorIntensity(Color.DarkGray, 1, 0.7f);
            }
            else
            {
                decreaseSizeButtonColor = Color.White;
            }
            //Adjust Increase Size Button color and value
            if (this.config.mainScale > 5 || this.config.mainScale == 5)
            {
                this.config.mainScale = 5;
                increaseSizeButtonColor = MyHelper.ChangeColorIntensity(Color.DarkGray, 1, 0.7f);
            }
            else
            {
                increaseSizeButtonColor = Color.White;
            }
            //Adjust Background Button color
            if (!this.config.ShowBoxBackground)
            {
                backgroundButtonColor = MyHelper.ChangeColorIntensity(Color.DarkGray, 1, 0.7f);
            }
            else
            {
                backgroundButtonColor = Color.White;
            }
            //Adjust Level up Button color
            if (!this.config.ShowLevelUp)
            {
                levelUpButtonColor = MyHelper.ChangeColorIntensity(Color.DarkGray, 1, 0.7f);
            }
            else
            {
                levelUpButtonColor = Color.White;
            }
            //Adjust Experience Button color
            if (!this.config.ShowExperienceInfo)
            {
                experienceButtonColor = MyHelper.ChangeColorIntensity(Color.DarkGray, 1, 0.7f);
            }
            else
            {
                experienceButtonColor = Color.White;
            }
            //Level up message duration
            if (this.config.LevelUpMessageDuration < 1)
            {
                this.config.LevelUpMessageDuration = 1;
            }
            saveInfo();
        }
        private void resetInfos()
        {
            this.config.mainPosX = 25;
            this.config.mainPosY = 125;
            this.config.mainScale = 3;
            this.config.ShowBoxBackground = true;
            this.config.ShowLevelUp = true;
            this.config.ShowExperienceInfo = true;
            this.config.LevelUpMessageDuration = 4;
            saveInfo();
            ajustInfos();
        }

        private void loadTextures()
        {
            backgroundTop = Helper.Content.Load<Texture2D>("assets/ui/boxes/backgroundTop.png", ContentSource.ModFolder);
            backgroundBottom = Helper.Content.Load<Texture2D>("assets/ui/boxes/backgroundBottom.png", ContentSource.ModFolder);
            backgroundFiller = Helper.Content.Load<Texture2D>("assets/ui/boxes/backgroundFiller.png", ContentSource.ModFolder);

            backgroundBar = Helper.Content.Load<Texture2D>("assets/ui/boxes/backgroundBar.png", ContentSource.ModFolder);
            barFiller = Helper.Content.Load<Texture2D>("assets/ui/boxes/barFiller.png", ContentSource.ModFolder);

            backgroundExp = Helper.Content.Load<Texture2D>("assets/ui/boxes/expHolder.png", ContentSource.ModFolder);
            backgroundLevelUp = Helper.Content.Load<Texture2D>("assets/ui/boxes/backgroundLevelUp.png", ContentSource.ModFolder);

            buttonConfig = Helper.Content.Load<Texture2D>("assets/ui/boxes/iconBoxConfig.png", ContentSource.ModFolder);
            buttonConfigApply = Helper.Content.Load<Texture2D>("assets/ui/boxes/checkIcon.png", ContentSource.ModFolder);
            buttonDecreaseSize = Helper.Content.Load<Texture2D>("assets/ui/boxes/decreaseSize.png", ContentSource.ModFolder);
            buttonIncreaseSize = Helper.Content.Load<Texture2D>("assets/ui/boxes/increaseSize.png", ContentSource.ModFolder);
            backgroundButton = Helper.Content.Load<Texture2D>("assets/ui/boxes/backgroundButton.png", ContentSource.ModFolder);
            levelUpButton = Helper.Content.Load<Texture2D>("assets/ui/boxes/levelUpButton.png", ContentSource.ModFolder);
            experienceButton = Helper.Content.Load<Texture2D>("assets/ui/boxes/experienceButton.png", ContentSource.ModFolder);
            buttonVisibility = Helper.Content.Load<Texture2D>("assets/ui/boxes/visibleIcon.png", ContentSource.ModFolder);
            buttonHidden = Helper.Content.Load<Texture2D>("assets/ui/boxes/hiddenIcon.png", ContentSource.ModFolder);
            buttonReset = Helper.Content.Load<Texture2D>("assets/ui/boxes/resetButton.png", ContentSource.ModFolder);

            backgroundLevelUp = Helper.Content.Load<Texture2D>("assets/ui/boxes/backgroundLevelUp.png", ContentSource.ModFolder);

            backgroundBoxConfig = Helper.Content.Load<Texture2D>("assets/ui/boxes/backgroundBoxConfig.png", ContentSource.ModFolder);

            icons = new Texture2D[]
            {
                Helper.Content.Load<Texture2D>("assets/ui/icons/farming.png", ContentSource.ModFolder),
                Helper.Content.Load<Texture2D>("assets/ui/icons/fishing.png", ContentSource.ModFolder),
                Helper.Content.Load<Texture2D>("assets/ui/icons/foraging.png", ContentSource.ModFolder),
                Helper.Content.Load<Texture2D>("assets/ui/icons/mining.png", ContentSource.ModFolder),
                Helper.Content.Load<Texture2D>("assets/ui/icons/combat.png", ContentSource.ModFolder),
                Helper.Content.Load<Texture2D>("assets/ui/icons/luck.png", ContentSource.ModFolder),
                Helper.Content.Load<Texture2D>("assets/ui/icons/cooking.png", ContentSource.ModFolder), //Cooking
                Helper.Content.Load<Texture2D>("assets/ui/icons/loveCooking.png", ContentSource.ModFolder), //Love of Cooking
                Helper.Content.Load<Texture2D>("assets/ui/icons/magic.png", ContentSource.ModFolder),
            };
        }
        private void loadColors()
        {
            colors = new Color[]
            {
                new Color( 115, 150, 56 ),  // [0] GREEN
                new Color( 117, 150, 150 ), // [1] BLUE
                new Color( 145, 102, 0 ),   // [2] BROWN
                new Color( 150, 80, 120 ), // [3] PINK GRAY
                new Color( 150, 31, 0 ),    // [4] RED
                new Color( 150, 150, 0 ),   // [5] YELLOW (Luck Skill)
                new Color( 165, 100, 30 ),   // [6] ORANGE (Cooking Skill)
                new Color( 150, 55, 5 ),    // [7] ORANGE RED (Love Cooking Skill)
                new Color( 155, 25, 135 ),  // [8] PURPLE (Magic Skill)
            };
            finalColors = new Color[]
            {
                new Color( 150, 175, 55 ),  // [9] GOLD (Max level)
                new Color( 150, 175, 55 ),  // [9] GOLD (Max level)
                new Color( 150, 175, 55 ),  // [9] GOLD (Max level)
                new Color( 150, 175, 55 ),  // [9] GOLD (Max level)
                new Color( 150, 175, 55 ),  // [9] GOLD (Max level)
                new Color( 150, 175, 55 ),  // [9] GOLD (Max level)
                new Color( 150, 175, 55 ),  // [9] GOLD (Max level)
                new Color( 150, 175, 55 ),  // [9] GOLD (Max level)
                new Color( 150, 175, 55 ),  // [9] GOLD (Max level)
                new Color( 150, 175, 55 ),  // [9] GOLD (Max level)
            };
            colorsRestoration = new Color[]
            {
                new Color( 115, 150, 56 ),  // [0] GREEN
                new Color( 117, 150, 150 ), // [1] BLUE
                new Color( 145, 102, 0 ),   // [2] BROWN
                new Color( 150, 80, 120 ), // [3] PINK GRAY
                new Color( 150, 31, 0 ),    // [4] RED
                new Color( 150, 150, 0 ),   // [5] YELLOW (Luck Skill)
                new Color( 165, 100, 30 ),   // [6] ORANGE (Cooking Skill)
                new Color( 150, 55, 5 ),    // [7] ORANGE RED (Love Cooking Skill)
                new Color( 155, 25, 135 ),  // [8] PURPLE (Magic Skill)
                new Color( 150, 175, 55 ),  // [9] GOLD (Max level)
            };
        }

        private void getCompatibleMods()
        {
            //
            if (this.Helper.ModRegistry.IsLoaded("spacechase0.LuckSkill"))
            {
                barQuantity++;
                luckCompatibility = true;
                this.Monitor.Log($"Found: Luck Skills - spacechase0", LogLevel.Trace);
            }
            if (this.Helper.ModRegistry.IsLoaded("spacechase0.CookingSkill"))
            {
                barQuantity++;
                cookingCompatibility = true;
                this.Monitor.Log($"Found: Cooking Skill - spacechase0", LogLevel.Trace);
            }
            if (this.Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking"))
            {
                barQuantity++;
                loveCookingCompatibility = true;
                this.Monitor.Log($"Found: Love of Cooking - bblueberry", LogLevel.Trace);
            }
            if (this.Helper.ModRegistry.IsLoaded("spacechase0.Magic"))
            {
                barQuantity++;
                magicCompatibility = true;
                this.Monitor.Log($"Found: Magic - spacechase0", LogLevel.Trace);
            }
        }

        private void onRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (canShowLevelUp && this.config.ShowLevelUp)
            {
                e.SpriteBatch.Draw(backgroundLevelUp, new Rectangle((Game1.uiViewport.Width / 2) - (backgroundLevelUp.Width * 3) / 2, levelUpPosY, backgroundLevelUp.Width * 3, backgroundLevelUp.Height * 3), Color.White);
                e.SpriteBatch.Draw(levelUpIcon, new Rectangle(((Game1.uiViewport.Width / 2) - (levelUpIcon.Width * 3) / 2) - 2, levelUpPosY + 16, levelUpIcon.Width * 3, levelUpIcon.Height * 3), Color.White);

                Vector2 centralizedStringPos = MyHelper.GetStringCenter(levelUpMessage, Game1.dialogueFont);
                e.SpriteBatch.DrawString(Game1.dialogueFont, levelUpMessage, new Vector2((Game1.uiViewport.Width / 2) - centralizedStringPos.X + 5, MyHelper.AdjustLanguagePosition(levelUpPosY + 63, Helper.Translation.LocaleEnum.ToString())), Color.Black);
            }

            if (!Context.IsWorldReady || Game1.CurrentEvent != null) return;

            //In-Config background
            if (inConfigMode)
            {
                e.SpriteBatch.Draw(backgroundBoxConfig, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Color(0, 0, 0, 0.50f));
            }

            if (this.config.ShowButtons)
            {
                if (inConfigMode)
                {
                    e.SpriteBatch.Draw(buttonConfigApply, new Rectangle(configButtonPosX, configButtonPosY, buttonConfig.Width * 3, buttonConfig.Height * 3), Color.White);
                    e.SpriteBatch.Draw(buttonReset, new Rectangle(configButtonPosX + 75, configButtonPosY, buttonReset.Width * 3, buttonReset.Height * 3), Color.White);
                }
                else
                {
                    e.SpriteBatch.Draw(buttonConfig, new Rectangle(configButtonPosX, configButtonPosY, buttonConfig.Width * 3, buttonConfig.Height * 3), Color.White);
                }
            }
            else
            {
                if (inConfigMode)
                {
                    e.SpriteBatch.Draw(buttonConfigApply, new Rectangle(configButtonPosX, configButtonPosY, buttonConfig.Width * 3, buttonConfig.Height * 3), Color.White);
                    e.SpriteBatch.Draw(buttonReset, new Rectangle(configButtonPosX + 75, configButtonPosY, buttonReset.Width * 3, buttonReset.Height * 3), Color.White);
                }
            }

            //Draw config button
            if (this.config.ShowButtons)
            {
                if (!inConfigMode)
                {
                    if (!this.config.ShowUI)
                    {
                        e.SpriteBatch.Draw(buttonHidden, new Rectangle(configButtonPosX + 75, configButtonPosY, buttonConfig.Width * 3, buttonConfig.Height * 3), Color.White);
                    }
                    else
                    {
                        e.SpriteBatch.Draw(buttonVisibility, new Rectangle(configButtonPosX + 75, configButtonPosY, buttonConfig.Width * 3, buttonConfig.Height * 3), Color.White);
                    }
                }
            }

            if (!this.config.ShowUI) return;
            //Draw adjust buttons
            if (inConfigMode)
            {
                e.SpriteBatch.Draw(buttonDecreaseSize, new Rectangle(this.config.mainPosX, this.config.mainPosY - 30, buttonDecreaseSize.Width * 3, buttonDecreaseSize.Height * 3), decreaseSizeButtonColor);
                e.SpriteBatch.Draw(buttonIncreaseSize, new Rectangle(this.config.mainPosX + 25, this.config.mainPosY - 30, buttonIncreaseSize.Width * 3, buttonIncreaseSize.Height * 3), increaseSizeButtonColor);
                e.SpriteBatch.Draw(backgroundButton, new Rectangle(this.config.mainPosX + 75, this.config.mainPosY - 30, backgroundButton.Width * 3, backgroundButton.Height * 3), backgroundButtonColor);
                e.SpriteBatch.Draw(levelUpButton, new Rectangle(this.config.mainPosX + 100, this.config.mainPosY - 30, levelUpButton.Width * 3, levelUpButton.Height * 3), levelUpButtonColor);
                e.SpriteBatch.Draw(experienceButton, new Rectangle(this.config.mainPosX + 125, this.config.mainPosY - 30, experienceButton.Width * 3, experienceButton.Height * 3), experienceButtonColor);
            }

            //Draw background
            if (this.config.ShowBoxBackground)
            {
                e.SpriteBatch.Draw(backgroundTop, new Rectangle(this.config.mainPosX, this.config.mainPosY, backgroundTop.Width * this.config.mainScale, backgroundTop.Height * this.config.mainScale), globalChangeColor);
                e.SpriteBatch.Draw(backgroundFiller, new Rectangle(this.config.mainPosX, this.config.mainPosY + (backgroundTop.Height * this.config.mainScale), backgroundTop.Width * this.config.mainScale, BarController.AdjustBackgroundSize(barQuantity, backgroundBar.Height * this.config.mainScale, barSpacement)), globalChangeColor);
                e.SpriteBatch.Draw(backgroundBottom, new Rectangle(this.config.mainPosX, this.config.mainPosY + (backgroundTop.Height * this.config.mainScale) + BarController.AdjustBackgroundSize(barQuantity, backgroundBar.Height * this.config.mainScale, barSpacement), backgroundTop.Width * this.config.mainScale, backgroundTop.Height * this.config.mainScale), globalChangeColor);
            }

            int posControlY = this.config.mainPosY + (backgroundTop.Height * this.config.mainScale) + (barSpacement / 2);
            this.luckCheck = this.cookingCheck = this.magicCheck = this.loveCookingCheck = false;
            //Draw inside background components
            for (var i = 0; i < barQuantity; i++)
            {
                getPlayerInformation();
                int barPosX = this.config.mainPosX + ((int)MyHelper.GetSpriteCenter(backgroundFiller, this.config.mainScale).X - (int)MyHelper.GetSpriteCenter(backgroundBar, this.config.mainScale).X);

                //Draw bars background
                e.SpriteBatch.Draw(backgroundBar, new Rectangle(barPosX, posControlY, backgroundBar.Width * this.config.mainScale, backgroundBar.Height * this.config.mainScale), globalChangeColor);

                if (i <= 4)
                {
                    //Draw icons
                    e.SpriteBatch.Draw(icons[i], new Rectangle(barPosX + (((26 * this.config.mainScale) / 2) - ((icons[i].Width * this.config.mainScale) / 2)), posControlY + (((24 * this.config.mainScale) / 2) - (int)MyHelper.GetSpriteCenter(icons[i], this.config.mainScale).Y), icons[i].Width * this.config.mainScale, icons[i].Height * this.config.mainScale), globalChangeColor);
                    //Draw level text
                    int posNumber;
                    if (playerLevels[i].ToString().Length == 1)
                    {
                        posNumber = 34;
                    }
                    else if (playerLevels[i].ToString().Length == 2)
                    {
                        posNumber = 37;
                    }
                    else
                    {
                        posNumber = 40;
                    }
                    NumberSprite.draw(playerLevels[i], e.SpriteBatch, new Vector2(barPosX + (posNumber * this.config.mainScale), posControlY + (12 * this.config.mainScale)), globalChangeColor, BarController.AdjustLevelScale(this.config.mainScale, playerLevels[i], maxLevel), 0, 1, 0);
                    //Draw experience bars
                    Color barColor;
                    if (draggingBox)
                    {
                        if (playerLevels[i] >= maxLevel)
                        {
                            barColor = MyHelper.ChangeColorIntensity(finalColors[i], 0.35f, 1);
                        }
                        else
                        {
                            barColor = MyHelper.ChangeColorIntensity(colors[i], 0.35f, 1);
                        }
                    }
                    else
                    {
                        if (playerLevels[i] >= maxLevel)
                        {
                            barColor = finalColors[i];
                        }
                        else
                        {
                            barColor = colors[i];
                        }
                    }
                    e.SpriteBatch.Draw(barFiller, BarController.GetExperienceBar(new Vector2(barPosX + (43 * this.config.mainScale), posControlY + ((25 * this.config.mainScale) / 2) - ((barFiller.Height * this.config.mainScale) / 2)), new Vector2(83, barFiller.Height), playerExperience[i], playerLevels[i], maxLevel, this.config.mainScale), barColor);
                    if (this.config.ShowExperienceInfo)
                    {
                        if (playerLevels[i] < maxLevel)
                        {
                            e.SpriteBatch.DrawString(Game1.dialogueFont, BarController.GetExperienceText(playerExperience[i], playerLevels[i], maxLevel), new Vector2(barPosX + (43 * this.config.mainScale) + 5, posControlY + ((25 * this.config.mainScale) / 2) - ((barFiller.Height * this.config.mainScale) / 2)), MyHelper.ChangeColorIntensity(colors[i], 0.45f, colors[i].A), 0f, Vector2.Zero, BarController.AdjustExperienceScale(this.config.mainScale), SpriteEffects.None, 1);
                        }
                        else
                        {
                            e.SpriteBatch.DrawString(Game1.dialogueFont, BarController.GetExperienceText(playerExperience[i], playerLevels[i], maxLevel), new Vector2(barPosX + (43 * this.config.mainScale) + 5, posControlY + ((25 * this.config.mainScale) / 2) - ((barFiller.Height * this.config.mainScale) / 2)), MyHelper.ChangeColorIntensity(finalColors[i], 0.45f, finalColors[i].A), 0f, Vector2.Zero, BarController.AdjustExperienceScale(this.config.mainScale), SpriteEffects.None, 1);
                        }
                    }
                    if (actualExpGainedMessage[i] && this.config.ShowExperienceInfo)
                    {
                        e.SpriteBatch.Draw(backgroundExp, new Rectangle(barPosX + expAdvicePositionX, posControlY + ((backgroundBar.Height * this.config.mainScale) / 2) - ((backgroundExp.Height * this.config.mainScale) / 2), backgroundExp.Width * this.config.mainScale, backgroundExp.Height * this.config.mainScale), MyHelper.ChangeColorIntensity(globalChangeColor, 1, expAlpha[i]));

                        Vector2 centralizedStringPos = MyHelper.GetStringCenter(expGained[i].ToString(), Game1.dialogueFont);
                        e.SpriteBatch.DrawString(Game1.dialogueFont, $"+{expGained[i]}", new Vector2(barPosX + expAdvicePositionX + ((backgroundExp.Width * this.config.mainScale) / 2) - centralizedStringPos.X, posControlY + ((25 * this.config.mainScale) / 2) - ((barFiller.Height * this.config.mainScale) / 2)), MyHelper.ChangeColorIntensity(colorsRestoration[i], 0.45f, expAlpha[i]), 0f, Vector2.Zero, BarController.AdjustExperienceScale(this.config.mainScale), SpriteEffects.None, 1);
                    }
                }
                else
                {
                    int tempIndex = CompatibilityController.GetActualAbility(i, Helper, this.instance);
                    int[] modInfos = CompatibilityController.GetModExp(Helper, tempIndex);

                    //Same as above
                    e.SpriteBatch.Draw(icons[tempIndex], new Rectangle(barPosX + (((26 * this.config.mainScale) / 2) - ((icons[tempIndex].Width * this.config.mainScale) / 2)), posControlY + (((24 * this.config.mainScale) / 2) - ((icons[tempIndex].Height * this.config.mainScale) / 2)), icons[tempIndex].Width * this.config.mainScale, icons[tempIndex].Height * this.config.mainScale), globalChangeColor);
                    int posModded;
                    if (modInfos[2].ToString().Length == 1)
                    {
                        posModded = 34;
                    }
                    else if (modInfos[2].ToString().Length == 2)
                    {
                        posModded = 37;
                    }
                    else
                    {
                        posModded = 40;
                    }
                    NumberSprite.draw(modInfos[2], e.SpriteBatch, new Vector2(barPosX + (posModded * this.config.mainScale), posControlY + (12 * this.config.mainScale)), globalChangeColor, BarController.AdjustLevelScale(this.config.mainScale, modInfos[2], maxLevel), 0, 1, 0);
                    Color barColor;
                    if (draggingBox)
                    {
                        if (modInfos[2] >= maxLevel)
                        {
                            barColor = MyHelper.ChangeColorIntensity(finalColors[tempIndex], 0.35f, 1);
                        }
                        else
                        {
                            barColor = MyHelper.ChangeColorIntensity(colors[tempIndex], 0.35f, 1);
                        }
                    }
                    else
                    {
                        if (modInfos[2] >= maxLevel)
                        {
                            barColor = finalColors[tempIndex];
                        }
                        else
                        {
                            barColor = colors[tempIndex];
                        }
                    }
                    e.SpriteBatch.Draw(barFiller, BarController.GetExperienceBar(new Vector2(barPosX + (43 * this.config.mainScale), posControlY + ((25 * this.config.mainScale) / 2) - ((barFiller.Height * this.config.mainScale) / 2)), new Vector2(83, barFiller.Height), modInfos[0], modInfos[2], maxLevel, this.config.mainScale), barColor);
                    if (this.config.ShowExperienceInfo)
                    {
                        if (modInfos[2] < maxLevel)
                        {
                            e.SpriteBatch.DrawString(Game1.dialogueFont, BarController.GetExperienceText(modInfos[0], modInfos[2], maxLevel), new Vector2(barPosX + (43 * this.config.mainScale) + 5, posControlY + ((25 * this.config.mainScale) / 2) - ((barFiller.Height * this.config.mainScale) / 2)), MyHelper.ChangeColorIntensity(colors[tempIndex], 0.45f, colors[tempIndex].A), 0f, Vector2.Zero, BarController.AdjustExperienceScale(this.config.mainScale), SpriteEffects.None, 1);
                        }
                        else
                        {
                            e.SpriteBatch.DrawString(Game1.dialogueFont, BarController.GetExperienceText(modInfos[0], modInfos[2], maxLevel), new Vector2(barPosX + (43 * this.config.mainScale) + 5, posControlY + ((25 * this.config.mainScale) / 2) - ((barFiller.Height * this.config.mainScale) / 2)), MyHelper.ChangeColorIntensity(finalColors[tempIndex], 0.45f, finalColors[tempIndex].A), 0f, Vector2.Zero, BarController.AdjustExperienceScale(this.config.mainScale), SpriteEffects.None, 1);
                        }
                    }
                    int prevIndex = 0;
                    if (this.Helper.ModRegistry.IsLoaded("spacechase0.LuckSkill")) prevIndex = i;
                    else prevIndex = i + 1;
                    if (actualExpGainedMessage[prevIndex] && this.config.ShowExperienceInfo)
                    {
                        e.SpriteBatch.Draw(backgroundExp, new Rectangle(barPosX + expAdvicePositionX, posControlY + ((backgroundBar.Height * this.config.mainScale) / 2) - ((backgroundExp.Height * this.config.mainScale) / 2), backgroundExp.Width * this.config.mainScale, backgroundExp.Height * this.config.mainScale), MyHelper.ChangeColorIntensity(globalChangeColor, 1, expAlpha[prevIndex]));

                        Vector2 centralizedStringPos = MyHelper.GetStringCenter(expGained[prevIndex].ToString(), Game1.dialogueFont);
                        e.SpriteBatch.DrawString(Game1.dialogueFont, $"+{expGained[prevIndex]}", new Vector2(barPosX + expAdvicePositionX + ((backgroundExp.Width * this.config.mainScale) / 2) - centralizedStringPos.X, posControlY + ((25 * this.config.mainScale) / 2) - ((barFiller.Height * this.config.mainScale) / 2)), MyHelper.ChangeColorIntensity(colorsRestoration[prevIndex], 0.45f, expAlpha[prevIndex]), 0f, Vector2.Zero, BarController.AdjustExperienceScale(this.config.mainScale), SpriteEffects.None, 1);
                    }
                }

                posControlY += barSpacement + (backgroundBar.Height * this.config.mainScale);
            }
        }

        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            getPlayerInformation();
            oldPlayerLevels = playerLevels;
            oldPlayerExperience = playerExperience;

            if (Helper.ModRegistry.IsLoaded("spacechase0.CookingSkill"))
            {
                oldPlayerModdedLevels[0] = Game1.player.GetCustomSkillLevel(Skills.GetSkill("cooking"));
                oldPlayerModdedExperience[0] = Game1.player.GetCustomSkillExperience(Skills.GetSkill("cooking"));
            }
            if (Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking"))
            {
                oldPlayerModdedLevels[1] = Game1.player.GetCustomSkillLevel(Skills.GetSkill("blueberry.LoveOfCooking.CookingSkill"));
                oldPlayerModdedExperience[1] = Game1.player.GetCustomSkillExperience(Skills.GetSkill("blueberry.LoveOfCooking.CookingSkill"));
            }
            if (Helper.ModRegistry.IsLoaded("spacechase0.Magic"))
            {
                oldPlayerModdedLevels[2] = Game1.player.GetCustomSkillLevel(Skills.GetSkill("magic"));
                oldPlayerModdedExperience[2] = Game1.player.GetCustomSkillExperience(Skills.GetSkill("magic"));
            }

            configButtonPosX = MyHelper.AdjustPositionMineLevelWidth(configButtonPosX, Game1.player.currentLocation, defaultButtonPosX);
        }

        private void onUpdate(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady && Game1.CurrentEvent == null) return;

            getPlayerInformation();

            animateThings();

            checkExpUp();
            expTimer();

            expAlphaChanger();

            checkLevelUp();
            levelUpTimer();

            if (draggingBox)
            {
                this.config.mainPosX = Game1.getMousePosition(true).X - ((backgroundTop.Width * this.config.mainScale) / 2);
                this.config.mainPosY = Game1.getMousePosition(true).Y - (BarController.AdjustBackgroundSize(barQuantity, backgroundBar.Height * this.config.mainScale, barSpacement) / 2) - (backgroundTop.Height * this.config.mainScale);
            }

            checkMousePosition();
            repositionExpInfo();

            if (Game1.player.currentLocation.name == "Club")
            {
                configButtonPosY = 90;
            }
            else
            {
                configButtonPosY = defaultButtonPosY;
            }
        }

        private void checkExpUp()
        {
            var prevIndex = 0;

            for (var i = 0; i < playerExperience.Length; i++)
            {
                if (playerExperience[i] != oldPlayerExperience[i])
                {
                    expGained[i] = playerExperience[i] - oldPlayerExperience[i];
                    oldPlayerExperience[i] = playerExperience[i];

                    showExpUpAdvice(i);
                }
            }

            if (Helper.ModRegistry.IsLoaded("spacechase0.CookingSkill"))
            {
                if (playerModdedExperience[0] != oldPlayerModdedExperience[0])
                {
                    prevIndex = 6;

                    expGained[prevIndex] = playerModdedExperience[0] - oldPlayerModdedExperience[0];
                    oldPlayerModdedExperience[0] = playerModdedExperience[0];

                    showExpUpAdvice(prevIndex);
                }
            }
            if (Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking"))
            {
                if (playerModdedExperience[1] != oldPlayerModdedExperience[1])
                {
                    if (Helper.ModRegistry.IsLoaded("spacechase0.CookingSkill")) prevIndex = 7;
                    else prevIndex = 6;

                    expGained[prevIndex] = playerModdedExperience[1] - oldPlayerModdedExperience[1];
                    oldPlayerModdedExperience[1] = playerModdedExperience[1];

                    showExpUpAdvice(prevIndex);
                }
            }
            if (Helper.ModRegistry.IsLoaded("spacechase0.Magic"))
            {
                if (playerModdedExperience[2] != oldPlayerModdedExperience[2])
                {
                    if (Helper.ModRegistry.IsLoaded("spacechase0.CookingSkill") && Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking")) prevIndex = 8;
                    else if (Helper.ModRegistry.IsLoaded("spacechase0.CookingSkill") && !Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking")) prevIndex = 7;
                    else if (!Helper.ModRegistry.IsLoaded("spacechase0.CookingSkill") && Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking")) prevIndex = 7;
                    else prevIndex = 6;

                    expGained[prevIndex] = playerModdedExperience[2] - oldPlayerModdedExperience[2];
                    oldPlayerModdedExperience[2] = playerModdedExperience[2];

                    showExpUpAdvice(prevIndex);
                }
            }
        }
        private void showExpUpAdvice(int skill)
        {
            for (var i = 0; i <= animateSkill.Length; i++)
            {
                if (skill == i)
                {
                    inIncrease[i] = true;
                    actualExpGainedMessage[i] = true;
                    timeExpMessageLeft[i] = 3 * 60;
                    expAlpha[i] = 0;

                    expIncreasing[i] = true;
                    animateSkill[i] = true;
                }
            }
        }
        private void expTimer()
        {
            for (var i = 0; i < animateSkill.Length; i++)
            {
                if (animateSkill[i])
                {
                    int actualSkillLevel = 0;
                    int toColor = i;

                    if (i < 6)
                    {
                        actualSkillLevel = playerLevels[i];
                    }
                    else
                    {
                        if (i == 6)
                        {
                            if (Helper.ModRegistry.IsLoaded("spacechase0.CookingSkill"))
                            {
                                actualSkillLevel = Game1.player.GetCustomSkillLevel(Skills.GetSkill("cooking"));
                                toColor = 6;
                            }
                            else if (Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking"))
                            {
                                actualSkillLevel = Game1.player.GetCustomSkillLevel(Skills.GetSkill("blueberry.LoveOfCooking.CookingSkill"));
                                toColor = 7;
                            }
                            else if (Helper.ModRegistry.IsLoaded("spacechase0.Magic"))
                            {
                                actualSkillLevel = Game1.player.GetCustomSkillLevel(Skills.GetSkill("magic"));
                                toColor = 8;
                            }
                        }
                        else if (i == 7)
                        {
                            if (Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking") && Helper.ModRegistry.IsLoaded("spacechase0.CookingSkill"))
                            {
                                actualSkillLevel = Game1.player.GetCustomSkillLevel(Skills.GetSkill("blueberry.LoveOfCooking.CookingSkill"));
                                toColor = 7;
                            }
                            else if (Helper.ModRegistry.IsLoaded("spacechase0.Magic"))
                            {
                                actualSkillLevel = Game1.player.GetCustomSkillLevel(Skills.GetSkill("magic"));
                                toColor = 8;
                            }
                        }
                        else if (i == 8)
                        {
                            if (Helper.ModRegistry.IsLoaded("spacechase0.Magic"))
                            {
                                actualSkillLevel = Game1.player.GetCustomSkillLevel(Skills.GetSkill("magic"));
                                toColor = 8;
                            }
                        }
                    }

                    int virtualColorValue;
                    byte intensity = 5;

                    if (expIncreasing[i])
                    {
                        if (actualSkillLevel < 10)
                        {
                            if (colors[toColor].R < 255 && colors[toColor].G < 255 && colors[toColor].B < 255)
                            {
                                virtualColorValue = colors[toColor].R + intensity;
                                if (virtualColorValue < 255) colors[toColor].R += intensity;
                                else colors[toColor].R = 255;

                                virtualColorValue = colors[toColor].G + intensity;
                                if (virtualColorValue < 255) colors[toColor].G += intensity;
                                else colors[toColor].G = 255;

                                virtualColorValue = colors[toColor].B + intensity;
                                if (virtualColorValue < 255) colors[toColor].B += intensity;
                                else colors[toColor].B = 255;
                            }
                            else
                            {
                                expIncreasing[i] = false;
                            }
                        }
                        else
                        {
                            if (finalColors[toColor].R < 255 && finalColors[toColor].G < 255 && finalColors[toColor].B < 255)
                            {
                                virtualColorValue = finalColors[toColor].R + intensity;
                                if (virtualColorValue < 255) finalColors[toColor].R += intensity;
                                else finalColors[toColor].R = 255;

                                virtualColorValue = finalColors[toColor].G + intensity;
                                if (virtualColorValue < 255) finalColors[toColor].G += intensity;
                                else finalColors[toColor].G = 255;

                                virtualColorValue = finalColors[toColor].B + intensity;
                                if (virtualColorValue < 255) finalColors[toColor].B += intensity;
                                else finalColors[toColor].B = 255;
                            }
                            else
                            {
                                expIncreasing[i] = false;
                            }
                        }
                    }
                    else
                    {
                        if (actualSkillLevel < 10)
                        {
                            if (colors[toColor] != colorsRestoration[toColor])
                            {
                                virtualColorValue = colors[toColor].R - intensity;
                                if (virtualColorValue > colorsRestoration[toColor].R) colors[toColor].R -= intensity;
                                else colors[toColor].R = colorsRestoration[toColor].R;

                                virtualColorValue = colors[toColor].G - intensity;
                                if (virtualColorValue > colorsRestoration[toColor].G) colors[toColor].G -= intensity;
                                else colors[toColor].G = colorsRestoration[toColor].G;

                                virtualColorValue = colors[toColor].B - intensity;
                                if (virtualColorValue > colorsRestoration[toColor].B) colors[toColor].B -= intensity;
                                else colors[toColor].B = colorsRestoration[toColor].B;
                            }
                            else
                            {
                                animateSkill[i] = false;
                            }
                        }
                        else
                        {
                            if (finalColors[toColor] != colorsRestoration[9])
                            {
                                virtualColorValue = finalColors[toColor].R - intensity;
                                if (virtualColorValue > colorsRestoration[9].R) finalColors[toColor].R -= intensity;
                                else finalColors[toColor].R = colorsRestoration[9].R;

                                virtualColorValue = finalColors[toColor].G - intensity;
                                if (virtualColorValue > colorsRestoration[9].G) finalColors[toColor].G -= intensity;
                                else finalColors[toColor].G = colorsRestoration[9].G;

                                virtualColorValue = finalColors[toColor].B - intensity;
                                if (virtualColorValue > colorsRestoration[9].B) finalColors[toColor].B -= intensity;
                                else finalColors[toColor].B = colorsRestoration[9].B;
                            }
                            else
                            {
                                animateSkill[i] = false;
                            }
                        }
                    }
                }
            }
        }
        private void expAlphaChanger()
        {
            int virtualAlphaValue;
            byte intensity = 12;

            for (var i = 0; i < actualExpGainedMessage.Length; i++)
            {
                if (actualExpGainedMessage[i])
                {
                    if (inIncrease[i])
                    {
                        virtualAlphaValue = expAlpha[i] + intensity;
                        if (virtualAlphaValue < 255) expAlpha[i] += intensity;
                        else
                        {
                            expAlpha[i] = 255;
                            inIncrease[i] = false;
                            inWait[i] = true;
                        }
                    }
                    else if (inWait[i])
                    {
                        if (timeExpMessageLeft[i] > 0)
                        {
                            timeExpMessageLeft[i]--;
                        }
                        else
                        {
                            inWait[i] = false;
                            inDecrease[i] = true;
                        }
                    }
                    else if (inDecrease[i])
                    {
                        virtualAlphaValue = expAlpha[i] - intensity;
                        if (virtualAlphaValue > 0) expAlpha[i] -= intensity;
                        else
                        {
                            expAlpha[i] = 0;
                            inDecrease[i] = false;
                            actualExpGainedMessage[i] = false;
                        }
                    }
                }
            }
        }

        private void checkLevelUp()
        {
            for (var i = 0; i < playerLevels.Length; i++)
            {
                if (playerLevels[i] != oldPlayerLevels[i])
                {
                    oldPlayerLevels[i] = playerLevels[i];

                    showLevelUpAdvice(playerLevels[i], i);

                    levelUpPosY = (int)new Vector2(0, 0 - (backgroundLevelUp.Height * 3) - 5).Y;
                    animLevelUpDir = "bottom"; animDestPosLevelUp = new Vector2(0, 150);
                    animatingLevelUp = true;
                }
            }
            if (Helper.ModRegistry.IsLoaded("spacechase0.CookingSkill"))
            {
                if (oldPlayerModdedLevels[0] != Game1.player.GetCustomSkillLevel(Skills.GetSkill("cooking")))
                {
                    oldPlayerModdedLevels[0] = Game1.player.GetCustomSkillLevel(Skills.GetSkill("cooking"));
                    showLevelUpAdvice(0, 6);

                    levelUpPosY = (int)new Vector2(0, 0 - (backgroundLevelUp.Height * 3) - 5).Y;
                    animLevelUpDir = "bottom"; animDestPosLevelUp = new Vector2(0, 150);
                    animatingLevelUp = true;
                }
            }
            if (Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking"))
            {
                if (oldPlayerModdedLevels[1] != Game1.player.GetCustomSkillLevel(Skills.GetSkill("blueberry.LoveOfCooking.CookingSkill")))
                {
                    oldPlayerModdedLevels[1] = Game1.player.GetCustomSkillLevel(Skills.GetSkill("blueberry.LoveOfCooking.CookingSkill"));
                    showLevelUpAdvice(0, 7);

                    levelUpPosY = (int)new Vector2(0, 0 - (backgroundLevelUp.Height * 3) - 5).Y;
                    animLevelUpDir = "bottom"; animDestPosLevelUp = new Vector2(0, 150);
                    animatingLevelUp = true;
                }
            }
            if (Helper.ModRegistry.IsLoaded("spacechase0.Magic"))
            {
                if (oldPlayerModdedLevels[2] != Game1.player.GetCustomSkillLevel(Skills.GetSkill("magic")))
                {
                    oldPlayerModdedLevels[2] = Game1.player.GetCustomSkillLevel(Skills.GetSkill("magic"));
                    showLevelUpAdvice(0, 8);

                    levelUpPosY = (int)new Vector2(0, 0 - (backgroundLevelUp.Height * 3) - 5).Y;
                    animLevelUpDir = "bottom"; animDestPosLevelUp = new Vector2(0, 150);
                    animatingLevelUp = true;
                }
            }
        }
        private void showLevelUpAdvice(int level, int skill)
        {
            levelUpIcon = icons[skill];
            levelUpMessage = Helper.Translation.Get("LevelUpMessage");


            timeLeft = this.config.LevelUpMessageDuration * 60;
            canShowLevelUp = true;
        }
        private void levelUpTimer()
        {
            if (!canShowLevelUp || !canCountTimer) return;

            if (timeLeft > 0)
            {
                timeLeft--;
            }
            else
            {
                canCountTimer = false;
                animLevelUpDir = "top"; animDestPosLevelUp = new Vector2(0, 0 - (backgroundLevelUp.Height * 3) - 5);
                animatingLevelUp = true;
            }
        }

        private void getPlayerInformation()
        {
            playerExperience = Game1.player.experiencePoints.ToArray();
            playerLevels = new int[]
            {
                Game1.player.farmingLevel,
                Game1.player.fishingLevel,
                Game1.player.foragingLevel,
                Game1.player.miningLevel,
                Game1.player.combatLevel,
                Game1.player.luckLevel,
            };
            if (Helper.ModRegistry.IsLoaded("spacechase0.CookingSkill"))
            {
                playerModdedExperience[0] = Game1.player.GetCustomSkillExperience(Skills.GetSkill("cooking"));
            }
            if (Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking"))
            {
                playerModdedExperience[1] = Game1.player.GetCustomSkillExperience(Skills.GetSkill("blueberry.LoveOfCooking.CookingSkill"));
            }
            if (Helper.ModRegistry.IsLoaded("spacechase0.Magic"))
            {
                playerModdedExperience[2] = Game1.player.GetCustomSkillExperience(Skills.GetSkill("magic"));
            }
        }

        private void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.CurrentEvent != null) return;

            if (e.Button == SButton.PageDown)
            {
                this.Monitor.Log($"Current location: {Game1.player.currentLocation.Name}", LogLevel.Info);
            }

            Vector2 mousePos;
            mousePos.X = Game1.getMousePosition(true).X;
            mousePos.Y = Game1.getMousePosition(true).Y;

            //Reset button check click
            if (e.Button == SButton.MouseLeft &&
                mousePos.X >= configButtonPosX + 75 && mousePos.X <= configButtonPosX + 75 + (buttonReset.Width * 3) &&
                mousePos.Y >= configButtonPosY && mousePos.Y <= configButtonPosY + (buttonReset.Height * 3) &&
                inConfigMode)
            {
                resetInfos();
            }
            //Reset Box Position - Button
            if (e.Button == this.config.ResetKey && inConfigMode)
            {
                resetInfos();
            }

            //Config button click check - Button
            if (e.Button == this.config.ConfigKey)
            {
                if (!this.config.ShowUI)
                {
                    toggleUI();
                }

                switch (inConfigMode)
                {
                    case true:
                        inConfigMode = false;
                        break;
                    case false:
                        inConfigMode = true;
                        break;
                }
            }
            //Toggle UI - Button
            if (e.Button == this.config.ToggleKey)
            {
                if (inConfigMode) return;
                toggleUI();
            }

            //Config button click check
            //Toggle UI
            if (this.config.ShowButtons)
            {
                if (e.Button == SButton.MouseLeft || e.Button == this.config.ConfigKey)
                {
                    if (mousePos.X >= configButtonPosX && mousePos.X <= configButtonPosX + (buttonConfig.Width * 3) &&
                        mousePos.Y >= configButtonPosY && mousePos.Y <= configButtonPosY + (buttonConfig.Height * 3))
                    {
                        if (!this.config.ShowUI)
                        {
                            toggleUI();
                        }

                        switch (inConfigMode)
                        {
                            case true:
                                inConfigMode = false;
                                break;
                            case false:
                                inConfigMode = true;
                                break;
                        }
                    }
                }


                if (e.Button == SButton.MouseLeft)
                {
                    if (mousePos.X >= configButtonPosX + 75 && mousePos.X <= configButtonPosX + 75 + (buttonConfig.Width * 3) &&
                        mousePos.Y >= configButtonPosY && mousePos.Y <= configButtonPosY + (buttonConfig.Height * 3))
                    {
                        if (inConfigMode) return;
                        toggleUI();
                    }
                }
            }
            else
            {
                if (inConfigMode)
                {
                    if (e.Button == SButton.MouseLeft || e.Button == this.config.ConfigKey)
                    {
                        if (mousePos.X >= configButtonPosX && mousePos.X <= configButtonPosX + (buttonConfig.Width * 3) &&
                            mousePos.Y >= configButtonPosY && mousePos.Y <= configButtonPosY + (buttonConfig.Height * 3))
                        {
                            if (!this.config.ShowUI)
                            {
                                toggleUI();
                            }

                            switch (inConfigMode)
                            {
                                case true:
                                    inConfigMode = false;
                                    break;
                                case false:
                                    inConfigMode = true;
                                    break;
                            }
                        }
                    }
                }
            }



            if (inConfigMode)
            {
                //Box click check
                int totalBackgroundSize = BarController.AdjustBackgroundSize(barQuantity, backgroundBar.Height * this.config.mainScale, barSpacement) + (backgroundTop.Height * this.config.mainScale) + (backgroundBottom.Height * this.config.mainScale);
                if (e.Button == SButton.MouseLeft &&
                    mousePos.X >= this.config.mainPosX && mousePos.X <= this.config.mainPosX + (backgroundTop.Width * this.config.mainScale) &&
                    mousePos.Y >= this.config.mainPosY && mousePos.Y <= this.config.mainPosY + totalBackgroundSize)
                {
                    draggingBox = true;
                    globalChangeColor = Color.DarkGray;
                }

                //Decrease button click check
                if (e.Button == SButton.MouseLeft &&
                    mousePos.X >= this.config.mainPosX && mousePos.X <= this.config.mainPosX + (buttonDecreaseSize.Width * 3) &&
                    mousePos.Y >= this.config.mainPosY - 30 && mousePos.Y <= (this.config.mainPosY - 30) + (buttonDecreaseSize.Height * 3))
                {
                    if (this.config.mainScale > 1)
                    {
                        decreaseSizeButtonColor = Color.White;
                        increaseSizeButtonColor = Color.White;
                        this.config.mainScale -= 1;
                        if (this.config.mainScale == 1)
                        {
                            decreaseSizeButtonColor = MyHelper.ChangeColorIntensity(Color.DarkGray, 1, 0.7f);
                        }
                        saveInfo();
                    }
                }
                //Increase button click check
                if (e.Button == SButton.MouseLeft &&
                    mousePos.X >= this.config.mainPosX + 25 && mousePos.X <= (this.config.mainPosX + 25) + (buttonDecreaseSize.Width * 3) &&
                    mousePos.Y >= this.config.mainPosY - 30 && mousePos.Y <= (this.config.mainPosY - 30) + (buttonDecreaseSize.Height * 3))
                {
                    if (this.config.mainScale < 5)
                    {
                        increaseSizeButtonColor = Color.White;
                        decreaseSizeButtonColor = Color.White;
                        this.config.mainScale += 1;
                        if (this.config.mainScale == 5)
                        {
                            increaseSizeButtonColor = MyHelper.ChangeColorIntensity(Color.DarkGray, 1, 0.7f);
                        }
                        saveInfo();
                    }
                }

                //Background toggler button check click
                if (e.Button == SButton.MouseLeft &&
                    mousePos.X >= this.config.mainPosX + 75 && mousePos.X <= (this.config.mainPosX + 75) + (buttonDecreaseSize.Width * 3) &&
                    mousePos.Y >= this.config.mainPosY - 30 && mousePos.Y <= (this.config.mainPosY - 30) + (buttonDecreaseSize.Height * 3))
                {
                    switch (this.config.ShowBoxBackground)
                    {
                        case true:
                            this.config.ShowBoxBackground = false;
                            backgroundButtonColor = MyHelper.ChangeColorIntensity(Color.DarkGray, 1, 0.7f);
                            break;
                        case false:
                            this.config.ShowBoxBackground = true;
                            backgroundButtonColor = Color.White;
                            break;
                    }
                    saveInfo();
                }

                //Levelup toggler button check click
                if (e.Button == SButton.MouseLeft &&
                    mousePos.X >= this.config.mainPosX + 100 && mousePos.X <= (this.config.mainPosX + 100) + (buttonDecreaseSize.Width * 3) &&
                    mousePos.Y >= this.config.mainPosY - 30 && mousePos.Y <= (this.config.mainPosY - 30) + (buttonDecreaseSize.Height * 3))
                {
                    switch (this.config.ShowLevelUp)
                    {
                        case true:
                            this.config.ShowLevelUp = false;
                            levelUpButtonColor = MyHelper.ChangeColorIntensity(Color.DarkGray, 1, 0.7f);
                            Game1.addHUDMessage(new HUDMessage("Level up popup DISABLED", 3));
                            break;
                        case false:
                            this.config.ShowLevelUp = true;
                            levelUpButtonColor = Color.White;
                            Game1.addHUDMessage(new HUDMessage("Level up popup ENABLED", 3));
                            break;
                    }
                    saveInfo();
                }

                //Experience toggler button check click
                if (e.Button == SButton.MouseLeft &&
                    mousePos.X >= this.config.mainPosX + 125 && mousePos.X <= (this.config.mainPosX + 125) + (buttonDecreaseSize.Width * 3) &&
                    mousePos.Y >= this.config.mainPosY - 30 && mousePos.Y <= (this.config.mainPosY - 30) + (buttonDecreaseSize.Height * 3))
                {
                    switch (this.config.ShowExperienceInfo)
                    {
                        case true:
                            this.config.ShowExperienceInfo = false;
                            experienceButtonColor = MyHelper.ChangeColorIntensity(Color.DarkGray, 1, 0.7f);
                            break;
                        case false:
                            this.config.ShowExperienceInfo = true;
                            experienceButtonColor = Color.White;
                            break;
                    }
                    saveInfo();
                }

            }
        }
        private void onButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.CurrentEvent != null) return;

            if (inConfigMode)
            {
                //Box release check
                if (e.Button == SButton.MouseLeft && draggingBox)
                {
                    draggingBox = false;
                    globalChangeColor = Color.White;
                    saveInfo();
                }
            }
        }

        private void toggleUI()
        {
            switch (this.config.ShowUI)
            {
                case true:
                    this.config.ShowUI = false;
                    break;

                case false:
                    this.config.ShowUI = true;
                    break;
            }
            saveInfo();
        }

        private void animateThings()
        {
            Vector2 levelUpPosHolder = new Vector2(0, levelUpPosY);

            if (animatingLevelUp)
            {
                if (levelUpPosHolder != animDestPosLevelUp)
                {
                    levelUpPosHolder = AnimController.Animate(levelUpPosHolder, animDestPosLevelUp, 8f, animLevelUpDir);
                    levelUpPosY = (int)levelUpPosHolder.Y;
                }
                else
                {
                    animatingLevelUp = false;
                    canCountTimer = true;
                }
            }
        }

        private void checkMousePosition()
        {
            if (!Context.IsWorldReady && Game1.CurrentEvent == null) return;

            Vector2 mousePos;
            mousePos.X = Game1.getMousePosition(true).X;
            mousePos.Y = Game1.getMousePosition(true).Y;
            int totalBackgroundSize = BarController.AdjustBackgroundSize(barQuantity, backgroundBar.Height * this.config.mainScale, barSpacement) + (backgroundTop.Height * this.config.mainScale) + (backgroundBottom.Height * this.config.mainScale);

            //Reset button check click
            if (mousePos.X >= configButtonPosX + 75 && mousePos.X <= configButtonPosX + 75 + (buttonReset.Width * 3) &&
                mousePos.Y >= configButtonPosY && mousePos.Y <= configButtonPosY + (buttonReset.Height * 3) &&
                inConfigMode)
            {
                blockActions();
            }
            //CONFIG KEY
            else if (mousePos.X >= configButtonPosX && mousePos.X <= configButtonPosX + (buttonConfig.Width * 3) &&
                mousePos.Y >= configButtonPosY && mousePos.Y <= configButtonPosY + (buttonConfig.Height * 3))
            {
                blockActions();
            }
            //TOGGLE UI
            else if (mousePos.X >= configButtonPosX + 75 && mousePos.X <= configButtonPosX + 75 + (buttonConfig.Width * 3) &&
                mousePos.Y >= configButtonPosY && mousePos.Y <= configButtonPosY + (buttonConfig.Height * 3))
            {
                blockActions();
            }
            //Box click check
            else if (mousePos.X >= this.config.mainPosX && mousePos.X <= this.config.mainPosX + (backgroundTop.Width * this.config.mainScale) &&
                mousePos.Y >= this.config.mainPosY && mousePos.Y <= this.config.mainPosY + totalBackgroundSize &&
                inConfigMode)
            {
                blockActions();
            }
            //Decrease button click check
            else if (mousePos.X >= this.config.mainPosX && mousePos.X <= this.config.mainPosX + (buttonDecreaseSize.Width * 3) &&
                mousePos.Y >= this.config.mainPosY - 30 && mousePos.Y <= (this.config.mainPosY - 30) + (buttonDecreaseSize.Height * 3) &&
                inConfigMode)
            {
                blockActions();
            }
            //Increase button click check
            else if (mousePos.X >= this.config.mainPosX + 25 && mousePos.X <= (this.config.mainPosX + 25) + (buttonDecreaseSize.Width * 3) &&
                mousePos.Y >= this.config.mainPosY - 30 && mousePos.Y <= (this.config.mainPosY - 30) + (buttonDecreaseSize.Height * 3) &&
                inConfigMode)
            {
                blockActions();
            }
            //Background toggler button check click
            else if (mousePos.X >= this.config.mainPosX + 75 && mousePos.X <= (this.config.mainPosX + 75) + (buttonDecreaseSize.Width * 3) &&
                mousePos.Y >= this.config.mainPosY - 30 && mousePos.Y <= (this.config.mainPosY - 30) + (buttonDecreaseSize.Height * 3) &&
                inConfigMode)
            {
                blockActions();
            }
            //Levelup toggler button check click
            else if (mousePos.X >= this.config.mainPosX + 100 && mousePos.X <= (this.config.mainPosX + 100) + (buttonDecreaseSize.Width * 3) &&
                mousePos.Y >= this.config.mainPosY - 30 && mousePos.Y <= (this.config.mainPosY - 30) + (buttonDecreaseSize.Height * 3) &&
                inConfigMode)
            {
                blockActions();
            }
            //Experience toggler button check click
            else if (mousePos.X >= this.config.mainPosX + 125 && mousePos.X <= (this.config.mainPosX + 125) + (buttonDecreaseSize.Width * 3) &&
                mousePos.Y >= this.config.mainPosY - 30 && mousePos.Y <= (this.config.mainPosY - 30) + (buttonDecreaseSize.Height * 3) &&
                inConfigMode)
            {
                blockActions();
            }
            else
            {
                unblockActions();
            }
        }

        private void blockActions()
        {
            Game1.player.canOnlyWalk = true;
        }
        private void unblockActions()
        {
            Game1.player.canOnlyWalk = false;
        }
    }
}
