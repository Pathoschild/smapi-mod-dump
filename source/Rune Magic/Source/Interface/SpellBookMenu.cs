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
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using RuneMagic.Source;
using RuneMagic.Source.Items;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using static SpaceCore.Skills;

namespace RuneMagic.Source.Interface
{
    public class SpellBookMenu : MagicMenu
    {
        public SpellBookMenu(SpellBook spellBook)
            : base(spellBook)
        {
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            DrawMemorizedSlots(b);
            DrawTooltip(b);
            drawMouse(b);
        }

        private void DrawMemorizedSlots(SpriteBatch b)
        {
            //var memorizedSpells = RuneMagic.PlayerStats.MemorizedSpells;
            int yOffset = 8;
            int xOffset = 28;
            foreach (var slot in SpellBook.MemorizedSpellSlots)
            {
                var index = SpellBook.MemorizedSpellSlots.IndexOf(slot);
                if (index < 5)
                    xOffset = 28;
                else if (index < 10)
                    xOffset = 31;
                else if (index < 15)
                    xOffset = 34;

                if (index >= Player.MagicStats.MemorizedSpells.Count)
                {
                    slot.Active = false;
                }

                slot.Bounds = GridRectangle(xOffset, yOffset, 4, 4);
                slot.Render(b);
                yOffset += 3;

                if (index == 4 || index == 9)
                    yOffset = 8;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            //if the current location is home
            if (Game1.currentLocation.Name == "FarmHouse" || Game1.currentLocation.Name == "WizardHouse")
            {
                foreach (var knownSlot in SpellBook.KnownSpellSlots)
                {
                    if (knownSlot.Bounds.Contains(x, y))
                    {
                        foreach (var memorizedSlot in SpellBook.MemorizedSpellSlots)
                        {
                            if (Player.MagicStats.MemorizedSpells.Contains(null))
                            {
                                int nullIndex = Player.MagicStats.MemorizedSpells.FindIndex(x => x == null);
                                Player.MagicStats.MemorizedSpells[nullIndex] = knownSlot.Spell;
                                SpellBook.MemorizedSpellSlots[nullIndex].Spell = knownSlot.Spell;
                                //RuneMagic.Instance.Monitor.Log($"{memorizedSlot.Spell.Name}");

                                return;
                            }
                        }
                    }
                }
                foreach (var memorizedSlot in SpellBook.MemorizedSpellSlots)
                {
                    if (memorizedSlot.Bounds.Contains(x, y))
                    {
                        if (memorizedSlot.Spell != null)
                        {
                            var index = SpellBook.MemorizedSpellSlots.IndexOf(memorizedSlot);
                            Player.MagicStats.MemorizedSpells[index] = null;
                            memorizedSlot.Spell = null;
                            memorizedSlot.Selected = false;
                            return;
                        }
                    }
                }
            }
        }
    }
}