/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewMods.Common.Extensions;
using StardewMods.Common.Models;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

/// <summary>
///     A component for picking a color using HSL sliders.
/// </summary>
internal class HslColorPicker : DiscreteColorPicker
{
    private const int Cells = 16;
    private const int Gap = 6;
    private const int Height = 558;
    private const int Width = 58;

    private static readonly Rectangle SelectRect = new(412, 495, 5, 4);
    private static readonly Range<float> UnitRange = new(0, 1);
    private static HslColor[]? CachedColorsHsl;

    private static Color[]? CachedColorsRgb;
    private static Range<int>? CachedHslTrack;
    private static Texture2D? CachedHueBar;

    private HslColor _hslColor;
    private int _hueCoord;
    private int _lightnessCoord;
    private int _saturationCoord;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HslColorPicker" /> class.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="x">The X coordinate to draw the HslColorPicker at.</param>
    /// <param name="y">The Y coordinate to draw the HslColorPicker at.</param>
    /// <param name="item">The item to draw next to the color picker.</param>
    public HslColorPicker(IModHelper helper, int x, int y, Item? item = default)
        : base(x, y, 0, item)
    {
        this.Helper = helper;
        this.width = HslColorPicker.Width;
        this.height = HslColorPicker.Height;

        var barWidth = this.width / 2 - HslColorPicker.Gap;
        var barHeight = (this.height - HslColorPicker.Gap - 36) / 2;
        var cellSize = barHeight / HslColorPicker.Cells;
        var centerX = this.xPositionOnScreen + this.width / 2;
        var top = this.yPositionOnScreen + 36;

        this.HueBarArea = new(this.xPositionOnScreen, this.yPositionOnScreen + 36, barWidth, this.height - 36);
        this.LightnessBar = Enumerable.Range(0, HslColorPicker.Cells)
                                      .Select(i => new Rectangle(centerX + HslColorPicker.Gap / 2, top + i * cellSize, barWidth, cellSize))
                                      .ToList();
        this.SaturationBar = Enumerable.Range(0, HslColorPicker.Cells)
                                       .Select(i => new Rectangle(centerX + HslColorPicker.Gap / 2, top + barHeight + HslColorPicker.Gap + i * cellSize, barWidth, cellSize))
                                       .ToList();

        this.HueTrack = new(this.HueBarArea.Top, this.HueBarArea.Bottom);
        this.LightnessTrack = new(this.LightnessBar.First().Top, this.LightnessBar.Last().Bottom);
        this.SaturationTrack = new(this.SaturationBar.First().Top, this.SaturationBar.Last().Bottom);

        this.NoColorButton = new(new(this.xPositionOnScreen - 2, this.yPositionOnScreen, 7, 7), Game1.mouseCursors, new(295, 503, 7, 7), Game1.pixelZoom);
        this.NoColorButtonArea = new(this.xPositionOnScreen - 6, this.yPositionOnScreen - 4, 36, 36);
    }

    private enum TrackThumb
    {
        None,
        Hue,
        Saturation,
        Luminance,
        Transparent,
    }

    private int ColorSelection { get; set; } = -1;

    private HslColor[] ColorsHsl
    {
        get => HslColorPicker.CachedColorsHsl ??= this.ColorsRgb.Select(HslColor.FromColor).Distinct().ToArray();
    }

    private Color[] ColorsRgb
    {
        get
        {
            if (HslColorPicker.CachedColorsRgb is not null)
            {
                return HslColorPicker.CachedColorsRgb;
            }

            HslColorPicker.CachedColorsRgb = new Color[this.HueBar.Width * this.HueBar.Height];
            this.HueBar.GetData(HslColorPicker.CachedColorsRgb);
            return HslColorPicker.CachedColorsRgb;
        }
    }

    private TrackThumb HeldThumb { get; set; } = TrackThumb.None;

    private IModHelper Helper { get; }

    private Range<int> HslTrack
    {
        get => HslColorPicker.CachedHslTrack ??= new(0, this.ColorsHsl.Length - 1);
    }

    private Texture2D HueBar
    {
        get => HslColorPicker.CachedHueBar ??= this.Helper.ModContent.Load<Texture2D>("assets/hue.png");
    }

    private Rectangle HueBarArea { get; }

    private int HueCoord
    {
        get => this._hueCoord;
        set
        {
            this._hueCoord = this.HueTrack.Clamp(value);
            var hueIndex = this.HslTrack.Clamp(this._hueCoord
                                                   .Remap(this.HueTrack, HslColorPicker.UnitRange)
                                                   .Remap(HslColorPicker.UnitRange, this.HslTrack));
            var hslColor = this.ColorsHsl[hueIndex];
            this._hslColor.H = hslColor.H;
            if (this.IsBlack)
            {
                this.SaturationCoord = hslColor.S.Remap(HslColorPicker.UnitRange, this.SaturationTrack);
                this.LightnessCoord = hslColor.L.Remap(HslColorPicker.UnitRange, this.LightnessTrack);
            }

            this.IsBlack = false;
        }
    }

    private Range<int> HueTrack { get; }

    private bool IsBlack { get; set; }

    private List<Rectangle> LightnessBar { get; }

    private int LightnessCoord
    {
        get => this._lightnessCoord;
        set
        {
            this._lightnessCoord = this.LightnessTrack.Clamp(value);
            this._hslColor.L = HslColorPicker.UnitRange.Clamp(value.Remap(this.LightnessTrack, HslColorPicker.UnitRange));
            this.IsBlack = false;
        }
    }

    private Range<int> LightnessTrack { get; }

    private ClickableTextureComponent NoColorButton { get; }

    private Rectangle NoColorButtonArea { get; }

    private List<Rectangle> SaturationBar { get; }

    private int SaturationCoord
    {
        get => this._saturationCoord;
        set
        {
            this._saturationCoord = this.SaturationTrack.Clamp(value);
            this._hslColor.S = HslColorPicker.UnitRange.Clamp(value.Remap(this.SaturationTrack, HslColorPicker.UnitRange));
            this.IsBlack = false;
        }
    }

    private Range<int> SaturationTrack { get; }

    /// <summary>Converts an int value to a <see cref="Microsoft.Xna.Framework.Color" /> object.</summary>
    /// <param name="selection">The int value to convert.</param>
    /// <returns>A Color object from the int value.</returns>
    public static Color GetColorFromSelection(int selection)
    {
        if (selection == 0)
        {
            return Color.Black;
        }

        var rgb = BitConverter.GetBytes(selection);
        return new(rgb[0], rgb[1], rgb[2]);
    }

    /// <summary>Converts a <see cref="Microsoft.Xna.Framework.Color" /> to an int value.</summary>
    /// <param name="c">The Color to convert.</param>
    /// <returns>An int value representing the color object.</returns>
    public static int GetSelectionFromColor(Color c)
    {
        if (c == Color.Black)
        {
            return 0;
        }

        return (c.R << 0) | (c.G << 8) | (c.B << 16);
    }

    /// <inheritdoc />
    public override void draw(SpriteBatch b)
    {
        if (!this.visible)
        {
            return;
        }

        // Menu Background
        HslColorPicker.drawTextureBox(
            b,
            this.xPositionOnScreen - HslColorPicker.borderWidth / 2,
            this.yPositionOnScreen - HslColorPicker.borderWidth / 2,
            this.width + HslColorPicker.borderWidth,
            this.height + HslColorPicker.borderWidth,
            Color.LightGray);

        // Transparent Selection Icon
        this.NoColorButton.draw(b);

        // Hue Bar
        b.Draw(this.HueBar, this.HueBarArea, Color.White);

        // Saturation Bar
        foreach (var (bar, color) in this.SaturationBar.Select((bar, index) => (bar, this.GetSaturationShade((float)index / HslColorPicker.Cells))))
        {
            b.Draw(Game1.staminaRect, bar, color);
        }

        // Lightness Bar
        foreach (var (bar, color) in this.LightnessBar.Select((bar, index) => (bar, this.GetLightnessShade((float)index / HslColorPicker.Cells))))
        {
            b.Draw(Game1.staminaRect, bar, color);
        }

        if (this.IsBlack)
        {
            HslColorPicker.drawTextureBox(
                b,
                Game1.mouseCursors,
                new(375, 357, 3, 3),
                this.NoColorButtonArea.Left,
                this.NoColorButtonArea.Top,
                this.NoColorButtonArea.Width,
                this.NoColorButtonArea.Height,
                Color.Black,
                4f,
                false);
        }

        // Hue Selection
        b.Draw(
            Game1.mouseCursors,
            new(this.HueBarArea.Left - 8, this.HueCoord, 20, 16),
            HslColorPicker.SelectRect,
            Color.White,
            MathHelper.PiOver2,
            new(2.5f, 4f),
            SpriteEffects.None,
            1);

        // Saturation Selection
        b.Draw(
            Game1.mouseCursors,
            new(this.SaturationBar.First().Left - 8, this.SaturationCoord, 20, 16),
            HslColorPicker.SelectRect,
            Color.White,
            MathHelper.PiOver2,
            new(2.5f, 4f),
            SpriteEffects.None,
            1);

        // Luminance Selection
        b.Draw(
            Game1.mouseCursors,
            new(this.LightnessBar.First().Left - 8, this.LightnessCoord, 20, 16),
            HslColorPicker.SelectRect,
            Color.White,
            MathHelper.PiOver2,
            new(2.5f, 4f),
            SpriteEffects.None,
            1);

        // Colored Item if Chest
        if (this.itemToDrawColored is Chest chest)
        {
            chest.draw(b, this.xPositionOnScreen, this.yPositionOnScreen - HslColorPicker.borderWidth / 2 - Game1.tileSize, 1f, true);
        }
    }

    /// <summary>Gets the currently selected color.</summary>
    /// <returns>The currently selected color.</returns>
    public Color GetCurrentColor()
    {
        return this.IsBlack ? Color.Black : this._hslColor.ToRgbColor();
    }

    /// <inheritdoc />
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    /// <inheritdoc />
    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    /// <inheritdoc />
    public override void receiveScrollWheelAction(int direction)
    {
        if (this.HeldThumb is not TrackThumb.None)
        {
            return;
        }

        var (x, y) = Game1.getMousePosition(true);
        if (this.HueBarArea.Contains(x, y))
        {
            this.HueCoord += direction;
        }
        else if (this.SaturationBar.Any(bar => bar.Contains(x, y)))
        {
            this.SaturationCoord += direction;
        }
        else if (this.LightnessBar.Any(bar => bar.Contains(x, y)))
        {
            this.LightnessCoord += direction;
        }
        else
        {
            return;
        }

        var color = this.GetCurrentColor();
        this.colorSelection = HslColorPicker.GetSelectionFromColor(color);
        if (this.itemToDrawColored is Chest chest)
        {
            chest.playerChoiceColor.Value = color;
            chest.resetLidFrame();
        }
    }

    /// <inheritdoc />
    public override void update(GameTime time)
    {
        base.update(time);
        if (!this.visible)
        {
            return;
        }

        if (this.ColorSelection != this.colorSelection)
        {
            this.ColorSelection = this.colorSelection;
            if (this.colorSelection != 0)
            {
                var initHsl = HslColor.FromColor(HslColorPicker.GetColorFromSelection(this.colorSelection));
                for (var coord = this.HueBarArea.Top; coord <= this.HueBarArea.Bottom; coord++)
                {
                    if (Math.Abs(initHsl.H - this.ColorsHsl[this.HslTrack.Clamp(coord.Remap(this.HueTrack, HslColorPicker.UnitRange).Remap(HslColorPicker.UnitRange, this.HslTrack))].H) < 0.001)
                    {
                        this.HueCoord = coord;
                        break;
                    }
                }

                this.SaturationCoord = initHsl.S.Remap(HslColorPicker.UnitRange, this.SaturationTrack);
                this.LightnessCoord = initHsl.L.Remap(HslColorPicker.UnitRange, this.LightnessTrack);
            }
            else
            {
                this.HueCoord = 0;
                this.SaturationCoord = 0;
                this.LightnessCoord = 0;
                this.IsBlack = true;
            }

            return;
        }

        var (x, y) = Game1.getMousePosition(true);
        switch (this.HeldThumb)
        {
            case TrackThumb.None when !this.Helper.Input.IsDown(SButton.MouseLeft):
                return;
            case TrackThumb.None when this.NoColorButtonArea.Contains(x, y):
                this.HeldThumb = TrackThumb.Transparent;
                this.HueCoord = 0;
                this.SaturationCoord = 0;
                this.LightnessCoord = 0;
                this.colorSelection = 0;
                this.IsBlack = true;
                Game1.playSound("coin");
                break;
            case TrackThumb.None when this.HueBarArea.Contains(x, y):
                this.HeldThumb = TrackThumb.Hue;
                Game1.playSound("coin");
                break;
            case TrackThumb.None when this.SaturationBar.Any(bar => bar.Contains(x, y)):
                this.HeldThumb = TrackThumb.Saturation;
                Game1.playSound("coin");
                break;
            case TrackThumb.None when this.LightnessBar.Any(bar => bar.Contains(x, y)):
                this.HeldThumb = TrackThumb.Luminance;
                Game1.playSound("coin");
                break;
            case TrackThumb.Hue when this.Helper.Input.IsDown(SButton.MouseLeft):
                this.HueCoord = y;
                break;
            case TrackThumb.Saturation when this.Helper.Input.IsDown(SButton.MouseLeft):
                this.SaturationCoord = y;
                break;
            case TrackThumb.Luminance when this.Helper.Input.IsDown(SButton.MouseLeft):
                this.LightnessCoord = y;
                break;
            case TrackThumb.Transparent when this.Helper.Input.IsDown(SButton.MouseLeft):
                return;
            case TrackThumb.Hue:
            case TrackThumb.Saturation:
            case TrackThumb.Luminance:
            case TrackThumb.Transparent:
            default:
                this.HeldThumb = TrackThumb.None;
                break;
        }

        var color = this.GetCurrentColor();
        this.ColorSelection = this.colorSelection = HslColorPicker.GetSelectionFromColor(color);
        if (this.itemToDrawColored is Chest chest)
        {
            chest.playerChoiceColor.Value = color;
            chest.resetLidFrame();
        }
    }

    private Color GetLightnessShade(float value)
    {
        return new HslColor
        {
            H = this._hslColor.H,
            S = this._hslColor.S,
            L = value,
        }.ToRgbColor();
    }

    private Color GetSaturationShade(float value)
    {
        return new HslColor
        {
            H = this._hslColor.H,
            S = value,
            L = Math.Max(0.01f, this._hslColor.L),
        }.ToRgbColor();
    }
}