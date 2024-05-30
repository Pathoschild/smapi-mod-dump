/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Skill
{
    public class Skill: SpaceCore.Skills.Skill
    {
        ModEntry modEntry;

        /// <summary>The unique ID for the magic skill.</summary>
        public static readonly string WizardrySkillId = "HeyImAmethyst.Wizardry";

        /// <summary>The level 5 'mana efficency' profession.</summary>
        public static GenericProfession ManaEfficiencyProfession;

        /// <summary>The level 10 'mana efficency II' profession.</summary>
        public static GenericProfession ManaEfficiency2Profession;

        /// <summary>The level 10 'mana conservation' profession.</summary>
        public static GenericProfession ManaConservationProfession;

        /// <summary>The level 5 'Mana Regen I' profession.</summary>
        public static GenericProfession ManaRegen1Profession;

        /// <summary>The level 10 'Mana Regen II' profession.</summary>
        public static GenericProfession ManaRegen2Profession;

        /// <summary>The level 10 'Mana Reserve' profession.</summary>
        public static GenericProfession ManaReserveProfession;

        public Skill(ModEntry modEntry) : base(Skill.WizardrySkillId)
        {
            this.modEntry = modEntry;

            this.Icon = modEntry.Helper.ModContent.Load<Texture2D>("assets/icon/interface/wizardry_exp_icon.png");
            this.SkillsPageIcon = modEntry.Helper.ModContent.Load<Texture2D>("assets/icon/interface/wizardry_skill_page_icon.png");

            this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };

            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(180, 62, 232);

            // Level 5
            Skill.ManaEfficiencyProfession = new ManaEfficiencyProfession(this, "ManaEfficiency1", 3)
            {
                Icon = modEntry.Helper.ModContent.Load<Texture2D>("assets/icon/interface/mana_effeciency_profession_icon.png"),
                Name = modEntry.Helper.Translation.Get("professions.ManaEfficiency1.name"),
                Description = modEntry.Helper.Translation.Get("professions.ManaEfficiency1.description")
            };

            this.Professions.Add(Skill.ManaEfficiencyProfession);

            Skill.ManaRegen1Profession = new GenericProfession(this, "ManaRegen1")
            {
                Icon = modEntry.Helper.ModContent.Load<Texture2D>("assets/icon/interface/mana_regen_profession_icon.png"),
                Name = modEntry.Helper.Translation.Get("professions.ManaRegen1.name"),
                Description = modEntry.Helper.Translation.Get("professions.ManaRegen1.description")
            };

            this.Professions.Add(Skill.ManaRegen1Profession);

            this.ProfessionsForLevels.Add(new ProfessionPair(5, Skill.ManaEfficiencyProfession, Skill.ManaRegen1Profession));

            // Level 10 - track A
            Skill.ManaEfficiency2Profession = new ManaEfficiencyProfession(this, "ManaEfficiency2", 2)
            {
                Icon = modEntry.Helper.ModContent.Load<Texture2D>("assets/icon/interface/mana_effeciency_2_profession_icon.png"),
                Name = modEntry.Helper.Translation.Get("professions.ManaEfficiency2.name"),
                Description = modEntry.Helper.Translation.Get("professions.ManaEfficiency2.description")
            };

            this.Professions.Add(Skill.ManaEfficiency2Profession);

            Skill.ManaConservationProfession = new GenericProfession(this, "ManaConservation")
            {
                Icon = modEntry.Helper.ModContent.Load<Texture2D>("assets/icon/interface/mana_conservation_profession_icon.png"),
                Name = modEntry.Helper.Translation.Get("professions.ManaConservation.name"),
                Description = modEntry.Helper.Translation.Get("professions.ManaConservation.description")
            };

            this.Professions.Add(Skill.ManaConservationProfession);

            this.ProfessionsForLevels.Add(new ProfessionPair(10, Skill.ManaEfficiency2Profession, Skill.ManaConservationProfession, Skill.ManaEfficiencyProfession));

            // Level 10 - track B
            Skill.ManaRegen2Profession = new GenericProfession(this, "ManaRegen2")
            {
                Icon = modEntry.Helper.ModContent.Load<Texture2D>("assets/icon/interface/mana_regen_2_profession_icon.png"),
                Name = modEntry.Helper.Translation.Get("professions.ManaRegen2.name"),
                Description = modEntry.Helper.Translation.Get("professions.ManaRegen2.description")
            };
            this.Professions.Add(Skill.ManaRegen2Profession);

            Skill.ManaReserveProfession = new ManaCapProfession(this, "ManaCap")
            {
                Icon = modEntry.Helper.ModContent.Load<Texture2D>("assets/icon/interface/mana_cap_profession_icon.png"),
                Name = modEntry.Helper.Translation.Get("professions.ManaCap.name"),
                Description = modEntry.Helper.Translation.Get("professions.ManaCap.description")
            };
            this.Professions.Add(Skill.ManaReserveProfession);

            this.ProfessionsForLevels.Add(new ProfessionPair(10, Skill.ManaRegen2Profession, Skill.ManaReserveProfession, Skill.ManaRegen1Profession));
        }

        public override string GetName()
        {
            return modEntry.Helper.Translation.Get("skill.wizardry.name");
        }

        public override List<string> GetExtraLevelUpInfo(int level)
        {
            return new()
            {
                modEntry.Helper.Translation.Get("skill.extra_level_up_info.text")
            };
        }

        public override string GetSkillPageHoverText(int level)
        {
            return "+" + level + " " + modEntry.Helper.Translation.Get("skill.skill_page_hover_text_mana_regen.text");
        }

        public override void DoLevelPerk(int level)
        {
            // fix wizardry info if invalid
            modEntry.FixManaPoolIfNeeded(Game1.player, overrideWizardryLevel: level - 1);

            // add level perk
            int curMana = Game1.player.GetMaxMana();
            
            if (level > 1 || curMana < modEntry.ManaPointsPerLevel) // skip increasing mana for first level, since we did it on learning the skill
                Game1.player.SetMaxMana(curMana + modEntry.ManaPointsPerLevel);

            //Game1.player.GetSpellBook().SetManaCostReductionAmount(1);
        }

    }
}
