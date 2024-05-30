/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using Microsoft.Xna.Framework;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;

namespace SkillPrestige.Yacs
{
    /// <summary>The mod entry class.</summary>
    internal class ModEntry : Mod, ISpaceCoreSkillMod
    {
        /// <summary>The cooking skill type.</summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable. - there isn't another solution that doesn't break everything.
        private SkillType SkillType;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>The unique ID for the Cooking Skill mod.</summary>
        private const string TargetModId = "moonslime.CookingSkill";
        public string SpaceCoreSkillId => "moonslime.Cooking";

        /// <summary>The name to display for the mod in the log.</summary>
        public string DisplayName { get; } = "Yet Another Cooking Skill";

        /// <summary>Whether the mod is found in SMAPI.</summary>
        public bool IsFound { get; private set; }

        /// <summary>The skills added by this mod.</summary>
        public IEnumerable<Skill> AdditionalSkills => this.GetAddedSkills();

        /// <summary>The prestiges added by this mod.</summary>
        public IEnumerable<Prestige> AdditionalPrestiges => this.GetAddedPrestiges();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.SkillType = new SkillType("Cooking", 6)
            {
                SpaceCoreSkillId = this.SpaceCoreSkillId
            };

            this.IsFound = helper.ModRegistry.IsLoaded(TargetModId);
            ModHandler.RegisterMod(this);
        }

        /// <summary>Get the skills added by this mod.</summary>
        private IEnumerable<Skill> GetAddedSkills()
        {
            if (!this.IsFound)
                yield break;

            yield return new Skill
            {
                Type = this.SkillType,
                SourceRectangleForSkillIcon = new Rectangle(0, 0, 16, 16),
                SkillIconTexture = Skills.GetSkillIcon(this.SpaceCoreSkillId),
                Professions = this.GetAddedProfessions(),
                GetSkillLevel = () => Skills.GetSkillLevel(Game1.player, this.SpaceCoreSkillId),
                SetSkillLevel = _ => { }, //is not set independently of the experience.
                GetSkillExperience = this.GetExperience,
                SetSkillExperience = this.SetExperience
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

        private IEnumerable<Profession> GetAddedProfessions()
        {
            var skill = Skills.GetSkill(this.SpaceCoreSkillId);
            IList<Profession> professions = new List<Profession>();
            IList<TierOneProfession> tierOne = new List<TierOneProfession>();
            foreach (var professionGroup in skill.ProfessionsForLevels)
                switch (professionGroup.Level)
                {
                    case 5:
                    {
                        var professionA = new SpaceCoreTierOneProfession(professionGroup.First);
                        var professionB = new SpaceCoreTierOneProfession(professionGroup.Second);
                        professions.Add(professionA);
                        professions.Add(professionB);
                        tierOne.Add(professionA);
                        tierOne.Add(professionB);
                        break;
                    }
                    case 10:
                    {
                        var requiredProfession = tierOne.First(p => p.DisplayName == professionGroup.Requires.GetName());

                        var professionA = new SpaceCoreTierTwoProfession(professionGroup.First)
                        {
                            TierOneProfession = requiredProfession,
                        };
                        var professionB = new SpaceCoreTierTwoProfession(professionGroup.Second)
                        {
                            TierOneProfession = requiredProfession,
                        };

                        professions.Add(professionA);
                        professions.Add(professionB);

                        requiredProfession.TierTwoProfessions = new[] { professionA, professionB };
                        break;
                    }
                }
            return professions;
        }

        /// <summary>Get the current skill XP.</summary>
        private int GetExperience()
        {
            return Skills.GetExperienceFor(Game1.player, this.SpaceCoreSkillId);
        }

        /// <summary>Set the current skill XP.</summary>
        /// <param name="amount">The amount to set.</param>
        private void SetExperience(int amount)
        {
            int addedExperience = amount - Game1.player.GetCustomSkillExperience(this.SpaceCoreSkillId);
            Game1.player.AddCustomSkillExperience(this.SpaceCoreSkillId, addedExperience);
        }
    }
}
