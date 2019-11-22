using System.Collections.Generic;

namespace BetterMixedSeedsConfigUpdater.ModConfigs
{
    class NewModConfig
    {
        public int PercentDropChanceForMixedSeedsWhenNotFiber { get; set; } = 5;

        public CropMod StardewValley { get; set; }
        public CropMod FantasyCrops { get; set; }
        public CropMod FreshMeat { get; set; }
        public CropMod FruitAndVeggies { get; set; }
        public CropMod MizusFlowers { get; set; }
        public CropMod CannabisKit { get; set; }
        public CropMod SixPlantableCrops { get; set; }
        public CropMod BonsterCrops { get; set; }
        public CropMod RevenantCrops { get; set; }
        public CropMod FarmerToFlorist { get; set; }
        public CropMod LuckyClover { get; set; }
        public CropMod FishsFlowers { get; set; }
        public CropMod StephansLotsOfCrops { get; set; }
        public CropMod EemiesCrops { get; set; }
        public CropMod TeaTime { get; set; }
        public CropMod ForageToFarm { get; set; }
        public CropMod GemAndMineralCrops { get; set; }
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
