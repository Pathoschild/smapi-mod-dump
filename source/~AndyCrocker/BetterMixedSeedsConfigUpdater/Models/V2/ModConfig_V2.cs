/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace BetterMixedSeedsConfigUpdater.Models.V2
{
    /// <summary>The V2 mod configuration.</summary>
    public class ModConfig_V2
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The percent chance that a mixed seed is dropped, when fiber isn't dropped, when cutting weeds.</summary>
        public int PercentDropChanceForMixedSeedsWhenNotFiber { get; set; } = 5;

        /// <summary>Whether mixed seeds can only plant seeds if the seed's year requirement is met.</summary>
        public bool UseCropYearRequirement { get; set; } = true;

        /// <summary>All the crops that are in the base game, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 StardewValley { get; set; }

        /// <summary>All the crops that are in the Fantasy Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 FantasyCrops { get; set; }

        /// <summary>All the crops that are in the Fresh Meat mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 FreshMeat { get; set; }

        /// <summary>All the crops that are in the Fruits and Veggies mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 FruitAndVeggies { get; set; }

        /// <summary>All the crops that are in the Mizus Flowers mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 MizusFlowers { get; set; }

        /// <summary>All the crops that are in the Cannabis Kit mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 CannabisKit { get; set; }

        /// <summary>All the crops that are in the Six Plantable Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 SixPlantableCrops { get; set; }

        /// <summary>All the crops that are in the Bonsters Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 BonsterCrops { get; set; }

        /// <summary>All the crops that are in the Revenants Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 RevenantCrops { get; set; }

        /// <summary>All the crops that are in the Farmer to Florist mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 FarmerToFlorist { get; set; }

        /// <summary>All the crops that are in the Lucky Clover mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 LuckyClover { get; set; }

        /// <summary>All the crops that are in the Fishs Flowers mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 FishsFlowers { get; set; }

        /// <summary>All the crops that are in the Fishs Flowers Compatibility Version mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 FishsFlowersCompatibilityVersion { get; set; } = new CropMod_V2
        (
            spring: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Grape Hyacinth", true, 1),
                    new Crop_V2("Pansy", true, 1)
                }),
            summer: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Pansy", true, 1)
                }),
            fall: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Pansy", true, 1)
                }),
            winter: null
        );

        /// <summary>All the crops that are in the Stephans Lots of Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 StephansLotsOfCrops { get; set; }

        /// <summary>All the crops that are in the Eemies Crops, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 EemiesCrops { get; set; }

        /// <summary>All the crops that are in the Tea Time mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 TeaTime { get; set; }

        /// <summary>All the crops that are in the Forage to Farm mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 ForageToFarm { get; set; }

        /// <summary>All the crops that are in the Gem and Mineral Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 GemAndMineralCrops { get; set; }

        /// <summary>All the crops that are in the Mouse Ear Cress mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 MouseEarCress { get; set; } = new CropMod_V2
        (
            spring: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Mouse-Ear Cress", true, 1)
                }),
            summer: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Mouse-Ear Cress", true, 1)
                }),
            fall: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Mouse-Ear Cress", true, 1)
                }),
            winter: null
        );

        /// <summary>All the crops that are in the Ancient Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 AncientCrops { get; set; } = new CropMod_V2
        (
            spring: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Ancient Coffee Plant", true, 1),
                    new Crop_V2("Ancient Fern", true, 1),
                    new Crop_V2("Ancient Flower", true, 1),
                    new Crop_V2("Ancient Nut", true, 1),
                    new Crop_V2("Ancient Olive Plant", true, 1),
                    new Crop_V2("Ancient Tuber", true, 1)
                }),
            summer: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Ancient Coffee Plant", true, 1),
                    new Crop_V2("Ancient Fern", true, 1),
                    new Crop_V2("Ancient Nut", true, 1),
                    new Crop_V2("Ancient Olive Plant", true, 1),
                    new Crop_V2("Ancient Tuber", true, 1)
                }),
            fall: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Ancient Coffee Plant", true, 1),
                    new Crop_V2("Ancient Fern", true, 1),
                    new Crop_V2("Ancient Nut", true, 1),
                    new Crop_V2("Ancient Olive Plant", true, 1),
                    new Crop_V2("Ancient Tuber", true, 1)
                }),
            winter: null
        );

        /// <summary>All the crops that are in the Poke Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 PokeCrops { get; set; } = new CropMod_V2
        (
            spring: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Cheri Berry", true, 1),
                    new Crop_V2("Lum Berry", true, 1),
                    new Crop_V2("Rawst Berry", true, 1),
                }),
            summer: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Lum Berry", true, 1),
                    new Crop_V2("Nanab Berry", true, 1),
                    new Crop_V2("Oran Berry", true, 1),
                    new Crop_V2("Pecha Berry", true, 1),
                }),
            fall: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Aspear Berry", true, 1),
                    new Crop_V2("Chesto Berry", true, 1),
                    new Crop_V2("Leppa Berry", true, 1),
                    new Crop_V2("Oran Berry", true, 1),
                    new Crop_V2("Persim Berry", true, 1),
                    new Crop_V2("Sitrus Berry", true, 1),
                }),
            winter: null
        );

        /// <summary>All the crops that are in the Starbound Valley mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 StarboundValley { get; set; } = new CropMod_V2
        (
            spring: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Automato", true, 1),
                    new Crop_V2("Eggshoot", true, 1),
                    new Crop_V2("Feathercrown", true, 1),
                    new Crop_V2("Oculemon", true, 1),
                    new Crop_V2("Pearlpea", true, 1),
                    new Crop_V2("Pussplum", true, 1),
                }),
            summer: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Avesmingo", true, 1),
                    new Crop_V2("Beakseed", true, 1),
                    new Crop_V2("Coralcreep", true, 1),
                    new Crop_V2("Currentcorn", true, 1),
                    new Crop_V2("Dirturchin", true, 1),
                    new Crop_V2("Neonmelon", true, 1),
                    new Crop_V2("Reefpod", true, 1),
                }),
            fall: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Beakseed", true, 1),
                    new Crop_V2("Boneboo", true, 1),
                    new Crop_V2("Coralcreep", true, 1),
                    new Crop_V2("Pussplum", true, 1),
                    new Crop_V2("Toxictop", true, 1),
                    new Crop_V2("Wartweed", true, 1),
                }),
            winter: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Automato", true, 1),
                    new Crop_V2("Boltbulb", true, 1),
                    new Crop_V2("Boneboo", true, 1),
                    new Crop_V2("Crystal Plant", true, 1),
                    new Crop_V2("Currentcorn", true, 1),
                    new Crop_V2("Diodia", true, 1),
                })
        );

        /// <summary>All the crops that are in the iKeychain's Winter Lychee Plant mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 IKeychainsWinterLycheePlant { get; set; } = new CropMod_V2
        (
            spring: null,
            summer: null,
            fall: null,
            winter: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Winter Lychee", true, 1)
                })
        );

        /// <summary>All the crops that are in the Green Pear mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 GreenPear { get; set; } = new CropMod_V2
        (
            spring: null,
            summer: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Green Pear", true, 1)
                }),
            fall: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Green Pear", true, 1)
                }),
            winter: null
        );

        /// <summary>All the crops that are in the Soda Vine mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 SodaVine { get; set; } = new CropMod_V2
        (
            spring: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Soda Vine", true, 1)
                }),
            summer: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Soda Vine", true, 1)
                }),
            fall: null,
            winter: null
        );

        /// <summary>All the crops that are in the Spoopy Valley mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 SpoopyValley { get; set; } = new CropMod_V2
        (
            spring: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Amethyst Basil", true, 1),
                    new Crop_V2("Black Huckleberry", true, 1),
                    new Crop_V2("Black Mulberry", true, 1),
                    new Crop_V2("Black Velvet Petunia", true, 1),
                    new Crop_V2("Hungarian Chile", true, 1),
                    new Crop_V2("Kulli Corn", true, 1),
                    new Crop_V2("Queen of the Night Tulip", true, 1)
                }),
            summer: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Black Carrot", true, 1),
                    new Crop_V2("Black Goji Berry", true, 1),
                    new Crop_V2("Black Huckleberry", true, 1),
                    new Crop_V2("Black Magic Viola", true, 1),
                    new Crop_V2("Black Mulberry", true, 1),
                    new Crop_V2("Kulli Corn", true, 1)
                }),
            fall: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Black Huckleberry", true, 1),
                    new Crop_V2("Futsu Pumpkin", true, 1),
                    new Crop_V2("Indigo Rose Tomato", true, 1),
                    new Crop_V2("Purple Beauty Bell Pepper", true, 1)
                }),
            winter: null
        );

        /// <summary>All the crops that are in the Stardew Bakery mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 StardewBakery { get; set; } = new CropMod_V2
        (
            spring: null,
            summer: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Cookie Plant", true, 1)
                }),
            fall: null,
            winter: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Cookie Plant", true, 1)
                })
        );

        /// <summary>All the crops that are in the Succulents mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 Succulents { get; set; } = new CropMod_V2
        (
            spring: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Decorative Succulents", true, 1),
                    new Crop_V2("Edible Succulents", true, 1)
                }),
            summer: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Decorative Succulents", true, 1),
                    new Crop_V2("Edible Succulents", true, 1)
                }),
            fall: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Decorative Succulents", true, 1),
                    new Crop_V2("Edible Succulents", true, 1)
                }),
            winter: null
        );

        /// <summary>All the crops that are in the Tropical Farm mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod_V2 TropicalFarm { get; set; } = new CropMod_V2
        (
            spring: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Tuberose", true, 1)
                }),
            summer: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Chinese Hibiscus", true, 1),
                    new Crop_V2("Night-Blooming Jasmine", true, 1),
                    new Crop_V2("Snakefruit", true, 1)
                }),
            fall: new Season_V2(
                new List<Crop_V2>
                {
                    new Crop_V2("Snakefruit", true, 1)
                }),
            winter: null
        );
    }
}
