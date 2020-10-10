/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace BetterMixedSeedsConfigUpdater.ModConfigs
{
    /// <summary>The new mod configuration.</summary>
    public class NewModConfig
    {
        /// <summary>The percent chance that a mixed seed is dropped, when fiber isn't dropped, when cutting weeds.</summary>
        public int PercentDropChanceForMixedSeedsWhenNotFiber { get; set; } = 5;

        /// <summary>All the crops that are in the base game, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod StardewValley { get; set; }

        /// <summary>All the crops that are in the Fantasy Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod FantasyCrops { get; set; }

        /// <summary>All the crops that are in the Fresh Meat mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod FreshMeat { get; set; }

        /// <summary>All the crops that are in the Fruits and Veggies mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod FruitAndVeggies { get; set; }

        /// <summary>All the crops that are in the Mizus Flowers mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod MizusFlowers { get; set; }

        /// <summary>All the crops that are in the Cannabis Kit mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod CannabisKit { get; set; }

        /// <summary>All the crops that are in the Six Plantable Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod SixPlantableCrops { get; set; }

        /// <summary>All the crops that are in the Bonsters Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod BonsterCrops { get; set; }

        /// <summary>All the crops that are in the Revenants Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod RevenantCrops { get; set; }

        /// <summary>All the crops that are in the Farmer to Florist mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod FarmerToFlorist { get; set; }

        /// <summary>All the crops that are in the Lucky Clover mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod LuckyClover { get; set; }

        /// <summary>All the crops that are in the Fishs Flowers mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod FishsFlowers { get; set; }

        /// <summary>All the crops that are in the Stephans Lots of Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod StephansLotsOfCrops { get; set; }

        /// <summary>All the crops that are in the Eemies Crops, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod EemiesCrops { get; set; }

        /// <summary>All the crops that are in the Tea Time mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod TeaTime { get; set; }

        /// <summary>All the crops that are in the Forage to Farm mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod ForageToFarm { get; set; }

        /// <summary>All the crops that are in the Gem and Mineral Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod GemAndMineralCrops { get; set; }

        /// <summary>All the crops that are in the Mouse Ear Cress mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod MouseEarCress { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Mouse-Ear Cress", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Mouse-Ear Cress", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Mouse-Ear Cress", true, 1)
                }),
            winter: null
        );

        /// <summary>All the crops that are in the Ancient Crops mod, by season, so the user can disable and choose the chance for each crop.</summary>
        public CropMod AncientCrops { get; set; } = new CropMod
        (
            spring: new Season(
                new List<Crop>
                {
                    new Crop("Ancient Coffee Plant", true, 1),
                    new Crop("Ancient Fern", true, 1),
                    new Crop("Ancient Flower", true, 1),
                    new Crop("Ancient Nut", true, 1),
                    new Crop("Ancient Olive Plant", true, 1),
                    new Crop("Ancient Tuber", true, 1)
                }),
            summer: new Season(
                new List<Crop>
                {
                    new Crop("Ancient Coffee Plant", true, 1),
                    new Crop("Ancient Fern", true, 1),
                    new Crop("Ancient Nut", true, 1),
                    new Crop("Ancient Olive Plant", true, 1),
                    new Crop("Ancient Tuber", true, 1)
                }),
            fall: new Season(
                new List<Crop>
                {
                    new Crop("Ancient Coffee Plant", true, 1),
                    new Crop("Ancient Fern", true, 1),
                    new Crop("Ancient Nut", true, 1),
                    new Crop("Ancient Olive Plant", true, 1),
                    new Crop("Ancient Tuber", true, 1)
                }),
            winter: null
        );
    }
}
