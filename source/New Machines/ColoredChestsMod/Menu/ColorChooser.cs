using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.ColoredChestsMod.Utils;
using Igorious.StardewValley.DynamicAPI.Menu;
using Igorious.StardewValley.DynamicAPI.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Objects;

namespace Igorious.StardewValley.ColoredChestsMod.Menu
{
    public sealed class ColorChooser : GridToolTip
    {
        public ColorChooser()
        {
            var colorsInRow = ColoredChestsMod.Config.MenuConfig.ColorsInRow;

            var colors = new List<Color>();
            foreach (var colorName in ColoredChestsMod.Config.MenuConfig.Colors)
            {
                var color = ColorParser.Parse(colorName);
                if (color != null)
                {
                    colors.Add(color.Value);
                }
                else
                {
                    Log.Fail($"«{colorName}» is not valid name for color!");
                }
            }

            for (var i = 0; i < colors.Count; ++i)
            {
                RegisterCell(new ColorCell(i / colorsInRow, i % colorsInRow, colors[i]));
            }
        }

        public override bool IsExclusive { get; protected set; } = true;

        public override bool NeedDraw()
        {
            if (!IsAltPressed()) return false;
            if (HoveredChest != null && ToolTipManager.Instance.CurrentToolTips.Contains(this) && LastBounds.Contains(MouseX, MouseY)) return true;
            HoveredChest = GetHoveredChest();
            HoveredChestPosition = new Point(NormalizedMouseX / TileSize * TileSize - Game1.viewport.X, NormalizedMouseY / TileSize * TileSize - Game1.viewport.Y);
            return HoveredChest != null;
        }

        private Chest HoveredChest { get; set; }
        private Point HoveredChestPosition { get; set; }

        public override void Draw()
        {
            Recalculate();
            var x0 = HoveredChestPosition.X;
            var y0 = HoveredChestPosition.Y + TileSize;
            Draw(x0, y0);

            var colorCell = GetCell(MouseX - x0, MouseY - y0) as ColorCell;
            if (colorCell != null)
            {
                var newColor = colorCell.Color;
                if (HoveredChest.tint != newColor)
                {
                    HoveredChest.tint = newColor;
                    var position = Game1.currentLocation.Objects.First(kv => kv.Value == HoveredChest).Key;
                    Log.Info($"Set chest (x={position.X}, y={position.Y}) color=#{newColor.R:X2}{newColor.G:X2}{newColor.B:X2}");
                }
            }

            if (LastBounds.Contains(MouseX, MouseY)) RedrawCursor();
        }

        private static void RedrawCursor()
        {
            Game1.spriteBatch.Draw(
                Game1.mouseCursors,
                new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16),
                Color.White,
                0,
                Vector2.Zero,
                4 + Game1.dialogueButtonScale / 150,
                SpriteEffects.None,
                1);
        }

        private static bool IsAltPressed()
        {
            var state = Keyboard.GetState();
            return state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt);
        }

        private static Chest GetHoveredChest()
        {
            return Game1.currentLocation.getObjectAt(NormalizedMouseX, NormalizedMouseY) as Chest;
        }
    }
}
