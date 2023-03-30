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
using Microsoft.Xna.Framework.Input;
using RuneMagic.Source.Items;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuneMagic.Source.Interface
{
    public class SpellActionBar
    {
        public float Scale { get; set; }
        public Texture2D ArrowTexture { get; set; }
        public Vector2 Center { get; set; }

        private SpellBook SpellBook;

        public SpellActionBar(SpellBook spellBook)
        {
            Scale = 5f;
            SpellBook = spellBook;
        }

        public void Render(SpriteBatch b)
        {
            if (Player.MagicStats.MemorizedSpells == null)
                return;

            //set data for the slot wheel
            Center = new Vector2(b.GraphicsDevice.Viewport.Width / 2, b.GraphicsDevice.Viewport.Height / 2);
            var slotSize = new Vector2(128, 128);
            var slotPosition = new Vector2();
            var radius = 150;
            var angleIncrement = 360f / SpellBook.MemorizedSpellSlots.Count(x => x.Spell != null && x.Active);
            var angle = -90f;
            // Find the slot closest to the mouse position
            var closestSlot = SpellBook.MemorizedSpellSlots.OrderBy(
                slot =>
                {
                    var center = new Vector2(slot.Bounds.Center.X, slot.Bounds.Center.Y);
                    return Vector2.Distance(center, new Vector2(Game1.getMouseX(), Game1.getMouseY()));
                }).First();
            foreach (var slot in SpellBook.MemorizedSpellSlots)
            {
                if (slot.Spell == null || !slot.Active)
                    continue;
                var angleRadians = angle * (Math.PI / 180);
                slotPosition.X = (float)(Center.X - slotSize.X / 2 + radius * Math.Cos(angleRadians));
                slotPosition.Y = (float)(Center.Y - slotSize.Y / 2 + radius * Math.Sin(angleRadians));
                slot.Render(b, new Rectangle((int)slotPosition.X, (int)slotPosition.Y, (int)slotSize.X, (int)slotSize.Y));
                if (slot != closestSlot)
                    slot.Selected = false;
                angle += angleIncrement;
            }

            //make the cursor disappear while selecting
            Game1.mouseCursorTransparency = 0;

            // Select the closest slot and initialize the spell

            //make closest slot rectangle bigger
            if (closestSlot.Spell != null)
            {
                closestSlot.Selected = true;

                SpellBook.SelectedSlot = closestSlot;
                var lastSize = closestSlot.Bounds;

                if (closestSlot.Active)
                {
                    var newSize = new Rectangle(lastSize.X - 20, lastSize.Y - 20, lastSize.Width + 40, lastSize.Height + 40);
                    closestSlot.Render(b, newSize);
                }

                SpellBook.InitializeSpell();
            }
        }
    }
}