/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mpcomplete/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Tubes
{
    internal delegate void DropDownOptionSelected(int selected);

    internal class DropdownComponent : OptionComponent
    {

        internal event DropDownOptionSelected DropDownOptionSelected;
        //
        // Static Fields
        //
        //public const int pixelsHigh = 11;

        public static Rectangle dropDownButtonSource = new Rectangle(437, 450, 10, 11);

        public static Rectangle dropDownBGSource = new Rectangle(433, 451, 3, 3);

        // DropDownBackground is the extension, dropDown is the unclicked state
        protected ClickableTextureComponent dropDownBackground = new ClickableTextureComponent(new Rectangle(0, 0, Game1.pixelZoom * OptionsDropDown.dropDownBGSource.Width, 0), Game1.mouseCursors, OptionsDropDown.dropDownBGSource, Game1.pixelZoom);

        protected ClickableTextureComponent dropDownButton = new ClickableTextureComponent(new Rectangle(0, 0, Game1.pixelZoom * OptionsDropDown.dropDownButtonSource.Width, Game1.pixelZoom * OptionsDropDown.dropDownButtonSource.Height), Game1.mouseCursors, OptionsDropDown.dropDownButtonSource, Game1.pixelZoom);

        protected ClickableTextureComponent dropDown = new ClickableTextureComponent(new Rectangle(0, 0, Game1.pixelZoom * OptionsDropDown.dropDownBGSource.Width, 11 * Game1.pixelZoom), Game1.mouseCursors, OptionsDropDown.dropDownBGSource, Game1.pixelZoom);

        //IClickableMenu.drawTextureBox(b, Game1.mouseCursors, OptionsDropDown.dropDownBGSource, this.dropDownBounds.X, this.dropDownBounds.Y, this.dropDownBounds.Width, this.dropDownBounds.Height, Color.White* scale, (float) Game1.pixelZoom, false);
        //b.Draw(Game1.mouseCursors, new Vector2((float) (this.bounds.X + this.bounds.Width - Game1.pixelZoom * 12), (float) (this.bounds.Y)), new Rectangle?(OptionsDropDown.dropDownButtonSource), Color.Wheat * scale, 0f, Vector2.Zero, (float) Game1.pixelZoom, SpriteEffects.None, 0.981f);


        public override int Width => dropDown.bounds.Width + dropDownButton.bounds.Width + (int)Game1.dialogueFont.MeasureString(this.label).X;
        public override int Height => dropDown.bounds.Height;
        public override int X => dropDown.bounds.X;
        public override int Y => dropDown.bounds.Y;

        //
        // Fields
        //

        internal override bool visible {
            set {
                base.visible = value;
                dropDown.visible = value;
                dropDownBackground.visible = value;
                dropDownButton.visible = value;
            }
        }

        public virtual int SelectionIndex {
            get { return (dropDownOptions.Count != 0) ? dropDownOptions.IndexOf(this.dropDownDisplayOptions[0]) : -1; }
            set {
                if (SelectionIndex == value)
                    return;

                var index = dropDownDisplayOptions.IndexOf(dropDownOptions[value]);

                dropDownDisplayOptions.Insert(0, dropDownOptions[value]);
                dropDownDisplayOptions.RemoveAt(index + 1);
                this.DropDownOptionSelected?.Invoke(value);
            }
        }

        private int hoveredChoice = 0;

        public override bool enabled {
            get {
                if (!_enabled)
                    return _enabled;
                else
                    return dropDownOptions.Count != 0;
            }
            protected set {
                _enabled = value;
            }
        }

        // Original List
        private List<string> _dropDownOptions = new List<string>();

        protected virtual List<string> dropDownOptions {
            get { return _dropDownOptions; }
        }

        // List where order can be changed
        protected List<string> _dropDownDisplayOptions = new List<string>();

        protected virtual List<string> dropDownDisplayOptions {
            get {
                if (_dropDownOptions.Count == 0) {
                    _dropDownDisplayOptions.Clear();
                } else {
                    var options = dropDownOptions;
                    var toRemove = _dropDownDisplayOptions.Except(options).ToList();
                    var toAdd = options.Except(_dropDownDisplayOptions).ToList();

                    _dropDownDisplayOptions.RemoveAll(x => toRemove.Contains(x));
                    _dropDownDisplayOptions.AddRange(toAdd);
                }

                dropDownBackground.bounds.Height = this.dropDown.bounds.Height * dropDownOptions.Count;
                return _dropDownDisplayOptions;
            }
        }

        protected virtual void SelectDisplayedOption(int DisplayedSelection)
        {
            if (DisplayedSelection == 0)
                return;
            var selected = dropDownDisplayOptions[DisplayedSelection];
            dropDownDisplayOptions.Insert(0, selected);
            dropDownDisplayOptions.RemoveAt(DisplayedSelection + 1);
            this.DropDownOptionSelected?.Invoke(SelectionIndex);
        }

        //
        // Constructors
        //

        public DropdownComponent(string label, int width, int x, int y, bool enabled = true) : base(label, x, y, enabled)
        {
            updateLocation(x, y, width);

            //this.dropDown.bounds = new Rectangle(x, y, width + Game1.pixelZoom * 12, 11 * Game1.pixelZoom);
            //this.dropDownBounds = new Rectangle(this.bounds.X, this.bounds.Y, width, this.bounds.Height * this.dropDownOptions.Count);
        }

        public DropdownComponent(List<string> choices, string label, int width, int x, int y, bool enabled = true) : this(label, width, x, y, enabled)
        {
            this._dropDownOptions.AddRange(choices);
        }


        // These contructors requires Draw(b,x,y) to move the object from origin
        public DropdownComponent(List<string> choices, string label, int width, bool enabled = true) : this(choices, label, width, 0, 0, enabled) { }

        protected DropdownComponent(string label, int width, bool enabled = true) : this(label, width, 0, 0, enabled) { }

        //
        // Methods
        //

        public void updateLocation(int x, int y, int width)
        {
            this.dropDown.bounds.X = x;
            this.dropDown.bounds.Y = y;
            this.dropDown.bounds.Width = width;

            this.dropDownButton.bounds.X = this.dropDown.bounds.Right;
            this.dropDownButton.bounds.Y = this.dropDown.bounds.Y;

            this.dropDownBackground.bounds.X = x;
            this.dropDownBackground.bounds.Y = y;
            this.dropDownBackground.bounds.Width = width;
        }

        public override void draw(SpriteBatch b, int x, int y)
        {
            updateLocation(x, y, this.dropDown.bounds.Width);
            this.draw(b);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            dropDownBackground.bounds.Height = this.dropDown.bounds.Height * dropDownDisplayOptions.Count;

            float buttonAlpha = (this.enabled) ? 1f : 0.33f;

            var labelSize = Game1.dialogueFont.MeasureString(this.label);

            // Draw Label
            Utility.drawTextWithShadow(b, this.label, Game1.dialogueFont, new Vector2((float)(this.dropDownButton.bounds.Right + Game1.pixelZoom * 2), (float)(this.dropDown.bounds.Y + ((this.dropDown.bounds.Height - labelSize.Y) / 2))), this.enabled ? Game1.textColor : (Game1.textColor * 0.33f), 1f, 0.1f, -1, -1, 1f, 3);

            // If menu is being clicked, and no other components are in use (to prevent click overlap of the dropdown)
            if (this.IsActiveComponent()) {
                // Draw Background of dropdown
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, OptionsDropDown.dropDownBGSource, this.dropDownBackground.bounds.X, this.dropDownBackground.bounds.Y, this.dropDownBackground.bounds.Width, this.dropDownBackground.bounds.Height, Color.White * buttonAlpha, (float)Game1.pixelZoom, false);
                //dropDownBackground.draw(b);

                for (int i = 0; i < this.dropDownDisplayOptions.Count; i++) {
                    if (i == this.hoveredChoice && dropDownBackground.containsPoint(Game1.getMouseX(), Game1.getMouseY())) {
                        b.Draw(Game1.staminaRect, new Rectangle(this.dropDown.bounds.X, this.dropDown.bounds.Y + i * this.dropDown.bounds.Height, this.dropDown.bounds.Width, this.dropDown.bounds.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Wheat, 0f, Vector2.Zero, SpriteEffects.None, 0.975f);
                    }
                    b.DrawString(Game1.smallFont, this.dropDownDisplayOptions[i], new Vector2((float)(this.dropDownBackground.bounds.X + Game1.pixelZoom), (float)(this.dropDownBackground.bounds.Y + Game1.pixelZoom * 2 + this.dropDown.bounds.Height * i)), Game1.textColor * buttonAlpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.98f);
                }
                dropDownButton.draw(b);
                //b.Draw(Game1.mouseCursors, new Vector2((float) (this.bounds.X + this.bounds.Width - Game1.pixelZoom * 12), (float) (this.bounds.Y)), new Rectangle?(OptionsDropDown.dropDownButtonSource), Color.Wheat * scale, 0f, Vector2.Zero, (float) Game1.pixelZoom, SpriteEffects.None, 0.981f);
            } else {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, OptionsDropDown.dropDownBGSource, this.dropDown.bounds.X, this.dropDown.bounds.Y, this.dropDown.bounds.Width, this.dropDown.bounds.Height, Color.White * buttonAlpha, (float)Game1.pixelZoom, false);
                //dropDown.draw(b);

                dropDownButton.draw(b, Color.White * buttonAlpha, 0.88f);

                b.DrawString(Game1.smallFont, (this.SelectionIndex >= this.dropDownDisplayOptions.Count || this.SelectionIndex < 0) ? string.Empty : this.dropDownDisplayOptions[0], new Vector2((float)(this.dropDown.bounds.X + Game1.pixelZoom), (float)(this.dropDown.bounds.Y + Game1.pixelZoom * 2)), Game1.textColor * buttonAlpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.88f);
                //b.Draw(Game1.mouseCursors, new Vector2((float) (this.bounds.X + this.bounds.Width - Game1.pixelZoom * 12), (float) (this.bounds.Y)), new Rectangle?(OptionsDropDown.dropDownButtonSource), Color.White * scale, 0f, Vector2.Zero, (float) Game1.pixelZoom, SpriteEffects.None, 0.88f);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            //base.receiveLeftClick(x, y, playSound);

            if (this.enabled && (this.dropDown.containsPoint(x, y) || this.dropDownButton.containsPoint(x, y)) && this.IsAvailableForSelection()) {
                this.RegisterAsActiveComponent();
                this.hoveredChoice = 0;
                this.leftClickHeld(x, y);
                Game1.playSound("shwip");
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);

            if (this.enabled && this.IsActiveComponent() && this.dropDownBackground.containsPoint(x, y)) {
                this.dropDownBackground.bounds.Y = Math.Min(this.dropDownBackground.bounds.Y, Game1.viewport.Height - this.dropDownBackground.bounds.Height);
                this.hoveredChoice = (int)Math.Max(Math.Min((float)(y - this.dropDownBackground.bounds.Y) / (float)this.dropDown.bounds.Height, (float)(this.dropDownOptions.Count - 1)), 0f);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);

            if (this.IsActiveComponent()) {
                this.UnregisterAsActiveComponent();

                if (this.dropDownBackground.containsPoint(x, y)) {
                    if (this.enabled && this.dropDownOptions.Count > 0) {
                        this.SelectDisplayedOption(this.hoveredChoice);
                    }

                }
            }
        }
    }
}
