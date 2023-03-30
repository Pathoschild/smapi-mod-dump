/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace Circuit.UI
{
    internal class StartingOptionCheckbox : ClickableTextureComponent
    {
        private static readonly Texture2D Sheet = Game1.mouseCursors;

        private static readonly Rectangle CheckedSource = new(236, 425, 9, 9);

        private static readonly Rectangle UncheckedSource = new(227, 425, 9, 9);

        private string Label { get; }

        public StartingOption StartingOption { get; }

        private bool isChecked = false;

        public bool Checked
        {
            get
            {
                return isChecked;
            }
            private set
            {
                isChecked = value;
                sourceRect = value ? CheckedSource : UncheckedSource;
            }
        }

        public StartingOptionCheckbox(string label, StartingOption startingOption, bool defaultState = false)
            : base(startingOption.ToString(), new Rectangle(0, 0, 9, 9), null, "", Sheet, UncheckedSource, 3f)
        {
            Label = label;
            StartingOption = startingOption;

            Checked = defaultState;
        }

        public void SetPosition(int x, int y)
        {
            bounds.X = x;
            bounds.Y = y;

            Vector2 labelSize = Game1.smallFont.MeasureString(Label);

            bounds.Width = 27 + (int)labelSize.X;
            bounds.Height = 27;
        }

        public void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (bounds.Contains(x, y))
            {
                if (playSound)
                    Game1.playSound("drumkit6");

                Checked = !Checked;
            }
        }

        public void Draw(SpriteBatch b)
        {
            draw(b);

            Utility.drawTextWithShadow(
                b,
                Label,
                Game1.smallFont,
                new(
                    bounds.X + 36,
                    bounds.Y - 2
                ),
                Game1.textColor
            );
        }
    }

    internal class GameSetupMenu : IClickableMenu
    {
        private SeedInputBox SeedTextBox;

        private ClickableComponent SeedTextBoxComponent;

        private ClickableTextureComponent RandomButton;

        private ClickableTextureComponent DoneButton;

        private Dropdown<int> RunDurationDropdown;

        private List<StartingOptionCheckbox> StartingOptionChecks { get; } = new();

        public GameSetupMenu()
        {
            allClickableComponents ??= new();

            SeedTextBox = new(Game1.smallFont, Game1.textColor);
            SeedTextBox.OnEnterPressed += textBoxEnter;
            Game1.keyboardDispatcher.Subscriber = SeedTextBox;
            SeedTextBox.Selected = true;

            SeedTextBoxComponent = new(Rectangle.Empty, "")
            {
                myID = 1001,
                rightNeighborID = 1002,
                downNeighborID = 1004
            };
            RandomButton = new(Rectangle.Empty, Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f)
            {
                myID = 1002,
                leftNeighborID = 1001,
                rightNeighborID = 1003
            };
            DoneButton = new(Rectangle.Empty, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
            {
                myID = 1003,
                leftNeighborID = 1002
            };

            int selectedDuration = ModEntry.Instance.NewRunDurationSeconds;
            RunDurationDropdown = new(0, 0, Game1.smallFont, selectedDuration, new int[] { 3600, 7200, 10800 }, GetSelectionLabel)
            {
                myID = 1004,
                upNeighborID = 1001
            };

            foreach (StartingOption option in Enum.GetValues<StartingOption>())
            {
                StartingOptionCheckbox checkbox = new(GetStartingOptionLabel(option), option, ModEntry.Instance.StartingOptions.Contains(option));
                StartingOptionChecks.Add(checkbox);
                allClickableComponents.Add(checkbox);
            }

            CalculatePositions();
            SeedTextBox.Text = ModEntry.Instance.RunRngSeed.ToString();

            allClickableComponents.Add(SeedTextBoxComponent);
            allClickableComponents.Add(RandomButton);
            allClickableComponents.Add(DoneButton);
            allClickableComponents.Add(RunDurationDropdown);

            if (Game1.options.gamepadControls)
                snapToDefaultClickableComponent();
        }

        public static string GetStartingOptionLabel(StartingOption option)
        {
            return option switch
            {
                StartingOption.BoatRepaired => "Boat Repaired",
                StartingOption.SkullKey => "Skull Key",
                StartingOption.SewerKey => "Rusty Key",
                StartingOption.TownKey => "Key to the Town",
                StartingOption.Kent => "Kent in Year 1",
                StartingOption.MinesElevators => "All Mines Elevators",
                _ => throw new NotImplementedException("whoops")
            };
        }

        private string GetSelectionLabel(int seconds)
        {
            return seconds switch
            {
                3600 => "1 hour",
                7200 => "2 hours",
                10800 => "3 hours",
                _ => "Unknown"
            };
        }

        private void CalculatePositions()
        {
            width = 600;
            height = 440;

            xPositionOnScreen = Game1.uiViewport.Width / 2 - (width / 2);
            yPositionOnScreen = Game1.uiViewport.Height / 2 - (height / 2);

            SeedTextBox.X = xPositionOnScreen + 32;
            SeedTextBox.Y = yPositionOnScreen + 94;
            SeedTextBox.Width = 370;
            SeedTextBox.Height = 186;

            SeedTextBoxComponent.bounds = new(SeedTextBox.X - 64, SeedTextBox.Y - 16, 400, 75);

            RandomButton.bounds = new(xPositionOnScreen + width - 150, yPositionOnScreen + 94, 64, 64);
            DoneButton.bounds = new(xPositionOnScreen + width - 90, yPositionOnScreen + 82, 64, 64);

            RunDurationDropdown.bounds.X = xPositionOnScreen + 20;
            RunDurationDropdown.bounds.Y = SeedTextBox.Y + 132;
            RunDurationDropdown.ReinitializeComponents();

            int lastCheckboxY = SeedTextBox.Y + 135;

            foreach (var checkbox in StartingOptionChecks)
            {
                checkbox.SetPosition(xPositionOnScreen + 300, lastCheckboxY);
                lastCheckboxY += checkbox.bounds.Height + 4;
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            CalculatePositions();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(1001);
            snapCursorToCurrentSnappedComponent();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            DoneButton.tryHover(x, y);
            RandomButton.tryHover(x, y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            SeedTextBox.Update();
            if (DoneButton.containsPoint(x, y))
            {
                textBoxEnter(SeedTextBox);
                Game1.playSound("smallSelect");
            }
            else if (RandomButton.containsPoint(x, y))
            {
                SeedTextBox.Text = Guid.NewGuid().GetHashCode().ToString();
                Game1.playSound("drumkit6");
            }

            foreach (var checkbox in StartingOptionChecks)
                checkbox.receiveLeftClick(x, y, playSound);

            RunDurationDropdown.TryClick(x, y);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);

            RunDurationDropdown.ReceiveScrollWheelAction(direction);
        }

        private void textBoxEnter(TextBox sender)
        {
            if (sender.Text.Length >= 1)
            {
                ModEntry.Instance.StartingOptions.Clear();

                ModEntry.Instance.RunRngSeed = int.Parse(sender.Text);
                ModEntry.Instance.RunRng = new(ModEntry.Instance.RunRngSeed);

                ModEntry.Instance.NewRunDurationSeconds = RunDurationDropdown.Selected;

                foreach (StartingOptionCheckbox checkbox in StartingOptionChecks)
                {
                    if (checkbox.Checked)
                        ModEntry.Instance.StartingOptions.Add(checkbox.StartingOption);
                }

                ModEntry.Instance.RecreateGameManagers();

                Game1.exitActiveMenu();
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            SpriteText.drawStringWithScrollCenteredAt(b, "Run Setup", Game1.uiViewport.Width / 2, yPositionOnScreen - 70, "Run Setup");

            drawTextureBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                Color.White,
                drawShadow: true
            );

            Utility.drawTextWithShadow(
                b,
                "Seed",
                Game1.dialogueFont,
                new Vector2(SeedTextBox.X, SeedTextBox.Y - 74),
                Game1.textColor
            );

            Utility.drawTextWithShadow(
                b,
                "Run Duration",
                Game1.dialogueFont,
                new Vector2(RunDurationDropdown.bounds.X + 10, RunDurationDropdown.bounds.Y - 50),
                Game1.textColor
            );

            Utility.drawTextWithShadow(
                b,
                "Start Options",
                Game1.dialogueFont,
                new Vector2(RunDurationDropdown.bounds.X + 275, RunDurationDropdown.bounds.Y - 50),
                Game1.textColor
            );

            SeedTextBox.Draw(b);
            DoneButton.draw(b);
            RandomButton.draw(b);
            RunDurationDropdown.Draw(b);

            foreach (var checkbox in StartingOptionChecks)
                checkbox.Draw(b);

            drawMouse(b);
        }
    }

    internal class SeedInputBox : TextBox
    {
        public SeedInputBox(SpriteFont font, Color textColor) : base(null, null, font, textColor) { }

        public override void RecieveTextInput(char inputChar)
        {
            if (Text.Length == 0 && inputChar == '-')
            {
                Text += inputChar;
                return;
            }

            if (!Selected || !char.IsDigit(inputChar))
                return;

            Text += inputChar;
        }
    }
}
