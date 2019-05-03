using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewValley;

namespace SkillPrestige.InputHandling
{
    /// <summary>
    /// Handles mouse interactions with Stardew Valley.
    /// </summary>
    internal static class Mouse
    {
        private static MouseState _lastMouseState;
        private static MouseState _currentMouseState;
        private static bool _mouseInitialized;
        private static bool _waitingForMouseRelease;
        private static Point MouseLastClickLocation { get; set; }

        public delegate void MouseClickEventHandler(MouseClickEventArguments e);
        public delegate void MouseMoveEventHandler(MouseMoveEventArguments e);

        public static event MouseClickEventHandler MouseClicked;
        public static event MouseMoveEventHandler MouseMoved;

        /// <summary>
        /// Draws the mouse cursor, which should be called last in any draw command so as to ensure the mouse is on top of the content.
        /// </summary>
        /// <param name="spriteBatch">The spriteBatch to draw to.</param>
        internal static void DrawCursor(SpriteBatch spriteBatch)
        {
            if (Game1.options.hardwareCursor) return;
            var mousePosition = new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY());
            var cursorTextureLocation = Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors,Game1.options.gamepadControls ? 44 : 0, 16, 16);
            spriteBatch.Draw(Game1.mouseCursors, mousePosition, cursorTextureLocation, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        /// <summary>
        /// Updates the mouse state, as the Stardew Valley Modding API mishandles zoom and other portions of the mouse state tracking.
        /// </summary>
        /// <param name="arguments"></param>
        internal static void HandleState(EventArgsMouseStateChanged arguments)
        {
            UpdateState(arguments);
            CheckForClick();
            if (_lastMouseState.X != _currentMouseState.X || _lastMouseState.Y != _currentMouseState.Y)
            {
                MouseMoved?.Invoke(new MouseMoveEventArguments(new Point(_lastMouseState.X, _lastMouseState.Y),
                        new Point(_currentMouseState.X, _currentMouseState.Y)));
            }
        }

        private static void CheckForClick()
        {
            if (_lastMouseState.LeftButton != _currentMouseState.LeftButton)
            {
                if (_currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    _waitingForMouseRelease = true;
                    MouseLastClickLocation = new Point(_currentMouseState.X, _currentMouseState.Y);
                    return;
                }
            }
            if (!_waitingForMouseRelease || _currentMouseState.LeftButton != ButtonState.Released)
            {
                return;
            }
            _waitingForMouseRelease = false;
            var releaseMouseLocation = new Point(_currentMouseState.X, _currentMouseState.Y);
            MouseClicked?.Invoke(new MouseClickEventArguments(MouseLastClickLocation, releaseMouseLocation));
        }


        private static void UpdateState(EventArgsMouseStateChanged args)
        {
            var newMouseState = new MouseState(Game1.getOldMouseX(), Game1.getOldMouseY(), args.NewState.ScrollWheelValue, args.NewState.LeftButton, args.NewState.MiddleButton, args.NewState.RightButton, args.NewState.XButton1, args.NewState.XButton2);
            if (!_mouseInitialized)
            {
                _mouseInitialized = true;
                _lastMouseState = newMouseState;
            }
            else
            {
                _lastMouseState = _currentMouseState;
            }
            _currentMouseState = newMouseState;
        }
    }
}
