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

using static DeluxeJournal.Menus.Components.ColorSliderBar;

namespace DeluxeJournal.Menus.Components
{
    public class ColorPickerComponent : IClickableComponentSupplier
    {
        public const string HueSliderName = $"{nameof(ColorSliderBar)}.Hue";
        public const string SaturationSliderName = $"{nameof(ColorSliderBar)}.Saturation";
        public const string ValueSliderName = $"{nameof(ColorSliderBar)}.Value";
        public const string AlphaSliderName = $"{nameof(ColorSliderBar)}.Alpha";

        private const int ColorIconSpacing = 64;

        private Rectangle _bounds;
        private ColorSliderBar? _heldSlider;
        private bool _enabled;
        private bool _enableAlphaSlider;

        /// <summary>Hue color component slider bar.</summary>
        public ColorSliderBar HueSliderBar { get; }

        /// <summary>Saturation color component slider bar.</summary>
        public ColorSliderBar SaturationSliderBar { get; }

        /// <summary>Value color component slider bar.</summary>
        public ColorSliderBar ValueSliderBar { get; }

        /// <summary>Alpha color component slider bar.</summary>
        public ColorSliderBar AlphaSliderBar { get; }

        /// <summary>Bounding box for this component.</summary>
        public Rectangle Bounds => _bounds;

        /// <summary>The color with RGB values normalized by the alpha channel for alpha blend mode.</summary>
        public Color AlphaBlendColor
        {
            get => ColorSchema.HSVToColor(HueSliderBar.Value * 360f, SaturationSliderBar.Value, ValueSliderBar.Value) * (AlphaSliderBar.Value * MaxAlpha);

            set
            {
                ColorSchema.ColorToHSV(value * (255f / value.A), out float hue, out float saturation, out float _value);

                HueSliderBar.Value = hue / 360f;
                SaturationSliderBar.Value = saturation;
                ValueSliderBar.Value = _value;
                AlphaSliderBar.Value = Math.Min(value.A / 255f, MaxAlpha);
            }
        }

        /// <summary>Whether this color picker should be interactable.</summary>
        public bool IsEnabled
        {
            get => _enabled;

            set
            {
                _enabled = value;
                HueSliderBar.IsEnabled = value;
                SaturationSliderBar.IsEnabled = value;
                ValueSliderBar.IsEnabled = value;
                AlphaSliderBar.IsEnabled = value;
            }
        }

        /// <summary>Whether the alpha channel slider bar is enabled.</summary>
        public bool EnableAlphaSlider
        {
            get => _enableAlphaSlider;

            set
            {
                _enableAlphaSlider = value;
                AlphaSliderBar.visible = value;
                _bounds.Height = SliderHeight * (value ? 4 : 3);
            }
        }

        /// <summary>Maximum alpha value for the alpha slider range.</summary>
        public float MaxAlpha { get; set; } = 1f;

        public ColorPickerComponent(int x, int y, int thumbIdStart = -500, int leftNeighborId = -1, int rightNeighborId = -1)
        {
            _bounds = new(x, y, SliderWidth + ColorIconSpacing, SliderHeight * 3);
            _enabled = true;

            bool leftNeighborImmutable = leftNeighborId == -1;
            bool rightNeighborImmutable = rightNeighborId == -1;

            HueSliderBar = new ColorSliderBar(x + ColorIconSpacing, y, HueSliderName, thumbIdStart)
            {
                leftNeighborID = leftNeighborId,
                rightNeighborID = rightNeighborId,
                leftNeighborImmutable = leftNeighborImmutable,
                rightNeighborImmutable = rightNeighborImmutable
            };

            SaturationSliderBar = new ColorSliderBar(
                x + ColorIconSpacing,
                y + SliderHeight,
                SaturationSliderName,
                thumbIdStart == -500 ? -500 : thumbIdStart + 1)
            {
                leftNeighborID = leftNeighborId,
                rightNeighborID = rightNeighborId,
                leftNeighborImmutable = leftNeighborImmutable,
                rightNeighborImmutable = rightNeighborImmutable
            };

            ValueSliderBar = new ColorSliderBar(
                x + ColorIconSpacing,
                y + SliderHeight * 2,
                ValueSliderName,
                thumbIdStart == -500 ? -500 : thumbIdStart + 2)
            {
                leftNeighborID = leftNeighborId,
                rightNeighborID = rightNeighborId,
                leftNeighborImmutable = leftNeighborImmutable,
                rightNeighborImmutable = rightNeighborImmutable
            };

            AlphaSliderBar = new ColorSliderBar(
                x + ColorIconSpacing,
                y + SliderHeight * 3,
                AlphaSliderName,
                thumbIdStart == -500 ? -500 : thumbIdStart + 3)
            {
                visible = false,
                leftNeighborID = leftNeighborId,
                rightNeighborID = rightNeighborId,
                leftNeighborImmutable = leftNeighborImmutable,
                rightNeighborImmutable = rightNeighborImmutable
            };
        }

        public IEnumerable<ClickableComponent> GetClickableComponents()
        {
            yield return HueSliderBar;
            yield return SaturationSliderBar;
            yield return ValueSliderBar;
            yield return AlphaSliderBar;
        }

        public void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (HueSliderBar.containsPoint(x, y))
            {
                HueSliderBar.SetValueFromPoint(x, y);
                _heldSlider = HueSliderBar;
            }
            else if (SaturationSliderBar.containsPoint(x, y))
            {
                SaturationSliderBar.SetValueFromPoint(x, y);
                _heldSlider = SaturationSliderBar;
            }
            else if (ValueSliderBar.containsPoint(x, y))
            {
                ValueSliderBar.SetValueFromPoint(x, y);
                _heldSlider = ValueSliderBar;
            }
            else if (AlphaSliderBar.containsPoint(x, y))
            {
                AlphaSliderBar.SetValueFromPoint(x, y);
                _heldSlider = AlphaSliderBar;
            }
        }

        public void LeftClickHeld(int x, int y)
        {
            _heldSlider?.SetValueFromPoint(x, y);
        }

        public void ReleaseLeftClick(int x, int y)
        {
            _heldSlider = null;
        }

        public void Draw(SpriteBatch b)
        {
            float hue = HueSliderBar.Value * 360f;
            float saturation = SaturationSliderBar.Value;
            float value = ValueSliderBar.Value;
            float alphaEffect = IsEnabled ? 1f : 0.5f;
            Color color = AlphaBlendColor;

            Utility.drawWithShadow(b, DeluxeJournalMod.UiTexture, new(_bounds.X, _bounds.Y + 6), new(64, 64, 12, 12), IsEnabled ? Color.White : Color.DarkGray, 0f, Vector2.Zero, 4f);
            b.Draw(Game1.staminaRect, new Rectangle(_bounds.X + 8, _bounds.Y + 14, 32, 32), Color.Gray);

            if (color.A < 255)
            {
                b.Draw(DeluxeJournalMod.UiTexture, new Rectangle(_bounds.X + 8, _bounds.Y + 14, 32, 32), new(56, 72, 8, 8), Color.White * alphaEffect);
            }

            b.Draw(Game1.staminaRect, new Rectangle(_bounds.X + 8, _bounds.Y + 14, 32, 32), color * alphaEffect);

            HueSliderBar.DrawHueBar(b, alphaEffect);
            SaturationSliderBar.DrawSaturationBar(b, hue, value, alphaEffect);
            ValueSliderBar.DrawValueBar(b, hue, saturation, alphaEffect);

            if (AlphaSliderBar.visible)
            {
                AlphaSliderBar.DrawAlphaBar(b, hue, saturation, value, MaxAlpha);
            }
        }
    }
}
