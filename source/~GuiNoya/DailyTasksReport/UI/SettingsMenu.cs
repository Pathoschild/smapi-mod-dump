using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace DailyTasksReport.UI
{
    internal class SettingsMenu : IClickableMenu
    {
        private const int ItemsPerPage = 8;

        private static IClickableMenu _previousMenu;
        private readonly ClickableTextureComponent _downArrow;
        private readonly List<OptionsElement> _options = new List<OptionsElement>();

        private readonly ModEntry _parent;
        private readonly ClickableTextureComponent _scrollBar;
        private readonly List<ClickableComponent> _slots = new List<ClickableComponent>();

        private readonly ClickableTextureComponent _upArrow;
        private readonly int _yMargin = Game1.tileSize / 4;

        private int _currentIndex;
        private Rectangle _scrollBarRunner;
        private int _yScrollBarOffsetHeld = -1;

        private SettingsMenu(ModEntry parent, int currentIndex = 0) :
            base(Game1.viewport.Width / 2 - Game1.tileSize * 10 / 2,
                Game1.viewport.Height / 2 - Game1.tileSize * 8 / 2,
                Game1.tileSize * 10,
                Game1.tileSize * 8,
                true)
        {
            _parent = parent;
            _currentIndex = currentIndex;

            Game1.playSound("bigSelect");

            upperRightCloseButton.bounds = new Rectangle(xPositionOnScreen + width + Game1.pixelZoom * 4,
                yPositionOnScreen - Game1.pixelZoom * 20, Game1.pixelZoom * 12, Game1.pixelZoom * 12);

            upperRightCloseButton.myID = 100;
            upperRightCloseButton.downNeighborID = 0;

            // Initialize UI components
            _upArrow = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width + Game1.tileSize / 4, yPositionOnScreen, Game1.pixelZoom * 11,
                    Game1.pixelZoom * 12), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);
            _downArrow = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width + Game1.tileSize / 4,
                    yPositionOnScreen + height - Game1.pixelZoom * 12, Game1.pixelZoom * 11, Game1.pixelZoom * 12),
                Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom);
            _scrollBar = new ClickableTextureComponent(
                new Rectangle(_upArrow.bounds.X + Game1.pixelZoom * 3,
                    _upArrow.bounds.Y + _upArrow.bounds.Height + Game1.pixelZoom, Game1.pixelZoom * 6,
                    Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), Game1.pixelZoom);
            _scrollBarRunner = new Rectangle(_scrollBar.bounds.X, _scrollBar.bounds.Y, _scrollBar.bounds.Width,
                height - _upArrow.bounds.Height * 2 - Game1.pixelZoom * 3);

            for (var i = 0; i < ItemsPerPage; ++i)
            {
                var clickableComponent = new ClickableComponent(new Rectangle(xPositionOnScreen,
                    yPositionOnScreen + _yMargin + (height - Game1.tileSize / 2) / ItemsPerPage * i, width,
                    (height - _yMargin * 2) / ItemsPerPage), i.ToString())
                {
                    myID = i,
                    fullyImmutable = true,
                    upNeighborID = i > 0 ? i - 1 : -7777,
                    downNeighborID = i < ItemsPerPage - 1 ? i + 1 : -7777
                };
                _slots.Add(clickableComponent);
            }

            // Add options
            _options.Add(new InputListener("Open Report Key", OptionsEnum.OpenReportKey, _slots[0].bounds.Width,
                parent.Config));
            _options.Add(new InputListener("Open Settings Key", OptionsEnum.OpenSettings, _slots[0].bounds.Width,
                parent.Config));
            _options.Add(new InputListener("Toggle Bubbles Key", OptionsEnum.ToggleBubbles, _slots[0].bounds.Width,
                _parent.Config));
            _options.Add(new Checkbox("Show report button", OptionsEnum.ShowReportButton, parent.Config));
            _options.Add(new Checkbox("Display bubbles", OptionsEnum.DisplayBubbles, parent.Config, true,
                _slots[0].bounds.Width));
            _options.Add(new Checkbox("Show detailed info", OptionsEnum.ShowDetailedInfo, parent.Config));
            _options.Add(new OptionsElement("Report:"));
            _options.Add(new Checkbox("New recipe on TV", OptionsEnum.NewRecipeOnTv, parent.Config));
            _options.Add(new Checkbox("Birthdays", OptionsEnum.Birthdays, parent.Config));
            _options.Add(new Checkbox("Traveling merchant in town", OptionsEnum.TravelingMerchant, parent.Config));
            _options.Add(new Checkbox("Unwatered crops", OptionsEnum.UnwateredCrops, parent.Config));
            _options.Add(new Checkbox("Unharvested crops", OptionsEnum.UnharvestedCrops, parent.Config));
            _options.Add(new Checkbox("Dead crops", OptionsEnum.DeadCrops, parent.Config));
            _options.Add(new QualityOption("Fruits in trees", OptionsEnum.FruitTrees, parent.Config, 0, 3));
            _options.Add(new Checkbox("Unpetted pet", OptionsEnum.UnpettedPet, parent.Config));
            _options.Add(new Checkbox("Unfilled pet bowl", OptionsEnum.UnfilledPetBowl, parent.Config));
            _options.Add(new Checkbox("Unpetted animals", OptionsEnum.UnpettedAnimals, parent.Config));
            // Animal products
            _options.Add(new Checkbox("Animal products:", OptionsEnum.AllAnimalProducts, parent.Config));
            _options.Add(new Checkbox("Cow milk", OptionsEnum.CowMilk, parent.Config, 1));
            _options.Add(new Checkbox("Goat milk", OptionsEnum.GoatMilk, parent.Config, 1));
            _options.Add(new Checkbox("Sheep wool", OptionsEnum.SheepWool, parent.Config, 1));
            _options.Add(new Checkbox("Chicken egg", OptionsEnum.ChickenEgg, parent.Config, 1));
            _options.Add(new Checkbox("Dinosaur egg", OptionsEnum.DinosaurEgg, parent.Config, 1));
            _options.Add(new Checkbox("Duck egg", OptionsEnum.DuckEgg, parent.Config, 1));
            _options.Add(new Checkbox("Duck feather", OptionsEnum.DuckFeather, parent.Config, 1));
            _options.Add(new Checkbox("Rabbit's wool", OptionsEnum.RabbitsWool, parent.Config, 1));
            _options.Add(new Checkbox("Rabbit's foot", OptionsEnum.RabbitsFoot, parent.Config, 1));
            _options.Add(new Checkbox("Truffle", OptionsEnum.Truffle, parent.Config, 1));
            _options.Add(new Checkbox("Slime ball", OptionsEnum.SlimeBall, parent.Config, 1));
            // Other configs
            _options.Add(new Checkbox("Missing hay", OptionsEnum.MissingHay, parent.Config));
            _options.Add(new Checkbox("Items in farm cave", OptionsEnum.FarmCave, parent.Config));
            _options.Add(new Checkbox("Uncollected crabpots", OptionsEnum.UncollectedCrabpots, parent.Config));
            _options.Add(new Checkbox("Not baited crabpots", OptionsEnum.NotBaitedCrabpots, parent.Config));
            // Machines
            _options.Add(new Checkbox("Machines:", OptionsEnum.AllMachines, parent.Config));
            _options.Add(new Checkbox("Bee house", OptionsEnum.BeeHouse, parent.Config, 1));
            _options.Add(new QualityOption("Cask", OptionsEnum.Cask, _parent.Config, 1));
            _options.Add(new Checkbox("Charcoal Kiln", OptionsEnum.CharcoalKiln, parent.Config, 1));
            _options.Add(new Checkbox("Cheese Press", OptionsEnum.CheesePress, parent.Config, 1));
            _options.Add(new Checkbox("Crystalarium", OptionsEnum.Crystalarium, parent.Config, 1));
            _options.Add(new Checkbox("Furnace", OptionsEnum.Furnace, parent.Config, 1));
            _options.Add(new Checkbox("Keg", OptionsEnum.Keg, parent.Config, 1));
            _options.Add(new Checkbox("Lightning Rod", OptionsEnum.LightningRod, parent.Config, 1));
            _options.Add(new Checkbox("Loom", OptionsEnum.Loom, parent.Config, 1));
            _options.Add(new Checkbox("Mayonnaise Machine", OptionsEnum.MayonnaiseMachine, parent.Config, 1));
            _options.Add(new Checkbox("Oil Maker", OptionsEnum.OilMaker, parent.Config, 1));
            _options.Add(new Checkbox("Preserves Jar", OptionsEnum.PreservesJar, parent.Config, 1));
            _options.Add(new Checkbox("Recycling Machine", OptionsEnum.RecyclingMachine, parent.Config, 1));
            _options.Add(new Checkbox("Seed Maker", OptionsEnum.SeedMaker, parent.Config, 1));
            _options.Add(new Checkbox("Slime Egg-Press", OptionsEnum.SlimeEggPress, parent.Config, 1));
            _options.Add(new Checkbox("Soda Machine", OptionsEnum.SodaMachine, parent.Config, 1));
            _options.Add(
                new Checkbox("Statue Of Endless Fortune", OptionsEnum.StatueOfEndlessFortune, parent.Config, 1));
            _options.Add(new Checkbox("Statue Of Perfection", OptionsEnum.StatueOfPerfection, parent.Config, 1));
            _options.Add(new Checkbox("Tapper", OptionsEnum.Tapper, parent.Config, 1));
            _options.Add(new Checkbox("Worm bin", OptionsEnum.WormBin, parent.Config, 1));

            ReportConfigChanged += SettingsMenu_ReportConfigChanged;

            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls) return;
            allClickableComponents = new List<ClickableComponent>(_slots) { upperRightCloseButton };
            currentlySnappedComponent = allClickableComponents[0];
            snapCursorToCurrentSnappedComponent();
        }

        private void SettingsMenu_ReportConfigChanged(object sender, EventArgs e)
        {
            RefreshOptionStatus();
            _parent.Helper.WriteConfig(_parent.Config);
        }

        public sealed override void snapToDefaultClickableComponent()
        {
            _currentIndex = 0;
            currentlySnappedComponent = allClickableComponents[0];
            snapCursorToCurrentSnappedComponent();
        }

        public sealed override void snapCursorToCurrentSnappedComponent()
        {
            if (currentlySnappedComponent?.myID < _options.Count)
                switch (_options[currentlySnappedComponent.myID + _currentIndex])
                {
                    case InputListener _:
                        Game1.setMousePosition(currentlySnappedComponent.bounds.Right - Game1.tileSize * 3 / 4,
                            currentlySnappedComponent.bounds.Center.Y);
                        break;

                    case Checkbox cb:
                        cb.CursorAboveOption();
                        break;

                    case QualityOption qo:
                        qo.CursorAboveOption();
                        break;

                    default:
                        Game1.setMousePosition(currentlySnappedComponent.bounds.Left + Game1.tileSize * 3 / 4,
                            currentlySnappedComponent.bounds.Center.Y);
                        break;
                }
            else
                base.snapCursorToCurrentSnappedComponent();
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldId)
        {
            if (oldId == ItemsPerPage - 1 && direction == 2 && _currentIndex < _options.Count - ItemsPerPage)
            {
                // Go down
                ++_currentIndex;
                AdjustScrollBarPosition();
                Game1.playSound("shiny4");
            }
            else if (oldId == 0 && direction == 0)
            {
                if (_currentIndex > 0)
                {
                    // Go up
                    --_currentIndex;
                    AdjustScrollBarPosition();
                    Game1.playSound("shiny4");
                }
                else
                {
                    // Go to close button
                    currentlySnappedComponent = allClickableComponents[ItemsPerPage];
                    snapCursorToCurrentSnappedComponent();
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
            {
                ReportConfigChanged -= SettingsMenu_ReportConfigChanged;
                Game1.activeClickableMenu = _previousMenu;
                _previousMenu = null;
                exitFunction?.Invoke();
                if (Game1.options.snappyMenus && Game1.options.gamepadControls)
                    Game1.activeClickableMenu?.snapCursorToCurrentSnappedComponent();
                return;
            }

            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
                return;

            applyMovementKey(key);

            if (currentlySnappedComponent.myID >= 0 && currentlySnappedComponent.myID < ItemsPerPage)
                _options[currentlySnappedComponent.myID + _currentIndex].receiveKeyPress(key);
        }

        public override void draw(SpriteBatch b)
        {
            _previousMenu?.draw(b);

            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            drawTextureBox(Game1.spriteBatch, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);
            var yTitleOffset = (int)(SpriteText.getHeightOfString("Daily Tasks Report Settings") * 1.6);
            SpriteText.drawStringWithScrollCenteredAt(b, "Daily Tasks Settings", xPositionOnScreen + width / 2,
                yPositionOnScreen - yTitleOffset);

            _upArrow.draw(b);
            _downArrow.draw(b);
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), _scrollBarRunner.X, _scrollBarRunner.Y,
                _scrollBarRunner.Width, _scrollBarRunner.Height, Color.White, Game1.pixelZoom);
            _scrollBar.draw(b);

            base.draw(b);

            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

            for (var i = 0; i < ItemsPerPage; ++i)
                _options[_currentIndex + i].draw(b, _slots[i].bounds.X, _slots[i].bounds.Y);

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            if (!Game1.options.hardwareCursor)
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()),
                    Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0.0f,
                    Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (_downArrow.bounds.Contains(x, y))
            {
                if (_currentIndex >= _options.Count - ItemsPerPage) return;

                ++_currentIndex;
                AdjustScrollBarPosition();
                Game1.playSound("shwip");
                return;
            }
            if (_upArrow.bounds.Contains(x, y))
            {
                if (_currentIndex <= 0) return;

                --_currentIndex;
                AdjustScrollBarPosition();
                Game1.playSound("shwip");
                return;
            }

            if (_scrollBar.bounds.Contains(x, y))
            {
                _yScrollBarOffsetHeld = y - _scrollBar.bounds.Y;
                return;
            }

            if (_scrollBarRunner.Contains(x, y))
            {
                _yScrollBarOffsetHeld = _scrollBar.bounds.Height / 2;
                return;
            }

            for (var i = 0; i < _slots.Count; ++i)
                if (_slots[i].bounds.Contains(x, y) &&
                    _options[_currentIndex + i].bounds.Contains(x - _slots[i].bounds.X, y - _slots[i].bounds.Y))
                {
                    _options[_currentIndex + i].receiveLeftClick(x - _slots[i].bounds.X, y - _slots[i].bounds.Y);
                    break;
                }

            // Check the close button
            if (upperRightCloseButton != null && readyToClose() && upperRightCloseButton.containsPoint(x, y) ||
                !isWithinBounds(x, y))
            {
                if (playSound)
                    Game1.playSound("bigDeSelect");
                ReportConfigChanged -= SettingsMenu_ReportConfigChanged;
                Game1.activeClickableMenu = _previousMenu;
                _previousMenu = null;
                exitFunction?.Invoke();
                if (Game1.options.snappyMenus && Game1.options.gamepadControls)
                    Game1.activeClickableMenu?.snapCursorToCurrentSnappedComponent();
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (_yScrollBarOffsetHeld < 0)
                return;

            SetCurrentIndexFromScrollBar();
            AdjustScrollBarPosition();
            _yScrollBarOffsetHeld = -1;
        }

        public override void leftClickHeld(int x, int y)
        {
            if (_yScrollBarOffsetHeld < 0)
                return;

            var scrollBarTop = y - _yScrollBarOffsetHeld;
            if (scrollBarTop < _scrollBarRunner.Y)
                scrollBarTop = _scrollBarRunner.Y;
            else if (scrollBarTop > _scrollBarRunner.Bottom - _scrollBar.bounds.Height)
                scrollBarTop = _scrollBarRunner.Bottom - _scrollBar.bounds.Height;
            _scrollBar.bounds.Y = scrollBarTop;
            var oldCurrentIndex = _currentIndex;
            SetCurrentIndexFromScrollBar();
            if (oldCurrentIndex != _currentIndex)
                Game1.playSound("shiny4");
        }

        private void AdjustScrollBarPosition()
        {
            _scrollBar.bounds.Y = (int)(_scrollBarRunner.Y +
                                         (double)(_scrollBarRunner.Height - _scrollBar.bounds.Height) /
                                         (_options.Count - ItemsPerPage) * _currentIndex);
        }

        private void SetCurrentIndexFromScrollBar()
        {
            _currentIndex = (_options.Count - ItemsPerPage) * (_scrollBar.bounds.Y - _scrollBarRunner.Y) /
                            (_scrollBarRunner.Height - _scrollBar.bounds.Height);
        }

        private void RefreshOptionStatus()
        {
            foreach (var option in _options)
                switch (option)
                {
                    case Checkbox cb:
                        cb.RefreshStatus();
                        break;

                    case QualityOption qo:
                        qo.RefreshStatus();
                        break;

                    default:
                        break;
                }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (direction > 0 && _currentIndex > 0)
            {
                --_currentIndex;
                AdjustScrollBarPosition();
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && _currentIndex < _options.Count - ItemsPerPage)
            {
                ++_currentIndex;
                AdjustScrollBarPosition();
                Game1.playSound("shiny4");
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            _upArrow.tryHover(x, y);
            _downArrow.tryHover(x, y);
            _scrollBar.tryHover(x, y);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            if (!(Game1.activeClickableMenu is SettingsMenu)) return;

            Game1.activeClickableMenu = new SettingsMenu(_parent, _currentIndex);
            _previousMenu?.gameWindowSizeChanged(oldBounds, newBounds);
        }

        public static void OpenMenu(ModEntry parent)
        {
            _previousMenu = Game1.activeClickableMenu;
            if (Game1.activeClickableMenu != null)
                Game1.exitActiveMenu();
            Game1.activeClickableMenu = new SettingsMenu(parent);
        }

        public static event EventHandler ReportConfigChanged;

        internal static void RaiseReportConfigChanged()
        {
            ReportConfigChanged?.Invoke(null, null);
        }
    }
}