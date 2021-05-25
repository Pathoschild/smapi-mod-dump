/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.Xna.Framework;
using SkillfulClothes.Effects;
using SkillfulClothes.Effects.Attributes;
using SkillfulClothes.Effects.Skills;
using SkillfulClothes.Effects.Special;
using SkillfulClothes.Types;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Objects.Clothing;
using HatDef = SkillfulClothes.Types.Hat;

namespace SkillfulClothes
{
    public class ItemDefinitions
    {        
        public static Dictionary<Shirt, ExtItemInfo> ShirtEffects = new Dictionary<Shirt, ExtItemInfo>() {
            { Shirt.MayoralSuspenders, ExtendItem.With.Description("Foo Foo").And.Effect(new IncreasePopularity()) },


            { Shirt.HeartShirt_Dyeable, ExtendItem.With.Effect(new IncreaseMaxHealth(15)) },

            { Shirt.CopperBreastplate, ExtendItem.With.Effect(new IncreaseDefense(1)) },
            { Shirt.SteelBreastplate, ExtendItem.With.Effect(new IncreaseDefense(2)) },
            { Shirt.GoldBreastplate, ExtendItem.With.Effect(new IncreaseDefense(3)) },
            { Shirt.IridiumBreastplate, ExtendItem.With.Effect(new IncreaseDefense(5)) },

            { Shirt.FakeMusclesShirt, ExtendItem.With.Effect(new IncreaseAttack(1)) },

            { Shirt.CavemanShirt, ExtendItem.With.Effect(new IncreaseAttack(2)) },

            { Shirt.FishingVest, ExtendItem.With.Description("A FISHING vesto").Effect(new IncreaseFishingBarByCaughtFish()) },
            { Shirt.FishShirt, ExtendItem.With.Effect(new MultiplyExperience(Skill.Fishing, 1.2f)) },
            { Shirt.ShirtOfTheSea, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Fishing, 1)) },
            { Shirt.SailorShirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Fishing, 1)) },
            { Shirt.SailorShirt_2, ExtendItem.With.Effect(new IncreaseFishingBarByCaughtFish()) },
            { Shirt.ShrimpEnthusiastShirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Fishing, 1)) },

            { Shirt.BridalShirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Luck, 1)) },

            { Shirt.TomatoShirt, ExtendItem.With.Effect(new MultiplyExperience(Skill.Farming, 1.2f)) },
            { Shirt.CrabCakeShirt, ExtendItem.With.Effect(new IncreaseSpeed(1), new IncreaseDefense(1)) },

            { Shirt.ArcaneShirt, ExtendItem.With.Effect(new HealthRegen()) },

            { Shirt.WhiteGi, ExtendItem.With.Effect(new IncreaseDefense(1)) },
            { Shirt.OrangeGi, ExtendItem.With.Effect(new IncreaseAttack(1)) },

            { Shirt.StuddedVest, ExtendItem.With.Effect(new IncreaseAttack(2)) },
            { Shirt.BlacksmithApron, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Combat, 1)) },

            { Shirt.IridiumEnergyShirt, ExtendItem.With.Effect(new StaminaRegen()) },
            { Shirt.HappyShirt, ExtendItem.With.Effect(new IncreasePopularity()) },

            { Shirt.BandanaShirt_ShieldFromHarm, ExtendItem.With.Effect(new IncreaseDefense(1)) },

            { Shirt.GreenThumbShirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Farming, 1)) },
            { Shirt.ExcavatorShirt, ExtendItem.With.Effect(new MultiplyExperience(Skill.Mining, 1.2f)) },

            { Shirt.YobaShirt, ExtendItem.With.Effect(new RingEffect(RingType.YobaRing)) },

            { Shirt.PrismaticShirt, ExtendItem.With.Effect(new IncreaseMaxHealth(25)) },
            { Shirt.PrismaticShirt_DarkSleeves, ExtendItem.With.Effect(new AutoRevive()) },
            { Shirt.PrismaticShirt_WhiteSleeves, ExtendItem.With.Effect(new IncreaseMaxEnergy(25)) },

            { Shirt.RangerUniform, ExtendItem.With.Effect(new MultiplyExperience(Skill.Foraging, 1.2f)) },

            { Shirt.GreenTunic, ExtendItem.With.Effect(new IncreaseAttack(1)) },
            { Shirt.LimeGreenTunic, ExtendItem.With.Effect(new IncreaseDefense(1)) },

            { Shirt.StarShirt, ExtendItem.With.Description("The star is glowing in a dim light").And.Effect(new GlowEffect(1f, new Color(120, 0, 0, 155))) },
            { Shirt.NightSkyShirt, ExtendItem.With.Effect( new OvernightStaminaBuff(30)) },
            { Shirt.GoodnightShirt, ExtendItem.With.Effect(new OvernightHealthBuff(25)) },

            {Shirt.SlimeShirt, ExtendItem.With.Effect(new RingEffect(RingType.SlimeCharmerRing)) }            
        };

        public static Dictionary<Pants, ExtItemInfo> PantsEffects = new Dictionary<Pants, ExtItemInfo>() {
            { Pants.FarmerPants, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Farming, 1)) },

            { Pants.DinosaurPants, ExtendItem.With.Effect(new IncreaseDefense(1)) },

            { Pants.PrismaticPants, ExtendItem.With.Effect(new IncreaseMaxHealth(10)) },
            { Pants.PrismaticGeniePants, ExtendItem.With.Effect(new IncreaseMaxEnergy(20)) },

            { Pants.GrassSkirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Foraging, 1)) }
        };

        public static Dictionary<HatDef, ExtItemInfo> HatEffects = new Dictionary<HatDef, ExtItemInfo>()
        {
            { HatDef.DinosaurHat, ExtendItem.With.Effect(new IncreaseDefense(1)) },
            { HatDef.WearableDwarfHelm, ExtendItem.With.Effect(new IncreaseDefense(2)) },
            { HatDef.PartyHat_Green, ExtendItem.With.Effect(new IncreasePopularity()) },
            { HatDef.PartyHat_Blue, ExtendItem.With.Effect(new IncreasePopularity()) },
            { HatDef.PartyHat_Red, ExtendItem.With.Effect(new IncreasePopularity()) },
            { HatDef.FishingHat, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Fishing, 1)) },
            { HatDef.BridalVeil, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Luck, 1)) },
            { HatDef.WitchHat, ExtendItem.With.Effect(new IncreaseImmunity(1)) },
            { HatDef.SwashbucklerHat, ExtendItem.With.Effect(new IncreaseAttack(1)) },
            { HatDef.Goggles, ExtendItem.With.Effect(new IncreaseDefense(1)) },
            { HatDef.ForagersHat, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Foraging, 1)) },
            { HatDef.WarriorHelmet, ExtendItem.With.Effect(new IncreaseAttack(2)) },

            { HatDef.StrawHat, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Farming, 1)) }
        };

        public static bool GetEffect(Item item, out IEffect effect)
        {
            ExtItemInfo extInfo = null;

            if (item is Clothing clothing)
            {
                if (clothing.clothesType.Value == (int)ClothesType.SHIRT)
                {
                    GetExtInfoByIndex<Shirt>(item.ParentSheetIndex, out extInfo);
                } else
                if (clothing.clothesType.Value == (int)ClothesType.PANTS)
                {
                    GetExtInfoByIndex<Pants>(item.ParentSheetIndex, out extInfo);
                }
            } else if (item is StardewValley.Objects.Hat hat)
            {
                GetExtInfoByIndex<HatDef>(hat.which.Value, out extInfo);
            }

            effect = extInfo?.Effect;

            return effect != null;
        }

        public static bool GetEffectByIndex<T>(int index, out IEffect effect)
        {
            if (GetExtInfoByIndex<T>(index, out ExtItemInfo extInfo))
            {
                effect = extInfo.Effect;                
            } else
            {
                effect = null;
            }
            
            return effect != null;
        }

        public static bool GetExtInfoByIndex<T>(int index, out ExtItemInfo extInfo)
        {
            if (typeof(T) == typeof(Shirt))
            {
                return ShirtEffects.TryGetValue((Shirt)index, out extInfo);
            } 
            
            if (typeof(T) == typeof(Pants))
            {
                return PantsEffects.TryGetValue((Pants)index, out extInfo);
            } 
            
            if (typeof(T) == typeof(HatDef))
            {
                return HatEffects.TryGetValue((HatDef)index, out extInfo);
            }

            extInfo = null;
            return false;
        }
    }
}
