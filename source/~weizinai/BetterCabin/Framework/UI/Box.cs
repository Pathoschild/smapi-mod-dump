/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.UI;

internal abstract class Box
{
    private readonly Building building;
    protected readonly Cabin Cabin;
    protected readonly ModConfig Config;

    private SpriteFont Font => Game1.smallFont;
    protected abstract Color TextColor { get; }
    protected abstract string Text { get; }
    private Point Size => this.Font.MeasureString(this.Text).ToPoint() + new Point(32, 32);
    private Point Position
    {
        get
        {
            var buildingPosition = new Point(this.building.tileX.Value * 64 - Game1.viewport.X, this.building.tileY.Value * 64 - Game1.viewport.Y);
            return new Point(buildingPosition.X - this.Size.X / 2 + this.Offset.X, buildingPosition.Y - this.Size.Y / 2 + this.Offset.Y);
        }
    }
    protected abstract Point Offset { get; }

    protected Box(Building building, Cabin cabin, ModConfig config)
    {
        this.building = building;
        this.Cabin = cabin;
        this.Config = config;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        IClickableMenu.drawTextureBox(spriteBatch, this.Position.X, this.Position.Y, this.Size.X, this.Size.Y, Color.White);
        Utility.drawTextWithShadow(spriteBatch, this.Text, this.Font, new Vector2(this.Position.X + 16, this.Position.Y + 16), this.TextColor, 1f, 1f);
    }
}