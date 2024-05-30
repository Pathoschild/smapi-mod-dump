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
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Menus;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using SpaceCore;
using SpaceCore.Interface;
using StardewModdingAPI;
using StardewValley;

namespace SkillPrestige.CookingSkill
{
    /// <summary>The mod entry class.</summary>
    // ReSharper disable once UnusedType.Global
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    internal class ModEntry : Mod, ISkillMod
    {
        /// <summary>The cooking skill icon.</summary>
        private Texture2D IconTexture;

        /// <summary>The cooking skill type.</summary>
        private SkillType SkillType;

        /// <summary>Whether the Luck Skill mod is loaded.</summary>
        private bool IsLuckSkillModLoaded;

        /// <summary>The unique ID for the Cooking skill registered with SpaceCore.</summary>
        private const string SpaceCoreSkillId = "spacechase0.Cooking";

        /// <summary>The unique ID for the Cooking Skill mod.</summary>
        private const string TargetModId = "spacechase0.CookingSkill";

        /// <summary>The name to display for the mod in the log.</summary>
        public string DisplayName => "Cooking Skill";

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
            this.IconTexture = this.Helper.ModContent.Load<Texture2D>("assets/icon.png");
            this.SkillType = new SkillType("Cooking", 6);
            this.IsFound = helper.ModRegistry.IsLoaded(TargetModId);
            this.IsLuckSkillModLoaded = helper.ModRegistry.IsLoaded("alphablackwolf.LuckSkillPrestigeAdapter");

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
                SkillIconTexture = this.IconTexture,
                Professions = this.GetAddedProfessions(),
                GetSkillLevel = this.GetLevel,
                SetSkillLevel = _ => { }, // no set necessary, as the level isn't stored independently of the experience
                GetSkillExperience = this.GetExperience,
                SetSkillExperience = this.SetExperience,
                LevelUpManager = new LevelUpManager
                {
                    IsMenu = menu => menu is SkillLevelUpMenu && this.Helper.Reflection.GetField<string>(menu, "currentSkill").GetValue() == SpaceCoreSkillId,
                    GetLevel = () => Game1.player.GetCustomSkillLevel(Skills.GetSkill(SpaceCoreSkillId)),
                    CreateNewLevelUpMenu = (skill, level) => new LevelUpMenuDecorator<SkillLevelUpMenu>(
                        skill: skill,
                        level: level,
                        internalMenu: new SkillLevelUpMenu(SpaceCoreSkillId, level),
                        professionsToChooseInternalName: "professionsToChoose",
                        leftProfessionDescriptionInternalName: "leftProfessionDescription",
                        rightProfessionDescriptionInternalName: "rightProfessionDescription",
                        getProfessionDescription: SkillLevelUpMenu.getProfessionDescription
                    )
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
                SkillType = this.SkillType
            };
        }

        /// <summary>Get the professions added by this mod.</summary>
        private IEnumerable<Profession> GetAddedProfessions()
        {
            var gourmet = new TierOneProfession
            {
                Id = 50,
                DisplayName = "Gourmet",
                EffectText = new[] { "+20% sell price" }
            };
            var satisfying = new TierOneProfession
            {
                Id = 51,
                DisplayName = "Satisfying",
                EffectText = new[] { "+25% buff duration once eaten" }
            };
            var efficient = new TierTwoProfession
            {
                Id = 52,
                DisplayName = "Efficient",
                EffectText = new[] { "15% chance to not consume ingredients" },
                TierOneProfession = gourmet
            };
            var professionalChef = new TierTwoProfession
            {
                Id = 53,
                DisplayName = "Prof. Chef",
                EffectText = new[] { "Home-cooked meals are always at least silver" },
                TierOneProfession = gourmet
            };
            var intenseFlavors = new TierTwoProfession
            {
                Id = 54,
                DisplayName = "Intense Flavors",
                EffectText = new[]
                {
                    "Food buffs are one level stronger",
                    "(+20% for max energy or magnetism)"
                },
                TierOneProfession = satisfying
            };
            var secretSpices = new TierTwoProfession
            {
                Id = 55,
                DisplayName = "Secret Spices",
                EffectText = new[] { "Provides a few random buffs when eating unbuffed food." },
                TierOneProfession = satisfying
            };
            gourmet.TierTwoProfessions = new List<TierTwoProfession>
            {
                efficient,
                professionalChef
            };
            satisfying.TierTwoProfessions = new List<TierTwoProfession>
            {
                intenseFlavors,
                secretSpices
            };
            return new Profession[]
            {
                gourmet,
                satisfying,
                efficient,
                professionalChef,
                intenseFlavors,
                secretSpices
            };
        }

        /// <summary>Get the current skill level.</summary>
        private int GetLevel()
        {
            //this.FixExpLength();
            return Game1.player.GetCustomSkillLevel(SpaceCoreSkillId);
        }

        /// <summary>Get the current skill XP.</summary>
        private int GetExperience()
        {
            return Game1.player.GetCustomSkillExperience(SpaceCoreSkillId);
        }

        /// <summary>Set the current skill XP.</summary>
        /// <param name="amount">The amount to set.</param>
        private void SetExperience(int amount)
        {
            int addedExperience = amount - Game1.player.GetCustomSkillExperience(SpaceCoreSkillId);
            Game1.player.AddCustomSkillExperience(SpaceCoreSkillId, addedExperience);
        }
    }
}
