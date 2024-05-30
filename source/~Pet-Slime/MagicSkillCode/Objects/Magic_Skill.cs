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
using StardewModdingAPI;
using MagicSkillCode.Core;

namespace MagicSkillCode.Objects
{
    public class Magic_Skill : SpaceCore.Skills.Skill
    {
        public static UpgradePointProfession Magic5a;
        public static KeyedProfession Magic5b;
        public static UpgradePointProfession Magic10a1;
        public static KeyedProfession Magic10a2;
        public static KeyedProfession Magic10b1;
        public static ManaCapProfession Magic10b2;
        public readonly IModHelper _modHelper;

        public Magic_Skill() : base("moonslime.Magic")
        {
            this.Icon = ModEntry.Assets.IconA;
            this.SkillsPageIcon = ModEntry.Assets.IconB;

            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(205, 127, 50);
            this.ExperienceCurve = new[] { 100, 380, 770, 1300, 2150, 3300, 4000, 6900, 10000, 15000 };
            this.AddProfessions(
                //Potential
                Magic5a = new UpgradePointProfession    (this, "Magic5a",    ModEntry.Assets.Magic5a,    ModEntry.Instance.I18N),
                //Mana Regen I
                Magic5b = new KeyedProfession           (this, "Magic5b",    ModEntry.Assets.Magic5b,    ModEntry.Instance.I18N),
                //Prodigy
                Magic10a1 = new UpgradePointProfession  (this, "Magic10a1",  ModEntry.Assets.Magic10a1,  ModEntry.Instance.I18N),
                //Memory
                Magic10a2 = new KeyedProfession         (this, "Magic10a2",  ModEntry.Assets.Magic10a2,  ModEntry.Instance.I18N),
                //Mana Regen II
                Magic10b1 = new KeyedProfession         (this, "Magic10b1",  ModEntry.Assets.Magic10b1,  ModEntry.Instance.I18N),
                //Mana Reserve
                Magic10b2 = new ManaCapProfession       (this, "Magic10b2",  ModEntry.Assets.Magic10b2,  ModEntry.Instance.I18N)
            );
        }

        private void AddProfessions(UpgradePointProfession lvl5A, KeyedProfession lvl5B, UpgradePointProfession lvl10A1, KeyedProfession lvl10A2, KeyedProfession lvl10B1, ManaCapProfession lvl10B2)
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
            return ModEntry.Instance.I18N.Get("skill.name");
        }

        public override List<string> GetExtraLevelUpInfo(int level)
        {
            List<string> result = new()
            {
                ModEntry.Instance.I18N.Get("skill.perk", new { bonus = 5 })
            };
            return result;
        }

        public override string GetSkillPageHoverText(int level)
        {
            return ModEntry.Instance.I18N.Get("skill.perk", new { bonus = 5 * level });
        }
    }
}
