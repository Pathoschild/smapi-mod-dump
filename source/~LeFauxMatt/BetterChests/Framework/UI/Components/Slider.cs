/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.UI.Components;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewMods.Common.Helpers;
using StardewMods.Common.Models;
using StardewValley.Menus;

/// <summary>Represents a slider control that allows the user to select a value within a range.</summary>
internal sealed class Slider
{
    private static readonly Range<float> Unit = new(0, 1);
    private readonly Func<float> getMethod;
    private readonly Action<float> setMethod;
    private readonly Func<float, Color> shadeFunction;
    private readonly Color[] shades;
    private readonly Texture2D? texture;
    private readonly Range<int> track;
    private Rectangle area;
    private int selected;

    /// <summary>Initializes a new instance of the <see cref="Slider" /> class.</summary>
    /// <param name="shadeFunction">A function that determines the color of each step based on its index.</param>
    /// <param name="getMethod">The getter for the value which the slider controls.</param>
    /// <param name="setMethod">The setter for the value which the slider controls.</param>
    /// <param name="area">The rectangular area in which the slider control will be displayed.</param>
    /// <param name="steps">The number of steps in the slider control.</param>
    public Slider(
        Func<float, Color> shadeFunction,
        Func<float> getMethod,
        Action<float> setMethod,
        Rectangle area,
        int steps)
        : this(getMethod, setMethod, area, steps)
    {
        this.shadeFunction = shadeFunction;
        this.shades = new Color[steps];
        this.UpdateShade();
    }

    /// <summary>Initializes a new instance of the <see cref="Slider" /> class.</summary>
    /// <param name="getMethod">The getter for the value which the slider controls.</param>
    /// <param name="setMethod">The setter for the value which the slider controls.</param>
    /// <param name="texture">Top texture to use as the background.</param>
    /// <param name="area">The rectangular area in which the slider control will be displayed.</param>
    /// <param name="steps">The number of steps in the slider control.</param>
    public Slider(Texture2D texture, Func<float> getMethod, Action<float> setMethod, Rectangle area, int steps)
        : this(getMethod, setMethod, area, steps) =>
        this.texture = texture;

    private Slider(Func<float> getMethod, Action<float> setMethod, Rectangle area, int steps)
    {
        this.shadeFunction ??= _ => Color.Black;
        this.shades ??= Array.Empty<Color>();
        this.getMethod = getMethod;
        this.setMethod = setMethod;
        this.area = area;
        this.track = new Range<int>(area.Top, area.Bottom);
        this.Bars = new ClickableComponent[steps];
        var height = this.area.Height / steps;
        var initId = (area.Y * area.Y) + area.X;
        for (var step = 0; step < steps; ++step)
        {
            this.Bars[step] = new ClickableComponent(
                new Rectangle(area.X, area.Y + (step * height), area.Width, height),
                string.Empty)
            {
                myID = step + initId,
            };
        }

        ClickableComponent.ChainNeighborsUpDown(this.Bars.ToList());

        // Initialize selected
        var y = this.getMethod().Remap(Slider.Unit, this.track);
        this.selected = this.GetSelected(y);
    }

    /// <summary>Gets the slider bars.</summary>
    public ClickableComponent[] Bars { get; }

    /// <summary>Gets or sets a value indicating whether the slider is currently being held or not.</summary>
    public bool Holding { get; set; }

    /// <summary>Draws the slider to the screen.</summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw track
        if (this.texture is not null)
        {
            spriteBatch.Draw(this.texture, this.area, Color.White);
        }
        else
        {
            for (var i = 0; i < this.Bars.Length; ++i)
            {
                spriteBatch.Draw(Game1.staminaRect, this.Bars[i].bounds, this.shades[i]);
            }
        }

        // Draw thumb
        spriteBatch.Draw(
            Game1.mouseCursors,
            new Rectangle(this.Bars[this.selected].bounds.Left - 8, this.Bars[this.selected].bounds.Center.Y, 20, 16),
            new Rectangle(412, 495, 5, 4),
            Color.White,
            MathHelper.PiOver2,
            new Vector2(2.5f, 4f),
            SpriteEffects.None,
            1);
    }

    /// <summary>Performs a left click at the specified coordinates on the screen.</summary>
    /// <param name="cursor">The mouse position.</param>
    /// <returns><c>true</c> if the area was clicked; otherwise, <c>false</c>.</returns>
    public bool LeftClick(Point cursor)
    {
        this.Holding = this.area.Contains(cursor);
        return this.Holding;
    }

    /// <summary>Updates the slider based on the mouse position.</summary>
    /// <param name="cursor">The mouse position.</param>
    public void Update(Point cursor)
    {
        var newSelection = this.Holding ? this.GetSelected(cursor.Y) : this.GetSelected();
        if (this.selected != newSelection)
        {
            this.selected = newSelection;

            if (this.Holding)
            {
                this.setMethod(cursor.Y.Remap(this.track, Slider.Unit));
            }
        }

        this.UpdateShade();
    }

    private int GetSelected(int mouseY = 0)
    {
        mouseY = mouseY == 0 ? this.getMethod().Remap(Slider.Unit, this.track) : this.track.Clamp(mouseY);

        if (mouseY <= this.Bars[0].bounds.Bottom)
        {
            return 0;
        }

        if (mouseY >= this.Bars[^1].bounds.Top)
        {
            return this.Bars.Length - 1;
        }

        for (var i = 1; i < this.Bars.Length - 1; ++i)
        {
            if (mouseY >= this.Bars[i].bounds.Top)
            {
                continue;
            }

            return i - 1;
        }

        return this.Bars.Length - 1;
    }

    private void UpdateShade()
    {
        if (this.texture is not null)
        {
            return;
        }

        for (var step = 0; step < this.shades.Length; ++step)
        {
            this.shades[step] = this.shadeFunction((float)step / this.shades.Length);
        }
    }
}