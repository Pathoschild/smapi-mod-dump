using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace CropTransplantMod
{
    public class ModConfig
    {

        public bool GetGradenPotEarlier;
        public float CropTransplantEnergyCost;
        public bool EnablePlacementOfCropsOutsideOutOfTheFarm;
        public bool EnablePlacementOfFruitTreesOutOfTheFarm;
        public bool EnablePlacementOfFruitTreesOnAnyTileType;
        public bool EnablePlacementOfFruitTreesNextToAnotherTree;
        public bool EnablePlacementOfTreesOnAnyTileType;
        public bool EnableSoilTileUnderTrees;
        public List<float> FruitTreeTransplantEnergyCostPerStage;
        public List<float> TreeTransplantEnergyCostPerStage;
        public int TreeTransplantMaxStage;
        public int FruitTreeTransplantMaxStage;

        public ModConfig()
        {
            GetGradenPotEarlier = false;
            CropTransplantEnergyCost = 4f;
            EnablePlacementOfCropsOutsideOutOfTheFarm = false;
            EnablePlacementOfFruitTreesOutOfTheFarm = false;
            EnablePlacementOfFruitTreesOnAnyTileType = false;
            EnablePlacementOfFruitTreesNextToAnotherTree = false;
            EnablePlacementOfTreesOnAnyTileType = false;
            EnableSoilTileUnderTrees = true;
            TreeTransplantEnergyCostPerStage = new List<float>() {4f, 4f, 4f, 4f, 20f };
            FruitTreeTransplantEnergyCostPerStage = new List<float>() { 4f, 4f, 8f, 12f, 20f};
            TreeTransplantMaxStage = 4;
            FruitTreeTransplantMaxStage = 3;
        }
    }
}
