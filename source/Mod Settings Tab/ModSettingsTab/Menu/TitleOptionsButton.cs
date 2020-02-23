using Microsoft.Xna.Framework;
using ModSettingsTab.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ModSettingsTab.Menu
{
    public static class TitleOptionsButton
    {
        private static readonly ClickableTextureComponent Button;
        
        private static IReflectedField<bool> _isTransitioningButtons;
        private static IReflectedField<bool> _titleInPosition;
        private static IReflectedField<bool> _transitioningCharacterCreationMenu;
        private static IReflectedField<int> _quitTimer;
        private static IReflectedMethod _shouldDrawCursor;

        static TitleOptionsButton()
        {
            Button = new ClickableTextureComponent(
                new Rectangle(
                    ModData.Config.TitleOptionsButtonX,
                    Game1.game1.Window.ClientBounds.Height - ModData.Config.TitleOptionsButtonY,
                    81, 75),
                ModData.Texture,
                new Rectangle(0, 247, 27, 25),
                3f)
            {
                myID = 9598,
                rightNeighborID = 81115
            };
            Helper.Events.Display.MenuChanged += MenuChanged;
            Update();
        }

        private static bool ShouldDrawButton()
        {
            return Game1.gameMode == 0 &&
                   TitleMenu.subMenu == null &&
                   !_isTransitioningButtons.GetValue() &&
                   _titleInPosition.GetValue() &&
                   !_transitioningCharacterCreationMenu.GetValue();
        }

        private static void ReceiveLeftClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button != SButton.MouseLeft && e.Button != SButton.ControllerA) return;
            if (!ShouldDrawButton() ||
                !Button.containsPoint((int) e.Cursor.ScreenPixels.X, (int) e.Cursor.ScreenPixels.Y)) return;
            TitleMenu.subMenu = new TitleOptionsPage();
            Game1.playSound("newArtifact");
        }

        private static void Update()
        {
            if (!(Game1.activeClickableMenu is TitleMenu menu)) return;
            _isTransitioningButtons = Helper.GetReflectedField<bool>(menu, "isTransitioningButtons");
            _titleInPosition = Helper.GetReflectedField<bool>(menu, "titleInPosition");
            _transitioningCharacterCreationMenu =
                Helper.GetReflectedField<bool>(menu, "transitioningCharacterCreationMenu");
            _quitTimer = Helper.GetReflectedField<int>(menu, "quitTimer");
            _shouldDrawCursor = Helper.GetReflectedMethod(menu, "ShouldDrawCursor");
            Helper.Events.Input.CursorMoved += PerformHoverAction;
            Helper.Events.Display.RenderedActiveMenu += Draw;
            Helper.Events.Display.WindowResized += WindowResized;
            Helper.Events.Input.ButtonPressed += ReceiveLeftClick;
            
        }

        private static void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is TitleMenu) Clear();
            if (e.NewMenu is TitleMenu) Update();
        }

        private static void WindowResized(object sender, WindowResizedEventArgs e)
        {
            UpdatePosition();
        }

        public static void UpdatePosition()
        {
            Button.bounds.X = ModData.Config.TitleOptionsButtonX;
            Button.bounds.Y = Game1.game1.Window.ClientBounds.Height - ModData.Config.TitleOptionsButtonY;
        }

        private static void PerformHoverAction(object sender, CursorMovedEventArgs e)
        {
            if (!ShouldDrawButton()) return;
            var x = (int) e.NewPosition.ScreenPixels.X;
            var y = (int) e.NewPosition.ScreenPixels.Y;
            Button.tryHover(x, y, 0.25f);
            if (Button.containsPoint(x, y))
            {
                if (Button.sourceRect.X == 0)
                    Game1.playSound("Cowboy_Footstep");
                Button.sourceRect.X = 27;
            }
            else
                Button.sourceRect.X = 0;
        }

        private static void Draw(object sender, RenderedActiveMenuEventArgs renderedActiveMenuEventArgs)
        {
            if (!ShouldDrawButton()) return;
            var quitTimer = _quitTimer.GetValue();
            if (quitTimer < 0) return;
            Button.draw(Game1.spriteBatch, Color.White, 0.1f);
            if (quitTimer > 0)
                Game1.spriteBatch.Draw(Game1.staminaRect, Button.bounds,
                    Color.Multiply(Color.Black, (float) (1.0 - quitTimer / 500.0)));
            if (!_shouldDrawCursor.Invoke<bool>())
                return;
            Game1.activeClickableMenu?.drawMouse(Game1.spriteBatch);
        }
        private static void Clear()
        {
            Helper.Events.Input.CursorMoved -= PerformHoverAction;
            Helper.Events.Display.RenderedActiveMenu -= Draw;
            Helper.Events.Display.WindowResized -= WindowResized;
            Helper.Events.Input.ButtonPressed -= ReceiveLeftClick;
        }
    }
}