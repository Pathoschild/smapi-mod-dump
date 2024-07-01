/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Spells;
using ArsVenefici.Framework.Util;
using SpaceCore;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Mods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static SpaceCore.Skills;

namespace ArsVenefici.Framework.Skill
{
    public class SpellPartSkillHelper : ISpellPartSkillHelper
    {

        private static SpellPartSkillHelper INSTANCE = new SpellPartSkillHelper();

        // <summary>The prefix added to mod data keys.</summary>
        private const string Prefix = "HeyImAmethyst.ArsVenifici";

        /// <summary>The data key for the player's known spell parts.</summary>
        private const string KnownSpellPartsKey = Prefix + "/KnownSpellPartSkills";

        /// <summary>The player's learned knowlege.</summary>
        public IDictionary<string, SpellPartSkill> KnownSpellPartSkills { get; set; }

        private SpellPartSkillHelper()
        {
            KnownSpellPartSkills = new Dictionary<string, SpellPartSkill>();
        }

        public static SpellPartSkillHelper Instance()
        {
            return INSTANCE;
        }

        public bool Knows(ModEntry modEntry, Farmer player, string skillID)
        {
            return KnownSpellPartSkills.Keys.Contains(skillID);
        }

        public bool Knows(ModEntry modEntry, Farmer player, SpellPartSkill skill)
        {
            return KnownSpellPartSkills.Values.Contains(skill);
        }

        public bool CanLearn(ModEntry modEntry, Farmer player, SpellPartSkill skill)
        {
            bool canLearn = false;

            if (!skill.Cost().Any())
            {
                canLearn = true;
            }
            else
            {
                foreach (Item item in skill.Cost().Keys)
                {
                    if (player.Items.CountId(item.QualifiedItemId) >= skill.Cost()[item])
                    {
                        canLearn = true;
                    }
                }
            }

            //return canLearn && ((KnownKnowlege.Values.Any() && KnownKnowlege.Values.All(value => skill.Parents().Contains(value))) || !skill.Parents().Any());
            return canLearn && ((skill.Parents().Any() && skill.Parents().All(value => KnownSpellPartSkills.Values.Contains(value))) || !skill.Parents().Any());
        }

        public void Learn(ModEntry modEntry, Farmer player, SpellPartSkill skill)
        {
            if (!KnownSpellPartSkills.Keys.Contains(skill.GetId()))
                KnownSpellPartSkills.Add(skill.GetId(), skill);

            SyncToPlayer(player);
            UpdateIfNeeded(modEntry, player);
        }

        public void Learn(ModEntry modEntry, Farmer player, string spellPartId)
        {
            foreach (SpellPartSkill skill in modEntry.spellPartSkillManager.spellPartSkills.Values)
            {
                if (skill.GetId().Equals(spellPartId) && !KnownSpellPartSkills.Keys.Contains(skill.GetId()))
                    KnownSpellPartSkills.Add(spellPartId, skill);
            }
            
            SyncToPlayer(player);
            UpdateIfNeeded(modEntry, player);
        }

        public void LearnAll(ModEntry modEntry, Farmer player)
        {
            foreach(SpellPartSkill spellPartSkill in modEntry.spellPartSkillManager.spellPartSkills.Values)
            {
                if (!KnownSpellPartSkills.Keys.Contains(spellPartSkill.GetId()))
                    KnownSpellPartSkills.Add(spellPartSkill.GetId(), spellPartSkill);

                //Learn(modEntry, player, spellPartSkill);
            }

            SyncToPlayer(player);
            UpdateIfNeeded(modEntry, player);
        }

        public void Forget(ModEntry modEntry, Farmer player, SpellPartSkill skill)
        {
            if(KnownSpellPartSkills.Keys.Contains(skill.GetId()))
                KnownSpellPartSkills.Remove(skill.GetId());

            SyncToPlayer(player);
            UpdateIfNeeded(modEntry, player);
        }

        public void Forget(ModEntry modEntry, Farmer player, string spellPartId)
        {
            foreach (SpellPartSkill skill in modEntry.spellPartSkillManager.spellPartSkills.Values)
            {
                if (skill.GetId().Equals(spellPartId) && KnownSpellPartSkills.Keys.Contains(skill.GetId()))
                    KnownSpellPartSkills.Remove(spellPartId);
            }

            SyncToPlayer(player);
            UpdateIfNeeded(modEntry, player);
        }

        public void ForgetAll(ModEntry modEntry, Farmer player)
        {
            foreach (SpellPartSkill spellPartSkill in modEntry.spellPartSkillManager.spellPartSkills.Values)
            {
                if (KnownSpellPartSkills.Keys.Contains(spellPartSkill.GetId()))
                    KnownSpellPartSkills.Remove(spellPartSkill.GetId());

                //Forget(modEntry, player, spellPartSkill);
            }

            SyncToPlayer(player);
            UpdateIfNeeded(modEntry, player);
        }

        public IDictionary<string, SpellPartSkill> GetKnownSpellPartSkills(ModEntry modEntry, Farmer player)
        {
            return this.KnownSpellPartSkills;
        }

        public void UpdateIfNeeded(ModEntry modEntry, Farmer player)
        {
            this.KnownSpellPartSkills = player.modData.GetCustom(KnownSpellPartsKey, parse => this.ParseKnownSpellPartSkills(modEntry, parse), suppressError: false) ?? new Dictionary<string, SpellPartSkill>();
        }

        private void SyncToPlayer(Farmer player)
        {
            player.modData.SetCustom(KnownSpellPartsKey, KnownSpellPartSkills.Values, serialize: this.SerializeSpellPartSkills);
        }

        /// <summary>Parse serialized known spell part skills.</summary>
        /// <param name="raw">The raw serialized string.</param>
        private IDictionary<string, SpellPartSkill> ParseKnownSpellPartSkills(ModEntry modEntry, string raw)
        {
            Dictionary<string, SpellPartSkill> spells = new();

            if (raw != null)
            {
                foreach (SpellPartSkill spell in this.ParseSpellPartSkill(modEntry, raw))
                {
                    //spells[spell.GetId()] = spell;

                    spells.Add(spell.GetId(), spell);
                }
            }

            return spells;
        }

        /// <summary>Parse a serialized spell part skill list.</summary>
        /// <param name="raw">The raw serialized string.</param>
        private List<SpellPartSkill> ParseSpellPartSkill(ModEntry modEntry, string raw)
        {
            List<SpellPartSkill> spellPartSkill = new();

            if (string.IsNullOrWhiteSpace(raw))
                return spellPartSkill;

            foreach (string rawSpellPart in raw.Split(','))
            {
                if (string.IsNullOrWhiteSpace(rawSpellPart))
                    spellPartSkill.Add(null);
                else
                {
                    modEntry.spellPartSkillManager.spellPartSkills.TryGetValue(rawSpellPart, out SpellPartSkill k);
                    spellPartSkill.Add(k);
                }
            }

            return spellPartSkill;
        }

        /// <summary>Serialize spell part skill for storage.</summary>
        /// <param name="spells">The spell part skills to serialize.</param>
        private string SerializeSpellPartSkills(IEnumerable<SpellPartSkill> spellPartSkills)
        {
            return string.Join(",", spellPartSkills.Select(spellPartSkill => spellPartSkill != null ? $"{spellPartSkill.GetId()}" : "")).TrimEnd(',');
        }
    }
}
