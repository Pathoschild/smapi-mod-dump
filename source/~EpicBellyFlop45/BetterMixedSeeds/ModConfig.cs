using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterMixedSeeds
{
    public class ModConfig
    {
        public int PercentDropChanceForMixedSeedsWhenNotFiber { get; set; } = 5;

        /// <summary>
        /// Below is the configuration for all base game crops, organized by season
        /// </summary>

        // Spring 
        public bool UseAncientFruit_SPRING { get; set; } = true;
        public bool UseBlueJazz { get; set; } = true;
        public bool UseCauliflower { get; set; } = true;
        public bool UseCoffeeBean { get; set; } = true;
        public bool UseGarlic { get; set; } = true;
        public bool UseGreenBean { get; set; } = true;
        public bool UseKale { get; set; } = true;
        public bool UseParsnip { get; set; } = true;
        public bool UsePotato { get; set; } = true;
        public bool UseRhubarb { get; set; } = true;
        public bool UseStrawberry { get; set; } = true;
        public bool UseTulip { get; set; } = true;

        // Summer
        public bool UseAncientFruit_SUMMER { get; set; } = true;
        public bool UseBlueberry { get; set; } = true;
        public bool UseCorn_SUMMER { get; set; } = true;
        public bool UseHops { get; set; } = true;
        public bool UseHotPepper { get; set; } = true;
        public bool UseMelon { get; set; } = true;
        public bool UsePoppy { get; set; } = true;
        public bool UseRadish { get; set; } = true;
        public bool UseRedCabbage { get; set; } = true;
        public bool UseStarfruit { get; set; } = true;
        public bool UseSummerSpangle { get; set; } = true;
        public bool UseSunflower_SUMMER { get; set; } = true;
        public bool UseTomato { get; set; } = true;
        public bool UseWheat_SUMMER { get; set; } = true;

        // Fall
        public bool UseAncientFruit_FALL { get; set; } = true;
        public bool UseAmaranth { get; set; } = true;
        public bool UseArtichoke { get; set; } = true;
        public bool UseBeet { get; set; } = true;
        public bool UseBokChoy { get; set; } = true;
        public bool UseCorn_FALL { get; set; } = true;
        public bool UseCranberries { get; set; } = true;
        public bool UseEggplant { get; set; } = true;
        public bool UseFairyRose { get; set; } = true;
        public bool UseGrape { get; set; } = true;
        public bool UsePumpkin { get; set; } = true;
        public bool UseSunflower_FALL { get; set; } = true;
        public bool UseSweetGemBerry { get; set; } = true;
        public bool UseWheat_FALL { get; set; } = true;
        public bool UseYam { get; set; } = true;
        
        // None (Can only be planted in the greenhouse)
        public bool UseCactusFruit { get; set; } = true;

        /// <summary>
        /// Below is the configuration for all crops from integrated mods
        /// </summary>

        // Fantasy Crops
        public bool UseCoal_Root_SPRING { get; set; } = true;
        public bool UseCoal_Root_SUMMER { get; set; } = true;
        public bool UseCopper_Leaf { get; set; } = true;
        public bool UseGold_Leaf { get; set; } = true;
        public bool UseIridium_Fern { get; set; } = true;
        public bool UseIron_Leaf { get; set; } = true;
        public bool UseMoney_Plant { get; set; } = true;

        // Fresh Meat
        public bool UseBeef_SPRING { get; set; } = true;
        public bool UseBeef_SUMMER { get; set; } = true;
        public bool UseChevon_SUMMER { get; set; } = true;
        public bool UseChevon_FALL { get; set; } = true;
        public bool UseChicken_SPRING { get; set; } = true;
        public bool UseChicken_SUMMER { get; set; } = true;
        public bool UseDuck_SUMMER { get; set; } = true;
        public bool UseDuck_FALL { get; set; } = true;
        public bool UseMutton_SPRING { get; set; } = true;
        public bool UseMutton_SUMMER { get; set; } = true;
        public bool UsePork_SUMMER { get; set; } = true;
        public bool UsePork_FALL { get; set; } = true;
        public bool UseRabbit_SPRING { get; set; } = true;
        public bool UseRabbit_SUMMER { get; set; } = true;

        // Fruit and Veggies
        public bool UseAdzuki_Bean { get; set; } = true;
        public bool UseAloe { get; set; } = true;
        public bool UseBarley { get; set; } = true;
        public bool UseBasil { get; set; } = true;
        public bool UseBell_Pepper { get; set; } = true;
        public bool UseBlackberry { get; set; } = true;
        public bool UseBroccoli { get; set; } = true;
        public bool UseCabbage_SPRING { get; set; } = true;
        public bool UseCabbage_FALL { get; set; } = true;
        public bool UseCarrot_FruitAndVeggies { get; set; } = true;
        public bool UseCassava { get; set; } = true;
        public bool UseCelery { get; set; } = true;
        public bool UseChives { get; set; } = true;
        public bool UseCotton_SUMMER { get; set; } = true;
        public bool UseCotton_FALL { get; set; } = true;
        public bool UseCucumber_FruitAndVeggies { get; set; } = true;
        public bool UseElderberry { get; set; } = true;
        public bool UseFennel { get; set; } = true;
        public bool UseGinger { get; set; } = true;
        public bool UseGooseberry { get; set; } = true;
        public bool UseGreen_Pea { get; set; } = true;
        public bool UseJuniper { get; set; } = true;
        public bool UseKiwi_SUMMER { get; set; } = true;
        public bool UseKiwi_FALL { get; set; } = true;
        public bool UseLettuce { get; set; } = true;
        public bool UseMint { get; set; } = true;
        public bool UseMuskmelon { get; set; } = true;
        public bool UseNavy_Bean { get; set; } = true;
        public bool UseOnion_FruitAndVeggies { get; set; } = true;
        public bool UseOregano { get; set; } = true;
        public bool UseParsley { get; set; } = true;
        public bool UsePassion_Fruit { get; set; } = true;
        public bool UsePeanut_FruitAndVeggies { get; set; } = true;
        public bool UsePineapple_FruitAndVeggies { get; set; } = true;
        public bool UseRaspberry { get; set; } = true;
        public bool UseRice_SPRING { get; set; } = true;
        public bool UseRice_SUMMER { get; set; } = true;
        public bool UseRice_FALL { get; set; } = true;
        public bool UseRosemary { get; set; } = true;
        public bool UseSage { get; set; } = true;
        public bool UseSoybean { get; set; } = true;
        public bool UseSpinach_SPRING { get; set; } = true;
        public bool UseSpinach_FALL { get; set; } = true;
        public bool UseSugar_Beet { get; set; } = true;
        public bool UseSugar_Cane { get; set; } = true;
        public bool UseSweet_Canary_Melon { get; set; } = true;
        public bool UseSweet_Potato { get; set; } = true;
        public bool UseTea_SPRING { get; set; } = true;
        public bool UseTea_SUMMER { get; set; } = true;
        public bool UseTea_FALL { get; set; } = true;
        public bool UseThyme { get; set; } = true;
        public bool UseWasabi { get; set; } = true;
        public bool UseWatermelon_Mizu { get; set; } = true;

        // Mizus Flowers
        public bool UseBee_Balm { get; set; } = true;
        public bool UseBlue_Mist { get; set; } = true;
        public bool UseChamomile { get; set; } = true;
        public bool UseClary_Sage { get; set; } = true;
        public bool UseFairy_Duster { get; set; } = true;
        public bool UseFall_Rose { get; set; } = true;
        public bool UseFragrant_Lilac { get; set; } = true;
        public bool UseHerbal_Lavender { get; set; } = true;
        public bool UseHoneysuckle_SPRING { get; set; } = true;
        public bool UseHoneysuckle_SUMMER { get; set; } = true;
        public bool UsePassion_Flower { get; set; } = true;
        public bool UsePink_Cat { get; set; } = true;
        public bool UsePurple_Coneflower { get; set; } = true;
        public bool UseRose_SPRING { get; set; } = true;
        public bool UseRose_SUMMER { get; set; } = true;
        public bool UseRose_FALL { get; set; } = true;
        public bool UseShaded_Violet { get; set; } = true;
        public bool UseSpring_Rose { get; set; } = true;
        public bool UseSummer_Rose { get; set; } = true;
        public bool UseSweet_Jasmine { get; set; } = true;

        // Cannabis Kit
        public bool UseBlue_Dream_SUMMER { get; set; } = true;
        public bool UseBlue_Dream_FALL { get; set; } = true;
        public bool UseCannabis_SPRING { get; set; } = true;
        public bool UseCannabis_SUMMER { get; set; } = true;
        public bool UseCannabis_FALL { get; set; } = true;
        public bool UseGirl_Scout_Cookies_SUMMER { get; set; } = true;
        public bool UseGirl_Scout_Cookies_FALL { get; set; } = true;
        public bool UseGreen_Crack_SUMMER { get; set; } = true;
        public bool UseGreen_Crack_FALL { get; set; } = true;
        public bool UseHemp_SPRING { get; set; } = true;
        public bool UseHemp_SUMMER { get; set; } = true;
        public bool UseHemp_FALL { get; set; } = true;
        public bool UseHybrid_SUMMER { get; set; } = true;
        public bool UseHybrid_FALL { get; set; } = true;
        public bool UseIndica { get; set; } = true;
        public bool UseNorthern_Lights { get; set; } = true;
        public bool UseOG_Kush_SUMMER { get; set; } = true;
        public bool UseOG_Kush_FALL { get; set; } = true;
        public bool UseSativa { get; set; } = true;
        public bool UseSour_Diesel { get; set; } = true;
        public bool UseStrawberry_Cough_SUMMER { get; set; } = true;
        public bool UseStrawberry_Cough_FALL { get; set; } = true;
        public bool UseTobacco_SPRING { get; set; } = true;
        public bool UseTobacco_SUMMER { get; set; } = true;
        public bool UseWhite_Widow_SUMMER { get; set; } = true;
        public bool UseWhite_Widow_FALL { get; set; } = true;

        // Six Plantable Crops for Winter
        public bool UseBlue_Rose { get; set; } = true;
        public bool UseDaikon { get; set; } = true;
        public bool UseGentian { get; set; } = true;
        public bool UseNapa_Cabbage { get; set; } = true;
        public bool UseSnowdrop { get; set; } = true;
        public bool UseWinter_Broccoli { get; set; } = true;

        // Bonster Crops
        public bool UseBlackcurrant { get; set; } = true;
        public bool UseBlue_Corn_SUMMER { get; set; } = true;
        public bool UseBlue_Corn_FALL { get; set; } = true;
        public bool UseCardamom { get; set; } = true;
        public bool UseCranberry_Beans { get; set; } = true;
        public bool UseMaypop { get; set; } = true;
        public bool UsePeppercorn_SUMMER { get; set; } = true;
        public bool UsePeppercorn_FALL { get; set; } = true;
        public bool UseRedCurrant { get; set; } = true;
        public bool UseRose_Hips_SPRING { get; set; } = true;
        public bool UseRose_Hips_SUMMER { get; set; } = true;
        public bool UseRoselle_Hibiscus { get; set; } = true;
        public bool UseSummer_Squash { get; set; } = true;
        public bool UseTaro_SUMMER { get; set; } = true;
        public bool UseTaro_FALL { get; set; } = true;
        public bool UseWhite_Currant { get; set; } = true;

        // Revenant Crops
        public bool UseEnoki_Mushroom_SPRING { get; set; } = true;
        public bool UseEnoki_Mushroom_SUMMER { get; set; } = true;
        public bool UseGai_Lan { get; set; } = true;
        public bool UseMaitake_Mushroom { get; set; } = true;
        public bool UseOyster_Mushroom { get; set; } = true;

        // Farmer to Florist
        public bool UseAllium { get; set; } = true;
        public bool UseCamellia_SPRING { get; set; } = true;
        public bool UseCamellia_FALL { get; set; } = true;
        public bool UseCarnation_SPRING { get; set; } = true;
        public bool UseCarnation_SUMMER { get; set; } = true;
        public bool UseChrysanthemum { get; set; } = true;
        public bool UseClematis { get; set; } = true;
        public bool UseDahlia { get; set; } = true;
        public bool UseDelphinium { get; set; } = true;
        public bool UseEnglish_Rose { get; set; } = true;
        public bool UseFreesia { get; set; } = true;
        public bool UseGeranium { get; set; } = true;
        public bool UseHerbalPeony { get; set; } = true;
        public bool UseHyacinth_FarmerToFlorist { get; set; } = true;
        public bool UseHydrangea { get; set; } = true;
        public bool UseIris { get; set; } = true;
        public bool UseLavender { get; set; } = true;
        public bool UseLilac { get; set; } = true;
        public bool UseLily { get; set; } = true;
        public bool UseLotus { get; set; } = true;
        public bool UsePetunia { get; set; } = true;
        public bool UseViolet { get; set; } = true;
        public bool UseWisteria { get; set; } = true;

        // Lucky Clover
        public bool UseLuckyClover { get; set; } = true;

        // Fish's Flowers
        public bool UseHyacinth_FishsFlowers { get; set; } = true;
        public bool UsePansy_SPRING { get; set; } = true;
        public bool UsePansy_SUMMER { get; set; } = true;
        public bool UsePansy_FALL { get; set; } = true;

        // Stephan's LotofCrops
        public bool UseCarrot_StephanLotsOfCrops { get; set; } = true;
        public bool UseCucumber_StephanLotsOfCrops { get; set; } = true;
        public bool UseOnion_StephanLotsOfCrops { get; set; } = true;
        public bool UsePea_Pod { get; set; } = true;
        public bool UsePeanut_StephanLotsOfCrops { get; set; } = true;
        public bool UsePineapple_StephanLotsOfCrops { get; set; } = true;
        public bool UseSpinach { get; set; } = true;
        public bool UseTurnip { get; set; } = true;
        public bool UseWatermelon { get; set; } = true;

        // Eemies Crops
        public bool UseAcorn_Squash { get; set; } = true;
        public bool UseBlack_Forest_Squash { get; set; } = true;
        public bool UseCantaloupe_Melon { get; set; } = true;
        public bool UseCharentais_Melon { get; set; } = true;
        public bool UseCrookneck_Squash { get; set; } = true;
        public bool UseGolden_Hubbard_Squash { get; set; } = true;
        public bool UseJack_O_Lantern_Pumpkin { get; set; } = true;
        public bool UseKorean_Melon { get; set; } = true;
        public bool UseLarge_Watermelon { get; set; } = true;
        public bool UseRich_Canary_Melon { get; set; } = true;
        public bool UseRich_Sweetness_Melon { get; set; } = true;
        public bool UseSweet_Lightning_Pumpkin { get; set; } = true;

        // Tea Time
        public bool UseMint_Tea_Plant_SPRING { get; set; } = true;
        public bool UseMint_Tea_Plant_SUMMER { get; set; } = true;
        public bool UseMint_Tea_Plant_FALL { get; set; } = true;
        public bool UseTea_Leaf_Plant_SPRING { get; set; } = true;
        public bool UseTea_Leaf_Plant_SUMMER { get; set; } = true;
        public bool UseTea_Leaf_Plant_FALL { get; set; } = true;

        // Forage to Farm
        public bool UseCave_Carrot_SPRING { get; set; } = true;
        public bool UseCave_Carrot_SUMMER { get; set; } = true;
        public bool UseCave_Carrot_FALL { get; set; } = true;
        public bool UseChanterelle_Mushroom { get; set; } = true;
        public bool UseCoconut { get; set; } = true;
        public bool UseCommon_Mushroom_SPRING { get; set; } = true;
        public bool UseCommon_Mushroom_FALL { get; set; } = true;
        public bool UseCrocus { get; set; } = true;
        public bool UseCrystal_Fruit { get; set; } = true;
        public bool UseDaffodil { get; set; } = true;
        public bool UseDandelion { get; set; } = true;
        public bool UseFiddlehead_Fern { get; set; } = true;
        public bool UseHazelnut { get; set; } = true;
        public bool UseHolly { get; set; } = true;
        public bool UseWild_Horseradish { get; set; } = true;
        public bool UseLeek { get; set; } = true;
        public bool UseMorel_Mushroom { get; set; } = true;
        public bool UsePurple_Mushroom { get; set; } = true;
        public bool UseRed_Mushroom_SUMMER { get; set; } = true;
        public bool UseRed_Mushroom_FALL { get; set; } = true;
        public bool UseSalmonberry { get; set; } = true;
        public bool UseSnow_Yam { get; set; } = true;
        public bool UseSpice_Berry { get; set; } = true;
        public bool UseSpring_Onion { get; set; } = true;
        public bool UseSweet_Pea { get; set; } = true;
        public bool UseWild_Blackberry { get; set; } = true;
        public bool UseWild_Plum { get; set; } = true;
        public bool UseWinter_Root { get; set; } = true;
    }
}
