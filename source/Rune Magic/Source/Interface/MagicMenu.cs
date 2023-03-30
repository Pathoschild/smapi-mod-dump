/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuneMagic.Source.Items;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneMagic.Source.Interface
{
    public class MagicMenu : IClickableMenu
    {
        public const int WindowWidth = 640;
        public const int WindowHeight = 480;
        public int RectSize = 16;
        public Point GridSize;
        public Point[,] Grid;
        public SpellBook SpellBook;

        public MagicMenu(SpellBook spellBook)
            : base((Game1.viewport.Width - WindowWidth) / 2, (Game1.viewport.Height - WindowHeight) / 2, WindowWidth, WindowHeight, true)
        {
            SpellBook = spellBook;
            SetGrid();
        }

        public override void draw(SpriteBatch b)
        {
            drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, WindowWidth, WindowHeight, Color.White);
            DrawSkillBar(b, Player.MagicStats.ActiveSchool);
            DrawKnownSlots(b);
            base.draw(b);
        }

        public Rectangle GridRectangle(int x, int y, int width, int height)
        {
            return new Rectangle(Grid[x, y], new Point(width * RectSize, height * RectSize));
        }

        private void SetGrid()
        {
            GridSize = new Point(WindowWidth / RectSize, WindowHeight / RectSize);

            Grid = new Point[GridSize.X, GridSize.Y];
            for (int x = 0; x < GridSize.X; x++)
            {
                for (int y = 0; y < GridSize.Y; y++)
                {
                    Grid[x, y] = new Point(xPositionOnScreen + (x * RectSize), yPositionOnScreen + (y * RectSize));
                }
            }
        }

        private void DrawSkillBar(SpriteBatch b, School skill)
        {
            //draw the skill icon at rect[1,1] of rectSize*3

            b.Draw(skill.Icon, GridRectangle(1, 1, 3, 3), Color.White);
            var xOffset = 4;

            for (int i = 0; i < 15; i++)
            {
                Texture2D texture;
                if (i == 4 || i == 9 || i == 14)
                {
                    if (i >= skill.Level)
                        texture = RuneMagic.Textures["icon_profession_empty"];
                    else
                        texture = RuneMagic.Textures["icon_profession_filled"];
                    xOffset--;
                    b.Draw(texture, GridRectangle(xOffset + (i * 2), 1, 6, 3), Color.White);

                    xOffset += 2;
                }
                else
                {
                    if (i >= skill.Level)
                        texture = RuneMagic.Textures["icon_level_empty"];
                    else
                        texture = RuneMagic.Textures["icon_level_filled"];
                    b.Draw(texture, GridRectangle(xOffset + (i * 2), 1, 3, 3), Color.White);
                }
            }
        }

        private void DrawKnownSlots(SpriteBatch b)
        {
            SpellBook.KnownSpellSlots.Clear();
            int xOffset = 2;
            int yOffset = 8;
            for (int level = 1; level <= 5; level++)
            {
                foreach (var spell in Player.MagicStats.KnownSpells.Where(s => s.Level == level))
                {
                    var slot = new MagicButton(spell, GridRectangle(xOffset, yOffset, 4, 4));
                    SpellBook.KnownSpellSlots.Add(slot);
                    slot.Render(b);
                    xOffset += 3;
                }
                xOffset = 2;
                yOffset += 3;
            }
        }

        public void DrawTooltip(SpriteBatch b)
        {
            foreach (var knownSlot in SpellBook.KnownSpellSlots)
            {
                var tooltipString = $"{knownSlot.Spell.Name}\n\n" +
                    $"{knownSlot.Spell.Description}";

                if (knownSlot.Bounds.Contains(Game1.getMouseX(), Game1.getMouseY()))
                    drawHoverText(b, tooltipString, Game1.smallFont);
            }
        }
    }
}