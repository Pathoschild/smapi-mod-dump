/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using MoonShared;
using Microsoft.Xna.Framework;
using SpaceCore;
using StardewValley;

namespace CookingSkill
{
    public class Cooking_Skill : SpaceCore.Skills.Skill
    {
        public static KeyedProfession Cooking5a;
        public static KeyedProfession Cooking5b;
        public static KeyedProfession Cooking10a1;
        public static KeyedProfession Cooking10a2;
        public static KeyedProfession Cooking10b1;
        public static KeyedProfession Cooking10b2;
        public static readonly IList<string> StartingRecipes = new List<string>();
        public static readonly IDictionary<int, IList<string>> CookingSkillLevelUpRecipes = new Dictionary<int, IList<string>>();

        public Cooking_Skill() : base("moonslime.Cooking")
        {
            this.Icon = ModEntry.Assets.IconA;

            switch (ModEntry.Config.AlternativeSkillPageIcon)
            {
                case 0:
                    this.SkillsPageIcon = ModEntry.Assets.IconB_0;
                    break;
                case 1:
                    this.SkillsPageIcon = ModEntry.Assets.IconB_1;
                    break;
                case 2:
                    this.SkillsPageIcon = ModEntry.Assets.IconB_2;
                    break;
            }

            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(196, 76, 255);

            this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4000, 6900, 10000, 15000 };
            this.AddProfessions(
                Cooking5a = new KeyedProfession(this, "Cooking5a", ModEntry.Assets.Cooking5a, ModEntry.Instance.I18n),
                Cooking5b = new KeyedProfession(this, "Cooking5b", ModEntry.Assets.Cooking5b, ModEntry.Instance.I18n),
                Cooking10a1 = new KeyedProfession(this, "Cooking10a1", ModEntry.Assets.Cooking10a1, ModEntry.Instance.I18n),
                Cooking10a2 = new KeyedProfession(this, "Cooking10a2", ModEntry.Assets.Cooking10a2, ModEntry.Instance.I18n),
                Cooking10b1 = new KeyedProfession(this, "Cooking10b1", ModEntry.Assets.Cooking10b1, ModEntry.Instance.I18n),
                Cooking10b2 = new KeyedProfession(this, "Cooking10b2", ModEntry.Assets.Cooking10b2, ModEntry.Instance.I18n)
            );
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
                ModEntry.Instance.I18n.Get("skill.perk", new { bonus = 3 })
            };            
            return result;
        }

        public override string GetSkillPageHoverText(int level)
        {
            return ModEntry.Instance.I18n.Get("skill.perk", new { bonus = 3 * level });
        }
    }
}
