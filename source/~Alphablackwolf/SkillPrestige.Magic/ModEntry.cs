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
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Magic.Framework;
using SkillPrestige.Menus;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using SpaceCore;
using SpaceCore.Interface;
using StardewModdingAPI;
using StardewValley;
using IMagicApi = Magic.IApi;
using IManaBarApi = ManaBar.IApi;

namespace SkillPrestige.Magic
{
    /// <summary>The mod entry class.</summary>
    internal class ModEntry : Mod, ISkillMod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The cooking skill icon.</summary>
        private Texture2D IconTexture;

        /// <summary>The cooking skill type.</summary>
        private SkillType MagicSkillType;

        /// <summary>Whether the Cooking Skill mod is loaded.</summary>
        private bool IsCookingSkillModLoaded;

        /// <summary>Whether the Luck Skill mod is loaded.</summary>
        private bool IsLuckSkillModLoaded;

        /// <summary>The unique ID for the Magic skill registered with SpaceCore.</summary>
        private readonly string SpaceCoreSkillId = "spacechase0.Magic";

        /// <summary>The unique ID for the Magic mod.</summary>
        private readonly string MagicModId = "spacechase0.Magic";

        /// <summary>The unique ID for the Mana Bar mod.</summary>
        private readonly string ManaBarModId = "spacechase0.ManaBar";


        /*********
        ** Accessors
        *********/
        /// <summary>The name to display for the mod in the log.</summary>
        public string DisplayName { get; } = "Magic Skill";

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
            this.IconTexture = helper.ModContent.Load<Texture2D>("assets/icon.png");
            this.MagicSkillType = new SkillType("Magic", 8);
            this.IsFound = helper.ModRegistry.IsLoaded(this.MagicModId);
            this.IsCookingSkillModLoaded = helper.ModRegistry.IsLoaded("Alphablackwolf.CookingSkillPrestigeAdapter");
            this.IsLuckSkillModLoaded = helper.ModRegistry.IsLoaded("alphablackwolf.LuckSkillPrestigeAdapter");

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

            int skillPosition = 6;
            if (this.IsLuckSkillModLoaded)
                ++skillPosition;
            if (this.IsCookingSkillModLoaded)
                ++skillPosition;

            yield return new Skill
            {
                Type = this.MagicSkillType,
                SkillScreenPosition = skillPosition, // fix potential conflict with order due to luck skill mod and cooking skill mod
                SourceRectangleForSkillIcon = new Rectangle(0, 0, 16, 16),
                SkillIconTexture = this.IconTexture,
                Professions = this.GetAddedProfessions(),
                GetSkillLevel = this.GetLevel,
                SetSkillLevel = _ => { }, // no set necessary, as the level isn't stored independently of the experience
                GetSkillExperience = this.GetExperience,
                SetSkillExperience = this.SetExperience,
                OnPrestige = this.OnPrestige,
                LevelUpManager = new LevelUpManager
                {
                    IsMenu = menu => menu is SkillLevelUpMenu && this.Helper.Reflection.GetField<string>(menu, "currentSkill").GetValue() == this.SpaceCoreSkillId,
                    GetLevel = () => Game1.player.GetCustomSkillLevel(Skills.GetSkill(this.SpaceCoreSkillId)),
                    CreateNewLevelUpMenu = (skill, level) => new LevelUpMenuDecorator<SkillLevelUpMenu>(
                        skill: skill,
                        level: level,
                        internalMenu: new SkillLevelUpMenu(this.SpaceCoreSkillId, level),
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
                SkillType = this.MagicSkillType
            };
        }

        /// <summary>Get the professions added by this mod.</summary>
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
                        var professionA = new TierOneProfession
                        {
                            DisplayName = professionGroup.First.GetName(),
                            Id = professionGroup.First.GetVanillaId(),
                            EffectText = new[] { professionGroup.First.GetDescription() },
                        };
                        var professionB = new TierOneProfession
                        {
                            DisplayName = professionGroup.Second.GetName(),
                            Id = professionGroup.Second.GetVanillaId(),
                            EffectText = new[] { professionGroup.Second.GetDescription() },
                        };

                        professions.Add(professionA);
                        professions.Add(professionB);
                        tierOne.Add(professionA);
                        tierOne.Add(professionB);
                        break;
                    }
                    case 10:
                    {
                        var requiredProfession = tierOne.First(p => p.DisplayName == professionGroup.Requires.GetName());

                        var professionA = new TierTwoProfession
                        {
                            DisplayName = professionGroup.First.GetName(),
                            Id = professionGroup.First.GetVanillaId(),
                            EffectText = new[] { professionGroup.First.GetDescription() },
                            TierOneProfession = requiredProfession,
                        };
                        var professionB = new TierTwoProfession
                        {
                            DisplayName = professionGroup.Second.GetName(),
                            Id = professionGroup.Second.GetVanillaId(),
                            EffectText = new[] { professionGroup.Second.GetDescription() },
                            TierOneProfession = requiredProfession,
                        };

                        professions.Add(professionA);
                        professions.Add(professionB);

                        requiredProfession.TierTwoProfessions = new[] { professionA, professionB };
                        break;
                    }
                }

            foreach (var profession in professions)
                profession.SpecialHandling = profession.DisplayName switch
                {
                    "Mana Reserve" => new ManaCapSpecialHandling(amount: 500, addMaxMana: points =>
                    {
                        var api = this.GetManaBarApi();
                        int maxMana = api.GetMaxMana(Game1.player);
                        api.SetMaxMana(Game1.player, maxMana + points);
                    }),
                    "Potential" or "Prodigy" => new UpgradePointSpecialHandling(amount: 2,
                        useSpellPoints: points => this.GetMagicApi().UseSpellPoints(this.ModManifest, points)),
                    _ => profession.SpecialHandling
                };


            return professions;
        }

        /// <summary>Get the current skill level.</summary>
        private int GetLevel()
        {
            //this.FixExpLength();
            return Game1.player.GetCustomSkillLevel(this.SpaceCoreSkillId);
        }

        /// <summary>Get the current skill XP.</summary>
        private int GetExperience()
        {
            return Game1.player.GetCustomSkillExperience(this.SpaceCoreSkillId);
        }

        /// <summary>Set the current skill XP.</summary>
        /// <param name="amount">The amount to set.</param>
        private void SetExperience(int amount)
        {
            int addedExperience = amount - Game1.player.GetCustomSkillExperience(this.SpaceCoreSkillId);
            Game1.player.AddCustomSkillExperience(this.SpaceCoreSkillId, addedExperience);
        }

        /// <summary>Get the Magic mod's API.</summary>
        private IMagicApi GetMagicApi()
        {
            return
                this.Helper.ModRegistry.GetApi<IMagicApi>(this.MagicModId)
                ?? throw new InvalidOperationException("Can't load the API for the Magic mod.");
        }

        /// <summary>Get the Mana Bar mod's API.</summary>
        private IManaBarApi GetManaBarApi()
        {
            return
                this.Helper.ModRegistry.GetApi<IManaBarApi>(this.ManaBarModId)
                ?? throw new InvalidOperationException("Can't load the API for the Mana Bar mod.");
        }

        /// <summary>Reset the upgrade points of the player on prestige. Points from professions are handled in <see cref="UpgradePointSpecialHandling"/>.</summary>
        private void OnPrestige()
        {
            this.GetMagicApi().ResetProgress(this.ModManifest);
        }
    }
}
