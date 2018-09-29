using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;

namespace StackSplitX
{
    /// <summary>Manages the UI for inputting the stack amount.</summary>
    public class StackSplitMenu
    {
        /// <summary>Delegate declaration for when text is submitted.</summary>
        /// <param name="input">The submitted text.</param>
        public delegate void TextSubmittedDelegate(string input);

        /// <summary>The amount being currently held by the player.</summary>
        public int HeldStackAmount { get; private set; }

        /// <summary>The dialogue title.</summary>
        public string Title { get; set; } = "Select Amount";

        /// <summary>The input text box.</summary>
        private InputTextBox InputTextBox;

        /// <summary>Callback to execute when the text is submitted.</summary>
        private TextSubmittedDelegate OnTextSubmitted;

        /// <summary>The OK button.</summary>
        private ClickableTextureComponent OKButton;

        /// <summary>Constructs an intance.</summary>
        /// <param name="textSubmittedCallback">The callback for when the text is submitted.</param>
        /// <param name="heldStackAmount">The default stack amount to set the text to.</param>
        public StackSplitMenu(TextSubmittedDelegate textSubmittedCallback, int heldStackAmount)
        {
            this.OnTextSubmitted = textSubmittedCallback;
            this.HeldStackAmount = heldStackAmount;

            // Character limit of 4 since max stack size of anything (afaik is 999).
            this.InputTextBox = new InputTextBox(4, heldStackAmount.ToString())
            {
                Position = new Vector2(Game1.getMouseX(), Game1.getMouseY() - Game1.tileSize),
                Extent = new Vector2(Game1.tileSize * 2, Game1.tileSize),
                NumbersOnly = true,
                Selected = true,
            };
            this.InputTextBox.OnSubmit += (sender) => Submit(sender.Text);
            Game1.keyboardDispatcher.Subscriber = this.InputTextBox;

            // TODO: clean up
            this.OKButton = new ClickableTextureComponent(
                new Rectangle((int)this.InputTextBox.Position.X + (int)this.InputTextBox.Extent.X + Game1.pixelZoom,
                              (int)this.InputTextBox.Position.Y,
                              Game1.tileSize, 
                              Game1.tileSize), 
                Game1.mouseCursors, 
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 
                1f, 
                false);
        }

        /// <summary>Closes the split menu so it stops receiving input.</summary>
        public void Close()
        {
            // Remove from the subscriber so it stops getting input.
            this.InputTextBox = null;
            Game1.keyboardDispatcher.Subscriber = null;
        }

        /// <summary>Draws the interface.</summary>
        /// <param name="b">Spritebatch to draw with.</param>
        public void draw(SpriteBatch b)
        {
            this.InputTextBox.Draw(b);
            this.OKButton.draw(b);
            
            // TODO: find a nicer way to do this or encapsulate it
            if (!Game1.options.hardwareCursor)
            {
                b.Draw(Game1.mouseCursors, new Vector2(
                    Game1.getMouseX(), 
                    Game1.getMouseY()), 
                    Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), 
                    Color.White * Game1.mouseCursorTransparency, 
                    0f, 
                    Vector2.Zero, 
                    Game1.pixelZoom + Game1.dialogueButtonScale / 150f, 
                    SpriteEffects.None, 
                    1f);
            }
        }

        /// <summary>Handles left clicks to check if the OK button was clicked.</summary>
        /// <param name="x">Mouse x position.</param>
        /// <param name="y">Mouse y position.</param>
        public void ReceiveLeftClick(int x, int y)
        {
            if (this.OKButton.containsPoint(x, y))
            {
                Game1.playSound("smallSelect");
                Submit(this.InputTextBox.Text);
            }
        }

        /// <summary>If this point lies in either the input box or OK button.</summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        public bool ContainsPoint(int x, int y)
        {
            return (this.OKButton.containsPoint(x, y) || this.InputTextBox.ContainsPoint(x, y));
        }

        /// <summary>Updates the input textbox.</summary>
        public void Update()
        {
            this.InputTextBox.Update();
        }

        /// <summary>Callback to the input textbox's submit event. Fires the callback passed to this class.</summary>
        /// <param name="text">The submitted text.</param>
        private void Submit(string text)
        {
            Debug.Assert(this.OnTextSubmitted != null);
            this.OnTextSubmitted(text);
        }
    }
}
