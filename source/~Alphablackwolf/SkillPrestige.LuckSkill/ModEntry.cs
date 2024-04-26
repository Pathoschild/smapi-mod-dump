/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Collections.Generic;
using LuckSkill;
using Microsoft.Xna.Framework;
using SkillPrestige.LuckSkill.Framework;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using StardewModdingAPI;
using StardewValley;

namespace SkillPrestige.LuckSkill
{
    /// <summary>The mod entry class.</summary>
    internal class ModEntry : Mod, ISkillMod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The luck skill type.</summary>
        private SkillType SkillType;

        /// <summary>The unique ID for the Luck Skill mod.</summary>
        private const string TargetModId = "spacechase0.LuckSkill";


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
        public IEnumerable<Prestige> AdditionalPrestiges => this.GetAddedPrestiges();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.SkillType = new SkillType("Luck", 5);
            this.IsFound = helper.ModRegistry.IsLoaded(TargetModId);

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
                Type = this.SkillType,
                SkillScreenPosition = 6,
                SourceRectangleForSkillIcon = new Rectangle(64, 0, 16, 16),
                Professions = this.GetAddedProfessions(),
                GetSkillLevel = () => Game1.player.luckLevel.Value,
                SetSkillLevel = level => Game1.player.luckLevel.Value = level
            };
        }

        /// <summary>Get the prestiges added by this mod.</summary>
        private IEnumerable<Prestige> GetAddedPrestiges()
        {
            if (!this.IsFound)
                yield break;

            yield return new Prestige
            {
                SkillType = this.SkillType
            };
        }

        /// <summary>Get the professions added by this mod.</summary>
        private IEnumerable<Profession> GetAddedProfessions()
        {
            var api = this.GetLuckSkillApi();
            var professions = api.GetProfessions();

            // ReSharper disable once MoveLocalFunctionAfterJumpStatement
            TProfession Create<TProfession>(int id, IProfessionSpecialHandling specialHandling = null) where TProfession : Profession, new()
            {
                var profession = professions[id];
                return new TProfession
                {
                    Id = id,
                    DisplayName = profession.Name,
                    EffectText = new[] { profession.Description },
                    SpecialHandling = specialHandling
                };
            }

            var fortunate = Create<TierOneProfession>(api.FortunateProfessionId);
            var popularHelper = Create<TierOneProfession>(api.PopularHelperProfessionId);
            var lucky = Create<TierTwoProfession>(api.LuckyProfessionId, new SpecialCharmSpecialHandling());
            var unUnlucky = Create<TierTwoProfession>(api.UnUnluckyProfessionId);
            var shootingStar = Create<TierTwoProfession>(api.ShootingStarProfessionId);
            var spiritChild = Create<TierTwoProfession>(api.SpiritChildProfessionId);

            lucky.TierOneProfession = fortunate;
            unUnlucky.TierOneProfession = fortunate;
            shootingStar.TierOneProfession = popularHelper;
            spiritChild.TierOneProfession = popularHelper;

            fortunate.TierTwoProfessions = new List<TierTwoProfession>
            {
                lucky,
                unUnlucky
            };
            popularHelper.TierTwoProfessions = new List<TierTwoProfession>
            {
                shootingStar,
                spiritChild
            };

            return new Profession[]
            {
                fortunate,
                popularHelper,
                lucky,
                unUnlucky,
                shootingStar,
                spiritChild
            };
        }

        /// <summary>Get the Luck Skill mod's API.</summary>
        private ILuckSkillApi GetLuckSkillApi()
        {
            return
                this.Helper.ModRegistry.GetApi<ILuckSkillApi>(TargetModId)
                ?? throw new InvalidOperationException("Can't load the API for the Luck Skill mod.");
        }
    }
}
