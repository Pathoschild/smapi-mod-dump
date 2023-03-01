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
using RuneMagic.Source.Items;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Threading;
using xTile.Dimensions;
using static SpaceCore.Skills;

namespace RuneMagic.Source
{
    // A class for tracking and manipulating the player's magical abilities and actions in the game
    public class PlayerStats
    {
        public Skill ActiveSkill { get; set; }
        public Dictionary<School, Skill> Skills { get; set; }
        public List<Spell> KnownSpells { get; set; }
        public Spell[] MemorizedSpells { get; set; }
        public List<SpellEffect> ActiveEffects { get; set; }
        public bool IsCasting { get; set; } = false; // Indicates whether the player is currently casting a spell
        public float CastingTimer { get; set; } = 0; // Tracks how long the player has been casting the current spell
        public int CastingFailureChance { get; set; } // Represents the chance that the player's casting attempt will fail

        private int PreviousHealth;

        public PlayerStats()
        {
            Skills = new Dictionary<School, Skill>();
            KnownSpells = new List<Spell>();
            MemorizedSpells = new Spell[5];
            for (int i = 0; i < MemorizedSpells.Length; i++)
            {
                MemorizedSpells[i] = null;
            }
            foreach (School school in Enum.GetValues(typeof(School)))
            {
                Skills.Add(school, new Skill(school));
            }
            ActiveSkill = Skills[School.Abjuration];
            ActiveEffects = new List<SpellEffect>();
        }

        // Attempts to cast a spell using the specified IMagicItem object
        public void Cast(IMagicItem item)
        {
            // Check if the player is holding a valid magic item and whether the "R" key is being pressed
            if (Game1.player.CurrentItem is not IMagicItem) return;
            if (!RuneMagic.Instance.Helper.Input.IsDown(SButton.R))
            {
                IsCasting = false;
                CastingTimer = 0;
                return;
            }

            if (!IsCasting)
            {
                // Store the player's current health before starting to cast the spell
                PreviousHealth = Game1.player.health;
                IsCasting = true;
            }

            // Check if the player has taken damage during the casting process and interrupt casting
            if (Game1.player.health < PreviousHealth)
            {
                IsCasting = false;
                CastingTimer = 0;
                RuneMagic.Instance.Helper.Input.Suppress(SButton.R);
                return;
            }

            // Enter a casting state and suppress certain movement keys
            if (item.Charges >= 1)
            {
                RuneMagic.Instance.Helper.Input.Suppress(SButton.W);
                RuneMagic.Instance.Helper.Input.Suppress(SButton.A);
                RuneMagic.Instance.Helper.Input.Suppress(SButton.S);
                RuneMagic.Instance.Helper.Input.Suppress(SButton.D);
            }

            // If the casting timer exceeds the casting time of the spell, activate the spell and
            // reset the casting state
            if (CastingTimer >= Math.Floor(item.Spell.CastingTime * 60))
            {
                item.Activate();
                RuneMagic.Instance.Helper.Input.Suppress(SButton.R);
                CastingTimer = 0;
            }
            else
            {
                CastingTimer += 1;
            }
        }

        // Adds crafting recipes for various magical items to the player's recipe list, based on the
        // player's current magic level and the list of spells
        public void LearnSpells()
        {
            // Add crafting recipes for spells that the player can learn at their level
            foreach (var spell in RuneMagic.Spells)
            {
                if (KnownSpells.Contains(spell)) continue;
                if (ActiveSkill.Level + 1 >= 1 && spell.Level == 1)
                    KnownSpells.Add(spell);
                if (ActiveSkill.Level + 1 >= 4 && spell.Level == 2)
                    KnownSpells.Add(spell);
                if (ActiveSkill.Level + 1 >= 8 && spell.Level == 3)
                    KnownSpells.Add(spell);
                if (ActiveSkill.Level + 1 >= 12 && spell.Level == 4)
                    KnownSpells.Add(spell);
                if (ActiveSkill.Level + 1 >= 14 && spell.Level == 5)
                    KnownSpells.Add(spell);
            }
        }

        public void LearnRecipes()
        {
            foreach (var skill in RuneMagic.PlayerStats.Skills.Values)
            {
                var level = skill.Level;
                if (level < 1) return;

                var craftingRecipes = new string[]
                {
                    "Runic Anvil",
                    "Inscription Table",
                    "Magic Grinder",
                    "Blank Rune",
                    "Blank Parchment"
                };

                // Add crafting recipes for the above items
                foreach (var recipe in craftingRecipes)
                {
                    if (!Game1.player.craftingRecipes.ContainsKey(recipe))
                        Game1.player.craftingRecipes.Add(recipe, 0);
                }

                // Add crafting recipes for spells that the player can learn at their level
                foreach (var spell in RuneMagic.Spells)
                {
                    var spellLevel = spell.Level;
                    if (level >= spellLevel && ((spellLevel < 5 && level >= spellLevel * 2 - 1) || (spellLevel == 5 && level >= 10)))
                    {
                        var runeName = $"Rune of {spell.Name}";
                        if (!Game1.player.craftingRecipes.ContainsKey(runeName))
                            Game1.player.craftingRecipes.Add(runeName, 0);

                        var scrollName = $"{spell.Name} Scroll";
                        if (!Game1.player.craftingRecipes.ContainsKey(scrollName))
                            Game1.player.craftingRecipes.Add(scrollName, 0);
                    }
                }
            }
        }
    }
}