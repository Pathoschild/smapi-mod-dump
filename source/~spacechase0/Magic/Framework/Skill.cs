/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Magic.Framework
{
    internal class Skill : SpaceCore.Skills.Skill
    {
        /// <summary>The unique ID for the magic skill.</summary>
        public static readonly string MagicSkillId = "spacechase0.Magic";

        public class GenericProfession : Profession
        {
            public GenericProfession(Skill skill, string theId)
                : base(skill, theId) { }

            internal string Name { get; set; }
            internal string Description { get; set; }

            public override string GetName()
            {
                return this.Name;
            }

            public override string GetDescription()
            {
                return this.Description;
            }
        }

        public class UpgradePointProfession : GenericProfession
        {
            public UpgradePointProfession(Skill skill, string theId)
                : base(skill, theId) { }

            public override void DoImmediateProfessionPerk()
            {
                Game1.player.GetSpellBook().UseSpellPoints(-2);
            }
        }

        public class ManaCapProfession : GenericProfession
        {
            public ManaCapProfession(Skill skill, string theId)
                : base(skill, theId) { }

            public override void DoImmediateProfessionPerk()
            {
                Game1.player.SetMaxMana(Game1.player.GetMaxMana() + 500);
            }
        }

        public static GenericProfession ProfessionUpgradePoint1;
        public static GenericProfession ProfessionUpgradePoint2;
        public static GenericProfession ProfessionFifthSpellSlot;
        public static GenericProfession ProfessionManaRegen1;
        public static GenericProfession ProfessionManaRegen2;
        public static GenericProfession ProfessionManaCap;

        public Skill()
            : base(Skill.MagicSkillId)
        {
            this.Icon = Mod.Instance.Helper.Content.Load<Texture2D>("assets/interface/magicexpicon.png");
            this.SkillsPageIcon = null; // TODO: Make an icon for this

            this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };

            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(0, 66, 255);

            // Level 5
            Skill.ProfessionUpgradePoint1 = new UpgradePointProfession(this, "UpgradePoints1")
            {
                Icon = null, // TODO
                Name = "Potential",
                Description = "+2 spell upgrade points"
            };
            this.Professions.Add(Skill.ProfessionUpgradePoint1);

            Skill.ProfessionManaRegen1 = new GenericProfession(this, "ManaRegen1")
            {
                Icon = null, // TODO
                Name = "Mana Regen I",
                Description = "+0.5 mana regen per level"
            };
            this.Professions.Add(Skill.ProfessionManaRegen1);

            this.ProfessionsForLevels.Add(new ProfessionPair(5, Skill.ProfessionUpgradePoint1, Skill.ProfessionManaRegen1));

            // Level 10 - track A
            Skill.ProfessionUpgradePoint2 = new UpgradePointProfession(this, "UpgradePoints2")
            {
                Icon = null, // TODO
                Name = "Prodigy",
                Description = "+2 spell upgrade points"
            };
            this.Professions.Add(Skill.ProfessionUpgradePoint2);

            Skill.ProfessionFifthSpellSlot = new GenericProfession(this, "FifthSpellSlot")
            {
                Icon = null, // TODO
                Name = "Memory",
                Description = "Adds a fifth spell per spell set."
            };
            this.Professions.Add(Skill.ProfessionFifthSpellSlot);

            this.ProfessionsForLevels.Add(new ProfessionPair(10, Skill.ProfessionUpgradePoint2, Skill.ProfessionFifthSpellSlot, Skill.ProfessionUpgradePoint1));

            // Level 10 - track B
            Skill.ProfessionManaRegen2 = new GenericProfession(this, "ManaRegen2")
            {
                Icon = null, // TODO
                Name = "Mana Regen II",
                Description = "+1 mana regen per level"
            };
            this.Professions.Add(Skill.ProfessionManaRegen2);

            Skill.ProfessionManaCap = new ManaCapProfession(this, "ManaCap")
            {
                Icon = null, // TODO
                Name = "Mana Reserve",
                Description = "+500 max mana"
            };
            this.Professions.Add(Skill.ProfessionManaCap);

            this.ProfessionsForLevels.Add(new ProfessionPair(10, Skill.ProfessionManaRegen2, Skill.ProfessionManaCap, Skill.ProfessionManaRegen1));
        }

        public override string GetName()
        {
            return "Magic";
        }

        public override List<string> GetExtraLevelUpInfo(int level)
        {
            return new()
            {
                "+1 mana regen"
            };
        }

        public override string GetSkillPageHoverText(int level)
        {
            return "+" + level + " mana regen";
        }

        public override void DoLevelPerk(int level)
        {
            // fix mana pool if invalid
            Magic.FixManaPoolIfNeeded(Game1.player, level - 1);

            // add level perk
            int curMana = Game1.player.GetMaxMana();
            if (level > 1 || curMana < Magic.ManaPointsPerLevel) // skip increasing mana for first level, since we did it on learning the skill
                Game1.player.SetMaxMana(curMana + Magic.ManaPointsPerLevel);

            Game1.player.GetSpellBook().UseSpellPoints(-1);
        }
    }
}
