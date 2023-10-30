/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tondorian/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Xml;
using LoveOfCooking;
using LoveOfCooking.Objects;
using Microsoft.Xna.Framework;
using SkillPrestige;
using SkillPrestige.Menus;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using SpaceCore;
using SpaceCore.Interface;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace LoveOfCookingPrestigeAdapter
{
    public class ModEntry : Mod, ISkillMod
    {
        public string DisplayName => "Cooking Skill";
        private int skillNumber;
        public bool IsFound => true;

        public IEnumerable<Skill> AdditionalSkills => GetAddedSkills();

        private IEnumerable<Prestige> GetPrestiges()
        {
            yield return new Prestige
            {
                SkillType = this.SkillType
            };
        }

        public IEnumerable<Prestige> AdditonalPrestiges => GetPrestiges();
        
        /// <summary>The cooking skill type.</summary>
        private SkillType SkillType;
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;            
        }

        private void SaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
         
               var skillList = SpaceCore.Skills.GetSkillList();

               for (int i = 0; i < skillList.Length; i++)
               {
                   if (skillList[i] == CookingSkill.InternalName)
                   {
                       skillNumber = i;
                       skillNumber += 6;
                       break;
                   }
               }
               if(Helper.ModRegistry.IsLoaded("spacechase0.LuckSkill"))
            {
                skillNumber++;
            }    
               SkillType = new SkillType("Cooking", 6);
               ModHandler.RegisterMod(this);
            Helper.Events
                .GameLoop.SaveLoaded -= this.SaveLoaded;
            Helper.Events.GameLoop.ReturnedToTitle += ResetMod;
 
        }

        private void ResetMod(object? sender, ReturnedToTitleEventArgs e)
        {
            Helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            Helper.Events.GameLoop.ReturnedToTitle -= ResetMod;
        }

        private IEnumerable<Skill> GetAddedSkills()
        {
           var skill =  SpaceCore.Skills.GetSkill(CookingSkill.InternalName);
            yield return new Skill()
            {
                Type = this.SkillType,

                SkillScreenPosition = skillNumber,
                SkillIconTexture = skill.Icon,
                SourceRectangleForSkillIcon = skill.Icon.Bounds,
                Professions = GetAddedProfessions(),
                GetSkillLevel = GetLevel,
                SetSkillLevel = level => { },
                GetSkillExperience = GetExperience,
                SetSkillExperience = SetExperience,
                LevelUpManager = new SkillPrestige.Menus.LevelUpManager
                {
                    IsMenu = IsMenu,
                    GetLevel = GetLevel,
                    CreateNewLevelUpMenu = (skill, level) => new LevelUpMenuDecorator<SkillLevelUpMenu>(skill, level,
                    new SkillLevelUpMenu(CookingSkill.InternalName, level),
                    professionsToChooseInternalName: "professionToChoose",
                    leftProfessionDescriptionInternalName: "leftProffessionDescription",
                    rightProfessionDescriptionInternalName: "rightProfessionDescription",
                    getProfessionDescription: SkillLevelUpMenu.getProfessionDescription)
                }
            };
        }

     private bool IsMenu(IClickableMenu menu )
        {
            return menu is SkillLevelUpMenu && this.Helper.Reflection.GetField<string>(menu, "currentSkill").GetValue() == CookingSkill.InternalName;
        }

        private IEnumerable<Profession> GetAddedProfessions()
        {
            var skill = SpaceCore.Skills.GetSkill(CookingSkill.InternalName);

            IList<Profession> profession = new List<Profession>();
            IList<TierOneProfession> tierOne = new List<TierOneProfession>();

            foreach(var professionGroup in skill.ProfessionsForLevels)
            {
                if(professionGroup.Level == 5)
                {
                    var professionA = new TierOneProfession
                    {
                        DisplayName = professionGroup.First.GetName(),
                        Texture =professionGroup.First.Icon,
                       
                        Id = professionGroup.First.GetVanillaId(),
                        EffectText = new[] { professionGroup.First.GetDescription() }
                    };
                    var professionB = new TierOneProfession
                    {
                        DisplayName = professionGroup.Second.GetName(),
                        Id = professionGroup.Second.GetVanillaId(),
                        Texture =professionGroup.First.Icon,
                        
                        EffectText = new[] { professionGroup.Second.GetDescription() }
                    };
                    profession.Add(professionA);
                    profession.Add(professionB);
                    tierOne.Add(professionA);
                    tierOne.Add(professionB);
                }
                else if(professionGroup.Level == 10)
                {
                    TierOneProfession tierOneProfession;
                    if(professionGroup.First.Id.Contains("path1"))
                    {
                        tierOneProfession = tierOne[0];
                    }
                    else
                    {
                        tierOneProfession = tierOne[1];
                    }
                    var professionA = new TierTwoProfession
                    {
                        DisplayName = professionGroup.First.GetName(),
                        Id = professionGroup.First.GetVanillaId(),
                        EffectText = new[] { professionGroup.First.GetDescription() },
                        Texture =professionGroup.First.Icon,
                        TierOneProfession = tierOneProfession,
                    };
                    var professionB = new TierTwoProfession
                    {
                        DisplayName = professionGroup.Second.GetName(),
                        Id = professionGroup.Second.GetVanillaId(),
                        EffectText = new[] { professionGroup.Second.GetDescription() },
                        Texture =professionGroup.First.Icon,
                        TierOneProfession = tierOneProfession,
                    };

                    profession.Add(professionA);
                    profession.Add(professionB);

                    //requiredProfession.TierTwoProfessions = new[] { professionA, professionB };
                }
            }    


            return profession;
        }

        private int GetLevel()
        {
            return Game1.player.GetCustomSkillLevel(CookingSkill.InternalName);
        }

        /// <summary>Get the current skill XP.</summary>
        private int GetExperience()
        {
            return Game1.player.GetCustomSkillExperience(CookingSkill.InternalName);
        }

        /// <summary>Set the current skill XP.</summary>
        /// <param name="amount">The amount to set.</param>
        private void SetExperience(int amount)
        {
            int addedExperience = amount - Game1.player.GetCustomSkillExperience(CookingSkill.InternalName);
            Game1.player.AddCustomSkillExperience(CookingSkill.InternalName, addedExperience);
        }
    }
}
