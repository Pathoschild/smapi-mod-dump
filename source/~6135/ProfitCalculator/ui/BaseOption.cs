/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace ProfitCalculator.ui
{
    /// <summary>
    ///  Base class for all options in the options menu. This might be usefull for other mods. Might clean this up later and make it a seperate mod or framework.
    /// </summary>
    public abstract class BaseOption
    {
        /// <summary> The sound to play when the option is clicked, or <c>null</c> to play no sound. </summary>
        public virtual string ClickedSound => null;

        /// <summary> Whether the option was clicked by the cursor. </summary>
        protected bool Clicked;

        /// <summary> The sound to play when the cursor hovers on the option, or <c>null</c> to play no sound. </summary>
        public virtual string HoveredSound => null;

        /// <summary> Whether the option is currently hovered by the cursor. </summary>
        public bool Hover { get; private set; }

        /// <summary>
        /// If the option was clicked by a left click
        /// </summary>
        public bool ClickGestured { get; private set; }

        private ClickableComponent clickableComponent;

        /// <summary>
        /// Sets the clickable component for this option
        /// </summary>
        /// <param name="position"> The position of the clickable component in Vector2 format</param>
        /// <param name="Size"> The size of the clickable component in Vector2 format</param>
        public void SetClickableComponent(Vector2 position, Vector2 Size)
        {
            ClickableComponent = new(
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    (int)Size.X,
                    (int)Size.Y
                ),
                Name(),
                Name()
            );
        }

        /// <summary>
        /// Sets the clickable component for this option. Sets Position Vector2 to the position of the clickable component
        /// </summary>
        public ClickableComponent ClickableComponent
        {
            get => clickableComponent;
            set
            {
                clickableComponent = value;
                Position = new Vector2(clickableComponent.bounds.X, clickableComponent.bounds.Y);
            }
        }

        /// <summary>The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</summary>
        public Func<string> Tooltip { get; }

        /// <summary>The Name to show in the form.</summary>
        public Func<string> Name { get; set; }

        /// <summary>The Label to show in the form.</summary>
        public Func<string> Label { get; set; }

        /// <summary> The position of the clickable component in Vector2 format for easy access</summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Creates a new BaseOption
        /// </summary>
        /// <param name="x"> The x position of the clickable component</param>
        /// <param name="y"> The y position of the clickable component</param>
        /// <param name="w"> The width of the clickable component</param>
        /// <param name="h"> The height of the clickable component</param>
        /// <param name="name"> The name of the clickable component</param>
        /// <param name="label"> The label of the clickable component</param>
        /// <param name="tooltip"> The tooltip of the clickable component</param>
        protected BaseOption(int x, int y, int w, int h, Func<string> name, Func<string> label, Func<string> tooltip)
        {
            ClickableComponent =
                new ClickableComponent(
                    new Rectangle(
                        x,
                        y,
                        w,
                        h
                    ),
                    name(),
                    label()
                );
            Tooltip = tooltip;
            this.Name = name;
            this.Label = label;
        }

        /// <summary>
        /// Draws the option to the screen. Abstract so it can be overriden by subclasses
        /// </summary>
        /// <param name="b"> The SpriteBatch to draw to</param>
        public abstract void Draw(SpriteBatch b);

        /// <summary>
        /// Behaviour before executing the left click action itself. Abstract so it can be overriden by subclasses
        /// </summary>
        /// <param name="x"> The x position of the mouse</param>
        /// <param name="y"> The y position of the mouse</param>
        public abstract void BeforeReceiveLeftClick(int x, int y);

        /// <summary>
        /// Behaviour for the left click action. Implemented here so it can be used by subclasses.
        /// </summary>
        /// <param name="x"> The x position of the mouse</param>
        /// <param name="y"> The y position of the mouse</param>
        /// <param name="stopSpread"> The action to stop the spread of the left click to Children or sibling components</param>
        public virtual void ReceiveLeftClick(int x, int y, Action stopSpread)
        {
            this.BeforeReceiveLeftClick(x, y);
            //check if x and y are within the bounds of the checkbox
            if (ClickableComponent.containsPoint(x, y))
                this.ExecuteClick();
        }

        /// <summary>
        /// Execution og left click action. Implemented here so it can be used by subclasses.
        /// </summary>
        public virtual void ExecuteClick()
        {
            Clicked = true;
            if (this.ClickedSound != null)
                Game1.playSound(this.ClickedSound);
        }

        /// <summary>
        /// Update event for the option. Abstract so it can be overriden by subclasses
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Called when the option the there's an hover action. Implemented here so it can be used by subclasses.
        /// </summary>
        /// <param name="x"> The x position of the mouse</param>
        /// <param name="y"> The y position of the mouse</param>
        public virtual void PerformHoverAction(int x, int y)
        {
        }
    }
}