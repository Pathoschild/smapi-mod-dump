/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProfitCalculator.main;
using ProfitCalculator.ui;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using static ProfitCalculator.Utils;

namespace ProfitCalculator.menus
{
    /// <summary>
    /// The main menu for the profit calculator. This menu is opened by pressing the "F8" key by default. It is used to set the settings for the profit calculator. It is also used to open the results menu. This menu is the parent menu for the <see cref="ProfitCalculatorResultsList"/> menu.
    /// </summary>
    public class ProfitCalculatorMainMenu : IClickableMenu
    {
        /// <summary> The day for planting. </summary>
        public uint Day { get; set; } = 1;

        /// <summary> The ammount of days a season can have. </summary>
        public uint MaxDay { get; set; } = 28;

        /// <summary> The minimum day a season can have. </summary>
        public uint MinDay { get; set; } = 1;

        /// <summary> The season for planting. </summary>
        public Season Season { get; set; } = Utils.Season.Spring;

        /// <summary>
        /// Sets the season for planting.
        /// </summary>
        /// <param name="season"> The season to set. String, case insensetive</param>
        public void SetSeason(string season)
        {
            Season = (Season)Season.Parse(typeof(Season), season, false);
        }

        /// <summary> The type of produce to calculate with, for now only raw works. </summary>
        public ProduceType ProduceType { get; set; } = Utils.ProduceType.Raw;

        /// <summary> The quality of fertilizer to use. </summary>
        public FertilizerQuality FertilizerQuality { get; set; } = Utils.FertilizerQuality.None;

        /// <summary>
        /// Whether the play wants to check which plants he can purchase with available cash.
        /// </summary>
        public bool PayForSeeds { get; set; } = true;

        /// <summary> Whether the play wants to check which plants he can purchase with available cash. </summary>
        public bool PayForFertilizer { get; set; } = false;

        /// <summary> The maximum ammount of money the player wants to spend in seeds. </summary>
        public uint MaxMoney { get; set; } = 0;

        /// <summary> Whether the player wants to use base stats or not. </summary>
        public bool UseBaseStats { get; set; } = false;

        private static readonly int widthOnScreen = 632 + borderWidth * 2;
        private static readonly int heightOnScreen = 600 + borderWidth * 2 + Game1.tileSize;
        private bool stopSpreadingClick = false;

        private readonly List<ClickableComponent> Labels = new();

        private readonly List<BaseOption> Options = new();

        private ClickableComponent calculateButton;
        private ClickableComponent resetButton;

        /// <summary> Whether the profit calculator is open or not.  </summary>
        public bool IsProfitCalculatorOpen { get; set; } = false;

        /// <summary>
        /// Constructor for the ProfitCalculatorMainMenu class.
        /// </summary>
        public ProfitCalculatorMainMenu() :
            base(
                (int)GetAppropriateMenuPosition().X,
                (int)GetAppropriateMenuPosition().Y,
                widthOnScreen,
                heightOnScreen)
        {
            behaviorBeforeCleanup = delegate
            {
                IsProfitCalculatorOpen = false;
            };

            this.xPositionOnScreen = (int)GetAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)GetAppropriateMenuPosition().Y;
            this.Reset();
        }

        #region Menu Button Setups

        private void SetUpPositions()
        {
            this.SetUpButtonPositions();
            //option order:
            //Day int

            this.SetUpDayOptionPositions();
            //Season dropdown
            this.SetUpSeasonOptionPositions();
            //Produce Type dropdown
            this.SetUpProduceTypeOptionPositions();
            //Fertilizer Quality dropdown
            this.SetUpFertilizerQualityPositions();
            //Pay for Seeds checkbox
            this.SetUpSeedsOptionPositions();
            //Pay for Fertilizer checkbox
            this.SetUpFertilizerOptionPositions();
            //Max Money int
            this.SetUpMoneyOptionPositions();
            //Use Base Stats checkbox
            this.SetUpBaseStatsOptionPositions();
        }

        private void SetUpButtonPositions()
        {
            calculateButton = new ClickableComponent(
                new Rectangle(
                    this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                    this.yPositionOnScreen + borderWidth * 2 + spaceToClearTopBorder + Game1.tileSize * 7,
                    Game1.tileSize * 2,
                    Game1.tileSize
                ),
                "calculate",
                Helper.Translation.Get("calculate")
            );

            resetButton = new ClickableComponent(
                new Rectangle(
                    this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 2 + Game1.tileSize / 4,
                    this.yPositionOnScreen + borderWidth * 2 + spaceToClearTopBorder + Game1.tileSize * 7,
                    Game1.tileSize * 2,
                    Game1.tileSize
                ),
                "reset",
                Helper.Translation.Get("reset")
                );
        }

        private void SetUpDayOptionPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "day",
                    Helper.Translation.Get("day") + ": "
                )
            );
            UIntOption dayOption =
               new(
                   this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5 - Game1.tileSize / 8,
                   this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4,
                   () => "day",
                   () => Helper.Translation.Get("day"),
                   valueGetter: () => this.Day,
                   max: () => this.MaxDay,
                   min: () => this.MinDay,
                   valueSetter: (string value) => this.Day = uint.Parse(value),
                   enableClamping: true
               );
            dayOption.SetTexture(Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "text_box_small.png")));
            Options.Add(dayOption);
        }

        private void SetUpSeasonOptionPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "season",
                    Helper.Translation.Get("season") + ": "
                )
            );

            DropdownOption seasonOption = new(
                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize + Game1.tileSize / 4,
                name: () => "season",
                label: () => Helper.Translation.Get("season"),
                choices: () => Utils.Season.GetNames(typeof(Utils.Season)),
                labels: () => Utils.GetAllTranslatedSeasons(),
                valueGetter: this.Season.ToString,
                valueSetter:
                    (string value) => this.Season = (Season)Utils.Season.Parse(typeof(Season), value, true)
            )
            {
                MaxValuesAtOnce = Enum.GetValues(typeof(Season)).Length//size of enum
            };

            Options.Add(seasonOption);
        }

        private void SetUpProduceTypeOptionPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 2,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "produceType",
                    Helper.Translation.Get("produce-type") + ": "
                )
            );

            DropdownOption produceTypeOption = new(
                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 2 + Game1.tileSize / 4,
                name: () => "produceType",
                label: () => Helper.Translation.Get("produce-type"),
                choices: () => Utils.ProduceType.GetNames(typeof(Utils.ProduceType)),
                labels: () => Utils.GetAllTranslatedProduceTypes(),
                valueGetter: this.ProduceType.ToString,
                valueSetter: (string value) => this.ProduceType = (ProduceType)Utils.ProduceType.Parse(typeof(ProduceType), value, true)
            )
            {
                MaxValuesAtOnce = Enum.GetValues(typeof(ProduceType)).Length//size of enum
            };
            Options.Add(produceTypeOption);
        }

        private void SetUpFertilizerQualityPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 3,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "fertilizerQuality",
                    Helper.Translation.Get("fertilizer-type") + ": "
                )
            );
            DropdownOption fertilizerQualityOption = new(
                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 3 + Game1.tileSize / 4,
                name: () => "fertilizerQuality",
                label: () => Helper.Translation.Get("fertilizer-quality"),
                choices: () => Utils.FertilizerQuality.GetNames(typeof(Utils.FertilizerQuality)),
                labels: () => Utils.GetAllTranslatedFertilizerQualities(),
                valueGetter: this.FertilizerQuality.ToString,
                valueSetter: (string value) => this.FertilizerQuality = (FertilizerQuality)Utils.FertilizerQuality.Parse(typeof(FertilizerQuality), value, true)
            )
            {
                MaxValuesAtOnce = Enum.GetValues(typeof(FertilizerQuality)).Length//size of enum
            };
            Options.Add(fertilizerQualityOption);
        }

        private void SetUpSeedsOptionPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 4,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "payForSeeds",
                    Helper.Translation.Get("pay-for-seeds") + ": "
                )
            );

            CheckboxOption payForSeeds = new(
                    this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                    this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 4 + Game1.tileSize / 4,
                    () => "payForSeeds",
                    () => Helper.Translation.Get("pay-for-seeds"),
                    () => this.PayForSeeds,
                    (bool value) => this.PayForSeeds = value

                );
            Options.Add(payForSeeds);
        }

        private void SetUpFertilizerOptionPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 5,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "payForFertilizer",
                    Helper.Translation.Get("pay-for-fertilizer") + ": "
                )
            );
            CheckboxOption payForFertilizer = new(
                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 5 + Game1.tileSize / 4,
                () => "payForFertilizer",
                () => Helper.Translation.Get("pay-for-fertilizer"),
                () => this.PayForFertilizer,
                (bool value) => this.PayForFertilizer = value
            );
            Options.Add(payForFertilizer);
        }

        private void SetUpMoneyOptionPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 6,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "maxMoney",
                    Helper.Translation.Get("max-money") + ": "
                )
        );
            UIntOption maxMoneyOption =
                new(
                   this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5 - Game1.tileSize / 8,
                   this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 6 + Game1.tileSize / 4,
                   () => "maxMoney",
                   () => Helper.Translation.Get("max-money"),
                   valueGetter: () => this.MaxMoney,
                   max: () => 99999999,
                   min: () => 0,
                   valueSetter: (string value) => this.MaxMoney = uint.Parse(value),
                   enableClamping: true
                );
            Options.Add(maxMoneyOption);
        }

        private void SetUpBaseStatsOptionPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 7,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "useBaseStats",
                    Helper.Translation.Get("base-stats") + ": "
                )
            );
            CheckboxOption useBaseStatsOptions = new(
                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 7 + Game1.tileSize / 4,
                () => "useBaseStats",
                () => Helper.Translation.Get("base-stats"),
                () => this.UseBaseStats,
                (bool value) => this.UseBaseStats = value
            );
            Options.Add(useBaseStatsOptions);
        }

        #endregion Menu Button Setups

        #region Draw Methods

        /// <summary>
        /// Draws the menu. This method is called by the game. Options are drawn in SpriteSortMode.FrontToBack, Actions and Labels are drawn in SpriteSortMode.Deferred. This is done to prevent the options from being drawn over the actions and labels. Including dropdowns.
        /// </summary>
        /// <param name="b"> The spritebatch to draw with. </param>
        public override void draw(SpriteBatch b)

        {
            //draw bottom up

            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, widthOnScreen, heightOnScreen, speaker: false, drawOnlyBox: true);

            // Draw Labels and Options and buttons
            this.DrawActions(b);
            this.DrawLabels(b);
            //print active sort mode from b (private field called _sortMode)
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            this.DrawOptions(b);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            if (shouldDrawCloseButton()) base.draw(b);
            if (!Game1.options.hardwareCursor) b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        private void DrawActions(SpriteBatch b)
        {
            // Draw the calculate button.
            IClickableMenu.drawTextureBox
            (
                b,
                Game1.mouseCursors,
                new Rectangle(432, 439, 9, 9),
                calculateButton.bounds.X,
                calculateButton.bounds.Y,
                calculateButton.bounds.Width,
                calculateButton.bounds.Height,
                (calculateButton.scale != 1.0001f) ? Color.Wheat : Color.White,
                4f,
                false
            );
            b.DrawString
            (
                Game1.smallFont,
                calculateButton.label,
                new Vector2
                (
                    (float)calculateButton.bounds.X
                        + (calculateButton.bounds.Width / 2)
                        - (Game1.smallFont.MeasureString(calculateButton.label).X / 2),
                    (float)calculateButton.bounds.Y
                        + (calculateButton.bounds.Height / 2)
                        - (Game1.smallFont.MeasureString(calculateButton.name).Y / 2)
                ),
                Game1.textColor
            );

            // Draw the reset button.
            IClickableMenu.drawTextureBox
            (
                b,
                Game1.mouseCursors,
                new Rectangle(432, 439, 9, 9),
                resetButton.bounds.X,
                resetButton.bounds.Y,
                resetButton.bounds.Width,
                resetButton.bounds.Height,
                (resetButton.scale != 1.0001f) ? Color.Wheat : Color.White,
                4f,
                false
            );
            b.DrawString
            (
                Game1.smallFont,
                resetButton.label,
                new Vector2
                (
                    (float)resetButton.bounds.X
                        + (resetButton.bounds.Width / 2)
                        - (Game1.smallFont.MeasureString(resetButton.label).X / 2),
                    (float)resetButton.bounds.Y
                        + (resetButton.bounds.Height / 2)
                        - (Game1.smallFont.MeasureString(resetButton.name).Y / 2)
                ),
                Game1.textColor
            );
        }

        private void DrawLabels(SpriteBatch b)
        {
            foreach (ClickableComponent label in Labels)
            {
                b.DrawString(
                    Game1.dialogueFont,
                    label.label,
                    new Vector2(
                        (float)label.bounds.X,
                        (float)label.bounds.Y + (label.bounds.Height / 2) - (Game1.smallFont.MeasureString(label.name).Y / 2)
                    ),
                    Game1.textColor
                );
            }
        }

        private void DrawOptions(SpriteBatch b)
        {
            foreach (BaseOption option in Options)
            {
                option.Draw(b);
            }
        }

        #endregion Draw Methods

        #region Event Handling

        /// <summary>
        /// Handles key presses received by the menu.
        /// </summary>
        /// <param name="key"> The key that was pressed. </param>
        public override void receiveKeyPress(Keys key)
        {
            switch (key)
            {
                case Keys.Enter:
                    DoCalculation();
                    Game1.playSound("select");
                    break;

                case Keys.Escape:
                    exitThisMenu();
                    break;
            }
        }

        /// <summary>
        /// Handles mouse hovers received by the menu.
        /// </summary>
        /// <param name="x"> The x position of the mouse. </param>
        /// <param name="y"> The y position of the mouse. </param>
        public override void performHoverAction(int x, int y)
        {
            //TODO: add hover actions for buttons
        }

        /// <summary>
        /// Handles mouse clicks received by the menu.
        /// </summary>
        /// <param name="x"> The x position of the mouse. </param>
        /// <param name="y"> The y position of the mouse. </param>
        /// <param name="playSound"> Whether to play a sound when the click is received. </param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (calculateButton.containsPoint(x, y))
            {
                this.DoCalculation();
                if (playSound) Game1.playSound("select");
                return;
            }
            if (resetButton.containsPoint(x, y))
            {
                this.Reset();
                if (playSound) Game1.playSound("dialogueCharacterClose");
                return;
            }
            //for each option, check if it was clicked
            foreach (BaseOption option in Options)
            {
                if (!stopSpreadingClick)
                    option.ReceiveLeftClick(x, y, () => this.stopSpreadingClick = !this.stopSpreadingClick);
                else
                {
                    this.stopSpreadingClick = !this.stopSpreadingClick;
                    return;
                }
            }
        }

        private void Reset()
        {
            //set all the options to default values
            //get day from game
            Day = (uint)Game1.dayOfMonth;
            Season = (Season)Season.Parse(typeof(Season), Game1.currentSeason, true);
            ProduceType = Utils.ProduceType.Raw;
            FertilizerQuality = Utils.FertilizerQuality.None;
            PayForSeeds = true;
            PayForFertilizer = false;
            MaxMoney = (uint)Game1.player.team.money.Value;
            UseBaseStats = false;
            this.UpdateMenu();
        }

        /// <summary>
        /// Updates the menu. Refreshes the positions of the buttons and options.
        /// </summary>
        public void UpdateMenu()
        {
            Labels.Clear();
            Options.Clear();
            this.SetUpPositions();
        }

        /// <summary>
        /// Propagates an update call to the menu and all of its children.
        /// </summary>
        /// <param name="time"></param>
        public override void update(GameTime time)
        {
            base.update(time);
            this.xPositionOnScreen = (int)GetAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)GetAppropriateMenuPosition().Y;
            //update all the options and labels and buttons
            foreach (BaseOption option in Options)
            {
                option.Update();
            }
        }

        /// <summary>
        /// Gets the appropriate position for the menu to be in.
        /// </summary>
        /// <returns> The appropriate position for the menu to be in, in Vector2 format </returns>
        public static Vector2 GetAppropriateMenuPosition()
        {
            Vector2 defaultPosition = new(
                (Game1.viewport.Width * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (widthOnScreen / 2),
                (Game1.viewport.Height * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - (heightOnScreen / 2)
            );
            //Force the viewport into a position that it should fit into on the screen???
            if (defaultPosition.X + widthOnScreen > Game1.viewport.Width)
            {
                defaultPosition.X = 0;
            }

            if (defaultPosition.Y + heightOnScreen > Game1.viewport.Height)
            {
                defaultPosition.Y = 0;
            }
            return defaultPosition;
        }

        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = (int)GetAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)GetAppropriateMenuPosition().Y;

            this.UpdateMenu();
            _childMenu?.gameWindowSizeChanged(oldBounds, newBounds);
        }

        #endregion Event Handling

        private void DoCalculation()
        {
            ModEntry.Calculator.SetSettings(Day, MaxDay, MinDay, Season, ProduceType, FertilizerQuality, PayForSeeds, PayForFertilizer, MaxMoney, UseBaseStats);

            Monitor.Log("Doing Calculation", LogLevel.Debug);
            List<CropInfo> cropList = ModEntry.Calculator.RetrieveCropInfos();

            ProfitCalculatorResultsList profitCalculatorResultsList = new(cropList);
            this.SetChildMenu(profitCalculatorResultsList);
        }
    }
}