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

namespace BinningSkill
{
    internal class BinningSkill : SpaceCore.Skills.Skill
    {
        public static KeyedProfession Recycler;
        public static KeyedProfession Sneak;
        public static KeyedProfession Environmentalist;
        public static KeyedProfession Salvager;
        public static KeyedProfession Upseller;
        public static KeyedProfession Reclaimer;

        public BinningSkill() : base("drbirbdev.Binning")
        {
            this.Icon = ModEntry.Assets.IconA;
            this.SkillsPageIcon = ModEntry.Assets.IconB;
            this.ExperienceBarColor = new Microsoft.Xna.Framework.Color(99, 107, 107);

            if (ModEntry.MargoLoaded)
            {
                this.ExperienceCurve = new[] { 100, 300, 770, 1300, 2150, 3300, 4000, 6900, 10000, 15000, 20000, 25000, 30000, 35000, 40000, 45000, 50000, 55000, 60000, 70000 };
                this.AddProfessions(
                    Recycler = new KeyedProfession(this, "Recycler", ModEntry.Assets.Recycler, ModEntry.Assets.RecyclerP, ModEntry.Instance.Helper),
                    Sneak = new KeyedProfession(this, "Sneak", ModEntry.Assets.Sneak, ModEntry.Assets.SneakP, ModEntry.Instance.Helper),
                    Environmentalist = new KeyedProfession(this, "Environmentalist", ModEntry.Assets.Environmentalist, ModEntry.Assets.EnvironmentalistP, ModEntry.Instance.Helper),
                    Salvager = new KeyedProfession(this, "Salvager", ModEntry.Assets.Salvager, ModEntry.Assets.SalvagerP, ModEntry.Instance.Helper),
                    Upseller = new KeyedProfession(this, "Upseller", ModEntry.Assets.Upseller, ModEntry.Assets.UpsellerP, ModEntry.Instance.Helper),
                    Reclaimer = new KeyedProfession(this, "Reclaimer", ModEntry.Assets.Reclaimer, ModEntry.Assets.ReclaimerP, ModEntry.Instance.Helper, ModEntry.Config)
                );
            }
            else
            {
                this.ExperienceCurve = new[] { 100, 300, 770, 1300, 2150, 3300, 4000, 6900, 10000, 15000 };
                this.AddProfessions(
                    Recycler = new KeyedProfession(this, "Recycler", ModEntry.Assets.Recycler, ModEntry.Instance.I18n),
                    Sneak = new KeyedProfession(this, "Sneak", ModEntry.Assets.Sneak, ModEntry.Instance.I18n),
                    Environmentalist = new KeyedProfession(this, "Environmentalist", ModEntry.Assets.Environmentalist, ModEntry.Instance.I18n),
                    Salvager = new KeyedProfession(this, "Salvager", ModEntry.Assets.Salvager, ModEntry.Instance.I18n),
                    Upseller = new KeyedProfession(this, "Upseller", ModEntry.Assets.Upseller, ModEntry.Instance.I18n),
                    Reclaimer = new KeyedProfession(this, "Reclaimer", ModEntry.Assets.Reclaimer, ModEntry.Instance.I18n, ModEntry.Config)
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
                ModEntry.Instance.I18n.Get("skill.perk", new { bonus = ModEntry.Config.PerLevelBonusDropChance })
            };
            if (level == 3 || level == 6 || level == 9)
            {
                string rarity = ModEntry.Instance.I18n.Get($"skill.perk_rarity_{level}");
                result.Add(ModEntry.Instance.I18n.Get("skill.perk_bonus", new { rarity }));
            }
            
            return result;
        }

        public override string GetSkillPageHoverText(int level)
        {
            return ModEntry.Instance.I18n.Get("skill.perk", new { bonus = level * ModEntry.Config.PerLevelBonusDropChance });
        }
    }
}
