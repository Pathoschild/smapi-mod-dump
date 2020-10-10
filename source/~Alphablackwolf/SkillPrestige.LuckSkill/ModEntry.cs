/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SkillPrestige.LuckSkill.Framework;
using SkillPrestige.Menus;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using StardewModdingAPI;
using StardewValley;
using OriginalMod = LuckSkill;

namespace SkillPrestige.LuckSkill
{
    /// <summary>The mod entry class.</summary>
    public class ModEntry : Mod, ISkillMod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The luck skill type.</summary>
        private SkillType LuckSkillType;


        /*********
        ** Accessors
        *********/
        /// <summary>The name to display for the mod in the log.</summary>
        public string DisplayName { get; } = "Luck Skill";

        /// <summary>Whether the mod is found in SMAPI.</summary>
        public bool IsFound { get; private set; }

        /// <summary>The skills added by this mod.</summary>
        public IEnumerable<Skill> AdditionalSkills => this.GetAddedSkills();

        /// <summary>The prestiges added by this mod.</summary>
        public IEnumerable<Prestige> AdditonalPrestiges => this.GetAddedPrestiges();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.LuckSkillType = new SkillType("Luck", 5);
            this.IsFound = helper.ModRegistry.IsLoaded("spacechase0.LuckSkill");

            ModHandler.RegisterMod(this);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the skills added by this mod.</summary>
        private IEnumerable<Skill> GetAddedSkills()
        {
            if (!this.IsFound)
                yield break;

            yield return new Skill
            {
                Type = this.LuckSkillType,
                SkillScreenPosition = 6,
                SourceRectangleForSkillIcon = new Rectangle(64, 0, 16, 16),
                Professions = this.GetAddedProfessions(),
                SetSkillLevel = level => Game1.player.luckLevel.Value = level,
                GetSkillLevel = () => Game1.player.luckLevel.Value,
                SetSkillExperience = this.SetLuckExperience,
                LevelUpManager = new LevelUpManager
                {
                    IsMenu = menu => menu is OriginalMod.LuckLevelUpMenu,
                    GetLevel = () => (int)(Game1.activeClickableMenu as OriginalMod.LuckLevelUpMenu).GetInstanceField("currentLevel"),
                    GetSkill = () => Skill.AllSkills.Single(x => x.Type == this.LuckSkillType),
                    CreateNewLevelUpMenu = (skill, level) => new LevelUpMenuDecorator<OriginalMod.LuckLevelUpMenu>(skill, level, new OriginalMod.LuckLevelUpMenu(skill.Type.Ordinal, level),
                        "professionsToChoose", "leftProfessionDescription", "rightProfessionDescription", OriginalMod.LuckLevelUpMenu.getProfessionDescription)
                }
            };
        }

        /// <summary>Get the prestiges added by this mod.</summary>
        private IEnumerable<Prestige> GetAddedPrestiges()
        {
            if (!this.IsFound)
                yield break;

            yield return new Prestige
            {
                SkillType = this.LuckSkillType
            };
        }

        /// <summary>Get the professions added by this mod.</summary>
        private IEnumerable<Profession> GetAddedProfessions()
        {
            var lucky = new TierOneProfession
            {
                Id = 30,
                DisplayName = "Lucky",
                EffectText = new[] { "Better daily luck." }
            };
            var quester = new TierOneProfession
            {
                Id = 31,
                DisplayName = "Quester",
                EffectText = new[] { "Quests are more likely to appear each day." },
            };
            var specialCharm = new TierTwoProfession
            {
                Id = 32,
                DisplayName = "Special Charm",
                EffectText = new[] { "Great daily luck most of the time." },
                SpecialHandling = new SpecialCharmSpecialHandling(),
                TierOneProfession = lucky
            };
            var luckA2 = new TierTwoProfession
            {
                Id = 33,
                DisplayName = "Luck A2",
                EffectText = new[] { "Does...nothing, yet." },
                TierOneProfession = lucky
            };
            var nightOwl = new TierTwoProfession
            {
                Id = 34,
                DisplayName = "Night Owl",
                EffectText = new[] { "Nightly events occur twice as often." },
                TierOneProfession = quester
            };
            var luckB2 = new TierTwoProfession
            {
                Id = 35,
                DisplayName = "Luck B2",
                EffectText = new[] { "Does...nothing, yet." },
                TierOneProfession = quester
            };
            quester.TierTwoProfessions = new List<TierTwoProfession>
            {
                nightOwl,
                luckB2
            };
            lucky.TierTwoProfessions = new List<TierTwoProfession>
            {
                specialCharm,
                luckA2
            };

            return new Profession[]
            {
                lucky,
                quester,
                specialCharm,
                luckA2,
                nightOwl,
                luckB2
            };
        }

        /// <summary>Set the current luck skill XP.</summary>
        /// <param name="amount">The amount to set.</param>
        private void SetLuckExperience(int amount)
        {
            int skillId = this.LuckSkillType.Ordinal;
            if (amount <= Game1.player.experiencePoints[skillId])
                Game1.player.experiencePoints[skillId] = amount;
            else
            {
                var addedExperience = amount - Game1.player.experiencePoints[skillId];
                OriginalMod.Mod.gainLuckExp(addedExperience);
            }
        }
    }
}
