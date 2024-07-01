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
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Task;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>A drop-down menu component for a <see cref="TaskParameter"/>.</summary>
    public class TaskParameterDropDown : DropDownComponent, ITaskParameterComponent
    {
        private readonly Texture2D? _backgroundTexture;
        private readonly IList<KeyValuePair<Texture2D, Rectangle>> _options;

        /// <summary>Get the selected option as an item quality.</summary>
        public int Quality => SelectedOption == 3 ? 4 : SelectedOption;

        public ClickableComponent ClickableComponent => this;

        public TaskParameter Parameter { get; set; }

        public string Label { get; set; } = string.Empty;

        public TaskParameterDropDown(TaskParameter parameter, IList<KeyValuePair<Texture2D, Rectangle>> options, Rectangle bounds)
            : base(Enumerable.Repeat(string.Empty, options.Count), bounds, parameter.Attribute.Name, true)
        {
            if (parameter.Value is not int value)
            {
                throw new ArgumentException($"{nameof(TaskParameterDropDown)} must have a parameter of type int.");
            }
            else
            {
                SelectedOption = value;
            }

            _backgroundTexture = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
            _options = options.ToList();
            Parameter = parameter;
        }

        public IEnumerable<ClickableComponent> GetClickableComponents()
        {
            yield return ClickableComponent;
        }

        public void TryHover(int x, int y)
        {
        }

        public override void LeftClickReleased(int x, int y)
        {
            base.LeftClickReleased(x, y);

            if (Active)
            {
                if (Parameter.Attribute.Tag == TaskParameterTag.Quality)
                {
                    Parameter.TrySetValue(Quality);
                }
                else
                {
                    Parameter.TrySetValue(SelectedOption);
                }
            }
        }

        protected override void DrawBackground(SpriteBatch b, Rectangle bgBounds, bool dropDown)
        {
            if (_backgroundTexture == null)
            {
                base.DrawBackground(b, bgBounds, dropDown);
                return;
            }

            Color color = Active ? Color.White : Color.DimGray;
            bgBounds.X -= 4;
            bgBounds.Width += 4;

            if (dropDown)
            {
                bgBounds.Height += 8;

                // left border
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X, bgBounds.Y, 12, bgBounds.Height - 12),
                    new Rectangle(4, 12, 12, 24),
                    color);

                // right border
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + bgBounds.Width - 12, bgBounds.Y, 12, bgBounds.Height - 12),
                    new Rectangle(_backgroundTexture.Bounds.Width - 12, 12, 12, 24),
                    color);

                // bottom-left corner
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X, bgBounds.Y + bgBounds.Height - 12, 12, 12),
                    new Rectangle(4, 36, 12, 12),
                    color);

                // bottom-center border
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + 12, bgBounds.Y + bgBounds.Height - 12, bgBounds.Width - 24, 12),
                    new Rectangle(16, 36, 4, 12),
                    color);

                // bottom-right corner
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + bgBounds.Width - 12, bgBounds.Y + bgBounds.Height - 12, 12, 12),
                    new Rectangle(_backgroundTexture.Bounds.Width - 12, 36, 12, 12),
                    color);

                // fill
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + 12, bgBounds.Y, bgBounds.Width - 24, bgBounds.Height - 12),
                    new Rectangle(16, 12, 4, 24),
                    color);
            }
            else
            {
                // left side
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X, bgBounds.Y, 12, bgBounds.Height + 4),
                    new Rectangle(4, 0, 12, 48),
                    color);

                // center
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + 12, bgBounds.Y, bgBounds.Width - 24, bgBounds.Height + 4),
                    new Rectangle(16, 0, 4, 48),
                    color);

                // right side
                b.Draw(_backgroundTexture,
                    new Rectangle(bgBounds.X + bgBounds.Width - 12, bgBounds.Y, 12, bgBounds.Height + 4),
                    new Rectangle(_backgroundTexture.Bounds.Width - 12, 0, 12, 48),
                    color);
            }
        }

        protected override void DrawOption(SpriteBatch b, Rectangle optionBounds, int whichOption, float layerDepth)
        {
            Texture2D texture = _options[whichOption].Key;
            Rectangle optionSource = _options[whichOption].Value;
            float scale = 3f;

            b.Draw(texture,
                new(optionBounds.X + (optionBounds.Width - optionSource.Width * scale) / 2,
                    optionBounds.Y + (optionBounds.Height - optionSource.Height * scale) / 2),
                optionSource,
                Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }
    }
}
