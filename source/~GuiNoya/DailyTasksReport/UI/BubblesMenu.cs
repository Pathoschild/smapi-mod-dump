using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System.Collections.Generic;

namespace DailyTasksReport.UI
{
    public sealed class BubblesMenu : IClickableMenu
    {
        private const int ItemsPerPage = 11;
        private readonly int _yMargin = Game1.tileSize / 4;

        private readonly ModConfig _config;
        private static IClickableMenu _previousMenu;
        private readonly List<OptionsElement> _options = new List<OptionsElement>();
        private readonly List<ClickableComponent> _slots = new List<ClickableComponent>();

        private BubblesMenu(ModConfig config) :
            base(Game1.viewport.Width / 2 - Game1.tileSize * 11 / 2,
                Game1.viewport.Height / 2 - Game1.tileSize * 9 / 2,
                Game1.tileSize * 11,
                Game1.tileSize * 9,
                true)
        {
            _config = config;

            Game1.playSound("bigSelect");

            upperRightCloseButton.bounds = new Rectangle(xPositionOnScreen + width + Game1.pixelZoom * 4,
                yPositionOnScreen - Game1.pixelZoom * 20, Game1.pixelZoom * 12, Game1.pixelZoom * 12);

            upperRightCloseButton.myID = 100;
            upperRightCloseButton.downNeighborID = 0;

            // Initialize UI components
            for (var i = 0; i < ItemsPerPage; ++i)
            {
                var clickableComponent = new ClickableComponent(new Rectangle(xPositionOnScreen,
                    yPositionOnScreen + _yMargin + (height - Game1.tileSize / 2) / ItemsPerPage * i, width,
                    (height - _yMargin * 2) / ItemsPerPage), i.ToString())
                {
                    myID = i,
                    fullyImmutable = true,
                    upNeighborID = i > 0 ? i - 1 : -7777,
                    downNeighborID = i + 1
                };
                _slots.Add(clickableComponent);
            }

            _options.Add(new Checkbox("Unwatered crops", OptionsEnum.DrawUnwateredCrops, config));
            _options.Add(new Checkbox("Unharvested crops", OptionsEnum.DrawUnharvestedCrops, config));
            _options.Add(new Checkbox("Dead crops", OptionsEnum.DrawDeadCrops, config));
            _options.Add(new Checkbox("Unpetted pet", OptionsEnum.DrawUnpettedPet, config));
            _options.Add(new Checkbox("Unpetted animals", OptionsEnum.DrawUnpettedAnimals, config));
            _options.Add(new Checkbox("Animals with produce", OptionsEnum.DrawAnimalsWithProduce, config));
            _options.Add(new Checkbox("Buildings with produce inside", OptionsEnum.DrawBuildingsWithProduce, config));
            _options.Add(new Checkbox("Buildings missing hay", OptionsEnum.DrawBuildingsMissingHay, config));
            _options.Add(new Checkbox("Truffles", OptionsEnum.DrawTruffles, config));
            _options.Add(new Checkbox("Crabpots not baited", OptionsEnum.DrawCrabpotsNotBaited, config));
            _options.Add(new Checkbox("Casks with lower quality items", OptionsEnum.DrawCask, config));

            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls) return;
            allClickableComponents = new List<ClickableComponent>(_slots) { upperRightCloseButton };
            currentlySnappedComponent = allClickableComponents[0];
            snapCursorToCurrentSnappedComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = allClickableComponents[0];
            snapCursorToCurrentSnappedComponent();
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            base.customSnapBehavior(direction, oldRegion, oldID);

            if (oldID != 0 || direction != 0) return;

            currentlySnappedComponent = upperRightCloseButton;
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
            {
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
                _options[currentlySnappedComponent.myID].receiveKeyPress(key);
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            if (currentlySnappedComponent?.myID < _options.Count)
            {
                if (_options[currentlySnappedComponent.myID] is Checkbox cb)
                    cb.CursorAboveOption();
            }
            else
            {
                base.snapCursorToCurrentSnappedComponent();
            }
        }

        public override void draw(SpriteBatch b)
        {
            _previousMenu?.draw(b);

            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            drawTextureBox(Game1.spriteBatch, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);
            var yTitleOffset = (int)(SpriteText.getHeightOfString("Bubble Settings") * 1.6);
            SpriteText.drawStringWithScrollCenteredAt(b, "Bubble Settings", xPositionOnScreen + width / 2,
                yPositionOnScreen - yTitleOffset);

            base.draw(b);

            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

            for (var i = 0; i < ItemsPerPage; ++i)
                _options[i].draw(b, _slots[i].bounds.X, _slots[i].bounds.Y);

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            if (!Game1.options.hardwareCursor)
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()),
                    Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0.0f,
                    Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            for (var i = 0; i < _slots.Count; ++i)
                if (_slots[i].bounds.Contains(x, y) &&
                    _options[i].bounds.Contains(x - _slots[i].bounds.X, y - _slots[i].bounds.Y))
                {
                    _options[i].receiveLeftClick(x - _slots[i].bounds.X, y - _slots[i].bounds.Y);
                    break;
                }

            // Check the close button
            if (upperRightCloseButton != null && readyToClose() && upperRightCloseButton.containsPoint(x, y) ||
            !isWithinBounds(x, y))
            {
                if (playSound)
                    Game1.playSound("bigDeSelect");
                Game1.activeClickableMenu = _previousMenu;
                _previousMenu = null;
                exitFunction?.Invoke();
                if (Game1.options.snappyMenus && Game1.options.gamepadControls)
                    Game1.activeClickableMenu?.snapCursorToCurrentSnappedComponent();
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            if (!(Game1.activeClickableMenu is BubblesMenu)) return;

            Game1.activeClickableMenu = new BubblesMenu(_config);
            _previousMenu?.gameWindowSizeChanged(oldBounds, newBounds);
        }

        internal static void OpenMenu(ModConfig config)
        {
            _previousMenu = Game1.activeClickableMenu;
            if (Game1.activeClickableMenu != null)
                Game1.exitActiveMenu();
            Game1.activeClickableMenu = new BubblesMenu(config);
        }
    }
}