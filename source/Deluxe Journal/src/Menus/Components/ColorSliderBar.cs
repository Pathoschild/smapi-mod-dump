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

using static DeluxeJournal.Task.ColorSchema;

namespace DeluxeJournal.Menus.Components
{
    public class ColorSliderBar : ClickableComponent
    {
        public const int SliderWidth = 168;
        public const int SliderHeight = 20;
        public const int BarHeight = 8;
        
        private const int CheckeredTileWidth = 5;

        private float _value;

        /// <summary>Number of color samples to draw.</summary>
        public int Samples { get; set; } = 24;

        /// <summary>Percentage slider value in the range <c>[0f,1f]</c>.</summary>
        public float Value
        {
            get => _value;
            set => _value = Math.Clamp(value, 0f, 1f);
        }

        /// <summary>Slider value in the range <c>[0,255]</c>.</summary>
        public int ValueInt
        {
            get => (int)(_value * 255f);
            set => _value = Math.Clamp(value / 255f, 0f, 1f);
        }

        /// <summary>Whether this slider should be interactable.</summary>
        public bool IsEnabled { get; set; }

        public ColorSliderBar(int x, int y, string name, int myId = -500, float value = 0.5f)
            : base(new(x, y, SliderWidth, SliderHeight), name)
        {
            myID = myId;
            upNeighborID = SNAP_AUTOMATIC;
            downNeighborID = SNAP_AUTOMATIC;
            Value = value;
            IsEnabled = true;
        }

        public void SetValueFromPoint(int x, int y)
        {
            if (containsPoint(x, y))
            {
                Value = (x - bounds.X) / (float)SliderWidth;
            }
        }

        public void DrawHueBar(SpriteBatch b, float alpha = 1f)
        {
            DrawSliderBar(b, -1f, 0.9f, 0.9f, alpha, IsEnabled ? Game1.textColor : Game1.unselectedOptionColor);
        }

        public void DrawSaturationBar(SpriteBatch b, float hue, float value, float alpha = 1f)
        {
            DrawSliderBar(b, hue, -1f, value, alpha, IsEnabled ? Game1.textColor : Game1.unselectedOptionColor);
        }

        public void DrawValueBar(SpriteBatch b, float hue, float saturation, float alpha = 1f)
        {
            DrawSliderBar(b, hue, saturation, -1f, alpha, IsEnabled ? Game1.textColor : Game1.unselectedOptionColor);
        }

        public void DrawAlphaBar(SpriteBatch b, float hue, float saturation, float value, float maxAlpha = 1f)
        {
            DrawSliderBar(b, hue, saturation, value, maxAlpha, IsEnabled ? Game1.textColor : Game1.unselectedOptionColor);
        }

        private void DrawSliderBar(SpriteBatch b, float hue, float saturation, float value, float alpha, Color textColor)
        {
            if (!visible)
            {
                return;
            }

            int sampleWidth = SliderWidth / Samples;
            int samplesInCheckeredTile = Math.Max(CheckeredTileWidth * Samples / SliderWidth, 1);
            Rectangle sampleBounds = new Rectangle(bounds.X, bounds.Y + (SliderHeight - BarHeight) / 2, sampleWidth, BarHeight);

            for (int i = 0; i < Samples; i++)
            {
                float sampleValue = i / (float)Samples;
                Color color;

                if (hue < 0)
                {
                    color = HSVToColor(sampleValue * 360f, saturation, value);
                }
                else if (saturation < 0)
                {
                    color = HSVToColor(hue, sampleValue, value);
                }
                else if (value < 0)
                {
                    color = HSVToColor(hue, saturation, sampleValue);
                }
                else
                {
                    color = HSVToColor(hue, saturation, value);
                    color *= sampleValue * alpha;

                    if (color.A < 255)
                    {
                        b.Draw(DeluxeJournalMod.UiTexture, sampleBounds, new(56 + i / samplesInCheckeredTile % 2 * 2, 72, 2, 4), Color.White);
                    }

                    goto DrawSample;
                }

                if (alpha < 1f)
                {
                    color *= alpha;
                }

            DrawSample:
                b.Draw(Game1.staminaRect, sampleBounds, color);
                sampleBounds.X += sampleWidth;
            }

            b.Draw(Game1.mouseCursors, new Vector2(bounds.X + SliderWidth * Value, bounds.Y), new(64, 256, 32, 20), Color.White, 0f, new(16f, 0f), 1f, SpriteEffects.None, 0.86f);
            Utility.drawTextWithShadow(b, ((int)(Value * 100f)).ToString(), Game1.smallFont, new(bounds.X + SliderWidth + 16, bounds.Y + SliderHeight - Game1.smallFont.LineSpacing), textColor);
        }
    }
}
