/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Framework.InputHandling;
using SkillPrestige.Framework.Menus.Elements.Buttons;
using SkillPrestige.Logging;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Framework.Menus.Dialogs
{
    /// <summary>Represents a message dialog box to display information to the user.</summary>
    internal class MessageDialog : IClickableMenu, IInputHandler
    {
        /*********
        ** Fields
        *********/
        private bool ButtonInstantiated;
        private int DebounceTimer = 10;
        private TextureButton OkayButton;
        private readonly string Message;


        /*********
        ** Public methods
        *********/
        public override void draw(SpriteBatch spriteBatch)
        {
            if (this.DebounceTimer > 0)
                this.DebounceTimer--;

            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            this.upperRightCloseButton?.draw(spriteBatch);
            this.DrawDecorations(spriteBatch);
            this.DrawMessage(spriteBatch);
            if (!this.ButtonInstantiated)
                this.InstantiateButtons();
            this.OkayButton.Draw(spriteBatch);
            this.OkayButton.DrawHoverText(spriteBatch);
            Mouse.DrawCursor(spriteBatch);
        }

        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="e">The event data.</param>
        public void OnCursorMoved(CursorMovedEventArgs e)
        {
            if (this.DebounceTimer > 0)
                return;

            this.OkayButton.OnCursorMoved(e);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event data.</param>
        /// <param name="isClick">Whether the button press is a click.</param>
        public void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
        {
            if (this.DebounceTimer > 0)
                return;

            this.OkayButton.OnButtonPressed(e, isClick);
        }

        /*********
        ** Protected methods
        *********/
        protected MessageDialog(Rectangle bounds, string message)
            : base(bounds.X, bounds.Y, bounds.Width, bounds.Height, true)
        {
            this.Message = message;
        }

        protected virtual void DrawMessage(SpriteBatch spriteBatch)
        {
            int textPadding = 2 * Game1.pixelZoom;
            int xLocationOfMessage = this.xPositionOnScreen + spaceToClearSideBorder * 2 + textPadding;
            int yLocationOfMessage = this.yPositionOnScreen + spaceToClearTopBorder + textPadding;
            this.DrawMessage(spriteBatch, Game1.dialogueFont, new Vector2(xLocationOfMessage, yLocationOfMessage), this.width - spaceToClearSideBorder * 2);
        }

        protected virtual void DrawMessage(SpriteBatch spriteBatch, SpriteFont font, Vector2 textPosition, int textWidth)
        {
            spriteBatch.DrawString(font, this.Message.WrapText(font, textWidth), textPosition, Game1.textColor);
        }

        protected virtual void DrawDecorations(SpriteBatch spriteBatch) { }

        private void Okay()
        {
            Logger.LogVerbose("Message Dialog - Okay button called.");
            this.exitThisMenu(false);
        }

        private void InstantiateButtons()
        {
            if (this.ButtonInstantiated)
                return;
            this.ButtonInstantiated = true;
            Logger.LogVerbose("Message Dialog - Instantiating Okay button...");
            const int buttonSize = Game1.tileSize;
            var okayButtonBounds = new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + this.height - buttonSize, buttonSize, buttonSize);
            this.OkayButton = new TextureButton(okayButtonBounds, Game1.mouseCursors, new Rectangle(128, 256, 64, 64), this.Okay);
            Logger.LogVerbose("Message Dialog - Okay button instantiated.");

        }
    }
}
