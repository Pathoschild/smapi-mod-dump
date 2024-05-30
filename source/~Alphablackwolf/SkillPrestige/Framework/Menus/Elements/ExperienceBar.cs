/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Framework.Menus.Elements;

public class ExperienceBar
{
    //Code shamelessly stolen from ExpBarsMod, with naming and some minor things tidied up for reuse.

    private readonly Color BarBorderColor = Color.DarkGoldenrod;
    private readonly Color BarBackgroundColor = Color.DarkSlateGray;
    private readonly Color BarForegroundColor = new (150, 150, 150);
    private readonly Color BarBackgroundTickColor = new (50, 50, 50);
    private readonly Color BarForegroundTickColor = new (120, 120, 120);
    private Texture2D BackgroundTexture, ForegroundTexture;
    private Point _location = new(0, 0);
    public Point Location
    {
        get => this._location;
        set
        {
            this._location = value;
            this.IsHovered = false;
        }
    }

    private Point Size;
    private float Scale = 4f;
    public string HoverText;
    private bool IsHovered;

    private Rectangle Bounds => new (this.Location, this.Size);

    public float Progress = 0f;
    private Color FillColor;
    public ExperienceBar(Point size, Color fillColor)
    {
        this.Size = new Point((size.X / this.Scale).Floor(), (size.Y / this.Scale).Floor());
        this.FillColor = fillColor;
        this.Load();
    }

    public void OnCursorMoved()
    {
        var mousePosition = Game1.getMousePosition(true);
        var actualDisplaySize = new Point((this.Size.X * this.Scale).Floor(), (this.Size.Y * this.Scale).Floor());
        var hitbox = new Rectangle(this.Location, actualDisplaySize);
        this.IsHovered = hitbox.Contains(mousePosition);
    }

    private void Load()
    {
        decimal tenPercentOfWidth = this.Size.X / 10m;
        var xIndicesToAddTickMarksTo = Enumerable.Range(1, 10).Select(index => (tenPercentOfWidth * index).Floor() + 1).ToList();
        this.BackgroundTexture = new Texture2D(Game1.graphics.GraphicsDevice, this.Size.X, this.Size.Y);
        this.ForegroundTexture = new Texture2D(Game1.graphics.GraphicsDevice, this.Size.X, this.Size.Y);
        var emptyColors = new Color[this.Size.X * this.Size.Y];
        var fillColors = new Color[this.Size.X * this.Size.Y];
        for (int pixelXIndex = 0; pixelXIndex < this.Size.X; ++pixelXIndex)
        {
            for (int pixelYIndex = 0; pixelYIndex < this.Size.Y; ++pixelYIndex)
            {
                var backgroundColor = this.BarBackgroundColor;
                var foregroundColor = this.BarForegroundColor;
                if (pixelXIndex == 0 || pixelYIndex == 0 || pixelXIndex == this.Size.X - 1 || pixelYIndex == this.Size.Y - 1)
                {
                    backgroundColor = this.BarBorderColor;
                    foregroundColor = Color.Transparent;

                    // make corners transparent
                    if (pixelXIndex == 0 && pixelYIndex == 0
                        || pixelXIndex == this.Size.X - 1 && pixelYIndex == 0
                        || pixelXIndex == 0 && pixelYIndex == this.Size.Y - 1
                        || pixelXIndex == this.Size.X - 1 && pixelYIndex == this.Size.Y - 1)
                    {
                        backgroundColor = Color.Transparent;
                    }
                }
                else if (xIndicesToAddTickMarksTo.Contains(pixelXIndex))
                {
                    backgroundColor = this.BarBackgroundTickColor;
                    foregroundColor = this.BarForegroundTickColor;
                }

                float scale = pixelYIndex switch
                {
                    1 => 1.3f,
                    2 => 1.7f,
                    3 => 2.0f,
                    4 => 1.9f,
                    5 => 1.5f,
                    6 => 1.3f,
                    7 => 1.0f,
                    8 => 0.8f,
                    9 => 0.4f,
                    _ => 1
                };
                backgroundColor = Color.Multiply(backgroundColor, scale);
                foregroundColor = Color.Multiply(foregroundColor, scale);

                emptyColors[pixelXIndex + pixelYIndex * this.Size.X] = backgroundColor;
                fillColors[pixelXIndex + pixelYIndex * this.Size.X] = foregroundColor;
            }
        }
        this.BackgroundTexture.SetData(emptyColors);
        this.ForegroundTexture.SetData(fillColors);

    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(this.BackgroundTexture, this.Location.ToVector2(),
            new Rectangle(0, 0, this.Size.X, this.Size.Y), Color.White, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 1f);

        var barFillRectangle = new Rectangle(this.Location.X, this.Location.Y, (this.Size.X * this.Progress).Floor(), this.Size.Y);
        spriteBatch.Draw(this.ForegroundTexture, this.Location.ToVector2(), new Rectangle(0, 0, barFillRectangle.Width, this.Size.Y), this.FillColor, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, 1f);
    }

    public void DrawHover(SpriteBatch spriteBatch)
    {
        if (!this.IsHovered) return;
        IClickableMenu.drawHoverText(spriteBatch, this.HoverText, Game1.smallFont);
    }
}
