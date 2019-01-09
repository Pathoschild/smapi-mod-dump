using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterMixedSeeds
{
    public class ModConfig
    {
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
        /// Below is the configuration for all crops in the Project Populate mod, organized by sub-mod (Fantasy Crops, Fresh Meat, Fruit and Veggies, Mizus Flowers)
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
        public bool UseCarrot { get; set; } = true;
        public bool UseCassava { get; set; } = true;
        public bool UseCelery { get; set; } = true;
        public bool UseChives { get; set; } = true;
        public bool UseCotton_SUMMER { get; set; } = true;
        public bool UseCotton_FALL { get; set; } = true;
        public bool UseCucumber { get; set; } = true;
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
        public bool UseOnion { get; set; } = true;
        public bool UseOregano { get; set; } = true;
        public bool UseParsley { get; set; } = true;
        public bool UsePassion_Fruit { get; set; } = true;
        public bool UsePeanut { get; set; } = true;
        public bool UsePineapple { get; set; } = true;
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
        public bool UseStrawberry_Cough_SUMMER { get; set; }
        public bool UseStrawberry_Cough_FALL { get; set; }
    }
}
