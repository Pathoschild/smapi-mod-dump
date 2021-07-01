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
            { Shirt.MayoralSuspenders, ExtendItem.With.And.Effect(new IncreasePopularity()) },

            { Shirt.HeartShirt_Dyeable, ExtendItem.With.Effect(new IncreaseMaxHealth(15)) },

            { Shirt.CopperBreastplate, ExtendItem.With.Description("Light armor made from copper").Effect(new IncreaseDefense(1)).SoldBy(Shop.AdventureGuild, 2000, SellingCondition.SkillLevel_2).And.CannotBeCrafted },
            { Shirt.SteelBreastplate, ExtendItem.With.Description("Light armor made from steel").Effect(new IncreaseDefense(2)).SoldBy(Shop.AdventureGuild, 9000, SellingCondition.SkillLevel_4).And.CannotBeCrafted },
            { Shirt.GoldBreastplate, ExtendItem.With.Description("Medium armor made from solid gold").Effect(new IncreaseDefense(3)).SoldBy(Shop.AdventureGuild, 18000, SellingCondition.SkillLevel_6).And.CannotBeCrafted },
            { Shirt.IridiumBreastplate, ExtendItem.With.Description("Heavy armor made from pure iridium").Effect(new IncreaseDefense(5)).SoldBy(Shop.AdventureGuild, 30000, SellingCondition.SkillLevel_10).And.CannotBeCrafted },

            { Shirt.FakeMusclesShirt, ExtendItem.With.Description("You could hit the gym, or you could just wear this instead. Strangely, you even feel a bit stronger wearing it").Effect(new IncreaseAttack(1)) },

            { Shirt.CavemanShirt, ExtendItem.With.Effect(new IncreaseAttack(2)) },

            { Shirt.FishingVest, ExtendItem.With.Description("Helps you feel really professional while casting your fishing rod").Effect(new IncreaseFishingBarByCaughtFish()).SoldBy(Shop.Willy, 46000, SellingCondition.FriendshipHearts_6).And.CannotBeCrafted },
            { Shirt.FishShirt, ExtendItem.With.Description("To catch fish, you must think like a fish").Effect(new MultiplyExperience(Skill.Fishing, 1.2f)).SoldBy(Shop.Willy, 8000, SellingCondition.SkillLevel_2).And.CannotBeCrafted },
            { Shirt.ShirtOfTheSea, ExtendItem.With.Description("It smells like the brine of the sea and helps you focus while fishing").And.Effect(new IncreaseSkillLevel(Skill.Fishing, 1)).SoldBy(Shop.Willy, 6500, SellingCondition.SkillLevel_4).And.CannotBeCrafted },
            { Shirt.SailorShirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Fishing, 1)).SoldBy(Shop.Willy, 6000, SellingCondition.FriendshipHearts_4).And.CannotBeCrafted },
            { Shirt.SailorShirt_2, ExtendItem.With.Effect(new IncreaseFishingBarByCaughtFish()).SoldBy(Shop.Willy, 22000, SellingCondition.SkillLevel_10).And.CannotBeCrafted },
            { Shirt.ShrimpEnthusiastShirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Fishing, 1)).SoldBy(Shop.Willy, 4500, SellingCondition.SkillLevel_6).And.CannotBeCrafted },

            { Shirt.OceanShirt, ExtendItem.With.Description("Show your love for the ocean").Effect(new ShopDiscount(Shop.Willy, 0.05)) },

            { Shirt.CaptainsUniform, ExtendItem.With.Effect(new IncreaseFishingTreasureChestChance()) },

            { Shirt.BridalShirt, ExtendItem.With.Description("A beautiful white top, perfect for a bride. You feel lucky wearing this.").Effect(new IncreaseSkillLevel(Skill.Luck, 1)) },

            { Shirt.TomatoShirt, ExtendItem.With.Description("The best farmers become one with their produce").And.Effect(new MultiplyExperience(Skill.Farming, 1.2f)) },
            { Shirt.CrabCakeShirt, ExtendItem.With.Description("Reminds you of your favorite dish").Effect(new IncreaseSpeed(1), new IncreaseDefense(1)) },

            { Shirt.ArcaneShirt, ExtendItem.With.Description("This garment seems to affect your body in mysterious ways.").And.Effect(new HealthRegen()) },

            { Shirt.WhiteGi, ExtendItem.With.Description("\"He who attacks must vanquish. He who defends must merely survive\"").Effect(new IncreaseDefense(2)) },
            { Shirt.OrangeGi, ExtendItem.With.Description("The best defense is a good offense").Effect(new IncreaseAttack(1)) },

            { Shirt.StuddedVest, ExtendItem.With.Description("A black vest studded with metal spikes. Monsters dread the very sight of it").And.Effect(new IncreaseAttack(2)) },
            { Shirt.BlacksmithApron, ExtendItem.With.Description("Shows your admiration of craftsmanship").And.Effect(new ShopDiscount(Shop.Clint, 0.05)) },

            { Shirt.IridiumEnergyShirt, ExtendItem.With.Description("Wearing this you can feel the iridium’s energy flowing through you.").And.Effect(new StaminaRegen()).SoldBy(Shop.Krobus, 120000).And.CannotBeCrafted },
            { Shirt.HappyShirt, ExtendItem.With.Effect(new IncreasePopularity()) },

            { Shirt.BandanaShirt_ShieldFromHarm, ExtendItem.With.Effect(new IncreaseDefense(1)) },

            { Shirt.GreenThumbShirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Farming, 1)).SoldBy(Shop.Pierre, 6000, SellingCondition.FriendshipHearts_4).CannotBeCrafted },
            { Shirt.ExcavatorShirt, ExtendItem.With.Effect(new MultiplyExperience(Skill.Mining, 1.2f)) },

            { Shirt.YobaShirt, ExtendItem.With.Effect(new RingEffect(RingType.YobaRing)) },

            { Shirt.PrismaticShirt, ExtendItem.With.Description("The shimmering prismatic energy vitalizes the wearer’s body").And.Effect(new IncreaseMaxHealth(25)) },
            { Shirt.PrismaticShirt_DarkSleeves, ExtendItem.With.Description("It’s shimmering prismatic energy makes you feel safe").Effect(new AutoRevive()) },
            { Shirt.PrismaticShirt_WhiteSleeves, ExtendItem.With.Description("The shimmering prismatic energy strengthens the wearer’s body").Effect(new IncreaseMaxEnergy(25)) },

            { Shirt.RangerUniform, ExtendItem.With.Description("Now you look like a professional while chopping down trees").And.Effect(new MultiplyExperience(Skill.Foraging, 1.2f)) },

            { Shirt.GreenTunic, ExtendItem.With.Effect(new ShopDiscount(Shop.AdventureGuild, 0.05)) },
            { Shirt.LimeGreenTunic, ExtendItem.With.Description("A padded, bright green tunic with a belt").Effect(new IncreaseDefense(1)) },

            { Shirt.StarShirt, ExtendItem.With.Description("The star is glowing in a dim light").And.Effect(new GlowEffect(0.65f, new Color(200, 0, 0))) },
            { Shirt.NightSkyShirt, ExtendItem.With.Effect( new OvernightStaminaBuff(30)) },
            { Shirt.GoodnightShirt, ExtendItem.With.Effect(new OvernightHealthBuff(25)) },

            { Shirt.SlimeShirt, ExtendItem.With.Description("Identifies you as a friend of slimes").And.Effect(new RingEffect(RingType.SlimeCharmerRing)).SoldBy(Shop.AdventureGuild, 21000, SellingCondition.SkillLevel_8).And.CannotBeCrafted }
        };

        public static Dictionary<Pants, ExtItemInfo> PantsEffects = new Dictionary<Pants, ExtItemInfo>() {
            { Pants.FarmerPants, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Farming, 1)) },

            { Pants.DinosaurPants, ExtendItem.With.Description("These pants are reinforced with real dinosaur scales").And.Effect(new IncreaseDefense(1)) },

            { Pants.GeniePants, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Luck, 1)) },

            { Pants.PrismaticPants, ExtendItem.With.Effect(new IncreaseMaxHealth(10)) },
            { Pants.PrismaticGeniePants, ExtendItem.With.Effect(new IncreaseMaxEnergy(20)) },

            { Pants.GrassSkirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Foraging, 1)).SoldBy(Shop.Marnie, 6000, SellingCondition.SkillLevel_4) },

            { Pants.TrimmedLuckyPurpleShorts, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Luck, 1), new LewisDisapprovesEffect()) }
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

            { HatDef.ChickenMask, ExtendItem.With.Description("Show your chickens that you want to be one of them").And.Effect(new PetAnimalOnTouch(AnimalType.Chicken)) },
            
            { HatDef.CowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { HatDef.CowgalHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { HatDef.BlueCowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { HatDef.DarkCowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { HatDef.RedCowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { HatDef.DeluxeCowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { HatDef.MagicCowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },

            { HatDef.PirateHat, ExtendItem.With.Effect(new IncreaseFishingTreasureChestChance()) },
            { HatDef.DeluxePirateHat, ExtendItem.With.Description("Uncle Jack always said: \"Take what you can, give nothing back\"").Effect(new KeepTreasureChestWhenFishEscapes()) },

            { HatDef.SailorsCap, ExtendItem.With.Description("You must be a fellow sailor, aye?").Effect(new ShopDiscount(Shop.Willy, 0.05)) },

            { HatDef.StrawHat, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Farming, 1)) },

            { HatDef.Beanie, ExtendItem.With.Effect(new SeasonalEffect(Season.Winter, EffectSet.Of(new IncreaseMaxEnergy(5), new IncreaseMaxHealth(5)))) },
            { HatDef.FloppyBeanie, ExtendItem.With.Effect(new SeasonalEffect(Season.Winter, EffectSet.Of(new IncreaseMaxEnergy(5), new IncreaseMaxHealth(5)))) },

            { HatDef.WhiteTurban, ExtendItem.With.Effect(new LocationalEffect(LocationGroup.DesertPlaces, new IncreaseMaxHealth(15))) },
            { HatDef.GreenTurban, ExtendItem.With.Effect(new LocationalEffect(LocationGroup.DesertPlaces, new IncreaseMaxEnergy(15))) },
            { HatDef.ElegantTurban, ExtendItem.With.Effect(new LocationalEffect(LocationGroup.DesertPlaces, EffectSet.Of(new IncreaseMaxHealth(20), new IncreaseMaxEnergy(20)))) },
            { HatDef.MagicTurban,  ExtendItem.With.Effect(new LocationalEffect(LocationGroup.DesertPlaces, new HealthRegen())) }            
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

        public static bool GetExtInfo(Item item, out ExtItemInfo extInfo)
        {
            if (item is Clothing clothing)
            {
                if (clothing.clothesType.Value == (int)ClothesType.SHIRT)
                {
                    return GetExtInfoByIndex<Shirt>(item.ParentSheetIndex, out extInfo);
                }
                else
                if (clothing.clothesType.Value == (int)ClothesType.PANTS)
                {
                    return GetExtInfoByIndex<Pants>(item.ParentSheetIndex, out extInfo);
                }
            }
            else if (item is StardewValley.Objects.Hat hat)
            {
                return GetExtInfoByIndex<HatDef>(hat.which.Value, out extInfo);
            }

            extInfo = null;
            return false;
        }

        public static bool GetExtInfo<T>(T value, out ExtItemInfo extInfo)
        {
            return GetExtInfoByIndex<T>((int)(object)value, out extInfo);
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
