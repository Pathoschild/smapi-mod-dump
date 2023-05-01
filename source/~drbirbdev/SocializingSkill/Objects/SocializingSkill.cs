/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using BirbShared;

namespace SocializingSkill
{
    internal class SocializingSkill : SpaceCore.Skills.Skill
    {
        public static KeyedProfession Friendly;
        public static KeyedProfession SmoothTalker;
        public static KeyedProfession Gifter;
        public static KeyedProfession Helpful;
        public static KeyedProfession Haggler;
        public static KeyedProfession Beloved;

        public SocializingSkill() : base("drbirbdev.Socializing")
        {
            this.Icon = ModEntry.Assets.IconA;
            this.SkillsPageIcon = ModEntry.Assets.IconB;
            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(259, 28, 90);

            if (ModEntry.MargoLoaded)
            {
                this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4000, 6900, 10000, 15000, 20000, 25000, 30000, 35000, 40000, 45000, 50000, 55000, 60000, 70000 };

                this.AddProfessions(
                    Friendly = new KeyedProfession(this, "Friendly", ModEntry.Assets.Friendly, ModEntry.Assets.FriendlyP, ModEntry.Instance.Helper),
                    Helpful = new KeyedProfession(this, "Helpful", ModEntry.Assets.Helpful, ModEntry.Assets.HelpfulP, ModEntry.Instance.Helper),
                    SmoothTalker = new KeyedProfession(this, "SmoothTalker", ModEntry.Assets.SmoothTalker, ModEntry.Assets.SmoothTalkerP, ModEntry.Instance.Helper),
                    Gifter = new KeyedProfession(this, "Gifter", ModEntry.Assets.Gifter, ModEntry.Assets.GifterP, ModEntry.Instance.Helper),
                    Haggler = new KeyedProfession(this, "Haggler", ModEntry.Assets.Haggler, ModEntry.Assets.HelpfulP, ModEntry.Instance.Helper),
                    Beloved = new KeyedProfession(this, "Beloved", ModEntry.Assets.Beloved, ModEntry.Assets.BelovedP, ModEntry.Instance.Helper)
                );
            }
            else
            {
                this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4000, 6900, 10000, 15000 };

                this.AddProfessions(
                    Friendly = new KeyedProfession(this, "Friendly", ModEntry.Assets.Friendly, ModEntry.Instance.I18n),
                    Helpful = new KeyedProfession(this, "Helpful", ModEntry.Assets.Helpful, ModEntry.Instance.I18n),
                    SmoothTalker = new KeyedProfession(this, "SmoothTalker", ModEntry.Assets.SmoothTalker, ModEntry.Instance.I18n),
                    Gifter = new KeyedProfession(this, "Gifter", ModEntry.Assets.Gifter, ModEntry.Instance.I18n),
                    Haggler = new KeyedProfession(this, "Haggler", ModEntry.Assets.Haggler, ModEntry.Instance.I18n),
                    Beloved = new KeyedProfession(this, "Beloved", ModEntry.Assets.Beloved, ModEntry.Instance.I18n)
                );
            }

        }

        private void AddProfessions(KeyedProfession lvl5A, KeyedProfession lvl5B, KeyedProfession lvl10A1, KeyedProfession lvl10A2, KeyedProfession lvl10B1, KeyedProfession lvl10B2)
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
            return ModEntry.Instance.I18n.Get("skill.name");
        }

        public override List<string> GetExtraLevelUpInfo(int level)
        {
            List<string> result = new()
            {
                ModEntry.Instance.I18n.Get("skill.perk", new { bonus = ModEntry.Config.ChanceNoFriendshipDecayPerLevel })
            };

            return result;
        }

        public override string GetSkillPageHoverText(int level)
        {
            return ModEntry.Instance.I18n.Get("skill.perk", new { bonus = level * ModEntry.Config.ChanceNoFriendshipDecayPerLevel });
        }
    }
}
