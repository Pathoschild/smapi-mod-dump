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
    /// Draws a checkbox option in the options menu setting true or false to a value
    /// </summary>
    public class CheckboxOption : BaseOption
    {
        /// <summary> The mouse texture to draw for the checkbox. </summary>
        public Texture2D Texture { get; set; } = Game1.mouseCursors;

        /// <summary> The rectangle of the texture to draw for the checkbox when it is checked. </summary>
        public Rectangle CheckedTextureRect { get; set; } = OptionsCheckbox.sourceRectChecked;

        /// <summary> The rectangle of the texture to draw for the checkbox when it is unchecked. </summary>
        public Rectangle UncheckedTextureRect { get; set; } = OptionsCheckbox.sourceRectUnchecked;

        /// <summary> Gets the current value of the option. </summary>
        private readonly Func<bool> ValueGetter;

        /// <summary> Sets the current value of the option. </summary>
        private readonly Action<bool> ValueSetter;

        /// <inheritdoc />
        public override string ClickedSound => "drumkit6";

        /// <summary>
        ///  Constructor for the CheckboxOption class.
        /// </summary>
        /// <param name="x"> The x position of the option. </param>
        /// <param name="y"> The y position of the option. </param>
        /// <param name="name"> The name of the option. </param>
        /// <param name="label"> The label of the option. </param>
        /// <param name="valueGetter"> The value getter of the option. Type Func. This should return the current value for the option </param>
        /// <param name="valueSetter"> The value setter of the option. Type Action. This should be the action executed when the checkbox is clicked. I.e. set a variable to false or true. </param>
        public CheckboxOption(
            int x,
            int y,
            Func<string> name,
            Func<string> label,
            Func<bool> valueGetter,
            Action<bool> valueSetter)
            : base(x, y, 0, 0, name, label, label)
        {
            ClickableComponent.bounds.Width = OptionsCheckbox.sourceRectChecked.Width * 4;
            ClickableComponent.bounds.Height = OptionsCheckbox.sourceRectChecked.Height * 4;
            ValueGetter = valueGetter;
            ValueSetter = valueSetter;
        }

        /// <summary>
        /// Draws the checkbox option
        /// </summary>
        public override void Draw(SpriteBatch b)
        {
            b.Draw(
                this.Texture,
                this.Position,
                this.ValueGetter() ? this.CheckedTextureRect : this.UncheckedTextureRect,
                Color.White,
                0,
                Vector2.Zero,
                4,
                SpriteEffects.None,
                0
            );
            Game1.activeClickableMenu?.drawMouse(b);
        }

        /// <summary>
        /// Executes the click action for the checkbox option
        /// </summary>
        public override void ExecuteClick()
        {
            this.ValueSetter(!this.ValueGetter());
            Game1.playSound(this.ClickedSound);
        }

        /// <summary>
        /// Behaviour before executing the left click action itself.
        /// </summary>
        /// <param name="x"> The x position of the mouse</param>
        /// <param name="y"> The y position of the mouse</param>
        public override void BeforeReceiveLeftClick(int x, int y)
        {
        }

        /// <summary>
        /// Update event for the option.
        /// </summary>
        public override void Update()
        {
        }
    }
}