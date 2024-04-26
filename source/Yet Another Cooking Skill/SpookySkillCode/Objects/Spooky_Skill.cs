/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System.Collections.Generic;
using MoonShared;
using SpookySkillCode.Objects;
using StardewModdingAPI;
using StardewValley;

namespace SpookySkill
{
    public class Spooky_Skill : SpaceCore.Skills.Skill
    {
        public static SpookyProfession Spooky5a;
        public static SpookyProfession Spooky5b;
        public static SpookyProfession Spooky10a1;
        public static SpookyProfession Spooky10a2;
        public static SpookyProfession Spooky10b1;
        public static SpookyProfession Spooky10b2;

        public Spooky_Skill() : base("moonslime.Spooky")
        {
            if (!ModEntry.Config.DeScary) {
                this.Icon = ModEntry.Assets.IconA_Scary;
                this.SkillsPageIcon = ModEntry.Assets.IconB_Scary;

                this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(205, 127, 50);
                this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4000, 6900, 10000, 15000 };
                this.AddProfessions(
                    Spooky5a = new SpookyProfession(this, "Spooky5a", ModEntry.Assets.Spooky5a_Scary, ModEntry.Instance.I18N),
                    Spooky5b = new SpookyProfession(this, "Spooky5b", ModEntry.Assets.Spooky5b_Scary, ModEntry.Instance.I18N),
                    Spooky10a1 = new SpookyProfession(this, "Spooky10a1", ModEntry.Assets.Spooky10a1_Scary, ModEntry.Instance.I18N),
                    Spooky10a2 = new SpookyProfession(this, "Spooky10a2", ModEntry.Assets.Spooky10a2_Scary, ModEntry.Instance.I18N),
                    Spooky10b1 = new SpookyProfession(this, "Spooky10b1", ModEntry.Assets.Spooky10b1_Scary, ModEntry.Instance.I18N),
                    Spooky10b2 = new SpookyProfession(this, "Spooky10b2", ModEntry.Assets.Spooky10b2_Scary, ModEntry.Instance.I18N)
                );
            } else
            {
                this.Icon = ModEntry.Assets.IconA_Thief;

                switch (ModEntry.Config.ThiefIcon)
                {
                    case 1:
                        this.SkillsPageIcon = ModEntry.Assets.IconB_Thief_1;
                        break;
                    case 2:
                        this.SkillsPageIcon = ModEntry.Assets.IconB_Thief_2;
                        break;
                    case 3:
                        this.SkillsPageIcon = ModEntry.Assets.IconB_Thief_3;
                        break;
                    default:
                        this.SkillsPageIcon = ModEntry.Assets.IconB_Thief_1;
                        break;
                }

                this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(205, 127, 50);
                this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4000, 6900, 10000, 15000 };
                this.AddProfessions(
                    Spooky5a = new SpookyProfession(this, "Spooky5a", ModEntry.Assets.Spooky5a_Thief, ModEntry.Instance.I18N),
                    Spooky5b = new SpookyProfession(this, "Spooky5b", ModEntry.Assets.Spooky5b_Thief, ModEntry.Instance.I18N),
                    Spooky10a1 = new SpookyProfession(this, "Spooky10a1", ModEntry.Assets.Spooky10a1_Thief, ModEntry.Instance.I18N),
                    Spooky10a2 = new SpookyProfession(this, "Spooky10a2", ModEntry.Assets.Spooky10a2_Thief, ModEntry.Instance.I18N),
                    Spooky10b1 = new SpookyProfession(this, "Spooky10b1", ModEntry.Assets.Spooky10b1_Thief, ModEntry.Instance.I18N),
                    Spooky10b2 = new SpookyProfession(this, "Spooky10b2", ModEntry.Assets.Spooky10b2_Thief, ModEntry.Instance.I18N)
                );
            }


        }

        private void AddProfessions(SpookyProfession lvl5A, SpookyProfession lvl5B, SpookyProfession lvl10A1, SpookyProfession lvl10A2, SpookyProfession lvl10B1, SpookyProfession lvl10B2)
        {
            this.Professions.Add(lvl5A);
            this.Professions.Add(lvl5B);
            this.ProfessionsForLevels.Add(new ProfessionPair(5, lvl5A, lvl5B));

            this.Professions.Add(lvl10A1);
            this.Professions.Add(lvl10A2);
            this.ProfessionsForLevels.Add(new ProfessionPair(10, lvl10A1, lvl10A2, lvl5A));

            this.Professions.Add(lvl10B1);
            this.Professions.Add(lvl10B2);
            this.ProfessionsForLevels.Add(new ProfessionPair(10, lvl10B1, lvl10B2, lvl5B));
        }

        public override string GetName()
        {
            if (ModEntry.Config.DeScary)
            {
                return ModEntry.Instance.I18N.Get("skill.Spooky.Thief.name");
            }
            else
            {
                return ModEntry.Instance.I18N.Get("skill.Spooky.Scary.name");
            }
        }

        public override List<string> GetExtraLevelUpInfo(int level)
        {

            if (ModEntry.Config.DeScary)
            {

                List<string> result = new()
                {
                    ModEntry.Instance.I18N.Get("skill.Spooky.Thief.perk", new { bonus = 2 })
                };
                if (level == 3)
                {
                    result.Add(ModEntry.Instance.I18N.Get("skill.Spooky.Thief.perk.level_3"));
                }
                if (level == 7)
                {
                    result.Add(ModEntry.Instance.I18N.Get("skill.Spooky.Thief.perk.level_6"));
                }
                return result;

            }
            else
            {

                List<string> result = new()
                {
                    ModEntry.Instance.I18N.Get("skill.Spooky.Scary.perk", new { bonus = 2 })
                };
                if (level == 3)
                {
                    result.Add(ModEntry.Instance.I18N.Get("skill.Spooky.Scary.perk.level_3"));
                }
                if (level == 7)
                {
                    result.Add(ModEntry.Instance.I18N.Get("skill.Spooky.Scary.perk.level_6"));
                }
                return result;



            }
        }

        public override string GetSkillPageHoverText(int level)
        {
            if (ModEntry.Config.DeScary)
            {
                return ModEntry.Instance.I18N.Get("skill.Spooky.Thief.perk", new { bonus = 2 * level });
            }
            else
            {
                return ModEntry.Instance.I18N.Get("skill.Spooky.Scary.perk", new { bonus = 2 * level });
            }
        }



    }
}
