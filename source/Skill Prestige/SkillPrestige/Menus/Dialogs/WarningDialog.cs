using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.InputHandling;
using SkillPrestige.Logging;
using SkillPrestige.Menus.Elements.Buttons;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus.Dialogs
{
    internal class WarningDialog : IClickableMenu
    {
        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        public delegate void OkayCallback();

        public delegate void CancelCallback();

        private OkayCallback OnOkay { get; }
        private CancelCallback OnCancel { get; }
        private static bool _buttonClickRegistered;
        private bool _buttonsInstantiated;
        private int _debouceWaitTime;
        private TextureButton _okayButton;
        private TextureButton _cancelButton;
        private readonly string _message;


        public WarningDialog(Rectangle bounds, string message, OkayCallback okayCallback, CancelCallback cancelCallback)
            : base(bounds.X, bounds.Y, bounds.Width, bounds.Height, true)
        {
            OnOkay = okayCallback;
            OnCancel = cancelCallback;
            exitFunction = Cancel;
            _message = message;
        }

        private void Cancel()
        {
            Logger.LogInformation("Warning Dialog - Cancel/Close called.");
            DeregisterMouseEvents();
            OnCancel.Invoke();
        }

        private void RegisterMouseEvents()
        {
            if (_buttonClickRegistered) return;
            _buttonClickRegistered = true;
            Logger.LogVerbose("Warning Dialog - Registering Mouse Events...");
            Mouse.MouseMoved += _okayButton.CheckForMouseHover;
            Mouse.MouseMoved += _cancelButton.CheckForMouseHover;
            Mouse.MouseClicked += _okayButton.CheckForMouseClick;
            Mouse.MouseClicked += _cancelButton.CheckForMouseClick;
            Logger.LogVerbose("Warning Dialog - Mouse Events Registered.");
        }

        private void DeregisterMouseEvents()
        {
            if (!_buttonClickRegistered) return;
            Logger.LogVerbose("Warning Dialog - Deregistering Mouse Events.");
            Mouse.MouseMoved -= _okayButton.CheckForMouseHover;
            Mouse.MouseMoved -= _cancelButton.CheckForMouseHover;
            Mouse.MouseClicked -= _okayButton.CheckForMouseClick;
            Mouse.MouseClicked -= _cancelButton.CheckForMouseClick;
            Logger.LogVerbose("Warning Dialog - Mouse Events Deregistered.");
            _buttonClickRegistered = false;
        }

        private void InstantiateButtons()
        {
            if (_buttonsInstantiated) return;
            _buttonsInstantiated = true;
            Logger.LogVerbose("Warning Dialog - Instantiating Okay/Cancel buttons...");
            var buttonSize = Game1.tileSize;
            var buttonPadding = Game1.tileSize * 4;
            var okayButtonBounds = new Rectangle(xPositionOnScreen + width - buttonSize - spaceToClearSideBorder * 3, yPositionOnScreen + height - (buttonSize * 1.5).Floor(), buttonSize, buttonSize);
            _okayButton = new TextureButton(okayButtonBounds, Game1.mouseCursors, new Rectangle(128, 256, 64, 64), Okay, "Prestige Skill");
            Logger.LogVerbose("Warning Dialog - Okay button instantiated.");
            var cancelButtonBounds = okayButtonBounds;
            cancelButtonBounds.X -= buttonSize + buttonPadding;
            _cancelButton = new TextureButton(cancelButtonBounds, Game1.mouseCursors, new Rectangle(192, 256, 64, 64), () => exitThisMenu(false), "Cancel");
            Logger.LogVerbose("Warning Dialog - Cancel button instantiated.");

        }

        private void Okay()
        {
            Logger.LogVerbose("Warning Dialog - Okay button called.");
            OnOkay.Invoke();
            exitThisMenu(false);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (_debouceWaitTime < 10)
            {
                _debouceWaitTime++;
            }
            else
            {
                RegisterMouseEvents();
            }
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            var textPadding = 2 * Game1.pixelZoom;
            Game1.spriteBatch.DrawString(Game1.dialogueFont,
                _message.WrapText(Game1.dialogueFont, width - spaceToClearSideBorder * 2),
                new Vector2(xPositionOnScreen + spaceToClearSideBorder * 2 + textPadding,
                    yPositionOnScreen + spaceToClearTopBorder + textPadding), Game1.textColor);
            upperRightCloseButton?.draw(spriteBatch);
            if (!_buttonsInstantiated) InstantiateButtons();
            _okayButton.Draw(spriteBatch);
            _cancelButton.Draw(spriteBatch);
            _okayButton.DrawHoverText(spriteBatch);
            _cancelButton.DrawHoverText(spriteBatch);
            Mouse.DrawCursor(spriteBatch);
        }
    }
}
