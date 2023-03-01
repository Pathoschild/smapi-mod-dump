/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using RuneMagic.Source;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using static SpaceCore.Skills;

namespace RuneMagic.Source.Interface
{
    internal class SpellBookMenu : IClickableMenu
    {
        private const int WindowWidth = 640;
        private const int WindowHeight = 480;
        private int RectSize = 16;
        private Point GridSize;
        private Point[,] Grid;

        private List<KnownSpellSlot> KnownSpellSlots;
        private MemorizedSpellSlot[] MemorizedSpellSlots = new MemorizedSpellSlot[5];

        public SpellBookMenu()
            : base((Game1.viewport.Width - WindowWidth) / 2, (Game1.viewport.Height - WindowHeight) / 2, WindowWidth, WindowHeight)
        {
            KnownSpellSlots = new List<KnownSpellSlot>();
            MemorizedSpellSlots = new MemorizedSpellSlot[5];
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

        public override void draw(SpriteBatch b)
        {
            drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, WindowWidth, WindowHeight, Color.White);
            DrawSkillBar(b, RuneMagic.PlayerStats.ActiveSkill);
            SetSlots();
            DrawSlots(b);
            base.draw(b);
            drawMouse(b);
        }

        private Rectangle GridRectangle(int x, int y, int width, int height)
        {
            return new Rectangle(Grid[x, y], new Point(width * RectSize, height * RectSize));
        }

        private void DrawSkillBar(SpriteBatch b, Skill skill)
        {
            //draw the skill icon at rect[1,1] of rectSize*3

            b.Draw(skill.Icon, GridRectangle(1, 1, 3, 3), Color.White);
            var xOffset = 4;

            for (int i = 0; i < 15; i++)
            {
                Texture2D texture;
                if (i == 4 || i == 9 || i == 14)
                {
                    if (i > skill.Level)
                        texture = RuneMagic.Textures["icon_profession_empty"];
                    else
                        texture = RuneMagic.Textures["icon_profession_filled"];
                    xOffset--;
                    b.Draw(texture, GridRectangle(xOffset + (i * 2), 1, 6, 3), Color.White);

                    xOffset += 2;
                }
                else
                {
                    if (i > skill.Level)
                        texture = RuneMagic.Textures["icon_level_empty"];
                    else
                        texture = RuneMagic.Textures["icon_level_filled"];
                    b.Draw(texture, GridRectangle(xOffset + (i * 2), 1, 3, 3), Color.White);
                }
            }
        }

        private void SetSlots()
        {
            int xOffset = 2;
            int yOffset = 8;
            for (int level = 1; level <= 5; level++)
            {
                foreach (var spell in RuneMagic.PlayerStats.KnownSpells.Where(s => s.Level == level))
                {
                    var spellSlot = new KnownSpellSlot(spell, GridRectangle(xOffset, yOffset, 4, 4));
                    KnownSpellSlots.Add(spellSlot);
                    xOffset += 3;
                }
                xOffset = 2;
                yOffset += 3;
            }
            yOffset = 8;
            xOffset = 30;
            for (int i = 0; i < MemorizedSpellSlots.Length; i++)
            {
                if (RuneMagic.PlayerStats.MemorizedSpells[i] != null)
                {
                    var spellSlot = new MemorizedSpellSlot(RuneMagic.PlayerStats.MemorizedSpells[i], GridRectangle(xOffset, yOffset, 4, 4));
                    MemorizedSpellSlots[i] = spellSlot;
                }
                else
                {
                    var emptySlot = new MemorizedSpellSlot(null, GridRectangle(xOffset, yOffset, 4, 4));
                    MemorizedSpellSlots[i] = emptySlot;
                }
                yOffset += 3;
            }
        }

        private void DrawSlots(SpriteBatch b)
        {
            foreach (var slot in KnownSpellSlots)
            {
                b.Draw(slot.ButtonTexture, slot.Rectangle, slot.Color);
                b.Draw(slot.Icon, slot.Rectangle, slot.Color);
                //RuneMagic.Instance.Monitor.LogOnce(slot.ToString());
            }
            int xOffset = 30;
            int yOffset = 8;

            foreach (var slot in MemorizedSpellSlots)
            {
                if (slot.Spell == null)
                {
                    b.Draw(RuneMagic.Textures["spellslot_active"], GridRectangle(xOffset, yOffset, 4, 4), Color.White);
                }
                else
                {
                    b.Draw(slot.ButtonTexture, slot.Rectangle, slot.Color);
                    b.Draw(slot.Icon, slot.Rectangle, slot.Color);
                }
                yOffset += 3;
                //RuneMagic.Instance.Monitor.LogOnce($"{slot.Spell}");
            }
        }

        public void MemorizeSpell()
        {
            foreach (var knownSlot in KnownSpellSlots)
            {
                if (knownSlot.Rectangle.Contains(Game1.getMouseX(), Game1.getMouseY()))
                {
                    if (!RuneMagic.PlayerStats.MemorizedSpells.Contains(knownSlot.Spell) && RuneMagic.PlayerStats.MemorizedSpells.Contains(null))
                    {
                        foreach (var memorizedSlot in MemorizedSpellSlots)
                        {
                            if (memorizedSlot.Spell == null)
                            {
                                var index = Array.IndexOf(MemorizedSpellSlots, memorizedSlot);
                                RuneMagic.PlayerStats.MemorizedSpells[index] = knownSlot.Spell;
                                knownSlot.SetButtonTexture();
                                return;
                            }
                        }
                    }
                }
            }
            foreach (var memorizedSlot in MemorizedSpellSlots)
            {
                if (memorizedSlot.Rectangle.Contains(Game1.getMouseX(), Game1.getMouseY()))
                {
                    if (memorizedSlot.Spell != null)
                    {
                        var index = Array.IndexOf(MemorizedSpellSlots, memorizedSlot);
                        RuneMagic.PlayerStats.MemorizedSpells[index] = null;
                        memorizedSlot.SetButtonTexture();
                        return;
                    }
                }
            }
        }
    }
}