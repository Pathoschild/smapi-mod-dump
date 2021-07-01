/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pepoluan/StackSplitRedux
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System;

namespace StackSplitRedux.UI
    {
    /// <summary>Manages the UI for inputting the stack amount.</summary>
    public class StackSplitMenu
        {
        // Character limit of 4 since max stack size of anything (afaik is 999).
        private const int CHAR_LIMIT = 4;

        /// <summary>Delegate declaration for when text is submitted.</summary>
        /// <param name="input">The submitted text.</param>
        public delegate void TextSubmittedDelegate(string input);

        /// <summary>The amount being currently held by the player.</summary>
        public int HeldStackAmount { get; private set; }

        /// <summary>The dialogue title.</summary>
        public string Title { get; set; } = Mod.I18n.Get("ui.stacksplitmenu.title");

        /// <summary>The input text box.</summary>
        private InputTextBox InputTextBox;

        /// <summary>The OK button.</summary>
        private ClickableTextureComponent OKButton;

        /// <summary>Callback to execute when the text is submitted.</summary>
        private readonly TextSubmittedDelegate OnTextSubmitted;

        #region Cached stuff to speed up Draw()
        private readonly Rectangle MouseRect = Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, Game1.smallestTileSize, Game1.smallestTileSize);
        private readonly Color MouseTransparency = Color.White * Game1.mouseCursorTransparency;
        private readonly float MouseScale = Game1.pixelZoom + (Game1.dialogueButtonScale / 150f);
        #endregion

        private readonly Guid GUID = Guid.NewGuid();

        /// <summary>Constructs an instance.</summary>
        /// <param name="textSubmittedCallback">The callback for when the text is submitted.</param>
        /// <param name="heldStackAmount">The default stack amount to set the text to.</param>
        public StackSplitMenu(TextSubmittedDelegate textSubmittedCallback, int heldStackAmount) {
            Log.TraceIfD($"[{nameof(StackSplitMenu)}] Instantiated with textSubmittedCallback = {textSubmittedCallback}, heldStackAmount = {heldStackAmount}");
            Log.TraceIfD($"[{nameof(StackSplitMenu)}] GUID = {GUID}");

            this.OnTextSubmitted = textSubmittedCallback;
            this.HeldStackAmount = heldStackAmount;

            var inputBox = this.InputTextBox = new InputTextBox(CHAR_LIMIT, heldStackAmount.ToString()) {
                Position = new Vector2(Game1.getMouseX(), Game1.getMouseY() - Game1.tileSize),
                Extent = new Vector2(Game1.tileSize * 2, Game1.tileSize),
                NumbersOnly = true,
                Selected = true,
                };
            inputBox.OnSubmit += (sender) => Submit(sender.Text);
            Game1.keyboardDispatcher.Subscriber = inputBox;

            this.OKButton = new ClickableTextureComponent(
                new Rectangle(
                    (int)inputBox.Position.X + (int)inputBox.Extent.X + Game1.pixelZoom, // pixelzoom used to give gap
                    (int)inputBox.Position.Y,
                    Game1.tileSize,
                    Game1.tileSize
                    ),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1),
                1f,
                false);
            }

        ~StackSplitMenu() {
            Log.TraceIfD($"[{nameof(StackSplitMenu)}] Finalized for textSubmittedCallback = {this.OnTextSubmitted}, heldStackAmount = {this.HeldStackAmount}");
            Log.TraceIfD($"[{nameof(StackSplitMenu)}] GUID = {GUID}");
            }

        /// <summary>Closes the split menu so it stops receiving input.</summary>
        public void Close() {
            // Remove from the subscriber so it stops getting input.
            Game1.keyboardDispatcher.Subscriber = null;
            // Cleanup
            this.InputTextBox = null;
            this.OKButton = null;
            }

        /// <summary>Draws the interface.</summary>
        /// <param name="b">Spritebatch to draw with.</param>
        public void Draw(SpriteBatch b) {
            this.InputTextBox.Draw(b);
            this.OKButton.draw(b);

            if (Game1.options.hardwareCursor) return;
            b.Draw(
                Game1.mouseCursors,
                new Vector2(Game1.getMouseX(), Game1.getMouseY()),
                this.MouseRect,
                this.MouseTransparency,
                0f,
                Vector2.Zero,
                this.MouseScale,
                SpriteEffects.None,
                1f);
            }

        /// <summary>Handles left clicks to check if the OK button was clicked.</summary>
        /// <param name="x">Mouse x position.</param>
        /// <param name="y">Mouse y position.</param>
        public void ReceiveLeftClick(int x, int y) {
            if (this.OKButton.containsPoint(x, y)) {
                Game1.playSound("smallSelect");
                Submit(this.InputTextBox.Text);
                }
            }

        /// <summary>If this point lies in either the input box or OK button.</summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        public bool ContainsPoint(int x, int y) {
            return this.OKButton.containsPoint(x, y) || this.InputTextBox.ContainsPoint(x, y);
            }

        /// <summary>Updates the input textbox.</summary>
        public void Update() {
            this.InputTextBox.Update();
            }

        /// <summary>Callback to the input textbox's submit event. Fires the callback passed to this class.</summary>
        /// <param name="text">The submitted text.</param>
        private void Submit(string text) {
            Debug.Assert(this.OnTextSubmitted != null);
            this.OnTextSubmitted(text);
            }
        }
    }
