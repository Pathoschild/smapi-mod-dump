using System.Collections.Generic;
using System.Linq;
using CookingSkill;
using SkillPrestige;
using SkillPrestige.Menus;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using Microsoft.Xna.Framework;
using StardewValley;

namespace SkillPrestigeCookingAdapter
{
    /// <summary>
    /// The Cooking prestige skill mod.
    /// </summary>
    // ReSharper disable once UnusedMember.Global - invoked by another application through reflection.
    public class CookingSkillMod : SkillMod
    {
        private static readonly int[] ExpNeededForLevel =
        {
            100,
            380,
            770,
            1300,
            2150,
            3300,
            4800,
            6900,
            10000,
            15000
        };

        public override string DisplayName => "Cooking Skill";

        protected override string UniqueId => "spacechase0.CookingSkill";


        public override IEnumerable<Skill> AdditionalSkills => GetSkills();
        
        private IEnumerable<Skill> GetSkills()
        {
            {
                if (IsFound)
                {
                    yield return new Skill

                    {
                        Type = CookingSkillType,
                        SkillScreenPosition = SkillPrestigeCookingAdapterMod.IsLuckSkillModLoaded ? 7 : 6, // fix potential conflict with order due to luck skill mod
                        SourceRectangleForSkillIcon = new Rectangle(0, 0, 16, 16),
                        SkillIconTexture = SkillPrestigeCookingAdapterMod.IconTexture,
                        Professions = GetCookingProfessions(),
                        SetSkillLevel = x => { }, // no set necessary, as the level isn't stored independently from the experience
                        GetSkillLevel = GetCookingLevel,
                        SetSkillExperience = SetCookingExperience,
                        LevelUpManager = new LevelUpManager
                        {
                            MenuType = typeof(CookingLevelUpMenu),
                            GetLevel = () => (int)(Game1.activeClickableMenu as CookingLevelUpMenu).GetInstanceField("currentLevel"),
                            GetSkill = () => Skill.AllSkills.Single(x => x.Type == CookingSkillType),
                            CreateNewLevelUpMenu = (skill, level) => new LevelUpMenuDecorator<CookingLevelUpMenu>(skill, level, new CookingLevelUpMenu(level),
                                "professionsToChoose", "leftProfessionDescription", "rightProfessionDescription", CookingLevelUpMenu.getProfessionDescription)
                        }
                    };
                }
            }
        }

        public override IEnumerable<Prestige> AdditonalPrestiges => IsFound ? new List<Prestige>
        {
            new Prestige
            {
                SkillType = CookingSkillType
            }
        } : null;

        private static SkillType CookingSkillType => _cookingSkillType ?? (_cookingSkillType = new SkillType("Cooking", 6));

        private static SkillType _cookingSkillType;

        private static int GetCookingLevel()
        {
            FixExpLength();
            for (var index = ExpNeededForLevel.Length - 1; index >= 0; --index)
            {
                if (Game1.player.experiencePoints[6] >= ExpNeededForLevel[index])
                    return index + 1;
            }
            return 0;
        }

        private static void FixExpLength()
        {
            if (Game1.player.experiencePoints.Length >= 7) return;
            var newExperienceArray = new int[7];
            for (var index = 0; index < 6; ++index)
                newExperienceArray[index] = Game1.player.experiencePoints[index];
            Game1.player.experiencePoints = newExperienceArray;
        }



        private static void SetCookingExperience(int amount)
        {
            if (amount <= Game1.player.experiencePoints[6])
            {
                Game1.player.experiencePoints[6] = amount;
            }
            else
            {
                var addedExperience = amount - Game1.player.experiencePoints[6];
                Mod.addCookingExp(addedExperience);
            }
        }

        private static IEnumerable<Profession> GetCookingProfessions()
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
            return new List<Profession>
            {
                gourmet,
                satisfying,
                efficient,
                professionalChef,
                intenseFlavors,
                secretSpices
            };
        }

    }
}
