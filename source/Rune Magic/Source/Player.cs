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
using Microsoft.Xna.Framework.Input;
using Netcode;
using RuneMagic.Source.Interface;
using RuneMagic.Source.Items;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using xTile.Dimensions;
using static SpaceCore.Skills;

namespace RuneMagic.Source
{
    // A class for tracking and manipulating the player's magical abilities and actions in the game
    public class Player
    {
        public static readonly Player MagicStats = new();

        public School ActiveSchool { get; set; } = null;

        public bool MagicLearned { get; set; } = false;
        public bool RuneCraftingLearned { get; set; } = false;
        public bool ScrollScribingLearned { get; set; } = false;

        public CastBar CastBar { get; set; }
        public float CastingTime { get; set; } = 0;

        public List<SpellEffect> ActiveEffects { get; set; }

        public List<Spell> KnownSpells { get; set; }
        public List<Spell> MemorizedSpells { get; set; }

        private int _healthBeforeCasting;
        private int _prevoiusMagicSchoolLevel = 0;

        public Player()
        {
            KnownSpells = new List<Spell>();
            MemorizedSpells = new List<Spell>();
            ActiveSchool = School.Alteration;
            ActiveEffects = new List<SpellEffect>();
            CastBar = new CastBar();
        }

        public void Update()
        {
            ActiveSchool.SetLevel();

            ActivateSpellCastingItem(Game1.player.CurrentItem as ISpellCastingItem);
            LearnSpells();

            //Effects
            for (int i = 0; i < ActiveEffects.Count; i++)
                ActiveEffects[i].Update();
            if (_prevoiusMagicSchoolLevel != ActiveSchool.Level)
                //if the player has a SpellBook in the inventory
                if (Game1.player.Items.Any(item => item is SpellBook))
                {
                    //get the first SpellBook in the inventory
                    var spellBook = Game1.player.Items.First(item => item is SpellBook) as SpellBook;
                    //update the spell slots
                    spellBook.UpdateSpellSlots();
                }
            _prevoiusMagicSchoolLevel = ActiveSchool.Level;

            foreach (ISpellCastingItem spellaCastingItem in Game1.player.Items.Where(item => item is ISpellCastingItem))
            {
                spellaCastingItem.Update();
            }
        }

        public void ActivateSpellCastingItem(ISpellCastingItem item)
        {
            if (Game1.player.CurrentItem is not ISpellCastingItem) return;

            if (!RuneMagic.Instance.Helper.Input.IsDown(RuneMagic.Config.CastKey))
            {
                CastingTime = 0;
                return;
            }

            if (CastingTime == 0)
            {
                _healthBeforeCasting = Game1.player.health;
            }

            if (Game1.player.health < _healthBeforeCasting)
            {
                CastingTime = 0;
                RuneMagic.Instance.Helper.Input.Suppress(RuneMagic.Config.CastKey); return;
            }

            RuneMagic.Instance.Helper.Input.Suppress(SButton.W);
            RuneMagic.Instance.Helper.Input.Suppress(SButton.A);
            RuneMagic.Instance.Helper.Input.Suppress(SButton.S);
            RuneMagic.Instance.Helper.Input.Suppress(SButton.D);

            if (item.Spell != null && CastingTime >= Math.Floor(item.Spell.CastingTime * 60))
            {
                item.Activate();
                RuneMagic.Instance.Helper.Input.Suppress(RuneMagic.Config.CastKey);
                CastingTime = 0;
            }
            else { CastingTime += 1; }
        }

        public void LearnSpells()
        {
            //add null to memorized spells to allow for empty slots
            if (MemorizedSpells.Count < ActiveSchool.Level + 1)
            {
                MemorizedSpells.Add(null);
            }

            // Add crafting recipes for spells that the player can learn at their level
            foreach (var spell in Spell.List)
            {
                if (ActiveSchool != null)
                {
                    if (KnownSpells.Contains(spell)) continue;
                    if (ActiveSchool.Level + 1 >= 1 && spell.Level == 1)
                        KnownSpells.Add(spell);
                    if (ActiveSchool.Level + 1 >= 4 && spell.Level == 2)
                        KnownSpells.Add(spell);
                    if (ActiveSchool.Level + 1 >= 8 && spell.Level == 3)
                        KnownSpells.Add(spell);
                    if (ActiveSchool.Level + 1 >= 12 && spell.Level == 4)
                        KnownSpells.Add(spell);
                    if (ActiveSchool.Level + 1 >= 14 && spell.Level == 5)
                        KnownSpells.Add(spell);
                }
            }
        }

        public void LearnRecipes()
        {
            if (Player.MagicStats.ScrollScribingLearned)
            {
                if (!Game1.player.craftingRecipes.ContainsKey("Inscription Table"))
                    Game1.player.craftingRecipes.Add("Inscription Table", 0);
                if (!Game1.player.craftingRecipes.ContainsKey("Magic Grinder"))
                    Game1.player.craftingRecipes.Add("Magic Grinder", 0);
                foreach (var spell in Player.MagicStats.KnownSpells)
                {
                    if (!Game1.player.craftingRecipes.ContainsKey($"{spell.Name} Scroll"))
                        Game1.player.craftingRecipes.Add($"{spell.Name} Scroll", 0);
                }
            }
            if (Player.MagicStats.RuneCraftingLearned)
            {
                if (!Game1.player.craftingRecipes.ContainsKey("Runic Anvil"))
                    Game1.player.craftingRecipes.Add("Runic Anvil", 0);
                if (!Game1.player.craftingRecipes.ContainsKey("Magic Grinder"))
                    Game1.player.craftingRecipes.Add("Magic Grinder", 0);
                foreach (var spell in Player.MagicStats.KnownSpells)
                {
                    if (!Game1.player.craftingRecipes.ContainsKey($"Rune of {spell.Name}"))
                        Game1.player.craftingRecipes.Add($"Rune of {spell.Name}", 0);
                }
            }
        }

        public void MagicCraftingActions()
        {
            var cursorLocation = Game1.currentCursorTile;
            var objectUnderCursor = Game1.currentLocation.getObjectAtTile((int)cursorLocation.X, (int)cursorLocation.Y);

            if (objectUnderCursor != null && objectUnderCursor.ParentSheetIndex == RuneMagic.JsonAssetsApi.GetBigCraftableId("Runic Anvil"))
            {
                foreach (Item item in Game1.player.Items)
                {
                    if (item is SpellBook spellBook)
                    {
                        var runeCraftingMenu = new RuneCraftingMenu(spellBook);
                        if (Game1.activeClickableMenu != runeCraftingMenu)
                            Game1.activeClickableMenu = runeCraftingMenu;
                        return;
                    }
                }
            }
            if (objectUnderCursor != null && objectUnderCursor.ParentSheetIndex == RuneMagic.JsonAssetsApi.GetBigCraftableId("Inscription Table"))
            {
                foreach (Item item in Game1.player.Items)
                {
                    if (item is SpellBook spellBook)
                    {
                        var scrollScribingMenu = new ScrollScribingMenu(spellBook);
                        if (Game1.activeClickableMenu != scrollScribingMenu)
                            Game1.activeClickableMenu = scrollScribingMenu;
                        return;
                    }
                }
            }
        }
    }
}