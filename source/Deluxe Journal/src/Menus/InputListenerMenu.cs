/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace DeluxeJournal.Menus
{
    public class InputListenerMenu : IClickableMenu
    {
        /// <summary>Buttons pressed callback.</summary>
        /// <param name="pressed">Keys pressed by the player.</param>
        public delegate void ButtonsPressed(Keybind pressed);

        /// <summary>Predicate function to test if the keys pressed are allowed to be bound.</summary>
        /// <param name="pressed">Keys pressed by the player.</param>
        /// <returns>Whether the <see cref="Keybind"/> should be bound.</returns>
        public delegate bool ButtonsPredicate(Keybind pressed);

        private readonly IModEvents _events;
        private readonly string _message;
        private readonly Point _messageSize;

        /// <summary>Callback fired when the button input is received and passes the <see cref="Predicate"/> condition.</summary>
        public ButtonsPressed? Callback { get; set; }

        /// <summary>Predicate function to test if the button input should be bound. All button input accepted if <c>null</c>.</summary>
        public ButtonsPredicate? Predicate { get; set; }

        public InputListenerMenu(IModEvents events, ButtonsPressed callback)
            : this(events, callback, ExcludeGameOptionKeysInUse)
        {
        }

        public InputListenerMenu(IModEvents events, ButtonsPressed callback, ButtonsPredicate? predicate)
            : base()
        {
            _events = events;
            _message = Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsElement.cs.11225");
            _messageSize = Game1.dialogueFont.MeasureString(_message).ToPoint();
            Callback = callback;
            Predicate = predicate;

            events.Input.ButtonsChanged += OnButtonsChanged;

            Game1.playSound("breathin");
        }

        /// <summary><see cref="ButtonsPredicate"/> to test for keys bound in <see cref="Game1.options"/>.</summary>
        /// <param name="keybind">Keys pressed by the player.</param>
        /// <returns>Whether the <see cref="Keybind"/> contains keys bound in <see cref="Game1.options"/>.</returns>
        public static bool ExcludeGameOptionKeysInUse(Keybind keybind)
        {
            foreach (SButton button in keybind.Buttons)
            {
                if (button.TryGetKeyboard(out Keys key) && Game1.options.isKeyInUse(key))
                {
                    return false;
                }
                else if (button.TryGetController(out Buttons gamePadButton))
                {
                    key = Utility.mapGamePadButtonToKey(gamePadButton);

                    if (key != Keys.None && Game1.options.isKeyInUse(key))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (IsExitKey(key))
            {
                exitThisMenu();
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            b.DrawString(Game1.dialogueFont, _message, Utility.getTopLeftPositionForCenteringOnScreen(_messageSize.X, _messageSize.Y), Color.White);
        }

        protected override void cleanupBeforeExit()
        {
            _events.Input.ButtonsChanged -= OnButtonsChanged;
        }

        private static bool IsExitKey(Keys key)
        {
            return key == Keys.Escape || (Game1.options.SnappyMenus && Game1.options.doesInputListContain(Game1.options.menuButton, key));
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (e.Pressed.Any())
            {
                Keybind keybind = new(e.Pressed
                    .Where(button => button != SButton.MouseLeft && button != SButton.MouseRight)
                    .ToArray());

                foreach (SButton button in keybind.Buttons)
                {
                    if ((button.TryGetKeyboard(out Keys key) && IsExitKey(key))
                        || (button.TryGetController(out Buttons gamePadButton) && IsExitKey(Utility.mapGamePadButtonToKey(gamePadButton))))
                    {
                        return;
                    }
                }

                if (keybind.IsBound && (Predicate == null || Predicate(keybind)))
                {
                    closeSound = "coin";
                    exitThisMenu();
                    Callback?.Invoke(keybind);
                }
                else
                {
                    Game1.playSound("bob");
                }
            }
        }
    }
}
