using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SkillPrestige.Menus;
using SkillPrestige.Professions;
using StardewValley;
using OriginalMod = LuckSkill;
namespace SkillPrestige.Mods.MyLuckSkill
{
    /// <summary>
    /// The Luck Skill Mod's representation in SkillPrestige.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global - referenced through reflection.
    public class LuckSkillMod : SkillMod
    {
        public override string DisplayName => "Luck Skill";
        protected override string UniqueId => "spacechase0.LuckSkill";

        public override IEnumerable<Skill> AdditionalSkills => IsFound
            ? new List<Skill>
            {
                new Skill
                {
                    Type = SkillTypes.SkillType.Luck,
                    SkillScreenPosition = 6,
                    SourceRectangleForSkillIcon = new Rectangle(64, 0, 16, 16),
                    Professions = Profession.LuckProfessions,
                    SetSkillLevel = x => Game1.player.luckLevel = x,
                    GetSkillLevel = () => Game1.player.luckLevel,
                    SetSkillExperience = SetLuckExperience,
                    LevelUpManager = new LevelUpManager
                    {
                        MenuType = typeof(OriginalMod.LuckLevelUpMenu),
                        GetLevel = () => (int)(Game1.activeClickableMenu as OriginalMod.LuckLevelUpMenu).GetInstanceField("currentLevel"),
                        GetSkill = () => Skill.AllSkills.Single(x => x.Type == SkillTypes.SkillType.Luck),
                        CreateNewLevelUpMenu = (skill, level) => new LevelUpMenuDecorator<OriginalMod.LuckLevelUpMenu>(skill, level, new OriginalMod.LuckLevelUpMenu(skill.Type.Ordinal, level),
                            "professionsToChoose", "leftProfessionDescription", "rightProfessionDescription", OriginalMod.LuckLevelUpMenu.getProfessionDescription)
                    }
                }
            }
            : null;

        public override IEnumerable<Prestige> AdditonalPrestiges => IsFound ? new List<Prestige>
        {
            new Prestige
            {
                SkillType = SkillTypes.SkillType.Luck
            }
        } : null;

        private static void SetLuckExperience(int amount)
        {
            if (amount <= Game1.player.experiencePoints[SkillTypes.SkillType.Luck.Ordinal])
            {
                Game1.player.experiencePoints[SkillTypes.SkillType.Luck.Ordinal] = amount;
            }
            else
            {
                var addedExperience = amount - Game1.player.experiencePoints[SkillTypes.SkillType.Luck.Ordinal];
                OriginalMod.Mod.gainLuckExp(addedExperience);
            }
        }
    }
}
