using System.Collections.Generic;

namespace CropTransplantMod
{
    public class ModConfig
    {
        public bool GetGradenPotEarlier;
        public float CropTransplantEnergyCost = 4;
        public bool EnablePlacementOfCropsOutsideOutOfTheFarm;
        public bool EnablePlacementOfFruitTreesOutOfTheFarm;
        public bool EnablePlacementOfFruitTreesOnAnyTileType;
        public bool EnablePlacementOfFruitTreesNextToAnotherTree;
        public bool EnablePlacementOfTreesOnAnyTileType;
        public bool EnableSoilTileUnderTrees = true;
        public List<float> FruitTreeTransplantEnergyCostPerStage = new List<float> { 4f, 4f, 4f, 4f, 20f };
        public List<float> TreeTransplantEnergyCostPerStage = new List<float> { 4f, 4f, 8f, 12f, 20f };
        public int TreeTransplantMaxStage = 4;
        public int FruitTreeTransplantMaxStage = 3;
    }
}
