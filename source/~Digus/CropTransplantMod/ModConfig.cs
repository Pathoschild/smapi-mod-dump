/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

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
