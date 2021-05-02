/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

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

namespace SkillfulClothes.Effects
{
    public class PredefinedEffects
    {
        public static Dictionary<Shirt, IEffect> ShirtEffects = new Dictionary<Shirt, IEffect>() {
            { Shirt.MayoralSuspenders, new IncreasePopularity() }, // debug

            { Shirt.HeartShirt_Dyeable, new IncreaseMaxHealth(15) },

            { Shirt.CopperBreastplate, new IncreaseDefense(1) },
            { Shirt.SteelBreastplate, new IncreaseDefense(2) },
            { Shirt.GoldBreastplate, new IncreaseDefense(3) },
            { Shirt.IridiumBreastplate, new IncreaseDefense(5) },

            { Shirt.FakeMusclesShirt, new IncreaseAttack(1) },

            { Shirt.CavemanShirt, new IncreaseAttack(2) },

            { Shirt.FishingVest, new IncreaseFishingBarByCaughtFish() },
            { Shirt.FishShirt, new MultiplyExperience(Skill.Fishing, 1.2f) },
            { Shirt.ShirtOfTheSea, new IncreaseSkillLevel(Skill.Fishing, 1) },
            { Shirt.SailorShirt, new IncreaseSkillLevel(Skill.Fishing, 1) },
            { Shirt.SailorShirt_2, new IncreaseFishingBarByCaughtFish() },
            { Shirt.ShrimpEnthusiastShirt, new IncreaseSkillLevel(Skill.Fishing, 1) },

            { Shirt.BridalShirt, new IncreaseSkillLevel(Skill.Luck, 1) },

            { Shirt.TomatoShirt, new MultiplyExperience(Skill.Farming, 1.2f) },
            { Shirt.CrabCakeShirt, EffectSet.Of(new IncreaseSpeed(1), new IncreaseDefense(1)) },

            { Shirt.ArcaneShirt, new HealthRegen() },

            { Shirt.WhiteGi, new IncreaseDefense(1) },
            { Shirt.OrangeGi, new IncreaseAttack(1) },

            { Shirt.StuddedVest, new IncreaseAttack(2) },
            { Shirt.BlacksmithApron, new IncreaseSkillLevel(Skill.Combat, 1) },

            { Shirt.IridiumEnergyShirt, new StaminaRegen() },
            { Shirt.HappyShirt, new IncreasePopularity() },

            { Shirt.BandanaShirt_ShieldFromHarm, new IncreaseDefense(1) },

            { Shirt.GreenThumbShirt, new IncreaseSkillLevel(Skill.Farming, 1) },
            { Shirt.ExcavatorShirt, new MultiplyExperience(Skill.Mining, 1.2f) },

            { Shirt.YobaShirt, new IncreaseDefense(1) },

            { Shirt.PrismaticShirt, new IncreaseMaxHealth(25) },
            { Shirt.PrismaticShirt_DarkSleeves, new AutoRevive() },
            { Shirt.PrismaticShirt_WhiteSleeves, new IncreaseMaxEnergy(25) },

            { Shirt.RangerUniform, new MultiplyExperience(Skill.Foraging, 1.2f) },

            { Shirt.GreenTunic, new IncreaseAttack(1) },
            { Shirt.LimeGreenTunic, new IncreaseDefense(1) },
        };

        public static Dictionary<Pants, IEffect> PantsEffects = new Dictionary<Pants, IEffect>() {
            { Pants.FarmerPants, new IncreaseSkillLevel(Skill.Farming, 1) },
            
            { Pants.DinosaurPants, new IncreaseDefense(1) },

            { Pants.PrismaticPants, new IncreaseMaxHealth(10) },
            { Pants.PrismaticGeniePants, new IncreaseMaxEnergy(20) },

            { Pants.GrassSkirt, new IncreaseSkillLevel(Skill.Foraging, 1) }
        };

        public static Dictionary<Types.Hat, IEffect> HatEffects = new Dictionary<Types.Hat, IEffect>()
        {            
            { Types.Hat.DinosaurHat, new IncreaseDefense(1) },
            { Types.Hat.WearableDwarfHelm, new IncreaseDefense(2) },
            { Types.Hat.PartyHat_Green, new IncreasePopularity() },
            { Types.Hat.PartyHat_Blue, new IncreasePopularity() },
            { Types.Hat.PartyHat_Red, new IncreasePopularity() },
            { Types.Hat.FishingHat, new IncreaseSkillLevel(Skill.Fishing, 1) },
            { Types.Hat.BridalVeil, new IncreaseSkillLevel(Skill.Luck, 1) },
            { Types.Hat.WitchHat, new IncreaseImmunity(1) },
            { Types.Hat.SwashbucklerHat, new IncreaseAttack(1) },
            { Types.Hat.Goggles, new IncreaseDefense(1) },
            { Types.Hat.ForagersHat, new IncreaseSkillLevel(Skill.Foraging, 1) },
            { Types.Hat.WarriorHelmet, new IncreaseAttack(2) },

            { Types.Hat.StrawHat, new IncreaseSkillLevel(Skill.Farming, 1) }
        };

        public static bool GetEffect(Item item, out IEffect effect)
        {
            if (item is Clothing clothing)
            {
                if (clothing.clothesType.Value == (int)ClothesType.SHIRT)
                {
                    return ShirtEffects.TryGetValue((Shirt)item.ParentSheetIndex, out effect);
                }

                if (clothing.clothesType.Value == (int)ClothesType.PANTS)
                {
                    return PantsEffects.TryGetValue((Pants)item.ParentSheetIndex, out effect);
                }

                effect = null;
                return false;
            }

            if (item is StardewValley.Objects.Hat hat)
            {
                return HatEffects.TryGetValue((Types.Hat)hat.which.Value, out effect);
            }            

            effect = null;
            return false;
        }
    }
}
