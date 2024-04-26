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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Objects.Clothing;
using HatDef = SkillfulClothes.Types.Hat;

namespace SkillfulClothes
{
    public class ItemDefinitions
    {
        public static Dictionary<Shirt, ExtItemInfo> ShirtEffects = new Dictionary<Shirt, ExtItemInfo>() {
            { KnownShirts.MayoralSuspenders, ExtendItem.With.And.Effect(new IncreasePopularity()) },

            { KnownShirts.HeartShirt_Dyeable, ExtendItem.With.Effect(new IncreaseMaxHealth(15)) },

            { KnownShirts.CopperBreastplate, ExtendItem.With.Description("Light armor made from copper").Effect(new IncreaseDefense(1)).SoldBy(Shop.AdventureGuild, 2000, SellingCondition.SkillLevel_2).And.CannotBeCrafted },
            { KnownShirts.SteelBreastplate, ExtendItem.With.Description("Light armor made from steel").Effect(new IncreaseDefense(2)).SoldBy(Shop.AdventureGuild, 9000, SellingCondition.SkillLevel_4).And.CannotBeCrafted },
            { KnownShirts.GoldBreastplate, ExtendItem.With.Description("Medium armor made from solid gold").Effect(new IncreaseDefense(3)).SoldBy(Shop.AdventureGuild, 18000, SellingCondition.SkillLevel_6).And.CannotBeCrafted },
            { KnownShirts.IridiumBreastplate, ExtendItem.With.Description("Heavy armor made from pure iridium").Effect(new IncreaseDefense(5)).SoldBy(Shop.AdventureGuild, 30000, SellingCondition.SkillLevel_10).And.CannotBeCrafted },

            { KnownShirts.FakeMusclesShirt, ExtendItem.With.Description("You could hit the gym, or you could just wear this instead. Strangely, you even feel a bit stronger wearing it").Effect(new IncreaseAttack(1)) },

            { KnownShirts.CavemanShirt, ExtendItem.With.Effect(new IncreaseAttack(2)) },

            { KnownShirts.FishingVest, ExtendItem.With.Description("Helps you feel really professional while casting your fishing rod").Effect(new IncreaseFishingBarByCaughtFish()).SoldBy(Shop.Willy, 46000, SellingCondition.FriendshipHearts_6).And.CannotBeCrafted },
            { KnownShirts.FishShirt, ExtendItem.With.Description("To catch fish, you must think like a fish").Effect(new MultiplyExperience(Skill.Fishing, 1.2f)).SoldBy(Shop.Willy, 8000, SellingCondition.SkillLevel_2).And.CannotBeCrafted },
            { KnownShirts.ShirtOfTheSea, ExtendItem.With.Description("It smells like the brine of the sea and helps you focus while fishing").And.Effect(new IncreaseSkillLevel(Skill.Fishing, 1)).SoldBy(Shop.Willy, 6500, SellingCondition.SkillLevel_4).And.CannotBeCrafted },
            { KnownShirts.SailorShirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Fishing, 1)).SoldBy(Shop.Willy, 6000, SellingCondition.FriendshipHearts_4).And.CannotBeCrafted },
            { KnownShirts.SailorShirt_2, ExtendItem.With.Effect(new IncreaseFishingBarByCaughtFish()).SoldBy(Shop.Willy, 22000, SellingCondition.SkillLevel_10).And.CannotBeCrafted },
            { KnownShirts.ShrimpEnthusiastShirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Fishing, 1)).SoldBy(Shop.Willy, 4500, SellingCondition.SkillLevel_6).And.CannotBeCrafted },

            { KnownShirts.OceanShirt, ExtendItem.With.Description("Show your love for the ocean").Effect(new ShopDiscount(Shop.Willy, 0.05)) },

            { KnownShirts.CaptainsUniform, ExtendItem.With.Effect(new IncreaseFishingTreasureChestChance()) },

            { KnownShirts.BridalShirt, ExtendItem.With.Description("A beautiful white top, perfect for a bride. You feel lucky wearing this.").Effect(new IncreaseSkillLevel(Skill.Luck, 1)) },

            { KnownShirts.TomatoShirt, ExtendItem.With.Description("The best farmers become one with their produce").And.Effect(new MultiplyExperience(Skill.Farming, 1.2f)) },
            { KnownShirts.CrabCakeShirt, ExtendItem.With.Description("Reminds you of your favorite dish").Effect(new IncreaseSpeed(1), new IncreaseDefense(1)) },

            { KnownShirts.ArcaneShirt, ExtendItem.With.Description("This garment seems to affect your body in mysterious ways.").And.Effect(new HealthRegen()) },

            { KnownShirts.WhiteGi, ExtendItem.With.Description("\"He who attacks must vanquish. He who defends must merely survive\"").Effect(new IncreaseDefense(2)) },
            { KnownShirts.OrangeGi, ExtendItem.With.Description("The best defense is a good offense").Effect(new IncreaseAttack(1)) },

            { KnownShirts.StuddedVest, ExtendItem.With.Description("A black vest studded with metal spikes. Monsters dread the very sight of it").And.Effect(new IncreaseAttack(2)) },
            { KnownShirts.BlacksmithApron, ExtendItem.With.Description("Shows your admiration of craftsmanship").And.Effect(new ShopDiscount(Shop.Clint, 0.05)) },

            { KnownShirts.IridiumEnergyShirt, ExtendItem.With.Description("Wearing this you can feel the iridium’s energy flowing through you.").And.Effect(new StaminaRegen()).SoldBy(Shop.Krobus, 120000).And.CannotBeCrafted },
            { KnownShirts.HappyShirt, ExtendItem.With.Effect(new IncreasePopularity()) },

            { KnownShirts.BandanaShirt_ShieldFromHarm, ExtendItem.With.Effect(new IncreaseDefense(1)) },

            { KnownShirts.GreenThumbShirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Farming, 1)).SoldBy(Shop.Pierre, 6000, SellingCondition.FriendshipHearts_4).CannotBeCrafted },
            { KnownShirts.ExcavatorShirt, ExtendItem.With.Effect(new MultiplyExperience(Skill.Mining, 1.2f)) },

            { KnownShirts.YobaShirt, ExtendItem.With.Effect(new RingEffect(RingType.YobaRing)) },

            { KnownShirts.PrismaticShirt, ExtendItem.With.Description("The shimmering prismatic energy vitalizes the wearer’s body").And.Effect(new IncreaseMaxHealth(25)) },
            { KnownShirts.PrismaticShirt_DarkSleeves, ExtendItem.With.Description("It’s shimmering prismatic energy makes you feel safe").Effect(new AutoRevive()) },
            { KnownShirts.PrismaticShirt_WhiteSleeves, ExtendItem.With.Description("The shimmering prismatic energy strengthens the wearer’s body").Effect(new IncreaseMaxEnergy(25)) },

            { KnownShirts.RangerUniform, ExtendItem.With.Description("Now you look like a professional while chopping down trees").And.Effect(new MultiplyExperience(Skill.Foraging, 1.2f)) },

            { KnownShirts.GreenTunic, ExtendItem.With.Effect(new ShopDiscount(Shop.AdventureGuild, 0.05)) },
            { KnownShirts.LimeGreenTunic, ExtendItem.With.Description("A padded, bright green tunic with a belt").Effect(new IncreaseDefense(1)) },

            { KnownShirts.StarShirt, ExtendItem.With.Description("The star is glowing in a dim light").And.Effect(new GlowEffect(0.65f, new Color(200, 0, 0))) },
            { KnownShirts.NightSkyShirt, ExtendItem.With.Effect( new OvernightStaminaBuff(30)) },
            { KnownShirts.GoodnightShirt, ExtendItem.With.Effect(new OvernightHealthBuff(25)) },

            { KnownShirts.SlimeShirt, ExtendItem.With.Description("Identifies you as a friend of slimes").And.Effect(new RingEffect(RingType.SlimeCharmerRing)).SoldBy(Shop.AdventureGuild, 21000, SellingCondition.SkillLevel_8).And.CannotBeCrafted },            

            { KnownShirts.GraySuit, ExtendItem.With.Effect(new ShopDiscount(Shop.JojaMarket, 0.001)) }
        };

        public static Dictionary<Pants, ExtItemInfo> PantsEffects = new Dictionary<Pants, ExtItemInfo>() {
            { KnownPants.FarmerPants, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Farming, 1)) },

            { KnownPants.DinosaurPants, ExtendItem.With.Description("These pants are reinforced with real dinosaur scales").And.Effect(new IncreaseDefense(1)) },

            { KnownPants.GeniePants, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Luck, 1)) },

            { KnownPants.PrismaticPants, ExtendItem.With.Effect(new IncreaseMaxHealth(10)) },
            { KnownPants.PrismaticGeniePants, ExtendItem.With.Effect(new IncreaseMaxEnergy(20)) },

            { KnownPants.GrassSkirt, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Foraging, 1)).SoldBy(Shop.Marnie, 6000, SellingCondition.SkillLevel_4) },

            { KnownPants.TrimmedLuckyPurpleShorts, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Luck, 1), new LewisDisapprovesEffect()) }
        };

        public static Dictionary<HatDef, ExtItemInfo> HatEffects = new Dictionary<HatDef, ExtItemInfo>()
        {
#if DEBUG
            { KnownHats.BlobfishMask, ExtendItem.With.Effect(new IncreaseSpeed(6), new ConstructDiscount(0.05), new DebugEffect()) },
#endif

            { KnownHats.DinosaurHat, ExtendItem.With.Effect(new IncreaseDefense(1)) },
            { KnownHats.WearableDwarfHelm, ExtendItem.With.Effect(new IncreaseDefense(2)) },
            { KnownHats.PartyHat_Green, ExtendItem.With.Effect(new IncreasePopularity()) },
            { KnownHats.PartyHat_Blue, ExtendItem.With.Effect(new IncreasePopularity()) },
            { KnownHats.PartyHat_Red, ExtendItem.With.Effect(new IncreasePopularity()) },
            { KnownHats.FishingHat, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Fishing, 1)) },
            { KnownHats.BridalVeil, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Luck, 1)) },
            { KnownHats.WitchHat, ExtendItem.With.Effect(new IncreaseImmunity(1)) },
            { KnownHats.SwashbucklerHat, ExtendItem.With.Effect(new IncreaseAttack(1)) },
            { KnownHats.Goggles, ExtendItem.With.Effect(new IncreaseDefense(1)) },
            { KnownHats.ForagersHat, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Foraging, 1)) },
            { KnownHats.WarriorHelmet, ExtendItem.With.Effect(new IncreaseAttack(2)) },

            { KnownHats.LuckyBow, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Luck, 1)) },
            { KnownHats.ChickenMask, ExtendItem.With.Description("Show your chickens that you want to be one of them").And.Effect(new PetAnimalOnTouch(AnimalType.Chicken)) },
            
            { KnownHats.CowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { KnownHats.CowgalHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { KnownHats.BlueCowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { KnownHats.DarkCowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { KnownHats.RedCowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { KnownHats.DeluxeCowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },
            { KnownHats.MagicCowboyHat, ExtendItem.With.Effect(new PetAnimalOnTouch(AnimalType.Cow)) },

            { KnownHats.PirateHat, ExtendItem.With.Effect(new IncreaseFishingTreasureChestChance()) },
            { KnownHats.DeluxePirateHat, ExtendItem.With.Description("Uncle Jack always said: \"Take what you can, give nothing back\"").Effect(new KeepTreasureChestWhenFishEscapes()) },

            { KnownHats.SailorsCap, ExtendItem.With.Description("You must be a fellow sailor, aye?").Effect(new ShopDiscount(Shop.Willy, 0.05)) },

            { KnownHats.StrawHat, ExtendItem.With.Effect(new IncreaseSkillLevel(Skill.Farming, 1)) },
            
            { KnownHats.Beanie, ExtendItem.With.Effect(new SeasonalEffect(Season.Winter, EffectSet.Of(new IncreaseMaxEnergy(5), new IncreaseMaxHealth(5)))) },
            { KnownHats.FloppyBeanie, ExtendItem.With.Effect(new SeasonalEffect(Season.Winter, EffectSet.Of(new IncreaseMaxEnergy(5), new IncreaseMaxHealth(5)))) },

            { KnownHats.WhiteTurban, ExtendItem.With.Effect(new LocationalEffect(LocationGroup.DesertPlaces, new IncreaseMaxHealth(15))) },
            { KnownHats.GreenTurban, ExtendItem.With.Effect(new LocationalEffect(LocationGroup.DesertPlaces, new IncreaseMaxEnergy(15))) },
            { KnownHats.ElegantTurban, ExtendItem.With.Effect(new LocationalEffect(LocationGroup.DesertPlaces, EffectSet.Of(new IncreaseMaxHealth(20), new IncreaseMaxEnergy(20)))) },
            { KnownHats.MagicTurban,  ExtendItem.With.Effect(new LocationalEffect(LocationGroup.DesertPlaces, new HealthRegen())) }            
        };

        public static bool GetEffect(Item item, out IEffect effect)
        {
            ExtItemInfo extInfo = null;

            if (item is Clothing clothing)
            {
                if (clothing.clothesType.Value == ClothesType.SHIRT)
                {
                    GetExtInfoByItemId<Shirt>(item.ItemId, out extInfo);
                } else
                if (clothing.clothesType.Value == ClothesType.PANTS)
                {
                    GetExtInfoByItemId<Pants>(item.ItemId, out extInfo);                    
                }
            } else if (item is StardewValley.Objects.Hat hat)
            {
                GetExtInfoByItemId<HatDef>(hat.ItemId, out extInfo);
            }

            effect = extInfo?.Effect;

            return effect != null;
        }

        public static bool GetEffectByItemId<T>(string itemId, out IEffect effect)
        {
            if (itemId != null && GetExtInfoByItemId<T>(itemId, out ExtItemInfo extInfo))
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
                if (clothing.clothesType.Value == ClothesType.SHIRT)
                {
                    return GetExtInfoByItemId<Shirt>(item.ItemId, out extInfo);
                }
                else
                if (clothing.clothesType.Value == ClothesType.PANTS)
                {
                    return GetExtInfoByItemId<Pants>(item.ItemId, out extInfo);
                }
            }
            else if (item is StardewValley.Objects.Hat hat)
            {
                return GetExtInfoByItemId<HatDef>(hat.ItemId, out extInfo);
            }

            extInfo = null;
            return false;
        }

        public static bool GetExtInfo<T>(T value, out ExtItemInfo extInfo)
        {
            if (value is Shirt shirt)
            {
                return ShirtEffects.TryGetValue(shirt, out extInfo);
            }

            if (value is Pants pants)
            {
                return PantsEffects.TryGetValue(pants, out extInfo);
            }

            if (value is HatDef hatDef)
            {
                return HatEffects.TryGetValue(hatDef, out extInfo);
            }

            extInfo = null;
            return false;
        }

        public static bool GetExtInfoByItemId<T>(string itemId, out ExtItemInfo extInfo)
        {
            if (itemId != null)
            {
                if (typeof(T) == typeof(Shirt))
                {
                    return ShirtEffects.TryGetValue(itemId, out extInfo);
                }

                if (typeof(T) == typeof(Pants))
                {
                    return PantsEffects.TryGetValue(itemId, out extInfo);
                }

                if (typeof(T) == typeof(HatDef))
                {
                    return HatEffects.TryGetValue(itemId, out extInfo);
                }
            }

            extInfo = null;
            return false;
        }

        public static T GetKnownItemById<T>(string itemId)
            where T : AlphanumericItemId
        {
            if (itemId == null)
            {
                if (typeof(T) == typeof(Shirt)) return KnownShirts.None as T;
                if (typeof(T) == typeof(Pants)) return KnownPants.None as T;
                if (typeof(T) == typeof(HatDef)) return KnownHats.None as T;
            }

            if (typeof(T) == typeof(Shirt))
            {
                return KnownShirts.GetById(itemId) as T;
            }

            if (typeof(T) == typeof(Pants))
            {
                return KnownPants.GetById(itemId) as T;
            }

            if (typeof(T) == typeof(HatDef))
            {
                return KnownHats.GetById(itemId) as T;
            }

            return null;
        }
    }
}
