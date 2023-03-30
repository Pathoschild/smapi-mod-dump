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
using RuneMagic.Source.Interface;
using RuneMagic.Source.Spells;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Object = StardewValley.Object;

namespace RuneMagic.Source.Items
{
    [XmlType("Mods_SpellBook")]
    public class SpellBook : Object, ISpellCastingItem
    {
        [XmlIgnore]
        public Spell Spell { get; set; }

        [XmlIgnore]
        public List<MagicButton> KnownSpellSlots { get; set; }

        [XmlIgnore]
        public List<MagicButton> MemorizedSpellSlots { get; set; }

        [XmlIgnore]
        public MagicButton SelectedSlot { get; set; }

        [XmlIgnore]
        public SpellBookMenu Menu { get; set; }

        [XmlIgnore]
        public SpellActionBar ActionBar { get; set; }

        public SpellBook() : base()
        {
            KnownSpellSlots = new List<MagicButton>();
            MemorizedSpellSlots = new List<MagicButton>();
            for (int i = 0; i < 15; i++)
                MemorizedSpellSlots.Add(new MagicButton());

            Menu = new SpellBookMenu(this);
            ActionBar = new SpellActionBar(this);
        }

        public SpellBook(int parentSheetIndex, int stack) : base(parentSheetIndex, stack)
        {
            KnownSpellSlots = new List<MagicButton>();
            MemorizedSpellSlots = new List<MagicButton>();
            for (int i = 0; i < 15; i++)
                MemorizedSpellSlots.Add(new MagicButton());

            Menu = new SpellBookMenu(this);
            ActionBar = new SpellActionBar(this);
        }

        public void InitializeSpell()
        {
            if (SelectedSlot != null)
                Spell = SelectedSlot.Spell;
            //else
            //    RuneMagic.Instance.Monitor.Log("No spell in Selected Slot");
        }

        public void Activate()
        {
            if (Spell != null && Spell.Cast())
            {
                Game1.playSound("flameSpell");
                SelectedSlot.Active = false;
                Player.MagicStats.MemorizedSpells.Remove(Spell);
                SelectedSlot.Spell = null;

                Spell = null;
                return;
            }
        }

        public void Update()
        {
        }

        public void UpdateSpellSlots()
        {
            foreach (var slot in MemorizedSpellSlots)
            {
                slot.Active = true;
                slot.Selected = false;
            }
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (Player.MagicStats.CastingTime > 0)
                base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        public override bool canBeShipped()
        {
            return false;
        }

        public override bool canBeGivenAsGift()
        {
            return false;
        }

        public override int maximumStackSize()
        {
            return 1;
        }
    }
}