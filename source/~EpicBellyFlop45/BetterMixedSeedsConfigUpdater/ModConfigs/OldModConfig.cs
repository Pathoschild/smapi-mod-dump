/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

namespace BetterMixedSeedsConfigUpdater.ModConfigs
{
    /// <summary>The old mod configuration.</summary>
    public class OldModConfig
    {
        /// <summary>The percent chance that a mixed seed is dropped, when fiber isn't dropped, when cutting weeds.</summary>
        public int PercentDropChanceForMixedSeedsWhenNotFiber { get; set; }

        // Below is the configuration for all base game crops, organized by season.
        // Spring 
        public bool UseAncientFruit_SPRING { get; set; }
        public bool UseBlueJazz { get; set; }
        public bool UseCauliflower { get; set; }
        public bool UseCoffeeBean { get; set; }
        public bool UseGarlic { get; set; }
        public bool UseGreenBean { get; set; }
        public bool UseKale { get; set; }
        public bool UseParsnip { get; set; }
        public bool UsePotato { get; set; }
        public bool UseRhubarb { get; set; }
        public bool UseStrawberry { get; set; }
        public bool UseTulip { get; set; }

        // Summer
        public bool UseAncientFruit_SUMMER { get; set; }
        public bool UseBlueberry { get; set; }
        public bool UseCorn_SUMMER { get; set; }
        public bool UseHops { get; set; }
        public bool UseHotPepper { get; set; }
        public bool UseMelon { get; set; }
        public bool UsePoppy { get; set; }
        public bool UseRadish { get; set; }
        public bool UseRedCabbage { get; set; }
        public bool UseStarfruit { get; set; }
        public bool UseSummerSpangle { get; set; }
        public bool UseSunflower_SUMMER { get; set; }
        public bool UseTomato { get; set; }
        public bool UseWheat_SUMMER { get; set; }

        // Fall
        public bool UseAncientFruit_FALL { get; set; }
        public bool UseAmaranth { get; set; }
        public bool UseArtichoke { get; set; }
        public bool UseBeet { get; set; }
        public bool UseBokChoy { get; set; }
        public bool UseCorn_FALL { get; set; }
        public bool UseCranberries { get; set; }
        public bool UseEggplant { get; set; }
        public bool UseFairyRose { get; set; }
        public bool UseGrape { get; set; }
        public bool UsePumpkin { get; set; }
        public bool UseSunflower_FALL { get; set; }
        public bool UseSweetGemBerry { get; set; }
        public bool UseWheat_FALL { get; set; }
        public bool UseYam { get; set; }

        // None (Can only be planted in the greenhouse)
        public bool UseCactusFruit { get; set; }

        // Below is the configuration for all crops from integrated mods.
        // Fantasy Crops
        public bool UseCoal_Root_SPRING { get; set; }
        public bool UseCoal_Root_SUMMER { get; set; }
        public bool UseCopper_Leaf { get; set; }
        public bool UseGold_Leaf { get; set; }
        public bool UseIridium_Fern { get; set; }
        public bool UseIron_Leaf { get; set; }
        public bool UseMoney_Plant { get; set; }

        // Fresh Meat
        public bool UseBeef_SPRING { get; set; }
        public bool UseBeef_SUMMER { get; set; }
        public bool UseChevon_SUMMER { get; set; }
        public bool UseChevon_FALL { get; set; }
        public bool UseChicken_SPRING { get; set; }
        public bool UseChicken_SUMMER { get; set; }
        public bool UseDuck_SUMMER { get; set; }
        public bool UseDuck_FALL { get; set; }
        public bool UseMutton_SPRING { get; set; }
        public bool UseMutton_SUMMER { get; set; }
        public bool UsePork_SUMMER { get; set; }
        public bool UsePork_FALL { get; set; }
        public bool UseRabbit_SPRING { get; set; }
        public bool UseRabbit_SUMMER { get; set; }

        // Fruit and Veggies
        public bool UseAdzuki_Bean { get; set; }
        public bool UseAloe { get; set; }
        public bool UseBarley { get; set; }
        public bool UseBasil { get; set; }
        public bool UseBell_Pepper { get; set; }
        public bool UseBlackberry { get; set; }
        public bool UseBroccoli { get; set; }
        public bool UseCabbage_SPRING { get; set; }
        public bool UseCabbage_FALL { get; set; }
        public bool UseCarrot_FruitAndVeggies { get; set; }
        public bool UseCassava { get; set; }
        public bool UseCelery { get; set; }
        public bool UseChives { get; set; }
        public bool UseCotton_SUMMER { get; set; }
        public bool UseCotton_FALL { get; set; }
        public bool UseCucumber_FruitAndVeggies { get; set; }
        public bool UseElderberry { get; set; }
        public bool UseFennel { get; set; }
        public bool UseGinger { get; set; }
        public bool UseGooseberry { get; set; }
        public bool UseGreen_Pea { get; set; }
        public bool UseJuniper { get; set; }
        public bool UseKiwi_SUMMER { get; set; }
        public bool UseKiwi_FALL { get; set; }
        public bool UseLettuce { get; set; }
        public bool UseMint { get; set; }
        public bool UseMuskmelon { get; set; }
        public bool UseNavy_Bean { get; set; }
        public bool UseOnion_FruitAndVeggies { get; set; }
        public bool UseOregano { get; set; }
        public bool UseParsley { get; set; }
        public bool UsePassion_Fruit { get; set; }
        public bool UsePeanut_FruitAndVeggies { get; set; }
        public bool UsePineapple_FruitAndVeggies { get; set; }
        public bool UseRaspberry { get; set; }
        public bool UseRice_SPRING { get; set; }
        public bool UseRice_SUMMER { get; set; }
        public bool UseRice_FALL { get; set; }
        public bool UseRosemary { get; set; }
        public bool UseSage { get; set; }
        public bool UseSoybean { get; set; }
        public bool UseSpinach_SPRING { get; set; }
        public bool UseSpinach_FALL { get; set; }
        public bool UseSugar_Beet { get; set; }
        public bool UseSugar_Cane { get; set; }
        public bool UseSweet_Canary_Melon { get; set; }
        public bool UseSweet_Potato { get; set; }
        public bool UseTea_SPRING { get; set; }
        public bool UseTea_SUMMER { get; set; }
        public bool UseTea_FALL { get; set; }
        public bool UseThyme { get; set; }
        public bool UseWasabi { get; set; }
        public bool UseWatermelon_Mizu { get; set; }

        // Mizus Flowers
        public bool UseBee_Balm { get; set; }
        public bool UseBlue_Mist { get; set; }
        public bool UseChamomile { get; set; }
        public bool UseClary_Sage { get; set; }
        public bool UseFairy_Duster { get; set; }
        public bool UseFall_Rose { get; set; }
        public bool UseFragrant_Lilac { get; set; }
        public bool UseHerbal_Lavender { get; set; }
        public bool UseHoneysuckle_SPRING { get; set; }
        public bool UseHoneysuckle_SUMMER { get; set; }
        public bool UsePassion_Flower { get; set; }
        public bool UsePink_Cat { get; set; }
        public bool UsePurple_Coneflower { get; set; }
        public bool UseRose_SPRING { get; set; }
        public bool UseRose_SUMMER { get; set; }
        public bool UseRose_FALL { get; set; }
        public bool UseShaded_Violet { get; set; }
        public bool UseSpring_Rose { get; set; }
        public bool UseSummer_Rose { get; set; }
        public bool UseSweet_Jasmine { get; set; }

        // Cannabis Kit
        public bool UseBlue_Dream_SUMMER { get; set; }
        public bool UseBlue_Dream_FALL { get; set; }
        public bool UseCannabis_SPRING { get; set; }
        public bool UseCannabis_SUMMER { get; set; }
        public bool UseCannabis_FALL { get; set; }
        public bool UseGirl_Scout_Cookies_SUMMER { get; set; }
        public bool UseGirl_Scout_Cookies_FALL { get; set; }
        public bool UseGreen_Crack_SUMMER { get; set; }
        public bool UseGreen_Crack_FALL { get; set; }
        public bool UseHemp_SPRING { get; set; }
        public bool UseHemp_SUMMER { get; set; }
        public bool UseHemp_FALL { get; set; }
        public bool UseHybrid_SUMMER { get; set; }
        public bool UseHybrid_FALL { get; set; }
        public bool UseIndica { get; set; }
        public bool UseNorthern_Lights { get; set; }
        public bool UseOG_Kush_SUMMER { get; set; }
        public bool UseOG_Kush_FALL { get; set; }
        public bool UseSativa { get; set; }
        public bool UseSour_Diesel { get; set; }
        public bool UseStrawberry_Cough_SUMMER { get; set; }
        public bool UseStrawberry_Cough_FALL { get; set; }
        public bool UseTobacco_SPRING { get; set; }
        public bool UseTobacco_SUMMER { get; set; }
        public bool UseWhite_Widow_SUMMER { get; set; }
        public bool UseWhite_Widow_FALL { get; set; }

        // Six Plantable Crops for Winter
        public bool UseBlue_Rose { get; set; }
        public bool UseDaikon { get; set; }
        public bool UseGentian { get; set; }
        public bool UseNapa_Cabbage { get; set; }
        public bool UseSnowdrop { get; set; }
        public bool UseWinter_Broccoli { get; set; }

        // Bonster Crops
        public bool UseBlackcurrant { get; set; }
        public bool UseBlue_Corn_SUMMER { get; set; }
        public bool UseBlue_Corn_FALL { get; set; }
        public bool UseCardamom { get; set; }
        public bool UseCranberry_Beans { get; set; }
        public bool UseMaypop { get; set; }
        public bool UsePeppercorn_SUMMER { get; set; }
        public bool UsePeppercorn_FALL { get; set; }
        public bool UseRedCurrant { get; set; }
        public bool UseRose_Hips_SPRING { get; set; }
        public bool UseRose_Hips_SUMMER { get; set; }
        public bool UseRoselle_Hibiscus { get; set; }
        public bool UseSummer_Squash { get; set; }
        public bool UseTaro_SUMMER { get; set; }
        public bool UseTaro_FALL { get; set; }
        public bool UseWhite_Currant { get; set; }

        // Revenant Crops
        public bool UseEnoki_Mushroom_SPRING { get; set; }
        public bool UseEnoki_Mushroom_SUMMER { get; set; }
        public bool UseGai_Lan { get; set; }
        public bool UseMaitake_Mushroom { get; set; }
        public bool UseOyster_Mushroom { get; set; }

        // Farmer to Florist
        public bool UseAllium { get; set; }
        public bool UseCamellia_SPRING { get; set; }
        public bool UseCamellia_FALL { get; set; }
        public bool UseCarnation_SPRING { get; set; }
        public bool UseCarnation_SUMMER { get; set; }
        public bool UseChrysanthemum { get; set; }
        public bool UseClematis { get; set; }
        public bool UseDahlia { get; set; }
        public bool UseDelphinium { get; set; }
        public bool UseEnglish_Rose { get; set; }
        public bool UseFreesia { get; set; }
        public bool UseGeranium { get; set; }
        public bool UseHerbalPeony { get; set; }
        public bool UseHyacinth_FarmerToFlorist { get; set; }
        public bool UseHydrangea { get; set; }
        public bool UseIris { get; set; }
        public bool UseLavender { get; set; }
        public bool UseLilac { get; set; }
        public bool UseLily { get; set; }
        public bool UseLotus { get; set; }
        public bool UsePetunia { get; set; }
        public bool UseViolet { get; set; }
        public bool UseWisteria { get; set; }

        // Lucky Clover
        public bool UseLuckyClover { get; set; }

        // Fish's Flowers
        public bool UseHyacinth_FishsFlowers { get; set; }
        public bool UsePansy_SPRING { get; set; }
        public bool UsePansy_SUMMER { get; set; }
        public bool UsePansy_FALL { get; set; }

        // Stephan's LotofCrops
        public bool UseCarrot_StephanLotsOfCrops { get; set; }
        public bool UseCucumber_StephanLotsOfCrops { get; set; }
        public bool UseOnion_StephanLotsOfCrops { get; set; }
        public bool UsePea_Pod { get; set; }
        public bool UsePeanut_StephanLotsOfCrops { get; set; }
        public bool UsePineapple_StephanLotsOfCrops { get; set; }
        public bool UseSpinach { get; set; }
        public bool UseTurnip { get; set; }
        public bool UseWatermelon { get; set; }

        // Eemies Crops
        public bool UseAcorn_Squash { get; set; }
        public bool UseBlack_Forest_Squash { get; set; }
        public bool UseCantaloupe_Melon { get; set; }
        public bool UseCharentais_Melon { get; set; }
        public bool UseCrookneck_Squash { get; set; }
        public bool UseGolden_Hubbard_Squash { get; set; }
        public bool UseJack_O_Lantern_Pumpkin { get; set; }
        public bool UseKorean_Melon { get; set; }
        public bool UseLarge_Watermelon { get; set; }
        public bool UseRich_Canary_Melon { get; set; }
        public bool UseRich_Sweetness_Melon { get; set; }
        public bool UseSweet_Lightning_Pumpkin { get; set; }

        // Tea Time
        public bool UseMint_Tea_Plant_SPRING { get; set; }
        public bool UseMint_Tea_Plant_SUMMER { get; set; }
        public bool UseMint_Tea_Plant_FALL { get; set; }
        public bool UseTea_Leaf_Plant_SPRING { get; set; }
        public bool UseTea_Leaf_Plant_SUMMER { get; set; }
        public bool UseTea_Leaf_Plant_FALL { get; set; }

        // Forage to Farm
        public bool UseCave_Carrot_SPRING { get; set; }
        public bool UseCave_Carrot_SUMMER { get; set; }
        public bool UseCave_Carrot_FALL { get; set; }
        public bool UseChanterelle_Mushroom { get; set; }
        public bool UseCoconut { get; set; }
        public bool UseCommon_Mushroom_SPRING { get; set; }
        public bool UseCommon_Mushroom_FALL { get; set; }
        public bool UseCrocus { get; set; }
        public bool UseCrystal_Fruit { get; set; }
        public bool UseDaffodil { get; set; }
        public bool UseDandelion { get; set; }
        public bool UseFiddlehead_Fern { get; set; }
        public bool UseHazelnut { get; set; }
        public bool UseHolly { get; set; }
        public bool UseWild_Horseradish { get; set; }
        public bool UseLeek { get; set; }
        public bool UseMorel_Mushroom { get; set; }
        public bool UsePurple_Mushroom { get; set; }
        public bool UseRed_Mushroom_SUMMER { get; set; }
        public bool UseRed_Mushroom_FALL { get; set; }
        public bool UseSalmonberry { get; set; }
        public bool UseSnow_Yam { get; set; }
        public bool UseSpice_Berry { get; set; }
        public bool UseSpring_Onion { get; set; }
        public bool UseSweet_Pea { get; set; }
        public bool UseWild_Blackberry { get; set; }
        public bool UseWild_Plum { get; set; }
        public bool UseWinter_Root { get; set; }

        // Gem and Mineral Crops
        public bool UseAerinite_Root { get; set; }
        public bool UseAquamarine_Rose { get; set; }
        public bool UseCelestine_Flower { get; set; }
        public bool UseDiamond_Flower { get; set; }
        public bool UseGhost_Rose { get; set; }
        public bool UseKyanite_Flower { get; set; }
        public bool UseOpal_Cat { get; set; }
        public bool UseSlate_Bean { get; set; }
        public bool UseSoap_Root { get; set; }
    }
}
